using System;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class TarefaDataManager : ITarefaDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public TarefaDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        public TarefaListagemResponse GetAllTarefas(SearchFilterRequest request)
        {
            TarefaListagemResponse response = new TarefaListagemResponse();

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.ConfigurarTarefas, _unitOfWork, CRUD.READ);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            try
            {
                response = _unitOfWork.TarefaRepository.GetAllTarefas(request);
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }

        public SelectDescriptionResponse GetAllTarefaAtivo()
        {
            SelectDescriptionResponse response = new SelectDescriptionResponse();
            try
            {
                List<SelectDescription> selects = _unitOfWork.TarefaRepository.GetAllTarefaAtivo().OrderBy(x => x.nome).ToList();
                response.selects = selects;
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }

        public ResponseBaseDataContract AddTarefaConfigurada(ConfigurarTarefaRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.ConfigurarTarefas, _unitOfWork, CRUD.CREATE);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            //validações

            Tarefa tarefa = BuildTarefaObject(request);

            Componentetexto componenteTexto = new Componentetexto();

            if (request.componenteTexto != null && request.componenteTexto.texto1)
            {
                componenteTexto = BuildComponenteTextoObject(request.componenteTexto);
            }

            List<Componenteaccoestarefa> componenteAccoesTarefa = new List<Componenteaccoestarefa>();

            if (request.componenteAccoesTarefa != null && request.componenteAccoesTarefa.Count > 0)
            {
                foreach (var accoesTarefa in request.componenteAccoesTarefa)
                {
                    componenteAccoesTarefa.Add(BuildComponenteAccoesTarefaObject(accoesTarefa));
                }
            }

            List<Componenteclassificacaosubclassific> componenteClassificacaoSubClassific = new List<Componenteclassificacaosubclassific>();

            if (request.componenteClassificacaoSubClassific != null && request.componenteClassificacaoSubClassific.Count > 0)
            {
                foreach (var classificacaoTarefa in request.componenteClassificacaoSubClassific)
                {
                    componenteClassificacaoSubClassific.Add(BuildComponenteClassificacaoTarefaObject(classificacaoTarefa));
                }
            }

            List<Componentecontroleacesso> componentePerfilUtilizadorTarefa = new List<Componentecontroleacesso>();

            if (request.componenteControleAcessoPerfil != null && request.componenteControleAcessoPerfil.Count > 0)
            {
                foreach (var perfil in request.componenteControleAcessoPerfil)
                {
                    componentePerfilUtilizadorTarefa.Add(BuildControleAcessoPerfil(perfil));
                }
            }

            if (request.componenteControleAcessoUtilizador != null && request.componenteControleAcessoUtilizador.Count > 0)
            {
                foreach (var utilizador in request.componenteControleAcessoUtilizador)
                {
                    componentePerfilUtilizadorTarefa.Add(BuildControleAcessoUtilizador(utilizador));
                }
            }

            List<Componentecarregardocumento> componenteDocumentoTarefa = new List<Componentecarregardocumento>();

            if (request.componenteCarregarDocumentos != null && request.componenteCarregarDocumentos.Count > 0)
            {
                foreach (var documento in request.componenteCarregarDocumentos)
                {
                    componenteDocumentoTarefa.Add(BuildComponenteCarregarDocumentosTarefaObject(documento));
                }
            }

            List<Reltarefacomponente> listaComponente = new List<Reltarefacomponente>();
            if (request.listaComponente != null && request.listaComponente.Count > 0)
            {
                for (int i = 0; i < request.listaComponente.Count; i++)
                {
                    if (request.listaComponente[i].select)
                    {
                        listaComponente.Add(BuildRelTarefaComponenteObject(request.listaComponente[i]));
                    }
                }
            }

            try
            {
                _unitOfWork.TarefaRepository.Add(tarefa);

                if (componenteTexto != null && request.componenteTexto.texto1)
                {
                    componenteTexto.TarefaFkNavigation = tarefa;
                    _unitOfWork.ComponenteTextoRepository.Add(componenteTexto);
                }

                if (componenteAccoesTarefa != null && componenteAccoesTarefa.Count > 0)
                {
                    foreach (var accoesTarefa in componenteAccoesTarefa)
                    {
                        accoesTarefa.TarefaFkNavigation = tarefa;
                        _unitOfWork.ComponenteAccoesTarefaRepository.Add(accoesTarefa);
                    }
                }

                if (componenteClassificacaoSubClassific != null && componenteClassificacaoSubClassific.Count > 0)
                {
                    foreach (var classificacaoTarefa in componenteClassificacaoSubClassific)
                    {
                        classificacaoTarefa.TarefaFkNavigation = tarefa;
                        _unitOfWork.ComponenteClassificacaoSubClassificRepository.Add(classificacaoTarefa);
                    }
                }

                if (componentePerfilUtilizadorTarefa != null && componentePerfilUtilizadorTarefa.Count > 0)
                {
                    foreach (var controleAcesso in componentePerfilUtilizadorTarefa)
                    {
                        controleAcesso.TarefaFkNavigation = tarefa;
                        _unitOfWork.ComponenteControloAcessoRepository.Add(controleAcesso);
                    }
                }

                if (componenteDocumentoTarefa != null && componenteDocumentoTarefa.Count > 0)
                {
                    foreach (var documentoTarefa in componenteDocumentoTarefa)
                    {
                        documentoTarefa.TarefaFkNavigation = tarefa;
                        _unitOfWork.ComponenteCarregarDocumentoRepository.Add(documentoTarefa);
                    }
                }

                if (listaComponente != null && listaComponente.Count > 0)
                {
                    for (int i = 0; i < listaComponente.Count; i++)
                    {
                        listaComponente[i].TarefaFkNavigation = tarefa;
                        listaComponente[i].Ordem = i + 1;
                        _unitOfWork.RelTarefaComponenteRepository.Add(listaComponente[i]);
                    }
                }

                if (request.componenteOrcamento != null)
                {
                    Componenteorcamento componenteOrcamento = BuildComponenteOrcamentoObject(request.componenteOrcamento);
                    componenteOrcamento.TarefaFkNavigation = tarefa;
                    _unitOfWork.ComponenteOrcamentoRepository.Add(componenteOrcamento);
                }

                if (request.componenteDespesa != null)
                {
                    Componentedespesa componenteDespesa = BuildComponenteDespesaObject(request.componenteDespesa);
                    componenteDespesa.TarefaFkNavigation = tarefa;
                    _unitOfWork.ComponenteDespesaRepository.Add(componenteDespesa);
                }

                if (request.componenteConciliacaoMovimentos != null)
                {
                    Componenteconciliacaomovimentos componenteConciliacaomovimentos = BuildComponenteconciliacaomovimentosObject(request.componenteConciliacaoMovimentos);
                    componenteConciliacaomovimentos.TarefaFkNavigation = tarefa;
                    _unitOfWork.ComponenteConciliacaoMovimentosRepository.Add(componenteConciliacaomovimentos);
                }

                if (request.componenteReceita != null)
                {
                    Componentereceita componenteReceita = BuildComponenteReceitaObject(request.componenteReceita);
                    componenteReceita.TarefaFkNavigation = tarefa;
                    _unitOfWork.ComponenteReceitaRepository.Add(componenteReceita);
                }

                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
                _unitOfWork.Rollback();
            }

            return response;
        }

        public ComponenteTarefaConfiguradaRespose GetAllComponentesByIdTarefa(int id, RequestBaseDataContract request)
        {
            ComponenteTarefaConfiguradaRespose response = new ComponenteTarefaConfiguradaRespose();

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.ConfigurarTarefas, _unitOfWork, CRUD.UPDATE);
            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            //validações
            response.listaComponente = new List<Componentes>();
            response.componenteAccoesTarefa = new List<ComponenteAccoesTarefa>();
            response.componenteClassificacaoSubClassific = new List<ComponenteClassificacaoSubClassificTarefa>();
            response.componenteControleAcessoPerfil = new List<ComponenteAcessoPerfil>();
            response.componenteControleAcessoUtilizador = new List<ComponenteAcessoUtilizador>();
            response.componenteCarregarDocumentos = new List<ComponenteDocumentoTarefa>();

            Tarefa tarefa = new Tarefa();
            ComponentesListagemResponse listaAllComponents = new ComponentesListagemResponse();
            List<Componentes> listaComponenteTarefa = new List<Componentes>();
            List<ComponenteAccoesTarefa> listaComponenteAccoesTarefa = new List<ComponenteAccoesTarefa>();
            List<ComponenteClassificacaoSubClassificTarefa> listaComponenteClassificacaoSubClassific = new List<ComponenteClassificacaoSubClassificTarefa>();
            List<ComponenteAcessoPerfil> listaComponenteControleAcessoPerfil = new List<ComponenteAcessoPerfil>();
            ComponenteAcessoPerfil componenteControleAcessoPerfil = new ComponenteAcessoPerfil();
            List<ComponenteAcessoUtilizador> listaComponenteControleAcessoUtilizador = new List<ComponenteAcessoUtilizador>();
            ComponenteAcessoUtilizador componenteControleAcessoUtilizador = new ComponenteAcessoUtilizador();
            List<Componentecontroleacesso> listaControloAcesso = new List<Componentecontroleacesso>();
            List<ComponenteDocumentoTarefa> listaComponenteCarregarDocumentos = new List<ComponenteDocumentoTarefa>();
            Utilizador utilizador = new Utilizador();

            try
            {
                listaAllComponents = _unitOfWork.ComponenteRepository.GetAllComponentes();
                listaComponenteTarefa = _unitOfWork.RelTarefaComponenteRepository.GetRelTarefaComponenteByIdTarefa(id);

                if (listaAllComponents != null && listaAllComponents.componentes != null && listaAllComponents.componentes.Count > 0)
                {
                    if (listaComponenteTarefa != null && listaComponenteTarefa.Count > 0)
                    {
                        foreach (var componenteTarefa in listaComponenteTarefa)
                        {
                            foreach (var componente in listaAllComponents.componentes)
                            {
                                if (componenteTarefa.id == componente.id)
                                {
                                    componente.expandir = componenteTarefa.expandir;
                                    componente.select = true;

                                    if (componenteTarefa.ordem > 0)
                                    {
                                        componente.ordem = componenteTarefa.ordem;
                                    }
                                }
                            }
                        }
                    }

                    foreach (var componente in listaAllComponents.componentes)
                    {
                        if (componente.ordem == null || componente.ordem == 0)
                        {
                            componente.ordem = 100;
                        }
                    }

                    response.listaComponente = listaAllComponents.componentes.OrderBy(o => o.ordem).ToList();
                }

                tarefa = _unitOfWork.TarefaRepository.Get(id);

                if (tarefa != null)
                {
                    response.listaTexto = (bool)tarefa.HistoricoTexto;
                    response.listaDocumento = (bool)tarefa.HistoricoDocumento;
                    response.componenteBotaoArquivar = tarefa.BotaoArquivar;
                    response.componenteCabecalhoProcesso = (bool)tarefa.CabecalhoProcesso;
                    response.nomeTarefa = tarefa.Nome;
                    response.prazoTarefa = (int)tarefa.PrazoTarefa;
                }

                Componentetexto? componenteTexto = _unitOfWork.ComponenteTextoRepository.GetComponenteByIdTarefa(id);
                if (componenteTexto != null)
                {
                    response.componenteTexto = BuildComponenteTextoDataContract(componenteTexto);
                }

                listaComponenteAccoesTarefa = _unitOfWork.ComponenteAccoesTarefaRepository.GetByIdTarefa(id);
                if (listaComponenteAccoesTarefa != null && listaComponenteAccoesTarefa.Count > 0)
                {
                    foreach (var componenteAccaoTarefa in listaComponenteAccoesTarefa)
                    {
                        response.componenteAccoesTarefa.Add(componenteAccaoTarefa);
                    }
                }

                listaComponenteClassificacaoSubClassific = _unitOfWork.ComponenteClassificacaoSubClassificRepository.GetByIdTarefa(id);
                if (listaComponenteClassificacaoSubClassific != null && listaComponenteClassificacaoSubClassific.Count > 0)
                {
                    foreach (var componenteClassificacaoSubClassific in listaComponenteClassificacaoSubClassific)
                    {
                        response.componenteClassificacaoSubClassific.Add(componenteClassificacaoSubClassific);
                    }
                }

                listaControloAcesso = _unitOfWork.ComponenteControloAcessoRepository.GetByIdTarefa(id);
                if (listaControloAcesso != null && listaControloAcesso.Count > 0)
                {
                    foreach (var componenteControloAcesso in listaControloAcesso)
                    {
                        if (componenteControloAcesso.PerfilFk != null)
                        {
                            componenteControleAcessoPerfil = new ComponenteAcessoPerfil();
                            componenteControleAcessoPerfil.id = (int)componenteControloAcesso.Id;
                            componenteControleAcessoPerfil.idPerfil = (int)componenteControloAcesso.PerfilFk;
                            componenteControleAcessoPerfil.nomePerfil = componenteControloAcesso.PerfilFkNavigation.Descricao;
                            response.componenteControleAcessoPerfil.Add(componenteControleAcessoPerfil);
                        }

                        if (componenteControloAcesso.UtilizadorFk != null)
                        {
                            componenteControleAcessoUtilizador = new ComponenteAcessoUtilizador();
                            componenteControleAcessoUtilizador.id = (int)componenteControloAcesso.Id;
                            componenteControleAcessoUtilizador.idUtilizador = (int)componenteControloAcesso.UtilizadorFk;

                            utilizador = _unitOfWork.UtilizadoresRepository.Get((int)componenteControloAcesso.UtilizadorFk);
                            if (utilizador != null)
                            {
                                componenteControleAcessoUtilizador.nomeUtilizador = utilizador.TrabalhadorFkNavigation.Nome;
                            }
                            response.componenteControleAcessoUtilizador.Add(componenteControleAcessoUtilizador);
                        }
                    }
                }

                listaComponenteCarregarDocumentos = _unitOfWork.ComponenteCarregarDocumentoRepository.GetByIdTarefa(id);
                if (listaComponenteCarregarDocumentos != null && listaComponenteCarregarDocumentos.Count > 0)
                {
                    foreach (var componenteDocumentoTarefa in listaComponenteCarregarDocumentos)
                    {
                        response.componenteCarregarDocumentos.Add(componenteDocumentoTarefa);
                    }
                }

                Componenteorcamento? componenteOrcamento = _unitOfWork.ComponenteOrcamentoRepository.GetByIdTarefa(id);
                if (componenteOrcamento != null)
                {
                    response.ComponenteOrcamento = BuildComponenteOrcamentoDataContract(componenteOrcamento);
                }

                Componentedespesa? componenteDespesa = _unitOfWork.ComponenteDespesaRepository.GetByIdTarefa(id);
                if (componenteDespesa != null)
                {
                    response.componenteDespesa = BuildComponenteDespesaDataContract(componenteDespesa);
                }

                Componenteconciliacaomovimentos? componenteconciliacaomovimentos = _unitOfWork.ComponenteConciliacaoMovimentosRepository.GetByIdTarefa(id);
                if (componenteconciliacaomovimentos != null)
                {
                    response.componenteConciliacaoMovimentos = BuildComponenteConciliacaoMovimentosDataContract(componenteconciliacaomovimentos);
                }

                Componentereceita? componenteReceita = _unitOfWork.ComponenteReceitaRepository.GetByIdTarefa(id);
                if (componenteReceita != null)
                {
                    response.componenteReceita = BuildComponenteReceitaDataContract(componenteReceita);
                }
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }

        public ResponseBaseDataContract EditarTarefa(TarefaRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };
            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.ConfigurarTarefas, _unitOfWork, CRUD.UPDATE);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            //validações
            Tarefa tarefa = new Tarefa();
            tarefa = _unitOfWork.TarefaRepository.Get(request.idTarefa);

            try
            {
                if (tarefa != null && tarefa.Id > 0)
                {
                    tarefa.Nome = request.nomeTarefa;
                    tarefa.CabecalhoProcesso = request.componenteCabecalhoProcesso;
                    tarefa.HistoricoTexto = request.listaTexto;
                    tarefa.HistoricoDocumento = request.listaDocumento;
                    tarefa.BotaoArquivar = request.componenteBotaoArquivar;
                    tarefa = _utils.UpdateDetailsToEntity(tarefa);
                    _unitOfWork.TarefaRepository.Update(tarefa);
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

        private Tarefa BuildTarefaObject(ConfigurarTarefaRequest request)
        {
            Tarefa tarefa = new Tarefa
            {
                Nome = request.nomeTarefa,
                PrazoTarefa = request.prazoTarefa,
                HistoricoDocumento = request.listaDocumento,
                HistoricoTexto = request.listaTexto,
                CabecalhoProcesso = request.componenteCabecalhoProcesso,
                BotaoArquivar = request.componenteBotaoArquivar,
                NumeroTarefa = _unitOfWork.TarefaRepository.GetNextNumeroTarefa(),
                IndActivo = true,
            };
            tarefa = _utils.SetDetailsToEntity(tarefa);
            return tarefa;
        }

        private Componentetexto BuildComponenteTextoObject(ComponenteTexto request)
        {
            Componentetexto componenteTexto = new Componentetexto
            {
                Expandir1 = request.expandir1,
                Expandir2 = request.expandir2,
                Texto1 = request.texto1,
                Texto2 = request.texto2,
                Titulo1 = request.titulo1,
                Titulo2 = request.titulo2,
                QuantCaracteres1 = request.quantCaracteres1,
                QuantCaracteres2 = request.quantCaracteres2,
                Obrigatorio1 = request.obrigatorio1,
                Obrigatorio2 = request.obrigatorio2,
                ObrigatorioAoArquivar1 = request.obrigatorioArquivar1,
                ObrigatorioAoArquivar2 = request.obrigatorioArquivar2,
                IndActivo = true,
            };
            componenteTexto = _utils.SetDetailsToEntity(componenteTexto);
            return componenteTexto;
        }

        private ComponenteTexto BuildComponenteTextoDataContract(Componentetexto componente)
        {
            return new ComponenteTexto()
            {
                id = componente.Id,
                expandir1 = (bool)componente.Expandir1,
                expandir2 = (bool)componente.Expandir2,
                texto1 = (bool)componente.Texto1,
                texto2 = (bool)componente.Texto2,
                titulo1 = componente.Titulo1,
                titulo2 = componente.Titulo2,
                quantCaracteres1 = (int)componente.QuantCaracteres1,
                quantCaracteres2 = (int)componente.QuantCaracteres2,
                obrigatorio1 = (bool)componente.Obrigatorio1,
                obrigatorio2 = (bool)componente.Obrigatorio2,
                obrigatorioArquivar1 = (bool)componente.ObrigatorioAoArquivar1,
                obrigatorioArquivar2 = (bool)componente.ObrigatorioAoArquivar2,
            };
        }

        private Componenteaccoestarefa BuildComponenteAccoesTarefaObject(ComponenteAccoesTarefa request)
        {
            Componenteaccoestarefa componenteAccoesTarefa = new Componenteaccoestarefa
            {
                TarefaSeguirFk = request.idTarefa,
                ApelidoDaTarefa = request.apelidoTarefa,
                IndActivo = true,
            };
            componenteAccoesTarefa = _utils.SetDetailsToEntity(componenteAccoesTarefa);
            return componenteAccoesTarefa;
        }

        private Componenteclassificacaosubclassific BuildComponenteClassificacaoTarefaObject(ComponenteClassificacaoSubClassificTarefa request)
        {
            Componenteclassificacaosubclassific componenteClassificacaoTarefa = new Componenteclassificacaosubclassific
            {
                SubclassificacaoFk = request.idSubClassificacao,
                IndActivo = true,
            };
            componenteClassificacaoTarefa = _utils.SetDetailsToEntity(componenteClassificacaoTarefa);
            return componenteClassificacaoTarefa;
        }

        private Componentecontroleacesso BuildControleAcessoPerfil(ComponenteAcessoPerfil request)
        {
            Componentecontroleacesso newEntity = new Componentecontroleacesso
            {
                PerfilFk = request.idPerfil,
                IndActivo = true,
            };
            newEntity = _utils.SetDetailsToEntity(newEntity);
            return newEntity;
        }

        private Componentecontroleacesso BuildControleAcessoUtilizador(ComponenteAcessoUtilizador request)
        {
            Componentecontroleacesso newEntity = new Componentecontroleacesso
            {
                UtilizadorFk = request.idUtilizador,
                IndActivo = true,
            };
            newEntity = _utils.SetDetailsToEntity(newEntity);
            return newEntity;
        }

        private Componentecarregardocumento BuildComponenteCarregarDocumentosTarefaObject(ComponenteDocumentoTarefa request)
        {
            Componentecarregardocumento componenteCarregarDocumento = new Componentecarregardocumento
            {
                DocumentoFk = request.idDocumento,
                Obrigatorio = request.obrigatorio,
                IndActivo = true,
                DataCriacao = DateTime.Now
            };
            componenteCarregarDocumento = _utils.SetDetailsToEntity(componenteCarregarDocumento);
            return componenteCarregarDocumento;
        }

        private Reltarefacomponente BuildRelTarefaComponenteObject(Componentes request)
        {
            Reltarefacomponente tarefaComponente = new Reltarefacomponente
            {
                ComponenteFk = request.id,
                IndActivo = true,
                Expandir = request.expandir,
                DataCriacao = DateTime.Now
            };
            tarefaComponente = _utils.SetDetailsToEntity(tarefaComponente);
            return tarefaComponente;
        }

        private ComponenteOrcamentoDataContract BuildComponenteOrcamentoDataContract(Componenteorcamento componenteOrcamento)
        {
            return new ComponenteOrcamentoDataContract()
            {
                Id = componenteOrcamento.Id,
                PermissaoAprovacao = componenteOrcamento.PermissaoAprovacao,
                PermissaoDatas = componenteOrcamento.PermissaoDatas,
                PermissaoDetalhes = componenteOrcamento.PermissaoDetalhes,
                PermissaoInsercoes = componenteOrcamento.PermissaoInsercoes,
                TarefaFk = componenteOrcamento.TarefaFk
            };
        }

        private Componenteorcamento BuildComponenteOrcamentoObject(ComponenteOrcamentoDataContract componente)
        {
            ComponenteOrcamentoDto componenteDto = new ComponenteOrcamentoDto
            {
                Id = componente.Id,
                TarefaFk = componente.TarefaFk,
                PermissaoAprovacao = componente.PermissaoAprovacao,
                PermissaoDatas = componente.PermissaoDatas,
                PermissaoDetalhes = componente.PermissaoDetalhes,
                PermissaoInsercoes = componente.PermissaoInsercoes,
                IndActivo = true,
            };

            if (componenteDto.Id > 0)
            {
                Componenteorcamento original = _unitOfWork.ComponenteOrcamentoRepository.Get(componenteDto.Id);
                componenteDto.UtilizadorCriacao = original.UtilizadorCriacao;
                componenteDto.DataCriacao = original.DataCriacao;
                componenteDto = _utils.UpdateDetailsToEntity(componenteDto);
            }
            else
                componenteDto = _utils.SetDetailsToEntity(componenteDto);

            return Utils.MappClassFromDto<ComponenteOrcamentoDto, Componenteorcamento>(componenteDto);
        }

        private ComponenteDespesaDataContract BuildComponenteDespesaDataContract(Componentedespesa componenteDespesa)
        {
            return new ComponenteDespesaDataContract()
            {
                id = componenteDespesa.Id,
                tarefaFk = componenteDespesa.TarefaFk,
                registarDespesa = componenteDespesa.RegistarDespesa,
                visualizarDespesaRParaA = componenteDespesa.VisualizarDespesaRparaA,
                visualizarDespesaAParaC = componenteDespesa.VisualizarDespesaAparaC,
                visualizarDespesaR = componenteDespesa.VisualizarDespesaR,
                visualizarDespesaA = componenteDespesa.VisualizarDespesaA,
                visualizarDespesaComCompromisso = componenteDespesa.VisualizarDespesaComCompromisso,
                visualiazarDespesaC = componenteDespesa.VisualiazarDespesaC,
                executarPagamentos = componenteDespesa.ExecutarPagamentos,
                visualizarExecucaoDespesaCabimentada = componenteDespesa.VisualizarExecucaoDespesaCabimentada,
                emitirOrdemPagamento = componenteDespesa.EmitirOrdemPagamento
            };
        }

        private ComponenteConciliacaoMovimentosDataContract BuildComponenteConciliacaoMovimentosDataContract(Componenteconciliacaomovimentos componenteconciliacaomovimentos)
        {
            return new ComponenteConciliacaoMovimentosDataContract()
            {
                id = componenteconciliacaomovimentos.Id,
                tarefaFk = componenteconciliacaomovimentos.TarefaFk,
                permissaoSelecionarMovimentos = componenteconciliacaomovimentos.PermissaoSelecionarMovimentos,
                permissaoMovimentosConciliar = componenteconciliacaomovimentos.PermissaoMovimentosConciliar,
                permissaoMovimentosBancarios = componenteconciliacaomovimentos.PermissaoMovimentosBancarios,
                permissaoVerMovimentosAconciliar = componenteconciliacaomovimentos.PermissaoVerMovimentosAconciliar,
                permissaoConciliar = componenteconciliacaomovimentos.PermissaoConciliar,
                permissaoDesfazerConciliar = componenteconciliacaomovimentos.PermissaoDesfazerConciliar
            };
        }

        private ComponenteReceitaDataContract BuildComponenteReceitaDataContract(Componentereceita componenteReceita)
        {
            return new ComponenteReceitaDataContract()
            {
                id = componenteReceita.Id,
                tarefaFk = componenteReceita.TarefaFk,
                classificarMovSelecionados = componenteReceita.ClassificarMovSelecionados,
                selecionarMovRecebidosParaRegisto = componenteReceita.SelecionarMovRecebidosParaRegisto,
                verificarExecucaoOrcamentoEditarSelecao = componenteReceita.VerificarExecucaoOrcamentoEditarSelecao,
            };
        }

        private Componentedespesa BuildComponenteDespesaObject(ComponenteDespesaDataContract componente)
        {
            ComponenteDespesaDto componenteDto = new ComponenteDespesaDto
            {
                Id = componente.id,
                RegistarDespesa = componente.registarDespesa,
                VisualizarDespesaRparaA = componente.visualizarDespesaRParaA,
                VisualizarDespesaAparaC = componente.visualizarDespesaAParaC,
                VisualizarDespesaR = componente.visualizarDespesaR,
                VisualizarDespesaA = componente.visualizarDespesaA,
                VisualizarDespesaComCompromisso = componente.visualizarDespesaComCompromisso,
                VisualiazarDespesaC = componente.visualiazarDespesaC,
                ExecutarPagamentos = componente.executarPagamentos,
                VisualizarExecucaoDespesaCabimentada = componente.visualizarExecucaoDespesaCabimentada,
                EmitirOrdemPagamento = componente.emitirOrdemPagamento,
                IndActivo = true,
            };

            if (componenteDto.Id > 0)
            {
                Componentedespesa original = _unitOfWork.ComponenteDespesaRepository.Get(componenteDto.Id);
                componenteDto.UtilizadorCriacao = original.UtilizadorCriacao;
                componenteDto.DataCriacao = original.DataCriacao;
                componenteDto = _utils.UpdateDetailsToEntity(componenteDto);
            }
            else
                componenteDto = _utils.SetDetailsToEntity(componenteDto);

            return Utils.MappClassFromDto<ComponenteDespesaDto, Componentedespesa>(componenteDto);
        }

        private Componenteconciliacaomovimentos BuildComponenteconciliacaomovimentosObject(ComponenteConciliacaoMovimentosDataContract componente)
        {
            ComponenteconciliacaomovimentosDto componenteDto = new ComponenteconciliacaomovimentosDto
            {
                Id = componente.id,
                PermissaoSelecionarMovimentos = componente.permissaoSelecionarMovimentos,
                PermissaoMovimentosConciliar = componente.permissaoMovimentosConciliar,
                PermissaoMovimentosBancarios = componente.permissaoMovimentosBancarios,
                PermissaoVerMovimentosAconciliar = componente.permissaoVerMovimentosAconciliar,
                PermissaoConciliar = componente.permissaoConciliar,
                PermissaoDesfazerConciliar = componente.permissaoDesfazerConciliar,
                IndActivo = true,
            };

            if (componenteDto.Id > 0)
            {
                Componenteconciliacaomovimentos original = _unitOfWork.ComponenteConciliacaoMovimentosRepository.Get(componenteDto.Id);
                componenteDto.UtilizadorCriacao = original.UtilizadorCriacao;
                componenteDto.DataCriacao = original.DataCriacao;
                componenteDto = _utils.UpdateDetailsToEntity(componenteDto);
            }
            else
                componenteDto = _utils.SetDetailsToEntity(componenteDto);

            return Utils.MappClassFromDto<ComponenteconciliacaomovimentosDto, Componenteconciliacaomovimentos>(componenteDto);
        }

        private Componentereceita BuildComponenteReceitaObject(ComponenteReceitaDataContract componente)
        {
            ComponenteReceitaDto componenteDto = new ComponenteReceitaDto
            {
                Id = componente.id,
                ClassificarMovSelecionados = componente.classificarMovSelecionados,
                SelecionarMovRecebidosParaRegisto = componente.selecionarMovRecebidosParaRegisto,
                VerificarExecucaoOrcamentoEditarSelecao = componente.verificarExecucaoOrcamentoEditarSelecao,
                IndActivo = true,
            };

            if (componenteDto.Id > 0)
            {
                Componentereceita original = _unitOfWork.ComponenteReceitaRepository.Get(componenteDto.Id);
                componenteDto.UtilizadorCriacao = original.UtilizadorCriacao;
                componenteDto.DataCriacao = original.DataCriacao;
                componenteDto = _utils.UpdateDetailsToEntity(componenteDto);
            }
            else
                componenteDto = _utils.SetDetailsToEntity(componenteDto);

            return Utils.MappClassFromDto<ComponenteReceitaDto, Componentereceita>(componenteDto);
        }

        public TarefasAtivasListagemResponse GetAllTarefasAtivas(SearchFilterRequest request)
        {
            TarefasAtivasListagemResponse response = new TarefasAtivasListagemResponse();

            try
            {
                List<int> FilteredTarefaIds = new List<int>();

                var perfilIds = _unitOfWork.RelUtilizadorPerfilRepository.GetPerfisIdsByUtilizador((int)request.UserId);

                var tarefaIdsSemControlo = _unitOfWork.TarefaAtivoRepository.GetAllTarefasAtivasSemControlo();
                FilteredTarefaIds.AddRange(tarefaIdsSemControlo);

                var allowedTarefaIds = _unitOfWork.ComponenteControloAcessoRepository.GetAllowedTarefaIdsByPerfilAndUser(perfilIds, (int)request.UserId);
                FilteredTarefaIds.AddRange(allowedTarefaIds);
                FilteredTarefaIds.Distinct();

                response = _unitOfWork.TarefaAtivoRepository.GetAllTarefasAtivas(request, FilteredTarefaIds);
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }

        public ResponseBaseDataContract LockTarefa(SwitchTarefaAtivoRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            var tarefaAtivo = _unitOfWork.TarefaAtivoRepository.GetTarefaAtivoById(request.id);

            if (tarefaAtivo == null)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.EntityDoesNotExist).ToString(), ErrorMessage = ErrorsDataContract.EntityDoesNotExist.ToString() });
                return response;
            }

            if (tarefaAtivo.UtilizadorResponsavel.HasValue && request.UserId != tarefaAtivo.UtilizadorResponsavel.Value)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.IsAlreadyLocked).ToString(), ErrorMessage = ErrorsDataContract.IsAlreadyLocked.ToString() });
                return response;
            }

            try
            {
                tarefaAtivo.UtilizadorResponsavel = (int?)request.UserId;
                _unitOfWork.TarefaAtivoRepository.Update(tarefaAtivo);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }

        public ResponseBaseDataContract UnlockTarefa(SwitchTarefaAtivoRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            var tarefaAtivo = _unitOfWork.TarefaAtivoRepository.GetTarefaAtivoById(request.id);

            if (tarefaAtivo == null)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.EntityDoesNotExist).ToString(), ErrorMessage = ErrorsDataContract.EntityDoesNotExist.ToString() });
                return response;
            }

            try
            {
                tarefaAtivo.UtilizadorResponsavel = null;
                _unitOfWork.TarefaAtivoRepository.Update(tarefaAtivo);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }

        public ComponenteHistoricoTextoListagemResponse GetHistoricoTexto(GetHistoricoTextoRequest request)
        {
            ComponenteHistoricoTextoListagemResponse response = new ComponenteHistoricoTextoListagemResponse();

            var tarefaAtivo = _unitOfWork.TarefaAtivoRepository.GetTarefaAtivoById(request.tarefaAtivoId);

            if (tarefaAtivo == null)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.EntityDoesNotExist).ToString(), ErrorMessage = ErrorsDataContract.EntityDoesNotExist.ToString() });
                return response;
            }

            var processoId = tarefaAtivo.ProcessoAtivoFk;

            var tarefasIdsDoProcesso = _unitOfWork.TarefaAtivoRepository.GetAllTarefasIdsByProcessoAtivoIdExcludingCurrent(processoId, request.tarefaAtivoId);

            response.historicoTextos = _unitOfWork.ComponenteTextoRegistoRepository.GetHistoryTextsByTarefaAtivoIds(tarefasIdsDoProcesso);

            return response;
        }

        public SelectDescriptionResponse GetAllTarefasASeguir(GetAllTarefasASeguirRequest request)
        {
            SelectDescriptionResponse response = new SelectDescriptionResponse();
            try
            {
                var idTarefa = _unitOfWork.TarefaAtivoRepository.GetTarefaAtivoById(request.tarefaAtivoId).TarefaconfigFk;
                List<SelectDescription> selects = _unitOfWork.ComponenteAccoesTarefaRepository.GetAllTarefasASeguir(idTarefa);

                response.selects = selects;
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }

        public TarefaDataResponse GetTarefaData(GetTarefaDataRequest request)
        {
            TarefaDataResponse response = new TarefaDataResponse();

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.PreenchimentoTarefa, _unitOfWork, CRUD.READ);
            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            try
            {
                var tarefaAtivo = _unitOfWork.TarefaAtivoRepository.GetTarefaAtivoById(request.tarefaAtivoId);
                var componentes = _unitOfWork.RelTarefaComponenteRepository.GetRelTarefaComponenteByIdTarefa(tarefaAtivo.TarefaconfigFk);
                response.data = new PreencherTarefa();

                //Cabeçalho zone
                response.data.cabecalho = new CabecalhoComponent
                {
                    nomeProcesso = tarefaAtivo.ProcessoAtivoFkNavigation.ProcessoConfigFkNavigation.Nome,
                    numProcesso = tarefaAtivo.ProcessoAtivoFkNavigation.NumeroProcesso,
                    nomeTarefa = tarefaAtivo.TarefaconfigFkNavigation.Nome
                };

                if (componentes.Any(x => x.descricao == "Classificação / Sub-Classificação"))
                {
                    //Classificação e subclassificação
                    response.data.classificacaoSubClass = tarefaAtivo.ComponenteclassificacaosubRegisto.FirstOrDefault()?.SubClassificacaoFk;
                }

                if (componentes.Any(x => x.descricao == "Texto"))
                {
                    var textoConfig = _unitOfWork.ComponenteTextoRepository.GetComponenteByIdTarefa(tarefaAtivo.TarefaconfigFk);
                    //Textos
                    response.data.textos = new TextosComponent
                    {
                        isExpanded = textoConfig.Expandir1.GetValueOrDefault(),
                        textTitle = textoConfig.Titulo1?.Length > 0 ? textoConfig.Titulo1 : null,
                        text = tarefaAtivo.ComponentetextoRegisto.FirstOrDefault(x => x.TituloTexto == textoConfig.Titulo1) != null ? tarefaAtivo.ComponentetextoRegisto.FirstOrDefault(x => x.TituloTexto == textoConfig.Titulo1)?.Texto : "",
                        charLimit = textoConfig.QuantCaracteres1,
                        obrigatorio = textoConfig.Obrigatorio1.GetValueOrDefault(),
                        obrigatorioAoArquivar = textoConfig.ObrigatorioAoArquivar1,
                        isExpanded2 = textoConfig.Expandir2.GetValueOrDefault(),
                        textTitle2 = textoConfig.Titulo2?.Length > 0 ? textoConfig.Titulo2 : null,
                        text2 = tarefaAtivo.ComponentetextoRegisto.FirstOrDefault(x => x.TituloTexto == textoConfig.Titulo2) != null ? tarefaAtivo.ComponentetextoRegisto.FirstOrDefault(x => x.TituloTexto == textoConfig.Titulo2)?.Texto : "",
                        charLimit2 = textoConfig.QuantCaracteres2,
                        obrigatorio2 = textoConfig.Obrigatorio2.GetValueOrDefault(),
                        obrigatorioAoArquivar2 = textoConfig.ObrigatorioAoArquivar2,
                        hasArquivar = tarefaAtivo.TarefaconfigFkNavigation.BotaoArquivar
                    };
                }

                if (componentes.Any(x => x.descricao == "Prazo da Tarefa"))
                {
                    //Prazo Tarefa
                    response.data.prazoTarefa = new PrazoTarefaComponent
                    {
                        deadline = tarefaAtivo.TarefaconfigFkNavigation.PrazoTarefa.GetValueOrDefault(),
                        limitDate = tarefaAtivo.DataCriacao.AddDays(tarefaAtivo.TarefaconfigFkNavigation.PrazoTarefa.GetValueOrDefault())
                    };
                }

                //Archive
                response.data.hasArchive = tarefaAtivo.TarefaconfigFkNavigation.BotaoArquivar;
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }

        public ResponseBaseDataContract SaveTarefaData(SaveTarefaDataRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.PreenchimentoTarefa, _unitOfWork, CRUD.UPDATE);
            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            var tarefaAtiva = _unitOfWork.TarefaAtivoRepository.GetTarefaAtivoById(request.tarefaAtivoId);

            //Validate if it is to close task and move to the next one
            // TODO: validations for every componnet that has been filled or has any rules to close task
            if (request.data.nextTarefaNumber.HasValue)
            {
                // Get all componentes from this task
                var relComponentes = _unitOfWork.RelTarefaComponenteRepository.GetRelTarefaComponenteByIdTarefa(tarefaAtiva.TarefaconfigFk);

                //Text Region
                if (request.data.textos != null)
                {
                    var textConfig = tarefaAtiva.TarefaconfigFkNavigation.Componentetexto.Where(x => x.IndActivo).FirstOrDefault();

                    if (textConfig.Obrigatorio1.Value && request.data.textos.text.Length <= 0 || textConfig.Obrigatorio2.Value && request.data.textos.text2.Length <= 0)
                        response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.RequiredTextMissing).ToString(), ErrorMessage = ErrorsDataContract.RequiredTextMissing.ToString() });
                }

                // SubClassificação Region
                if (!request.data.classificacaoSubClass.HasValue && relComponentes.Where(x => x.descricao == "Classificação / Sub-Classificação").Any())
                    response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.RequiredSubClass).ToString(), ErrorMessage = ErrorsDataContract.RequiredSubClass.ToString() });

                //Documents
                if (relComponentes.Where(x => x.descricao == "Carregar Documentos").Any())
                {
                    var requiredDocuments = tarefaAtiva.TarefaconfigFkNavigation.Componentecarregardocumento.Where(x => x.Obrigatorio && x.IndActivo);
                    var tarefaAtivaDocumentIds = tarefaAtiva.ComponentedocumentoRegisto.Where(x => x.IndActivo).Select(x => x.DocumentoFk).ToList();

                    if (requiredDocuments.Where(x => !tarefaAtivaDocumentIds.Contains(x.DocumentoFk)).Any())
                        response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.LackOfObligatoryDocuments).ToString(), ErrorMessage = ErrorsDataContract.LackOfObligatoryDocuments.ToString() });
                }

                // Despesa Region
                // Impedir avançar a tarefa enquanto existirem despesas deste processo ainda em
                // estado Registado (não Autorizadas) -- sem este check ficavam "esquecidas" numa
                // TarefaAtivo já fechada, sem nenhum ecrã que voltasse a mostrá-las para autorização
                // (ver [[next-task-missing-ad-completeness-check]]).
                //
                // CORRIGIDO: só bloquear se a PRÓXIMA tarefa já não tiver também o componente
                // "Despesa" -- ou seja, só quando esta transição for de facto a última oportunidade
                // de autorizar/eliminar a despesa antes de ela deixar de ter algum ecrã que a mostre.
                // A versão anterior bloqueava em QUALQUER tarefa de origem com "Despesa" (incluindo o
                // próprio registo inicial RD01, que nunca tem botão de Autorizar), criando um beco sem
                // saída: nunca era possível avançar de RD01 para RD02/RD03 (onde a autorização acontece)
                // porque a despesa nunca deixa de estar "R" antes dessa transição existir. Confirmado ao
                // vivo (ver [[next-task-missing-ad-completeness-check]]) com uma cadeia RD01→RD10
                // correctamente configurada -- bloqueava logo no primeiro passo, tornando a despesa
                // impossível de autorizar por qualquer via que não fosse apagá-la.
                if (relComponentes.Where(x => x.descricao == "Despesa").Any())
                {
                    var componentesTarefaSeguinte = _unitOfWork.RelTarefaComponenteRepository.GetRelTarefaComponenteByIdTarefa(request.data.nextTarefaNumber.Value);
                    bool proximaTarefaTambemTemDespesa = componentesTarefaSeguinte.Where(x => x.descricao == "Despesa").Any();

                    if (!proximaTarefaTambemTemDespesa)
                    {
                        var despesaRegistadaListagem = _unitOfWork.ComponenteDespesaRegistoRepository.GetAllDespesaRegistadaByTarefaAtivoId(request.tarefaAtivoId);
                        if (despesaRegistadaListagem != null && despesaRegistadaListagem.Any(d => d.estado == "R"))
                            response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.DespesaPendenteImpedeAvancoTarefa).ToString(), ErrorMessage = ErrorsDataContract.DespesaPendenteImpedeAvancoTarefa.ToString() });
                    }
                }
            }

            if (response.Errors.Count > 0)
                return response;

            try
            {
                // Text Region
                if (request.data.textos != null)
                {
                    UpsertTextos(request);
                }

                // SubClassificação Region
                if (request.data.classificacaoSubClass.HasValue)
                {
                    UpsertSubClassificacao(request);
                }

                if (request.data.nextTarefaNumber.HasValue)
                {
                    tarefaAtiva.IndActivo = false;
                    Tarefaativo proximaTarefa = new Tarefaativo
                    {
                        ProcessoAtivoFk = tarefaAtiva.ProcessoAtivoFk,
                        TarefaconfigFk = request.data.nextTarefaNumber.Value,
                        IndActivo = true
                    };
                    proximaTarefa = _utils.SetDetailsToEntity(proximaTarefa);
                    _unitOfWork.TarefaAtivoRepository.Add(proximaTarefa);
                }

                //Update details of tarefa ativa
                tarefaAtiva = _utils.UpdateDetailsToEntity(tarefaAtiva);
                _unitOfWork.TarefaAtivoRepository.Update(tarefaAtiva);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }

        public ResponseBaseDataContract ArquivarTarefa(SaveTarefaDataRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.PreenchimentoTarefa, _unitOfWork, CRUD.UPDATE);
            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            var tarefaAtiva = _unitOfWork.TarefaAtivoRepository.GetTarefaAtivoById(request.tarefaAtivoId);

            if (!tarefaAtiva.TarefaconfigFkNavigation.BotaoArquivar)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            // Specific Validations for archive method
            var relComponentes = _unitOfWork.RelTarefaComponenteRepository.GetRelTarefaComponenteByIdTarefa(tarefaAtiva.TarefaconfigFk);

            //Text Region
            if (request.data.textos != null)
            {
                var textConfig = tarefaAtiva.TarefaconfigFkNavigation.Componentetexto.Where(x => x.IndActivo).FirstOrDefault();

                if (textConfig.Obrigatorio1.Value && request.data.textos.text.Length <= 0 || textConfig.Obrigatorio2.Value && request.data.textos.text2.Length <= 0)
                    response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.RequiredTextMissing).ToString(), ErrorMessage = ErrorsDataContract.RequiredTextMissing.ToString() });
            }

            // SubClassificação Region
            if (!request.data.classificacaoSubClass.HasValue && relComponentes.Where(x => x.descricao == "Classificação / Sub-Classificação").Any())
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.RequiredSubClass).ToString(), ErrorMessage = ErrorsDataContract.RequiredSubClass.ToString() });

            //Documents
            if (relComponentes.Where(x => x.descricao == "Carregar Documentos").Any())
            {
                var requiredDocuments = tarefaAtiva.TarefaconfigFkNavigation.Componentecarregardocumento.Where(x => x.Obrigatorio && x.IndActivo);
                var tarefaAtivaDocumentIds = tarefaAtiva.ComponentedocumentoRegisto.Where(x => x.IndActivo).Select(x => x.DocumentoFk).ToList();

                if (requiredDocuments.Where(x => !tarefaAtivaDocumentIds.Contains(x.DocumentoFk)).Any())
                    response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.LackOfObligatoryDocuments).ToString(), ErrorMessage = ErrorsDataContract.LackOfObligatoryDocuments.ToString() });
            }

            // Despesa Region
            //Não é possível arquivar um processo com despesas com pagamentos executados 
            Tarefaativo tarefaativo = _unitOfWork.TarefaAtivoRepository.Get(request.tarefaAtivoId);
            List<Pagamentosexecutados> pagamentosexecutados = _unitOfWork.PagamentosExecutadosRepository.GetPagamentosexecutadosByIdProcessoAtivo(tarefaAtiva.ProcessoAtivoFk, true);

            if (pagamentosexecutados != null && pagamentosexecutados.Count > 0)
            {
                foreach (var pagamento in pagamentosexecutados)
                {
                    if (!pagamento.RelMovimentosporconciliarMovimentos.Any() || pagamento.RelMovimentosporconciliarMovimentos.Any(e => e.Estado != _unitOfWork.DominioRepository.getIdDominio("ESTADOMOVIMENTO", 1)))
                    {
                        response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.RequiredPaymentsState).ToString(), ErrorMessage = ErrorsDataContract.RequiredSubClass.ToString() });
                        break;
                    }
                    //if (pagamento.Estado == _unitOfWork.DominioRepository.getIdDominio("ESTADOPAGAMENTO", 1)
                    //    || pagamento.Estado == _unitOfWork.DominioRepository.getIdDominio("ESTADOPAGAMENTO", 2)
                    //    || pagamento.Estado == _unitOfWork.DominioRepository.getIdDominio("ESTADOPAGAMENTO", 3))
                    //{
                    //    response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.RequiredPaymentsState).ToString(), ErrorMessage = ErrorsDataContract.RequiredSubClass.ToString() });
                    //    break;
                    //}
                }
            }

            if (response.Errors.Count > 0)
                return response;

            try
            {
                // Text Region
                if (request.data.textos != null)
                {
                    UpsertTextos(request);
                }

                // SubClassificação Region
                if (request.data.classificacaoSubClass.HasValue)
                {
                    UpsertSubClassificacao(request);
                }

                var processoAtivo = tarefaAtiva.ProcessoAtivoFkNavigation;

                // Orçamento Region
                List<ComponenteorcamentoRegisto> orcamentosParaApagar = new List<ComponenteorcamentoRegisto>();
                processoAtivo.Tarefaativo.ToList().ForEach(x => x.ComponenteorcamentoRegisto.Where(y => y.IndActivo && !y.Aprovado).ToList().ForEach(z => orcamentosParaApagar.Add(z)));
                foreach(var orcamento in orcamentosParaApagar)
                {
                    if(orcamento.OrcamentoRetificadoFk == null)
                    {
                        foreach(var orcamentoValor in orcamento.Componenteorcamentovalor)
                        {
                            _unitOfWork.ComponenteOrcamentoValorRepository.Delete(orcamentoValor);
                        }
                        _unitOfWork.ComponenteOrcamentoRegistoRepository.Delete(orcamento);
                    }
                    else
                    {
                        foreach (var orcamentoValor in orcamento.Componenteorcamentovalor)
                        {
                            _unitOfWork.ComponenteOrcamentoValorRepository.Delete(orcamentoValor);
                        }
                        orcamento.OrcamentoRetificadoFk = null;
                        orcamento.IndActivo = false;
                        _utils.UpdateDetailsToEntity(orcamento);
                        _unitOfWork.ComponenteOrcamentoRegistoRepository.Update(orcamento);
                    }
                }

                // Closes tarefa ativa and archive processo
                tarefaAtiva.IndActivo = false;
                processoAtivo.Arquivado = true;
                processoAtivo.IndActivo = false;

                //Update details of tarefa ativa and processo ativo
                tarefaAtiva = _utils.UpdateDetailsToEntity(tarefaAtiva);
                processoAtivo = _utils.UpdateDetailsToEntity(processoAtivo);
                _unitOfWork.ProcessoAtivoRepository.Update(processoAtivo);
                _unitOfWork.TarefaAtivoRepository.Update(tarefaAtiva);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }

        #region SaveTarefa and ArchiveTarefa private methods

        private void UpsertTextos(SaveTarefaDataRequest request)
        {
            var registoTextosDaTarefa = _unitOfWork.ComponenteTextoRegistoRepository.GetAllByTarefaAtivoId(request.tarefaAtivoId);

            //First text
            if (request.data.textos.textTitle != null)
            {
                //Update
                if (registoTextosDaTarefa.Any(x => x.TituloTexto == request.data.textos.textTitle))
                {
                    var registoExistente = registoTextosDaTarefa.FirstOrDefault(x => x.TituloTexto == request.data.textos.textTitle);
                    if (registoExistente.Texto != request.data.textos.text)
                    {
                        registoExistente.Texto = request.data.textos.text;
                        registoExistente = _utils.UpdateDetailsToEntity(registoExistente);
                        _unitOfWork.ComponenteTextoRegistoRepository.Update(registoExistente);
                    }
                }
                //Insert
                else if (request.data.textos.text.Length > 0)
                {
                    ComponentetextoRegisto primeiroTexto = new ComponentetextoRegisto
                    {
                        TarefaAtivoFk = request.tarefaAtivoId,
                        TituloTexto = request.data.textos.textTitle,
                        Texto = request.data.textos.text,
                        IndActivo = true
                    };
                    primeiroTexto = _utils.SetDetailsToEntity(primeiroTexto);
                    _unitOfWork.ComponenteTextoRegistoRepository.Add(primeiroTexto);
                }
            }
            //Second text
            if (request.data.textos.textTitle2 != null)
            {
                //Update
                if (registoTextosDaTarefa.Any(x => x.TituloTexto == request.data.textos.textTitle2))
                {
                    var registoExistente = registoTextosDaTarefa.FirstOrDefault(x => x.TituloTexto == request.data.textos.textTitle2);
                    if (registoExistente.Texto != request.data.textos.text2)
                    {
                        registoExistente.Texto = request.data.textos.text2;
                        registoExistente = _utils.UpdateDetailsToEntity(registoExistente);
                        _unitOfWork.ComponenteTextoRegistoRepository.Update(registoExistente);
                    }
                }
                //Insert
                else if (request.data.textos.text2.Length > 0)
                {
                    ComponentetextoRegisto segundoTexto = new ComponentetextoRegisto
                    {
                        TarefaAtivoFk = request.tarefaAtivoId,
                        TituloTexto = request.data.textos.textTitle2,
                        Texto = request.data.textos.text2,
                        IndActivo = true
                    };
                    segundoTexto = _utils.SetDetailsToEntity(segundoTexto);
                    _unitOfWork.ComponenteTextoRegistoRepository.Add(segundoTexto);
                }
            }
        }

        private void UpsertSubClassificacao(SaveTarefaDataRequest request)
        {
            var RegistoSubClass = _unitOfWork.ComponenteClassificacaoSubClassificRegistoRepository.GetByTarefaAtivoId(request.tarefaAtivoId);
            //Update
            if (RegistoSubClass != null)
            {
                if (RegistoSubClass.SubClassificacaoFk != request.data.classificacaoSubClass)
                {
                    RegistoSubClass.SubClassificacaoFk = request.data.classificacaoSubClass.Value;
                    RegistoSubClass = _utils.UpdateDetailsToEntity(RegistoSubClass);
                    _unitOfWork.ComponenteClassificacaoSubClassificRegistoRepository.Update(RegistoSubClass);
                }
            }
            //Insert
            else
            {
                ComponenteclassificacaosubRegisto componenteSubClassRegisto = new ComponenteclassificacaosubRegisto
                {
                    TarefaAtivoFk = request.tarefaAtivoId,
                    SubClassificacaoFk = request.data.classificacaoSubClass.Value,
                    IndActivo = true
                };
                componenteSubClassRegisto = _utils.SetDetailsToEntity(componenteSubClassRegisto);
                _unitOfWork.ComponenteClassificacaoSubClassificRegistoRepository.Add(componenteSubClassRegisto);
            }
        }

        #endregion SaveTarefa and ArchiveTarefa private methods

        public string GetTituloListaPagamento(int tarefaActivoId)
        {
            try
            {
                var tarefa = _unitOfWork.TarefaAtivoRepository.Get(tarefaActivoId);
                return tarefa?.TituloListaPagamento ?? "Lista Pagamentu Saláriu Funcionáriu INSS";
            }
            catch
            {
                return "Lista Pagamentu Saláriu Funcionáriu INSS";
            }
        }
    }
}