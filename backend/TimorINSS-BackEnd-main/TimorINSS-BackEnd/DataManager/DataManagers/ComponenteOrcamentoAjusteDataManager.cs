using System;
using System.Linq;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    // Ajuste de orçamento por rubrica -- alternativa leve à Retificação (que substitui o envelope
    // inteiro e fica bloqueada assim que existam despesas executadas no período, ver bug/limitação
    // confirmada em 2026-08-11: [[prod-dev]] Regras_Negocio_Ciclo_Despesa_New_Mode.md #7).
    // Actualiza directamente o(s) Componenteorcamentovalor.Valor do envelope Aprovado corrente,
    // em 2 passos (Solicitar -> Aprovar/Rejeitar):
    //  - RubricaOrigemFk == null -> Bonificação: soma directo à RubricaDestinoFk, sem validar saldo.
    //  - RubricaOrigemFk != null -> Transferência: subtrai da origem, soma ao destino, MESMO valor
    //    para os dois lados (cancela-se automaticamente, sem precisar de validar "soma = 0").
    public class ComponenteOrcamentoAjusteDataManager : IComponenteOrcamentoAjusteDataManager
    {
        private const string ESTADO_PENDENTE = "Pendente";
        private const string ESTADO_APROVADO = "Aprovado";
        private const string ESTADO_REJEITADO = "Rejeitado";

        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public ComponenteOrcamentoAjusteDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        public SolicitarAjusteOrcamentoResponse SolicitarAjuste(SolicitarAjusteOrcamentoRequest request)
        {
            SolicitarAjusteOrcamentoResponse response = new SolicitarAjusteOrcamentoResponse();

            bool permission = _utils.ValidatePermission(request.UserId, (int)ModuleGestao.PreenchimentoTarefa, _unitOfWork, CRUD.CREATE);
            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            if (request.Valor <= 0)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.AjusteValorInvalido).ToString(), ErrorMessage = ErrorsDataContract.AjusteValorInvalido.ToString() });
                return response;
            }

            if (request.RubricaOrigemFk.HasValue && request.RubricaOrigemFk.Value == request.RubricaDestinoFk)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.AjusteRubricaOrigemIgualDestino).ToString(), ErrorMessage = ErrorsDataContract.AjusteRubricaOrigemIgualDestino.ToString() });
                return response;
            }

            Componenteorcamentovalor destino = _unitOfWork.ComponenteOrcamentoValorRepository.Get(request.RubricaDestinoFk);
            if (destino == null)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.EntityDoesNotExist).ToString(), ErrorMessage = ErrorsDataContract.EntityDoesNotExist.ToString() });
                return response;
            }

            if (request.RubricaOrigemFk.HasValue)
            {
                Componenteorcamentovalor origem = _unitOfWork.ComponenteOrcamentoValorRepository.Get(request.RubricaOrigemFk.Value);
                if (origem == null)
                {
                    response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.EntityDoesNotExist).ToString(), ErrorMessage = ErrorsDataContract.EntityDoesNotExist.ToString() });
                    return response;
                }

                Error saldoError = ValidarSaldoOrigem(origem, request.Valor, excluirAjusteId: null);
                if (saldoError != null)
                {
                    response.Errors.Add(saldoError);
                    return response;
                }
            }

            ComponenteOrcamentoAjuste ajuste = new ComponenteOrcamentoAjuste
            {
                ComponenteOrcamentoRegistoFk = destino.ComponenteOrcamentoRegistoFk,
                RubricaOrigemFk = request.RubricaOrigemFk,
                RubricaDestinoFk = request.RubricaDestinoFk,
                Valor = request.Valor,
                Estado = ESTADO_PENDENTE,
                Motivo = request.Motivo,
                UtilizadorSolicitacao = request.UserId,
                DataSolicitacao = DateTime.Now,
                IndActivo = true
            };

            try
            {
                _unitOfWork.ComponenteOrcamentoAjusteRepository.Add(ajuste);
                _unitOfWork.Commit();
                response.Id = ajuste.Id;
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                _unitOfWork.Rollback();
            }

            return response;
        }

        public ResponseBaseDataContract AprovarAjuste(AprovarAjusteOrcamentoRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            bool permission = _utils.ValidatePermission(request.UserId, (int)ModuleGestao.PreenchimentoTarefa, _unitOfWork, CRUD.UPDATE);
            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            ComponenteOrcamentoAjuste ajuste = _unitOfWork.ComponenteOrcamentoAjusteRepository.Get(request.Id);
            if (ajuste == null || !ajuste.IndActivo)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.AjusteNotFound).ToString(), ErrorMessage = ErrorsDataContract.AjusteNotFound.ToString() });
                return response;
            }

            if (ajuste.Estado != ESTADO_PENDENTE)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.AjusteEstadoInvalido).ToString(), ErrorMessage = ErrorsDataContract.AjusteEstadoInvalido.ToString() });
                return response;
            }

            Componenteorcamentovalor destino = _unitOfWork.ComponenteOrcamentoValorRepository.Get(ajuste.RubricaDestinoFk);
            if (destino == null)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.EntityDoesNotExist).ToString(), ErrorMessage = ErrorsDataContract.EntityDoesNotExist.ToString() });
                return response;
            }

            Componenteorcamentovalor origem = null;
            if (ajuste.RubricaOrigemFk.HasValue)
            {
                origem = _unitOfWork.ComponenteOrcamentoValorRepository.Get(ajuste.RubricaOrigemFk.Value);
                if (origem == null)
                {
                    response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.EntityDoesNotExist).ToString(), ErrorMessage = ErrorsDataContract.EntityDoesNotExist.ToString() });
                    return response;
                }

                // Revalida o saldo no momento da aprovação -- pode ter mudado desde a solicitação
                // (ex: nova AD registada entretanto na mesma rubrica de origem).
                Error saldoError = ValidarSaldoOrigem(origem, ajuste.Valor, excluirAjusteId: ajuste.Id);
                if (saldoError != null)
                {
                    response.Errors.Add(saldoError);
                    return response;
                }
            }

            try
            {
                if (origem != null)
                {
                    origem.Valor -= ajuste.Valor;
                    _unitOfWork.ComponenteOrcamentoValorRepository.Update(origem);
                }

                destino.Valor += ajuste.Valor;
                _unitOfWork.ComponenteOrcamentoValorRepository.Update(destino);

                ajuste.Estado = ESTADO_APROVADO;
                ajuste.UtilizadorAprovacao = request.UserId;
                ajuste.DataAprovacao = DateTime.Now;
                _unitOfWork.ComponenteOrcamentoAjusteRepository.Update(ajuste);

                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                _unitOfWork.Rollback();
            }

            return response;
        }

        public ResponseBaseDataContract RejeitarAjuste(RejeitarAjusteOrcamentoRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            bool permission = _utils.ValidatePermission(request.UserId, (int)ModuleGestao.PreenchimentoTarefa, _unitOfWork, CRUD.UPDATE);
            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            ComponenteOrcamentoAjuste ajuste = _unitOfWork.ComponenteOrcamentoAjusteRepository.Get(request.Id);
            if (ajuste == null || !ajuste.IndActivo)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.AjusteNotFound).ToString(), ErrorMessage = ErrorsDataContract.AjusteNotFound.ToString() });
                return response;
            }

            if (ajuste.Estado != ESTADO_PENDENTE)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.AjusteEstadoInvalido).ToString(), ErrorMessage = ErrorsDataContract.AjusteEstadoInvalido.ToString() });
                return response;
            }

            try
            {
                ajuste.Estado = ESTADO_REJEITADO;
                ajuste.MotivoRejeicao = request.MotivoRejeicao;
                ajuste.UtilizadorAprovacao = request.UserId;
                ajuste.DataAprovacao = DateTime.Now;
                _unitOfWork.ComponenteOrcamentoAjusteRepository.Update(ajuste);

                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                _unitOfWork.Rollback();
            }

            return response;
        }

        public GetAjustesOrcamentoResponse GetPendentes(GetAjustesOrcamentoRequest request)
        {
            GetAjustesOrcamentoResponse response = new GetAjustesOrcamentoResponse();

            var ajustes = _unitOfWork.ComponenteOrcamentoAjusteRepository.GetPendentesByComponenteOrcamentoRegistoFk(request.ComponenteOrcamentoRegistoFk);
            response.Ajustes = ajustes.Select(BuildDataContract).ToList();

            return response;
        }

        public GetAjustesOrcamentoResponse GetHistorico(GetAjustesOrcamentoRequest request)
        {
            GetAjustesOrcamentoResponse response = new GetAjustesOrcamentoResponse();

            var ajustes = _unitOfWork.ComponenteOrcamentoAjusteRepository.GetHistoricoByComponenteOrcamentoRegistoFk(request.ComponenteOrcamentoRegistoFk);
            response.Ajustes = ajustes.Select(BuildDataContract).ToList();

            return response;
        }

        // Lista as rubricas do envelope de orçamento Aprovado corrente (hoje), com o saldo
        // disponível de cada uma -- para preencher os selects de origem/destino no ecrã de
        // Ajuste de Orçamento sem precisar de estar dentro de uma Tarefa.
        public GetRubricasDisponiveisResponse GetRubricasDisponiveis(RequestBaseDataContract request)
        {
            GetRubricasDisponiveisResponse response = new GetRubricasDisponiveisResponse();

            ComponenteorcamentoRegisto envelope = _unitOfWork.ComponenteOrcamentoRegistoRepository.GetOrcamentoAprovadoByDataPInicioProcesso(DateTime.Now);
            if (envelope == null)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.OrcamentoNaoConfigurado).ToString(), ErrorMessage = ErrorsDataContract.OrcamentoNaoConfigurado.ToString() });
                return response;
            }

            response.ComponenteOrcamentoRegistoFk = envelope.Id;

            var rubricas = _unitOfWork.ComponenteOrcamentoValorRepository.getAllByRegistoId(envelope.Id);
            response.Rubricas = rubricas.Select(r => new RubricaDisponivelDataContract
            {
                Id = r.Id,
                Descricao = FormatRubricaLabel(r),
                Valor = r.Valor,
                SaldoDisponivel = GetSaldoDisponivel(r)
            }).OrderBy(r => r.Descricao).ToList();

            return response;
        }

        // Mesma fórmula de saldo disponível usada no registo de AD (RD01) -- ver
        // ComponenteDespesRegistoDataManager.AddEditComponenteDespesaRegisto, linhas 70-99 --
        // para que "saldo disponível" signifique sempre a mesma coisa em todo o Ciclo da Despesa.
        // excluirAjusteId: ao revalidar na aprovação, não conta este próprio ajuste como "já reservado".
        private Error ValidarSaldoOrigem(Componenteorcamentovalor origem, decimal valorAjuste, int? excluirAjusteId)
        {
            decimal saldoDisponivel = GetSaldoDisponivel(origem);

            // Outros ajustes já Pendentes com esta rubrica como origem também reservam saldo,
            // para não deixar propor 2 transferências que juntas excedam o saldo actual.
            decimal reservadoPorOutrosAjustesPendentes = _unitOfWork.ComponenteOrcamentoAjusteRepository
                .GetPendentesByComponenteOrcamentoRegistoFk(origem.ComponenteOrcamentoRegistoFk)
                .Where(a => a.RubricaOrigemFk == origem.Id && a.Id != excluirAjusteId)
                .Sum(a => a.Valor);

            decimal saldoRealDisponivel = saldoDisponivel - reservadoPorOutrosAjustesPendentes;

            if (valorAjuste > saldoRealDisponivel)
            {
                string errorCode = $"{(int)ErrorsDataContract.AjusteRubricaOrigemSaldoInsuficiente}ÿ{saldoRealDisponivel:0.00}ÿ{valorAjuste:0.00}";
                return new Error { ErrorCode = errorCode, ErrorMessage = ErrorsDataContract.AjusteRubricaOrigemSaldoInsuficiente.ToString() };
            }

            return null;
        }

        private decimal GetSaldoDisponivel(Componenteorcamentovalor rubrica)
        {
            var orcamentoMatch = _unitOfWork.ComponenteOrcamentoValorRepository
                .getOrcamentoValorByAgrupamentoFkOrcamentoRegistoFk(rubrica.AgrupamentoFk ?? 0, rubrica.ComponenteOrcamentoRegistoFk,
                    rubrica.InstitutionId ?? 0, rubrica.ActidadeFk ?? 0, rubrica.FuncionalFk ?? 0);
            decimal valorOrcamentado = orcamentoMatch != null ? orcamentoMatch.Sum(v => v.Valor) : 0;

            int estadoRegistado = _unitOfWork.DominioRepository.getIdDominio("ESTADODESPESA", 1);
            int estadoAutorizado = _unitOfWork.DominioRepository.getIdDominio("ESTADODESPESA", 2);
            int estadoCabimentado = _unitOfWork.DominioRepository.getIdDominio("ESTADODESPESA", 3);

            var despesas = _unitOfWork.ComponenteDespesaRegistoRepository
                .GetAllDespesaRegistadaByAgrupamentoConfigFk(rubrica.AgrupamentoFk ?? 0, rubrica.InstitutionId ?? 0, rubrica.ActidadeFk ?? 0, rubrica.FuncionalFk ?? 0);

            decimal valorJaReservado = despesas != null
                ? despesas.Where(d => d.Estado == estadoRegistado || d.Estado == estadoAutorizado || d.Estado == estadoCabimentado).Sum(d => d.Valor)
                : 0;

            return valorOrcamentado - valorJaReservado;
        }

        // Mostra o código COMPLETO da conta (concatenado com todos os ancestrais) junto ao nome --
        // só o nome (ex: "SALÁRIOS E VENCIMENTOS") não chega para identificar a rubrica com confiança
        // quando há várias com nomes parecidos, e o código LOCAL sozinho (Agrupamentoconfig.Codigo)
        // repete-se entre grupos-pai diferentes -- ver [[codigoconta-local-vs-full-code]] (feedback
        // 2026-08-11: "màn hình này cần hiện account code nữa, không chỉ có tên, khó hiểu", e o local
        // code mostrava "01 -" repetido em várias rubricas sem relação nenhuma entre si). Usa o mesmo
        // helper (`GetFullCodigo`, percorre ParentFkNavigation) já usado pelos outros ecrãs de
        // Orçamento (`ComponenteOrcamentoValorDataManager.cs:121`, `ComponenteOrcamentoRegistoDataManager.cs:96`).
        private string FormatRubricaLabel(Componenteorcamentovalor rubrica)
        {
            if (rubrica?.AgrupamentoFkNavigation == null)
            {
                return null;
            }

            string codigo = rubrica.AgrupamentoFk.HasValue
                ? _unitOfWork.AgrupamentoConfigRepository.GetFullCodigo(rubrica.AgrupamentoFk.Value)
                : rubrica.AgrupamentoFkNavigation.Codigo;
            string designacao = rubrica.AgrupamentoFkNavigation.Designacao;
            return string.IsNullOrEmpty(codigo) ? designacao : $"{codigo} - {designacao}";
        }

        private ComponenteOrcamentoAjusteDataContract BuildDataContract(ComponenteOrcamentoAjuste ajuste)
        {
            Componenteorcamentovalor destino = _unitOfWork.ComponenteOrcamentoValorRepository.GetWithAgrupamento(ajuste.RubricaDestinoFk);
            Componenteorcamentovalor origem = ajuste.RubricaOrigemFk.HasValue
                ? _unitOfWork.ComponenteOrcamentoValorRepository.GetWithAgrupamento(ajuste.RubricaOrigemFk.Value)
                : null;

            return new ComponenteOrcamentoAjusteDataContract
            {
                Id = ajuste.Id,
                ComponenteOrcamentoRegistoFk = ajuste.ComponenteOrcamentoRegistoFk,
                RubricaOrigemFk = ajuste.RubricaOrigemFk,
                RubricaOrigemDescricao = FormatRubricaLabel(origem),
                RubricaDestinoFk = ajuste.RubricaDestinoFk,
                RubricaDestinoDescricao = FormatRubricaLabel(destino),
                Valor = ajuste.Valor,
                Estado = ajuste.Estado,
                Motivo = ajuste.Motivo,
                MotivoRejeicao = ajuste.MotivoRejeicao,
                UtilizadorSolicitacao = ajuste.UtilizadorSolicitacao,
                DataSolicitacao = ajuste.DataSolicitacao,
                UtilizadorAprovacao = ajuste.UtilizadorAprovacao,
                DataAprovacao = ajuste.DataAprovacao
            };
        }
    }
}
