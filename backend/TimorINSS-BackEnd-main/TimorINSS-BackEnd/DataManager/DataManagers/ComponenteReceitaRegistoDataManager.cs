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
    public class ComponenteReceitaRegistoDataManager : IComponenteReceitaRegistoDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public ComponenteReceitaRegistoDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        public ResponseBaseDataContract AddEditComponenteReceitaRegisto(RegistoReceitaRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.PreenchimentoTarefa, _unitOfWork, CRUD.UPDATE);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }


            ComponentereceitaRegisto componenteReceitaRegisto = BuildComponenteReceitaRegistoObject(request.componenteReceitaRegisto);

            try
            {

                // se a receita já estiver registada, só atualiza a receita e os movimentos associados a cada uma das receitas
                if (componenteReceitaRegisto.Id > 0)
                {
                    componenteReceitaRegisto = _utils.UpdateDetailsToEntity(componenteReceitaRegisto);
                    _unitOfWork.ComponenteReceitaRegistoRepository.Update(componenteReceitaRegisto);

                    if (request.componenteReceitaRegisto.ListaMovimentosConciliados != null && request.componenteReceitaRegisto.ListaMovimentosConciliados.Count > 0)
                    {
                        var movimentosOriginais = _unitOfWork.ComponenteReceitaRegistoMovimentosRepository.GetMovimentosByIdReceita(componenteReceitaRegisto.Id);

                        componenteReceitaRegisto.ComponentereceitaRegistoMovimentos = movimentosOriginais;

                        foreach (var movimento in request.componenteReceitaRegisto.ListaMovimentosConciliados)
                        {
                            var movimentosPorConciliar = _unitOfWork.RelMovimentosPorConciliarMovimentosRepository.Get(movimento.id);

                            if (movimentosPorConciliar != null)
                            {
                                if (!movimentosOriginais.Any(e => e.RelMovimentosPorConciliarMovimentosId == movimento.id))
                                {
                                    componenteReceitaRegisto.ComponentereceitaRegistoMovimentos.Add(new ComponentereceitaRegistoMovimentos()
                                    {
                                        RelMovimentosPorConciliarMovimentosId = movimento.id
                                    });
                                    //ao registar uma receita, todos os movimentos associados a cada uma das receitas passam para o estado 
                                    //"selecionados para registo"
                                    movimentosPorConciliar.Estado = _unitOfWork.DominioRepository.getIdDominio("ESTADOMOVIMENTO", 2);
                                    movimentosPorConciliar = _utils.UpdateDetailsToEntity(movimentosPorConciliar);
                                    _unitOfWork.RelMovimentosPorConciliarMovimentosRepository.Update(movimentosPorConciliar);
                                }
                            }
                        }

                        foreach (var movimento in componenteReceitaRegisto.ComponentereceitaRegistoMovimentos)
                        {
                            if (!request.componenteReceitaRegisto.ListaMovimentosConciliados.Any(e => e.id == movimento.RelMovimentosPorConciliarMovimentosId))
                            {
                                var movimentosPorConciliar = _unitOfWork.RelMovimentosPorConciliarMovimentosRepository.Get(movimento.RelMovimentosPorConciliarMovimentosId);

                                if (movimentosPorConciliar != null)
                                {
                                    movimento.IndActivo = false;
                                    movimentosPorConciliar.Estado = _unitOfWork.DominioRepository.getIdDominio("ESTADOMOVIMENTO", 1);
                                    movimentosPorConciliar = _utils.UpdateDetailsToEntity(movimentosPorConciliar);
                                    _unitOfWork.RelMovimentosPorConciliarMovimentosRepository.Update(movimentosPorConciliar);
                                }
                            }
                        }
                    }
                }
                else
                {
                    // caso contrário se a receita não estiver registada, regista a despesa e atualiza o estado dos movimentos associados.
                    if (request.componenteReceitaRegisto.ListaMovimentosConciliados != null && request.componenteReceitaRegisto.ListaMovimentosConciliados.Count > 0)
                    {
                        ComponentereceitaRegisto componenteReceita = BuildComponenteReceitaRegistoObject(request.componenteReceitaRegisto);
                        foreach (var movimentos in request.componenteReceitaRegisto.ListaMovimentosConciliados)
                        {
                            var movimentosPorConciliar = _unitOfWork.RelMovimentosPorConciliarMovimentosRepository.Get(movimentos.id);
                            if (movimentosPorConciliar != null)
                            {
                                componenteReceita.ComponentereceitaRegistoMovimentos.Add(new ComponentereceitaRegistoMovimentos()
                                {
                                    RelMovimentosPorConciliarMovimentosId = movimentos.id
                                });
                                movimentosPorConciliar.Estado = _unitOfWork.DominioRepository.getIdDominio("ESTADOMOVIMENTO", 2);
                                movimentosPorConciliar = _utils.SetDetailsToEntity(movimentosPorConciliar);

                                _unitOfWork.RelMovimentosPorConciliarMovimentosRepository.Update(movimentosPorConciliar);
                            }
                        }
                        _unitOfWork.ComponenteReceitaRegistoRepository.Add(componenteReceita);
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

        public ComponentesReceitaRegistoResponseDataContract GetComponenteReceitaRegistoByContaOSSId(GetComponenteReceitaRegistoByIdContaOSSRequest request)
        {
            var response = new ComponentesReceitaRegistoResponseDataContract { RequestId = request.RequestId };

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.PreenchimentoTarefa, _unitOfWork, CRUD.UPDATE);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            response = _unitOfWork.ComponenteReceitaRegistoRepository.GetComponenteReceitaRegistoByContaOSSId(request);

            if (response != null && response.ComponenteReceita != null && response.ComponenteReceita.Count > 0)
            {
                foreach (var componenteReceita in response.ComponenteReceita)
                {
                    var movimentos = _unitOfWork.MovimentosPorConciliarRepository.GetMovimentosConciliadosByIdEstado(componenteReceita.MovimentosIds, _unitOfWork.DominioRepository.getIdDominio("ESTADOMOVIMENTO", 2));

                    componenteReceita.Movimentos = movimentos;
                }
            }

            return response;
        }

        public ResponseBaseDataContract DeleteReceita(DeleteReceitaRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.PreenchimentoTarefa, _unitOfWork, CRUD.UPDATE);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            ComponentereceitaRegisto receita = _unitOfWork.ComponenteReceitaRegistoRepository.Get(request.Id);

            if (receita != null)
            {
                receita.ComponentereceitaRegistoMovimentos.FirstOrDefault(e => e.RelMovimentosPorConciliarMovimentosId == request.MovimentoId).IndActivo = false;

                if (!receita.ComponentereceitaRegistoMovimentos.Any(e => e.IndActivo.Value)) {
                    receita.IndActivo = false;
                    receita = _utils.UpdateDetailsToEntity(receita);
                }

                var rel = _unitOfWork.RelMovimentosPorConciliarMovimentosRepository.Get(request.MovimentoId);

                if (rel != null)
                {
                    rel.Estado = _unitOfWork.DominioRepository.getIdDominio("ESTADOMOVIMENTO", 1);
                    rel = _utils.UpdateDetailsToEntity(rel);
                    _unitOfWork.RelMovimentosPorConciliarMovimentosRepository.Update(rel);
                }
            }

            try
            {
                if (receita != null)
                {
                    _unitOfWork.ComponenteReceitaRegistoRepository.Update(receita);
                }

                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }

        private ComponentereceitaRegisto BuildComponenteReceitaRegistoObject(ComponenteReceitaRegistoDataContract request)
        {
            ComponentereceitaRegisto componenteReceitaRegisto = new ComponentereceitaRegisto
            {
                Id = request.Id,
                ComponenteOrcamentoRegistoFk = request.IdOrcamentoRegistoAprovado,
                TarefaActivoFk = request.TarefaAtivoFK,
                DepartamentoFk = request.DepartamentoFk,
                CentroCustoFk = request.CentroCustoFk,
                TipoContaFk = request.TipoContaFk,
                CodigoContaFk = request.CodigoContaFk,
                CodigoContaDebitoFk = request.CodigoContaDebitoFk,
                AgrupamentoConfigFk = request.AgrupamentoConfigFk,
                InstitutionId = request.InstitutionId,
                Descricao = request.Descricao,
                Valor = request.Valor,
                IndActivo = true,
                ComponentereceitaRegistoMovimentos = new List<ComponentereceitaRegistoMovimentos>(),
            };
            if (componenteReceitaRegisto.Id > 0)
            {
                ComponentereceitaRegisto original = _unitOfWork.ComponenteReceitaRegistoRepository.Get(componenteReceitaRegisto.Id);
                componenteReceitaRegisto.UtilizadorCriacao = original.UtilizadorCriacao;
                componenteReceitaRegisto.DataCriacao = original.DataCriacao;
                componenteReceitaRegisto = _utils.UpdateDetailsToEntity(componenteReceitaRegisto);
            }
            else
                componenteReceitaRegisto = _utils.SetDetailsToEntity(componenteReceitaRegisto);
            return componenteReceitaRegisto;
        }

        public ExecucaoOrcamentalListagemResponse GetExecucaoOrcamental(RelatorioExecucaoOrcamentalListagemRequest request)
        {
            ExecucaoOrcamentalListagemResponse response = new ExecucaoOrcamentalListagemResponse();

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleRelatorios.Relatorios, _unitOfWork);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            try
            {
                response = _unitOfWork.ComponenteReceitaRegistoRepository.GetExecucaoOrcamental(request);
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }

        public ClassificacaoEconomicaExecucaoListagemResponse GetExecucaoOrcamentalPorClassificacaoEconomica(RelatorioClassificacaoEconomicaRequest request)
        {
            ClassificacaoEconomicaExecucaoListagemResponse response = new ClassificacaoEconomicaExecucaoListagemResponse();

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleRelatorios.Relatorios, _unitOfWork);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            try
            {
                response = _unitOfWork.ComponenteReceitaRegistoRepository.GetExecucaoOrcamentalPorClassificacaoEconomica(request);
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }

        public StringFileReponse GetExecucaoOrcamentalExcel(RelatorioExecucaoOrcamentalListagemRequest request)
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

            var data = GetExecucaoOrcamental(request);

            response.File = _unitOfWork.AgrupamentoConfigRepository.ExecucaoOrcamentalExcel(request, data.lista);

            return response;
        }

        public GetDespesasRelatoriosReponse ReceitasRelatorios(SearchFilterRequest request)
        {
            GetDespesasRelatoriosReponse response = new GetDespesasRelatoriosReponse();

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleRelatorios.Consultas, _unitOfWork);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            // validações
            try
            {
                response = _unitOfWork.ComponenteReceitaRegistoRepository.ReceitasRelatorios(request);
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }

        public StringFileReponse ReceitasRelatoriosExcel(SearchFilterRequest request)
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

            var data = ReceitasRelatorios(request);

            response.File = _unitOfWork.ComponenteReceitaRegistoRepository.ReceitasRelatoriosExcel(request, data.Despesas);

            return response;
        }

        public GetReceitasNaoConciliadasRelatoriosReponse ReceitasNaoConciliadasRelatorios(ReceitasNaoConciliadasRelatoriosRequest request)
        {
            GetReceitasNaoConciliadasRelatoriosReponse response = new GetReceitasNaoConciliadasRelatoriosReponse();

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleRelatorios.Consultas, _unitOfWork);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            // validações
            try
            {
                response = _unitOfWork.ComponenteReceitaRegistoRepository.ReceitasNaoConciliadasRelatorios(request);
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }

        public StringFileReponse ReceitasNaoConciliadasRelatoriosExcel(ReceitasNaoConciliadasRelatoriosRequest request)
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

            var data = ReceitasNaoConciliadasRelatorios(request);

            response.File = _unitOfWork.ComponenteReceitaRegistoRepository.ReceitasNaoConciliadasRelatoriosExcel(request, data.Receitas);

            return response;
        }
    }
}