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

        public ResponseBaseDataContract AddEditComponenteDespesaRegisto(RegistoDespesaRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.PreenchimentoTarefa, _unitOfWork, CRUD.UPDATE);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            ComponentedespesaRegisto componenteDespesaRegisto = BuildComponenteDespesaRegistoObject(request.despesa);
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
                (request.AgrupamentoFk, request.OrcamentoRegistoFk, request.InstitutionId, request.ActidadeFk,                request.FuncionalFk);
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
            List<ComponentedespesaRegisto> listaComponentedespesaRegisto = _unitOfWork.ComponenteDespesaRegistoRepository.GetAllDespesaRegistadaByAgrupamentoConfigFk(request.AgrupamentoFk                , request.InstitutionId, request.ActidadeFk,               request.FuncionalFk);
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
                    despesa.Estado = _unitOfWork.DominioRepository.getIdDominio("ESTADODESPESA", 3);
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
                            2, 3
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
                    case EstadoDespesaEnum.Obricacao:
                        estadoPagamento = 2;
                        break;
                    //case EstadoDespesaEnum.Executado:
                    //    estadoPagamento = 3;
                    //    break;
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
    }
}