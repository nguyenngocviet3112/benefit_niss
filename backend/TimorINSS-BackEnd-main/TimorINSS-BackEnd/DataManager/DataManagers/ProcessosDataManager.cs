using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.ExcelDocumentService;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;
using TimorINSSBackEnd.Resources;
using static TimorINSSBackEnd.ExcelDocumentService.Models;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class ProcessosDataManager : IProcessosDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public ProcessosDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils, IStringLocalizer<SharedResource> localizer)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
            _localizer = localizer;
        }

        public ProcessosListagemResponse GetAllProcessos(SearchFilterRequest request)
        {
            ProcessosListagemResponse response = new ProcessosListagemResponse();

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.ConfigurarProcessos, _unitOfWork);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            try
            {
                response = _unitOfWork.ProcessoConfigRepository.GetAllProcessos(request);
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }

        public ResponseBaseDataContract SwitchProcessoState(SwitchProcessoStateRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.ConfigurarProcessos, _unitOfWork, CRUD.UPDATE);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            //validações
            Processoconfig processoConfig = _unitOfWork.ProcessoConfigRepository.Get(request.Id);

            if (processoConfig == null)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.EntityDoesNotExist).ToString(), ErrorMessage = ErrorsDataContract.EntityDoesNotExist.ToString() });
                return response;
            }

            processoConfig.IndActivo = !processoConfig.IndActivo;
            processoConfig = _utils.UpdateDetailsToEntity(processoConfig);

            //update in DB
            if (permission)
            {
                try
                {
                    _unitOfWork.ProcessoConfigRepository.Update(processoConfig);
                    _unitOfWork.Commit();
                }
                catch (Exception e)
                {
                    response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                    _unitOfWork.Rollback();
                }
            }
            return response;
        }

        public ResponseBaseDataContract CreateProcessoConfig(ProcessoConfigRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.ConfigurarProcessos, _unitOfWork, CRUD.CREATE);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            if (request.Id.HasValue)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidField).ToString(), ErrorMessage = ErrorsDataContract.InvalidField.ToString() });
                return response;
            }

            //logic in creating insert object
            Processoconfig processoConfig = buildInsertConfigObject(request);

            // Fill relTarefas
            foreach (var tarefa in request.Tarefas)
            {
                var tarefaObj = new Relprocessoconfigtarefa()
                {
                    TarefaFk = tarefa.id,
                    IndActivo = true,
                    TarefaInicial = tarefa.tarefaInicial
                };
                tarefaObj = _utils.SetDetailsToEntity(tarefaObj);

                processoConfig.Relprocessoconfigtarefa.Add(tarefaObj);
            }

            // Fill relPerfis
            foreach (var perfil in request.Perfis)
            {
                var perfilObj = new Relprocessoconfigperfil()
                {
                    PerfilFk = perfil,
                    IndActivo = true
                };
                perfilObj = _utils.SetDetailsToEntity(perfilObj);

                processoConfig.Relprocessoconfigperfil.Add(perfilObj);
            }

            //DB
            try
            {
                _unitOfWork.ProcessoConfigRepository.Add(processoConfig);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                _unitOfWork.Rollback();
            }

            return response;
        }

        private Processoconfig buildInsertConfigObject(ProcessoConfigRequest request)
        {
            Processoconfig obj = new Processoconfig()
            {
                Nome = request.Nome,
                IndActivo = true
            };

            //Set common attributes
            obj = _utils.SetDetailsToEntity(obj);

            return obj;
        }

        public ProcessoConfigListagemResponse GetProcessoConfig(ListProcessoConfiRequest request)
        {
            var response = new ProcessoConfigListagemResponse();

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.ConfigurarProcessos, _unitOfWork, CRUD.READ);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            var processoConfig = _unitOfWork.ProcessoConfigRepository.GetWithActiveRelations(request.Id);

            if (processoConfig == null)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.EntityDoesNotExist).ToString(), ErrorMessage = ErrorsDataContract.EntityDoesNotExist.ToString() });
                return response;
            }

            var tarefasList = processoConfig.Relprocessoconfigtarefa.ToList().OrderByDescending(x => x.TarefaInicial);

            response.Id = processoConfig.Id;
            response.Nome = processoConfig.Nome;
            response.Tarefas = new List<DataContracts.ModelDataContract.RelProcessoConfigTarefaDataContract>();
            response.Perfis = new List<int>();

            foreach (var tarefa in tarefasList)
            {
                response.Tarefas.Add(new DataContracts.ModelDataContract.RelProcessoConfigTarefaDataContract { id = tarefa.TarefaFk, tarefaInicial = tarefa.TarefaInicial });
            }
            foreach (var perfil in processoConfig.Relprocessoconfigperfil)
            {
                response.Perfis.Add(perfil.PerfilFk);
            }

            return response;
        }

        public ResponseBaseDataContract UpdateProcessoConfig(ProcessoConfigRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.ConfigurarProcessos, _unitOfWork, CRUD.UPDATE);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            if (!request.Id.HasValue)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidField).ToString(), ErrorMessage = ErrorsDataContract.InvalidField.ToString() });
                return response;
            }

            //logic in creating insert object
            var processoConfig = _unitOfWork.ProcessoConfigRepository.Get(request.Id.Value);
            processoConfig = _utils.UpdateDetailsToEntity(processoConfig);
            List<int> tarefaIds = request.Tarefas.Select(x => x.id).ToList();

            processoConfig.Nome = request.Nome;

            // Add or Update existing Tarefas
            foreach (var tarefa in request.Tarefas)
            {
                var existingTarefa = processoConfig.Relprocessoconfigtarefa.FirstOrDefault(x => x.TarefaFk == tarefa.id && x.ProcessoConfigFk == request.Id);
                // Existing relation, map new values
                if (existingTarefa != null && existingTarefa.IndActivo)
                {
                    processoConfig.Relprocessoconfigtarefa.FirstOrDefault(x => x.TarefaFk == tarefa.id && x.ProcessoConfigFk == request.Id).TarefaInicial = tarefa.tarefaInicial;
                    _utils.UpdateDetailsToEntity(processoConfig.Relprocessoconfigtarefa.FirstOrDefault(x => x.TarefaFk == tarefa.id && x.ProcessoConfigFk == request.Id));
                }
                //Older relation but inactive
                else if (existingTarefa != null && !existingTarefa.IndActivo)
                {
                    processoConfig.Relprocessoconfigtarefa.FirstOrDefault(x => x.TarefaFk == tarefa.id && x.ProcessoConfigFk == request.Id).IndActivo = true;
                    processoConfig.Relprocessoconfigtarefa.FirstOrDefault(x => x.TarefaFk == tarefa.id && x.ProcessoConfigFk == request.Id).TarefaInicial = tarefa.tarefaInicial;
                    _utils.UpdateDetailsToEntity(processoConfig.Relprocessoconfigtarefa.FirstOrDefault(x => x.TarefaFk == tarefa.id && x.ProcessoConfigFk == request.Id));
                }
                // New relation
                else
                {
                    var tarefaObj = new Relprocessoconfigtarefa()
                    {
                        TarefaFk = tarefa.id,
                        IndActivo = true,
                        TarefaInicial = tarefa.tarefaInicial
                    };
                    tarefaObj = _utils.SetDetailsToEntity(tarefaObj);

                    processoConfig.Relprocessoconfigtarefa.Add(tarefaObj);
                }
            }

            // Add or Update existing Perfis
            foreach (var perfil in request.Perfis)
            {
                var existingPerfil = processoConfig.Relprocessoconfigperfil.FirstOrDefault(x => x.PerfilFk == perfil && x.ProcessoConfigFk == request.Id);

                //Older relation but inactive
                if (existingPerfil != null && !existingPerfil.IndActivo)
                {
                    processoConfig.Relprocessoconfigperfil.FirstOrDefault(x => x.PerfilFk == perfil && x.ProcessoConfigFk == request.Id).IndActivo = true;
                    _utils.UpdateDetailsToEntity(processoConfig.Relprocessoconfigperfil.FirstOrDefault(x => x.PerfilFk == perfil && x.ProcessoConfigFk == request.Id));
                }
                // New relation
                else if (existingPerfil == null)
                {
                    var perfilObj = new Relprocessoconfigperfil()
                    {
                        PerfilFk = perfil,
                        IndActivo = true
                    };
                    perfilObj = _utils.SetDetailsToEntity(perfilObj);

                    processoConfig.Relprocessoconfigperfil.Add(perfilObj);
                }
            }

            //Tarefas and Perfis to Delete
            var deletedTarefas = processoConfig.Relprocessoconfigtarefa.Where(x => !tarefaIds.Contains(x.TarefaFk) && x.IndActivo).ToList();
            var deletedPerfis = processoConfig.Relprocessoconfigperfil.Where(x => !request.Perfis.Contains(x.PerfilFk) && x.IndActivo).ToList();

            //DB
            try
            {
                _unitOfWork.ProcessoConfigRepository.Update(processoConfig);
                if (deletedTarefas.Count > 0)
                {
                    foreach (var tarefa in deletedTarefas)
                    {
                        tarefa.IndActivo = false;
                        _utils.UpdateDetailsToEntity(tarefa);
                        _unitOfWork.RelProcessoConfigTarefaRepository.Update(tarefa);
                    }
                }
                if (deletedPerfis.Count > 0)
                {
                    foreach (var perfil in deletedPerfis)
                    {
                        perfil.IndActivo = false;
                        _utils.UpdateDetailsToEntity(perfil);
                        _unitOfWork.RelProcessoConfigPerfilRepository.Update(perfil);
                    }
                }
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                _unitOfWork.Rollback();
            }

            return response;
        }

        public SelectDescriptionResponse ListIniciarProcessos(RequestBaseDataContract request)
        {
            SelectDescriptionResponse response = new SelectDescriptionResponse();

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.ConfigurarProcessos, _unitOfWork);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            try
            {
                var perfilIds = _unitOfWork.RelUtilizadorPerfilRepository.GetPerfisIdsByUtilizador((int)request.UserId);
                var allowedProcessConfigs = _unitOfWork.RelProcessoConfigPerfilRepository.GetProcessoIdsByPerfis(perfilIds);
                var isAdmin = request.UserId == _unitOfWork.UtilizadoresRepository.GetAdminUser().IdUtilizador;
                response = _unitOfWork.ProcessoConfigRepository.ListIniciarProcessos(allowedProcessConfigs, isAdmin);
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }

        public SelectDescriptionResponse ListIniciarProcessApprove(RequestBaseDataContract request)
        {
            SelectDescriptionResponse response = new SelectDescriptionResponse();

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.ConfigurarProcessos, _unitOfWork);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            try
            {
                var perfilIds = _unitOfWork.RelUtilizadorPerfilRepository.GetPerfisIdsByUtilizador((int)request.UserId);
                var allowedProcessConfigs = _unitOfWork.RelProcessoConfigPerfilRepository.GetProcessoIdsByPerfis(perfilIds);
                var isAdmin = request.UserId == _unitOfWork.UtilizadoresRepository.GetAdminUser().IdUtilizador;
                response = _unitOfWork.ProcessoConfigRepository.ListIniciarApprove(allowedProcessConfigs, isAdmin);
                
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }

        public ProcessosArquivadosListagemResponse GetAllProcessosArquivados(SearchFilterRequest request)
        {
            ProcessosArquivadosListagemResponse response = new ProcessosArquivadosListagemResponse();

            try
            {
                response = _unitOfWork.ProcessoAtivoRepository.GetAllProcessosArquivados(request);
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }

        public RelatorioProcessosListagemResponse GetProcessosRelatorios(RelatorioProcessosListagemRequest request)
        {
            RelatorioProcessosListagemResponse response = new RelatorioProcessosListagemResponse();

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleRelatorios.Consultas, _unitOfWork);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            try
            {
                response = _unitOfWork.ProcessoAtivoRepository.GetProcessosRelatorios(request);
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }

        public ProcessoDataResponse GetProcessoData(GetProcessoDataRequest request)
        {
            ProcessoDataResponse response = new ProcessoDataResponse();

            try
            {
                var processo = _unitOfWork.ProcessoAtivoRepository.Get(request.processoId);
                var utilizador = !processo.UtilizadorAlteracao.HasValue ? null : _unitOfWork.UtilizadoresRepository.Get(processo.UtilizadorAlteracao.Value);

                response.data = new ProcessoDetalhe()
                {
                    data = processo.DataAlteracao.Value,
                    arquivado = processo.Arquivado,
                    nome = processo.ProcessoConfigFkNavigation.Nome,
                    quantidadeTarefas = processo.Tarefaativo.Count,
                    nomeUtilizadorUltimaEdicao = utilizador?.Username,
                };
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }

        public ResponseBaseDataContract StartProcess(StartProcessRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            var perfilIds = _unitOfWork.RelUtilizadorPerfilRepository.GetPerfisIdsByUtilizador((int)request.UserId);
            var allowedProcessConfigs = _unitOfWork.RelProcessoConfigPerfilRepository.GetProcessoIdsByPerfis(perfilIds);

            if (!allowedProcessConfigs.Contains(request.Id))
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }
            int numeroProcessoSequencial = 0;
            var lastProcessoAtivo = _unitOfWork.ProcessoAtivoRepository.GetLastDto();

            if (DateTime.Now.Year == lastProcessoAtivo?.DataCriacao.Year)
            {
                numeroProcessoSequencial += lastProcessoAtivo.Id;
            }
            numeroProcessoSequencial += 1;

            //logic in creating main insert object
            Processoativo processoativo = new Processoativo
            {
                ProcessoConfigFk = request.Id,
                Arquivado = false,
                IndActivo = true,
                NumeroProcesso = String.Format("{0:D5}", numeroProcessoSequencial) + '/' + DateTime.Now.Year.ToString()
            };
            _utils.SetDetailsToEntity(processoativo);

            //logic in creating the rest of the relating objects
            var relTarefaInicial = _unitOfWork.RelProcessoConfigTarefaRepository.GetRelTarefaByProcesso(request.Id).FirstOrDefault(x => x.TarefaInicial && x.IndActivo);

            if (relTarefaInicial == null)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidConfiguration).ToString(), ErrorMessage = ErrorsDataContract.InvalidConfiguration.ToString() });
                return response;
            }

            Tarefaativo tarefaInicial = new Tarefaativo
            {
                TarefaconfigFk = relTarefaInicial.TarefaFk,
                IndActivo = true
            };
            _utils.SetDetailsToEntity(tarefaInicial);

            //Add relation to main object for entity framework to handle the rest
            processoativo.Tarefaativo.Add(tarefaInicial);

            //DB
            try
            {
                _unitOfWork.ProcessoAtivoRepository.Add(processoativo);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                _unitOfWork.Rollback();
            }

            return response;
        }

        public ComponenteHistoricoTextoListagemResponse GetHistoricoTexto(GetHistoricoTextoProcessoRequest request)
        {
            ComponenteHistoricoTextoListagemResponse response = new ComponenteHistoricoTextoListagemResponse();

            var processo = _unitOfWork.ProcessoAtivoRepository.Get(request.processoId);

            if (processo == null)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.EntityDoesNotExist).ToString(), ErrorMessage = ErrorsDataContract.EntityDoesNotExist.ToString() });
                return response;
            }

            var tarefasIdsDoProcesso = _unitOfWork.TarefaAtivoRepository.GetAllTarefasIdsByProcessoAtivoId(processo.Id);

            response.historicoTextos = _unitOfWork.ComponenteTextoRegistoRepository.GetHistoryTextsByTarefaAtivoIds(tarefasIdsDoProcesso);

            return response;
        }

        public GetTipoProcessosRelatoriosResponse GetTipoProcessosRelatorios(RequestBaseDataContract request)
        {
            GetTipoProcessosRelatoriosResponse response = new GetTipoProcessosRelatoriosResponse();

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleRelatorios.Consultas, _unitOfWork);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            try
            {
                response = _unitOfWork.ProcessoAtivoRepository.GetTipoProcessosRelatorios();
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }

        public StringFileReponse ExtractToExcelRelatorios(RelatorioProcessosListagemRequest request)
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
            request.filter.rows = 99999;

            var processos = _unitOfWork.ProcessoAtivoRepository.GetProcessosRelatorios(request);

            // Inicialização do documento excel
            var excelDocument = new ExcelDocument(_localizer["processos"].Value, new ExcelDocumentOptions()
            {
                TextStyles = Extensions.ServiceExtensions.ExcelDocumentTextStyles
            });

            // Adição dos títulos
            excelDocument.Pages[0].AddText(_localizer["consultaProcessos"].Value, new ExcelDocumentTextPosition(1, 1), "Header", new ExcelDocumentTextPosition(4, 1));

            var date = $"{request.filter.dateFilterBegin.Value.ToString("dd/MM/yyyy")} {_localizer["a"].Value} {request.filter.dateFilterEnd.Value.ToString("dd/MM/yyyy")}";

            excelDocument.Pages[0].AddText(date, new ExcelDocumentTextPosition(1, 2), "Header", new ExcelDocumentTextPosition(4, 2));

            // Adição da tabela
            excelDocument.Pages[0].AddTable(new ExcelDocumentTextPosition(1, 4), processos.processos, new List<ColumnOption<RelatorioProcessoDataContract>>()
            {
                new ColumnOption<RelatorioProcessoDataContract>()
                {
                    Name = _localizer["tipoProcesso"].Value,
                    ColumnTextStyleKey = "TableColumn",
                    DataTextStyleKey = "TableCell",
                    Value = (data) => data.tipo
                },
                new ColumnOption<RelatorioProcessoDataContract>()
                {
                    Name = _localizer["estado"].Value,
                    ColumnTextStyleKey = "TableColumn",
                    DataTextStyleKey = "TableCell",
                    Value = (data) => data.arquivado ? _localizer["arquivado"].Value : _localizer["emTramitacao"].Value
                },
                new ColumnOption<RelatorioProcessoDataContract>()
                {
                    Name = _localizer["ultimaTarefaPeriodo"].Value,
                    ColumnTextStyleKey = "TableColumn",
                    DataTextStyleKey = "TableCell",
                    Value = (data) => data.ultimaTarefa.nome
                },
                new ColumnOption<RelatorioProcessoDataContract>()
                {
                    Name = _localizer["perfisResponsaveisFinalPeriodo"].Value,
                    ColumnTextStyleKey = "TableColumn",
                    DataTextStyleKey = "TableCell",
                    Value = (data) => string.Join(", ", data.perfis)
                }
            });

            // Conversão do documento excel em base64
            response.File = excelDocument.GetFileString();
            return response;
        }
    }
}