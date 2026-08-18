using System;
using System.Collections.Generic;
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
    public class ComponenteDespesaRegistoDataManager : IComponenteDespesaRegistoDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public ComponenteDespesaRegistoDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        public AddEditDespesaRegistoResponse AddEditComponenteDespesaRegisto(RegistoDespesaRequest request)
        {
            var response = new AddEditDespesaRegistoResponse { RequestId = request.RequestId };

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.PreenchimentoTarefa, _unitOfWork, CRUD.UPDATE);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            ComponentedespesaRegisto componenteDespesaRegisto = BuildComponenteDespesaRegistoObject(request.despesa);

            // Só ao criar uma despesa nova (não ao editar uma já existente): se já existirem outras despesas
            // em curso (Registada ou Autorizada) para a MESMA combinação dos 5 parâmetros -- Institution +
            // Centro de Custo + Actividade + Funcional + rubrica -- o registo NÃO é bloqueado. Devolve-se a
            // lista dessas despesas para o utilizador ver o que já lá está (número do processo, valor, estado)
            // e decidir; se confirmar, o pedido volta com ConfirmarDespesasEmCurso = true e grava.
            //
            // Antes disto havia um bloqueio duro (JaExisteDespesaEmCursoNaRubrica) que comparava apenas 4
            // parâmetros, sem o Centro de Custo: duas despesas de centros de custo diferentes na mesma rubrica
            // eram tratadas como duplicados e a 2ª ficava impossível de registar, sem sequer dizer qual
            // processo estava a causar o bloqueio. O controlo de excesso de verba fica todo a cargo do saldo
            // disponível, verificado logo a seguir e outra vez no Cabimento e no Compromisso.
            //
            // [VI] Chỉ khi tạo despesa mới (không áp dụng khi sửa): nếu đã có despesa khác đang mở (Registada
            // hoặc Autorizada) cùng tổ hợp 5 tham số -- Institution + Centro de Custo + Actividade + Funcional
            // + rubrica -- hệ thống KHÔNG chặn. Nó trả về danh sách các despesa đó để người dùng thấy cái đang
            // tồn tại (số processo, giá trị, trạng thái) rồi tự quyết; nếu xác nhận, request gửi lại với
            // ConfirmarDespesasEmCurso = true và tiến hành lưu.
            //
            // Trước đây chỗ này chặn cứng (JaExisteDespesaEmCursoNaRubrica) và chỉ so 4 tham số, thiếu Centro
            // de Custo: hai despesa thuộc 2 centro de custo khác nhau trên cùng rubrica bị coi là trùng, cái
            // thứ hai không đăng ký nổi, mà cũng không nói cho biết processo nào đang gây chặn. Việc kiểm soát
            // tiêu vượt giao hết cho phần kiểm tra saldo disponível ngay bên dưới, và kiểm lại ở Cabimento
            // cùng Compromisso.
            if (componenteDespesaRegisto.Id == 0 && !request.ConfirmarDespesasEmCurso)
            {
                List<int> estadosEmCurso = new List<int>
                {
                    _unitOfWork.DominioRepository.getIdDominio("ESTADODESPESA", 1),
                    _unitOfWork.DominioRepository.getIdDominio("ESTADODESPESA", 2)
                };

                List<DespesaEmCursoDataContract> despesasEmCurso = _unitOfWork.ComponenteDespesaRegistoRepository
                    .GetDespesasEmCursoByChave(componenteDespesaRegisto.AgrupamentoConfigFk,
                        componenteDespesaRegisto.InstitutionId ?? 0, componenteDespesaRegisto.ActidadeFk ?? 0,
                        componenteDespesaRegisto.FuncionalFk ?? 0, componenteDespesaRegisto.CentroCustoFk,
                        componenteDespesaRegisto.ComponenteOrcamentoRegistoFk,
                        estadosEmCurso, componenteDespesaRegisto.Id);

                if (despesasEmCurso != null && despesasEmCurso.Count > 0)
                {
                    response.DespesasEmCurso = despesasEmCurso;
                    return response;
                }
            }

            // Impedir registar/editar uma despesa (ainda em estado Registado) com um valor que,
            // somado ao que já está reservado (Registado + Autorizado + Cabimentado) por outras
            // despesas da mesma rubrica, excederia o Valor Orçamentado -- mesma lógica do check
            // CabimentoExcedeSaldoDisponivel, mas aplicada logo no registo em vez de só no Cabimento
            // (ver [[registo-ad-missing-budget-check]]). Distingue "sem orçamento nenhum" de
            // "excede o disponível" para a mensagem ficar clara sobre o que fazer em cada caso.
            List<Componenteorcamentovalor> listaOrcamentoValorRegisto = _unitOfWork.ComponenteOrcamentoValorRepository
                .getOrcamentoValorByAgrupamentoFkOrcamentoRegistoFk(componenteDespesaRegisto.AgrupamentoConfigFk, componenteDespesaRegisto.ComponenteOrcamentoRegistoFk,
                    componenteDespesaRegisto.InstitutionId ?? 0, componenteDespesaRegisto.ActidadeFk ?? 0, componenteDespesaRegisto.FuncionalFk ?? 0,
                    componenteDespesaRegisto.CentroCustoFk);
            decimal valorOrcamentadoRegisto = listaOrcamentoValorRegisto != null ? listaOrcamentoValorRegisto.Sum(x => x.Valor) : 0;

            int estadoRegistadoRegisto = _unitOfWork.DominioRepository.getIdDominio("ESTADODESPESA", 1);
            int estadoAutorizadoRegisto = _unitOfWork.DominioRepository.getIdDominio("ESTADODESPESA", 2);
            int estadoCabimentadoRegisto = _unitOfWork.DominioRepository.getIdDominio("ESTADODESPESA", 3);
            List<ComponentedespesaRegisto> outrasDespesasNaRubricaRegisto = _unitOfWork.ComponenteDespesaRegistoRepository
                .GetAllDespesaRegistadaByAgrupamentoConfigFk(componenteDespesaRegisto.AgrupamentoConfigFk, componenteDespesaRegisto.InstitutionId ?? 0, componenteDespesaRegisto.ActidadeFk ?? 0, componenteDespesaRegisto.FuncionalFk ?? 0,
                    componenteDespesaRegisto.CentroCustoFk, componenteDespesaRegisto.ComponenteOrcamentoRegistoFk);
            decimal valorJaReservado = outrasDespesasNaRubricaRegisto != null
                ? outrasDespesasNaRubricaRegisto.Where(d => d.Id != componenteDespesaRegisto.Id
                    && (d.Estado == estadoRegistadoRegisto || d.Estado == estadoAutorizadoRegisto || d.Estado == estadoCabimentadoRegisto))
                    .Sum(d => d.Valor)
                : 0;

            decimal saldoDisponivelRegisto = valorOrcamentadoRegisto - valorJaReservado;

            if (componenteDespesaRegisto.Valor > saldoDisponivelRegisto)
            {
                // O saldo é calculado para uma combinação exacta de Actividade + Centro de Custo; sem os
                // nomear, a mensagem parece dizer que a rubrica inteira só tem aquele valor. Foi
                // exactamente isso que levou o cliente a suspeitar que a Actividade não estava a ser
                // considerada, quando na verdade tinha seleccionado outra Actividade.
                // [VI] Số dư được tính cho đúng 1 tổ hợp Actividade + Centro de Custo; không nêu tên ra
                // thì thông báo trông như thể cả rubrica chỉ có ngần ấy tiền. Chính điều này khiến khách
                // nghi hệ thống không xét Actividade, trong khi thực ra họ chọn nhầm Actividade khác.
                // Código completo + designação do próprio nó (não GetFullDesignacao, que concatena a
                // designação de todos os antepassados e daria uma linha enorme dentro da mensagem de erro).
                // [VI] Mã đầy đủ + tên của chính node (không dùng GetFullDesignacao vì hàm đó ghép tên của
                // toàn bộ tổ tiên, nhét vào thông báo lỗi sẽ dài lê thê).
                string rubricaLabel = BuildAgrupamentoLabel(componenteDespesaRegisto.AgrupamentoConfigFk);
                string actividadeLabel = BuildAgrupamentoLabel(componenteDespesaRegisto.ActidadeFk);
                string funcionalLabel = BuildAgrupamentoLabel(componenteDespesaRegisto.FuncionalFk);
                Centrocusto centroCusto = _unitOfWork.CentroCustoRepository.Get(componenteDespesaRegisto.CentroCustoFk);
                string centroCustoLabel = centroCusto != null ? centroCusto.Descricao : "-";

                if (valorOrcamentadoRegisto <= 0)
                {
                    string errorCodeSemOrcamento = $"{(int)ErrorsDataContract.RegistoSemOrcamentoAtribuido}ÿ{rubricaLabel}ÿ{centroCustoLabel}ÿ{actividadeLabel}ÿ{funcionalLabel}";
                    response.Errors.Add(new Error { ErrorCode = errorCodeSemOrcamento, ErrorMessage = ErrorsDataContract.RegistoSemOrcamentoAtribuido.ToString() });
                }
                else
                {
                    string errorCode = $"{(int)ErrorsDataContract.RegistoExcedeSaldoDisponivel}ÿ{saldoDisponivelRegisto:0.00}ÿ{componenteDespesaRegisto.Valor:0.00}ÿ{rubricaLabel}ÿ{centroCustoLabel}ÿ{actividadeLabel}ÿ{funcionalLabel}";
                    response.Errors.Add(new Error { ErrorCode = errorCode, ErrorMessage = ErrorsDataContract.RegistoExcedeSaldoDisponivel.ToString() });
                }
                return response;
            }

            try
            {
                if (componenteDespesaRegisto.Id > 0)
                {
                    _unitOfWork.ComponenteDespesaRegistoRepository.Update(componenteDespesaRegisto);
                }
                else
                {
                    _unitOfWork.ComponenteDespesaRegistoRepository.Add(componenteDespesaRegisto);
                }
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }

        public GetComponenteDespesaRegistoReponse GetAllDespesaRegistadaByTarefaAtivoId(GetAllDespesaRegistadaRequest request)
        {
            GetComponenteDespesaRegistoReponse response = new GetComponenteDespesaRegistoReponse();

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.PreenchimentoTarefa, _unitOfWork, CRUD.UPDATE);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            List<DespesaRegistadaDataContract> despesaRegistadaListagem = _unitOfWork.ComponenteDespesaRegistoRepository.GetAllDespesaRegistadaByTarefaAtivoId(request.TarefaAtivoId);

            if (despesaRegistadaListagem != null && despesaRegistadaListagem.Count > 0)
            {
                //obter descricao da contabilidade e do orcamento OSS
                foreach (var despesaRegistada in despesaRegistadaListagem)
                {
                    //var codigoContabilidade = _unitOfWork.CodigoContaRepository.GetFullCodigo(despesaRegistada.IdContabilidade);
                    //var descricaoCodigoConta = _unitOfWork.CodigoContaRepository.GetFullDesignacao(despesaRegistada.IdContabilidade);
                    //if (descricaoCodigoConta != null)
                    //{
                    //    despesaRegistada.descricaoContabilidade = descricaoCodigoConta;
                    //}
                    //if (codigoContabilidade != null)
                    //{
                    //    despesaRegistada.codigoContabilidade = codigoContabilidade;
                    //}

                    var codigoOrcamentoOSS = _unitOfWork.AgrupamentoConfigRepository.GetFullCodigo(despesaRegistada.idOrcamento);
                    var descricaoContaOSS = _unitOfWork.AgrupamentoConfigRepository.GetFullDesignacao(despesaRegistada.idOrcamento);
                    if (descricaoContaOSS != null)
                    {
                        despesaRegistada.descricaoOrcamento = descricaoContaOSS;
                    }
                    if (codigoOrcamentoOSS != null)
                    {
                        despesaRegistada.codigoOrcamento = codigoOrcamentoOSS;
                    }
                }

                response.ComponenteDespesaRegisto = despesaRegistadaListagem;
            }

            return response;
        }

        public ResponseBaseDataContract DeleteDespesa(DeleteDespesaRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.PreenchimentoTarefa, _unitOfWork, CRUD.UPDATE);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            // validações
            ComponentedespesaRegisto despesa = _unitOfWork.ComponenteDespesaRegistoRepository.Get(request.Id);

            // Nova validação, se tiver algum compromisso, não deixa apagar
            if (despesa.Compromisso.Any())
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.HasCommitment).ToString(), ErrorMessage = ErrorsDataContract.HasCommitment.ToString() });
                return response;
            }

            if (despesa != null)
            {
                despesa.IndActivo = false;
                despesa = _utils.UpdateDetailsToEntity(despesa);
            }

            try
            {
                if (despesa != null)
                {
                    _unitOfWork.ComponenteDespesaRegistoRepository.Update(despesa);
                }
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }

        public GetValoresDespesaByIdCodigoOrcamentoResponse GetValoresDespesaByIdCodigoOrcamento(GetValoresDespesaByIdCodigoOrcamentoRequest request)
        {
            GetValoresDespesaByIdCodigoOrcamentoResponse response = new GetValoresDespesaByIdCodigoOrcamentoResponse();

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.PreenchimentoTarefa, _unitOfWork, CRUD.UPDATE);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            //obter os valores do Orçamento Aprovado
            List<Componenteorcamentovalor> listaOrcamentoValor = _unitOfWork.ComponenteOrcamentoValorRepository.getOrcamentoValorByAgrupamentoFkOrcamentoRegistoFk
                (request.AgrupamentoFk, request.OrcamentoRegistoFk, request.InstitutionId, request.ActidadeFk,                request.FuncionalFk, request.CentroCustoFk);
            ValoresDespesaRegistadaDataContract valorDespesaRegisto = new ValoresDespesaRegistadaDataContract();
            decimal sumValorOrcamento = 0;

            if (listaOrcamentoValor != null && listaOrcamentoValor.Count > 0)
            {
                foreach (var orcamentoValor in listaOrcamentoValor)
                {
                    sumValorOrcamento = sumValorOrcamento + orcamentoValor.Valor;
                }
                valorDespesaRegisto.ValorOrcamentado = sumValorOrcamento;
            }

            //obter somatório valor despesa Cabimentada, executada, autorizada
            List<ComponentedespesaRegisto> listaComponentedespesaRegisto = _unitOfWork.ComponenteDespesaRegistoRepository.GetAllDespesaRegistadaByAgrupamentoConfigFk(request.AgrupamentoFk                , request.InstitutionId, request.ActidadeFk,               request.FuncionalFk, request.CentroCustoFk, request.OrcamentoRegistoFk);
            decimal sumValorAutorizado = 0;
            decimal sumValorCabimentado = 0;
            decimal sumValorExecutado = 0;

            if (listaComponentedespesaRegisto != null && listaComponentedespesaRegisto.Count > 0)
            {
                foreach (var componentedespesaRegisto in listaComponentedespesaRegisto)
                {

                    if (componentedespesaRegisto.Estado == _unitOfWork.DominioRepository.getIdDominio("ESTADODESPESA", 2))
                    {
                        sumValorAutorizado += componentedespesaRegisto.Valor;
                    }

                    if (componentedespesaRegisto.Estado == _unitOfWork.DominioRepository.getIdDominio("ESTADODESPESA", 3))
                    {
                        var listaPagamentos = _unitOfWork.PagamentosExecutadosRepository.GetPagamentosexecutadosByIdDespesaIdContaOSS(componentedespesaRegisto.Id, componentedespesaRegisto.AgrupamentoConfigFk);

                        if (listaPagamentos != null && listaPagamentos.Count > 0)
                        {
                            foreach (var pagamento in listaPagamentos)
                            {
                                sumValorExecutado += pagamento.ValorExecutado;
                            }
                        }

                        sumValorCabimentado += componentedespesaRegisto.Valor;

                    }
                }
                valorDespesaRegisto.ValorAutorizado = sumValorAutorizado;
                valorDespesaRegisto.ValorCabimentado = sumValorCabimentado;
                valorDespesaRegisto.ValorExecutado = sumValorExecutado;
            }
            response.ValoresDespesa = valorDespesaRegisto;

            return response;
        }

        public ResponseBaseDataContract DeleteAllDespesasRegistadas(DeleteListaDespesaRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.PreenchimentoTarefa, _unitOfWork, CRUD.UPDATE);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            List<ComponentedespesaRegisto> listaDespesasRegistadas = new List<ComponentedespesaRegisto>();

            if (request.Ids != null && request.Ids.Count > 0)
            {
                foreach (var id in request.Ids)
                {
                    var despesa = _unitOfWork.ComponenteDespesaRegistoRepository.Get(id);
                    if (despesa != null)
                    {
                        despesa.IndActivo = false;
                        despesa = _utils.UpdateDetailsToEntity(despesa);
                        listaDespesasRegistadas.Add(despesa);
                    }
                }
            }

            try
            {
                if (listaDespesasRegistadas != null && listaDespesasRegistadas.Count > 0)
                {
                    foreach (var despesaRegistada in listaDespesasRegistadas)
                    {
                        _unitOfWork.ComponenteDespesaRegistoRepository.Update(despesaRegistada);
                    }
                }
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }

        public ResponseBaseDataContract UpdateDespesa(UpdateDespesaRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.PreenchimentoTarefa, _unitOfWork, CRUD.UPDATE);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            ComponentedespesaRegisto despesa = _unitOfWork.ComponenteDespesaRegistoRepository.Get(request.Id);
            if (despesa != null)
            {
                // se estiver no estado Registado 'R' passa para o estado Autorizado
                if (request.Estado == "R")
                {
                    despesa.Estado = _unitOfWork.DominioRepository.getIdDominio("ESTADODESPESA", 2);
                }

                // se estiver no estabo Autorizado 'A' passa para o estado cabimentado
                if (request.Estado == "A")
                {
                    // Validar que o valor da despesa não excede o saldo disponível da rubrica
                    // (Valor Orçamentado - soma já Cabimentada por outras despesas da mesma rubrica)
                    // antes de promover para Cabimentado -- faltava este check (só existia um nível
                    // abaixo, Compromisso vs Cabimentado, em UpsertCompromisso).
                    List<Componenteorcamentovalor> listaOrcamentoValor = _unitOfWork.ComponenteOrcamentoValorRepository
                        .getOrcamentoValorByAgrupamentoFkOrcamentoRegistoFk(despesa.AgrupamentoConfigFk, despesa.ComponenteOrcamentoRegistoFk,
                            despesa.InstitutionId ?? 0, despesa.ActidadeFk ?? 0, despesa.FuncionalFk ?? 0, despesa.CentroCustoFk);
                    decimal valorOrcamentado = listaOrcamentoValor != null ? listaOrcamentoValor.Sum(x => x.Valor) : 0;

                    int estadoCabimentado = _unitOfWork.DominioRepository.getIdDominio("ESTADODESPESA", 3);
                    List<ComponentedespesaRegisto> outrasDespesasNaRubrica = _unitOfWork.ComponenteDespesaRegistoRepository
                        .GetAllDespesaRegistadaByAgrupamentoConfigFk(despesa.AgrupamentoConfigFk, despesa.InstitutionId ?? 0, despesa.ActidadeFk ?? 0, despesa.FuncionalFk ?? 0, despesa.CentroCustoFk, despesa.ComponenteOrcamentoRegistoFk);
                    decimal valorJaCabimentado = outrasDespesasNaRubrica != null
                        ? outrasDespesasNaRubrica.Where(d => d.Id != despesa.Id && d.Estado == estadoCabimentado).Sum(d => d.Valor)
                        : 0;

                    decimal saldoDisponivel = valorOrcamentado - valorJaCabimentado;

                    if (despesa.Valor > saldoDisponivel)
                    {
                        // ÿ separa os valores a substituir em {0}/{1} na mensagem traduzida no frontend
                        // (mesmo padrão usado noutros erros parametrizados, ex: OrcamentoARetificar)
                        string errorCode = $"{(int)ErrorsDataContract.CabimentoExcedeSaldoDisponivel}ÿ{saldoDisponivel:0.00}ÿ{despesa.Valor:0.00}";
                        response.Errors.Add(new Error { ErrorCode = errorCode, ErrorMessage = ErrorsDataContract.CabimentoExcedeSaldoDisponivel.ToString() });
                        return response;
                    }

                    despesa.Estado = estadoCabimentado;
                }
                despesa = _utils.UpdateDetailsToEntity(despesa);
            }

            try
            {
                if (despesa != null)
                {
                    _unitOfWork.ComponenteDespesaRegistoRepository.Update(despesa);
                }
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }

        public GetComponenteDespesaCabimentadaParaExecucaoReponse GetAllDespesaCabimentadasParaExecucaoByTarefaAtivoId(GetAllDespesaRegistadaRequest request)
        {
            GetComponenteDespesaCabimentadaParaExecucaoReponse response = new GetComponenteDespesaCabimentadaParaExecucaoReponse();

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.PreenchimentoTarefa, _unitOfWork, CRUD.UPDATE);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            Tarefaativo tarefaAtivo = _unitOfWork.TarefaAtivoRepository.GetTarefaAtivoById(request.TarefaAtivoId);

            //obter despesas no estado cabimentado
            List<DespesaCabimentadasParaExecucaoDataContract> despesaCabimentadasListagem = _unitOfWork.ComponenteDespesaRegistoRepository.GetAllDespesaCompromissosParaExecucaoByTarefaAtivoId(tarefaAtivo.ProcessoAtivoFk);

            if (despesaCabimentadasListagem != null && despesaCabimentadasListagem.Count > 0)
            {
                foreach (var despesa in despesaCabimentadasListagem)
                {
                    // vai buscar os pagamentos exceutados na despesa de acordo com o id da despesa
                    var listaPagamentosExecutados = _unitOfWork.PagamentosExecutadosRepository.GetPagamentosexecutadosByIdDespesa(despesa.Id);

                    if (listaPagamentosExecutados != null && listaPagamentosExecutados.Count > 0)
                    {
                        foreach (var pagamentoExecutado in listaPagamentosExecutados)
                        {
                            despesa.ValorExecutado += pagamentoExecutado.ValorExecutado;
                        }
                    }

                    despesa.FaltaExecutar = despesa.ValorCabimentado - despesa.ValorExecutado;
                }

                response.DespesasParaExecucao = despesaCabimentadasListagem;
            }

            return response;
        }

        private ComponentedespesaRegisto BuildComponenteDespesaRegistoObject(ComponenteDespesaRegistoDataContract request)
        {
            ComponentedespesaRegisto componenteDespesaRegisto = new ComponentedespesaRegisto
            {
                Id = request.Id,
                ComponenteOrcamentoRegistoFk = request.IdOrcamentoRegistoAprovado,
                TarefaActivoFk = request.TarefaAtivoFK,
                DepartamentoFk = request.DepartamentoFk,
                CentroCustoFk = request.CentroCustoFk,
                TipoContaFk = request.TipoContaFk,
                //CodigoContaFk = request.CodigoContaFk,
                AgrupamentoConfigFk = request.AgrupamentoConfigFk,
                InstitutionId = request.InstitutionId,
                ActidadeFk = request.ActidadeFk,
                
                FuncionalFk = request.FuncionalFk,
                Descricao = request.Descricao,
                Valor = request.Valor,
                IndActivo = true,
                Estado = _unitOfWork.DominioRepository.getIdDominio("ESTADODESPESA", 1)
            };
            if (componenteDespesaRegisto.Id > 0)
            {
                ComponentedespesaRegisto original = _unitOfWork.ComponenteDespesaRegistoRepository.Get(componenteDespesaRegisto.Id);
                componenteDespesaRegisto.UtilizadorCriacao = original.UtilizadorCriacao;
                componenteDespesaRegisto.DataCriacao = original.DataCriacao;
                componenteDespesaRegisto = _utils.UpdateDetailsToEntity(componenteDespesaRegisto);
            }
            else
                componenteDespesaRegisto = _utils.SetDetailsToEntity(componenteDespesaRegisto);
            return componenteDespesaRegisto;
        }

        public GetDespesasRelatoriosReponse GetDespesasRelatorio(GetDespesasRelatorioRequest request)
        {
            GetDespesasRelatoriosReponse response = new GetDespesasRelatoriosReponse();

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleRelatorios.Consultas, _unitOfWork);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            try
            {
                List<int> estadoValor = new List<int>();
                int? estadoPagamento = null;

                switch (request.EstadoDespesa)
                {
                    case EstadoDespesaEnum.Autorizado:
                        estadoValor = new List<int>()
                        {
                            2
                        };
                        break;
                    case EstadoDespesaEnum.Cabimentado:
                        estadoValor = new List<int>()
                        {
                            3
                        };
                        break;
                    case EstadoDespesaEnum.Compromisso:
                        estadoPagamento = 1;
                        break;
                    case EstadoDespesaEnum.OrdemPagamentoEmitida:
                        estadoPagamento = 2;
                        break;
                    // EstadoDespesaEnum.Obricacao and EstadoDespesaEnum.Executado are handled
                    // directly in the repository: Obricacao checks the Carregar Documentos +
                    // Texto entries registered at the RD05 "Liquidação" task (real Obrigação
                    // evidence), Executado checks the reconciled bank movement.
                    default:
                        break;
                }
                var estadosDespesa = estadoValor.Any() ? _unitOfWork.DominioRepository.getIdDominios("ESTADODESPESA", estadoValor) : estadoValor;
                var estadoPag = estadoPagamento.HasValue ? _unitOfWork.DominioRepository.getIdDominio("ESTADOPAGAMENTO", estadoPagamento.Value) : (int?)null;
                response = _unitOfWork.ComponenteDespesaRegistoRepository.GetDespesasRelatorio(request, estadosDespesa, estadoPag, request.EstadoDespesa);
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }

        public StringFileReponse GetDespesasRelatorioExcel(GetDespesasRelatorioRequest request)
        {
            StringFileReponse response = new StringFileReponse();

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleRelatorios.Consultas, _unitOfWork);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            request.filter.index = 0;
            request.filter.rows = 999999;

            var data = GetDespesasRelatorio(request);

            response.File = _unitOfWork.ComponenteDespesaRegistoRepository.GetDespesasRelatorioExcel(request, data.Despesas);

            return response;
        }

        public GetDespesasCompromissoResponse GetDespesasCompromissoByTarefaAtivoId(GetDespesasCompromissoRequest request)
        {
            GetDespesasCompromissoResponse response = new GetDespesasCompromissoResponse();

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.PreenchimentoTarefa, _unitOfWork, CRUD.UPDATE);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            List<DespesaCompromissoDataContract> despesaObrigacaoListagem = _unitOfWork.ComponenteDespesaRegistoRepository.GetAllDespesaCompromissoByTarefaAtivoId(request.TarefaAtivoId);

            response.ComponenteDespesaObrigacao = despesaObrigacaoListagem;

            return response;
        }

        public ResponseBaseDataContract DeleteCompromisso(DeleteRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.PreenchimentoTarefa, _unitOfWork, CRUD.UPDATE);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            // validações
            Compromisso compromisso = _unitOfWork.CompromissoRepository.Get(request.Id);
            if (compromisso != null)
            {
                compromisso.IndActivo = false;
                compromisso = _utils.UpdateDetailsToEntity(compromisso);
            }

            try
            {
                if (compromisso != null)
                {
                    _unitOfWork.CompromissoRepository.Update(compromisso);
                }
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }

        public ResponseBaseDataContract UpsertCompromisso(CompromissoUpsertRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.PreenchimentoTarefa, _unitOfWork, CRUD.UPDATE);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            Compromisso compromisso;

            ComponentedespesaRegisto componenteDespesaRegisto = _unitOfWork.ComponenteDespesaRegistoRepository.Get(request.Compromisso.despesaRegistadaFk);

            //adicionar logica de verificar se o valor do compromisso não excedo o valor da despesa cabimentada
            //é necessário validar todos os compromissos primeiros e somar para ver o restante que sobra da despesa cabimentada
            decimal existingValue = 0;
            if(componenteDespesaRegisto.Compromisso.Any())
                existingValue += componenteDespesaRegisto.Compromisso.Sum(x => x.Valor);

            var maxValueToCompromise = componenteDespesaRegisto.Valor - existingValue;

            if (request.Compromisso.valorCompromisso > maxValueToCompromise)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.ExceedsDespesaAmount).ToString(), ErrorMessage = ErrorsDataContract.ExceedsDespesaAmount.ToString() });
                return response;
            }

            if (request.Compromisso.id.HasValue)
            {
                compromisso = _unitOfWork.CompromissoRepository.Get(request.Compromisso.id.Value);
                compromisso.ComponenteDespesaRegistoFk = request.Compromisso.despesaRegistadaFk;
                compromisso.Nome = request.Compromisso.nomeCompromisso;
                compromisso.Valor = request.Compromisso.valorCompromisso;
                compromisso.Data = request.Compromisso.dataCompromisso;
                compromisso = _utils.UpdateDetailsToEntity(compromisso);
            }
            else
            {
                compromisso = new Compromisso
                {
                    ComponenteDespesaRegistoFk = request.Compromisso.despesaRegistadaFk,
                    Nome = request.Compromisso.nomeCompromisso,
                    Valor = request.Compromisso.valorCompromisso,
                    Data = request.Compromisso.dataCompromisso,
                    TarefaAtivoFk = request.TarefaAtivoId,
                    IndActivo = true
                };
                compromisso = _utils.SetDetailsToEntity(compromisso);
            }


            try
            {
                if (compromisso.Id != 0)
                {
                    _unitOfWork.CompromissoRepository.Update(compromisso);
                }
                else
                    _unitOfWork.CompromissoRepository.Add(compromisso);

                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }

        public GetComponenteDespesaCabimentadaParaExecucaoReponse GetAllDespesaCompromissosParaExecucaoByTarefaAtivoId(GetAllDespesaRegistadaRequest request)
        {
            GetComponenteDespesaCabimentadaParaExecucaoReponse response = new GetComponenteDespesaCabimentadaParaExecucaoReponse();

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.PreenchimentoTarefa, _unitOfWork, CRUD.UPDATE);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            Tarefaativo tarefaAtivo = _unitOfWork.TarefaAtivoRepository.GetTarefaAtivoById(request.TarefaAtivoId);

            //obter despesas no estado cabimentado
            List<DespesaCabimentadasParaExecucaoDataContract> despesaCabimentadasListagem = _unitOfWork.ComponenteDespesaRegistoRepository.GetAllDespesaCompromissosParaExecucaoByTarefaAtivoId(tarefaAtivo.ProcessoAtivoFk);

            if (despesaCabimentadasListagem != null && despesaCabimentadasListagem.Count > 0)
            {
                foreach (var despesa in despesaCabimentadasListagem)
                {
                    // vai buscar os pagamentos exceutados na despesa de acordo com o id da despesa
                    var listaPagamentosExecutados = _unitOfWork.PagamentosExecutadosRepository.GetPagamentosexecutadosByIdDespesa(despesa.Id);

                    if (listaPagamentosExecutados != null && listaPagamentosExecutados.Count > 0)
                    {
                        foreach (var pagamentoExecutado in listaPagamentosExecutados)
                        {
                            despesa.ValorExecutado += pagamentoExecutado.ValorExecutado;
                        }
                    }

                    despesa.FaltaExecutar = despesa.ValorCabimentado - despesa.ValorExecutado;
                }

                response.DespesasParaExecucao = despesaCabimentadasListagem;
            }

            return response;
        }

        public ResponseBaseDataContract UpdateDespesaCabimentada(UpdateDespesaCabimentadaRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.PreenchimentoTarefa, _unitOfWork, CRUD.UPDATE);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }
            decimal existingValue = 0;
            ComponentedespesaRegisto despesa = _unitOfWork.ComponenteDespesaRegistoRepository.Get(request.Id);

            if(despesa.Compromisso.Any())
                existingValue += despesa.Compromisso.Sum(x => x.Valor);

            if (request.Valor < existingValue)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.ValueIsInferiorToCommitted).ToString(), ErrorMessage = ErrorsDataContract.ValueIsInferiorToCommitted.ToString() });
                return response;
            }

            if (despesa != null)
            {
                despesa.Valor = request.Valor;
                despesa = _utils.UpdateDetailsToEntity(despesa);
            }

            try
            {
                if (despesa != null)
                {
                    _unitOfWork.ComponenteDespesaRegistoRepository.Update(despesa);
                }
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }

        // Etiqueta legível de um nó da árvore (rubrica/Actividade/Funcional): código completo +
        // designação do próprio nó -- o mesmo formato usado nas dropdowns do ecrã (ver
        // AgrupamentoConfigRepository.GetActidadesAgrupamentoConfigByOrcamentoConfig), para o
        // utilizador poder comparar directamente com o que seleccionou. Não usar GetFullDesignacao:
        // concatena a designação de todos os antepassados e daria uma linha enorme na mensagem.
        // [VI] Nhãn dễ đọc của một node (rubrica/Actividade/Funcional): mã đầy đủ + tên của chính
        // node -- đúng format dropdown trên màn hình, để người dùng đối chiếu thẳng với thứ họ chọn.
        // Không dùng GetFullDesignacao vì hàm đó ghép tên cả tổ tiên, nhét vào thông báo sẽ quá dài.
        private string BuildAgrupamentoLabel(int? agrupamentoId)
        {
            if (!agrupamentoId.HasValue || agrupamentoId.Value <= 0)
            {
                return "-";
            }

            Agrupamentoconfig agrupamento = _unitOfWork.AgrupamentoConfigRepository.Get(agrupamentoId.Value);
            if (agrupamento == null)
            {
                return "-";
            }

            return $"{_unitOfWork.AgrupamentoConfigRepository.GetFullCodigo(agrupamentoId.Value)} - {agrupamento.Designacao}";
        }

    }
}