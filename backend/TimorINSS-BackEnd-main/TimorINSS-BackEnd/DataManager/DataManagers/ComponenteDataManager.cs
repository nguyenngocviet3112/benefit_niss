 using System;
using System.Collections.Generic;
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
    public class ComponenteDataManager : IComponenteDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public ComponenteDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        public ComponentesListagemResponse GetAllComponentes()
        {
            ComponentesListagemResponse response = new ComponentesListagemResponse();
            try
            {
                //vai buscar todos os componentes existentes na BD
                response = _unitOfWork.ComponenteRepository.GetAllComponentes();
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }

        public ResponseBaseDataContract EditarComponenteTexto(ComponenteTextoRequest request)
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
            Componentetexto componentetexto = new Componentetexto();
            //devolve todos os componentes de texto de uma determinada tarefa config
            componentetexto = _unitOfWork.ComponenteTextoRepository.GetComponenteByIdTarefa(request.idTarefa);

            if (response.Errors.Count > 0)
            {
                return response;
            }


            try
            {
                Componentetexto componenteTextoAtualizar = new Componentetexto();
                //update in DB

                if (componentetexto != null && componentetexto.Id > 0)
                {
                    componenteTextoAtualizar = BuildComponenteTextoObject(request.componenteTexto);
                    componenteTextoAtualizar.TarefaFk = componentetexto.TarefaFk;

                    if (request.componenteTexto.id == 0 && !request.componenteTexto.texto1)
                    {
                        componentetexto.IndActivo = false;
                        componentetexto = _utils.UpdateDetailsToEntity(componentetexto);
                        _unitOfWork.ComponenteTextoRepository.Update(componentetexto);
                    }
                    else
                        _unitOfWork.ComponenteTextoRepository.Update(componenteTextoAtualizar);
                }
                else
                {
                    componenteTextoAtualizar = BuildComponenteTextoObject(request.componenteTexto);
                    componenteTextoAtualizar.TarefaFk = request.idTarefa;
                    _unitOfWork.ComponenteTextoRepository.Add(componenteTextoAtualizar);
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

        public ResponseBaseDataContract EditarComponentePrazoTarefa(ComponentePrazoTarefaRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };
            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.ConfigurarTarefas, _unitOfWork, CRUD.UPDATE);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            Tarefa tarefa = new Tarefa();
            tarefa = _unitOfWork.TarefaRepository.Get(request.idTarefa);

            //validações
            if (response.Errors.Count > 0)
            {
                return response;
            }

            //update in DB
            try
            {
                if (tarefa != null && tarefa.Id > 0)
                {
                    tarefa.PrazoTarefa = request.prazoTarefa;
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

        public ResponseBaseDataContract EditarComponenteAccaoTarefa(ComponenteAccaoTarefaRequest request)
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

            List<Componenteaccoestarefa> listaAccaoTarefa = new List<Componenteaccoestarefa>();
            listaAccaoTarefa = _unitOfWork.ComponenteAccoesTarefaRepository.GetComponenteByIdTarefa(request.idTarefa);

            List<Componenteaccoestarefa> listaToUpdate = new List<Componenteaccoestarefa>();
            List<Componenteaccoestarefa> listaToAdd = new List<Componenteaccoestarefa>();
            List<Componenteaccoestarefa> listaToInactive = new List<Componenteaccoestarefa>();

            // List to inactive

            if (listaAccaoTarefa != null && listaAccaoTarefa.Count > 0)
            {
                foreach (var componente in listaAccaoTarefa)
                {
                    componente.IndActivo = false;
                    var componenteToInactive = componente;
                    componenteToInactive = _utils.UpdateDetailsToEntity(componenteToInactive);
                    listaToInactive.Add(componenteToInactive);
                }
            }

            if (listaAccaoTarefa != null && listaAccaoTarefa.Count > 0)
            {
                foreach (var componente in request.accaoTarefa)
                {
                    var existeComponente = false;
                    foreach (var componenteAccaoTarefa in listaAccaoTarefa)
                    {
                        if (componente.id == componenteAccaoTarefa.Id)
                        {
                            listaToUpdate.Add(BuildComponenteAccoesTarefaObject(componente, request.idTarefa));
                            existeComponente = true;
                        }
                    }
                    if (!existeComponente)
                    {
                        listaToAdd.Add(BuildComponenteAccoesTarefaObject(componente, request.idTarefa));
                    }
                }
            }
            else
            {
                foreach (var componente in request.accaoTarefa)
                {
                    listaToAdd.Add(BuildComponenteAccoesTarefaObject(componente, request.idTarefa));
                }
            }

            //update in DB
            try
            {
                if (listaToInactive != null && listaToInactive.Count > 0)
                {
                    foreach (var componenteInactive in listaToInactive)
                    {
                        _unitOfWork.ComponenteAccoesTarefaRepository.Update(componenteInactive);
                    }
                }

                if (listaToUpdate != null && listaToUpdate.Count > 0)
                {
                    foreach (var componenteUpdate in listaToUpdate)
                    {
                        _unitOfWork.ComponenteAccoesTarefaRepository.Update(componenteUpdate);
                    }
                }

                if (listaToAdd != null && listaToAdd.Count > 0)
                {
                    foreach (var componenteAdd in listaToAdd)
                    {
                        _unitOfWork.ComponenteAccoesTarefaRepository.Add(componenteAdd);
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

        public ResponseBaseDataContract EditarComponenteDocumentoTarefa(ComponenteDocumentoTarefaRequest request)
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
            var listaDocumentoTarefa = _unitOfWork.ComponenteCarregarDocumentoRepository.GetComponenteByIdTarefa(request.idTarefa);

            List<Componentecarregardocumento> listaToUpdate = new List<Componentecarregardocumento>();
            List<Componentecarregardocumento> listaToAdd = new List<Componentecarregardocumento>();
            List<Componentecarregardocumento> listaToInactive = new List<Componentecarregardocumento>();

            // List to inactive

            if (listaDocumentoTarefa != null && listaDocumentoTarefa.Count > 0)
            {
                foreach (var componente in listaDocumentoTarefa)
                {
                    componente.IndActivo = false;
                    var componenteToInactive = componente;
                    componenteToInactive = _utils.UpdateDetailsToEntity(componenteToInactive);
                    listaToInactive.Add(componenteToInactive);
                }
            }

            if (listaDocumentoTarefa != null && listaDocumentoTarefa.Count > 0)
            {
                foreach (var componente in request.documentoTarefa)
                {
                    var existeComponente = false;
                    foreach (var componenteDocumentoTarefa in listaDocumentoTarefa)
                    {
                        if (componente.id == componenteDocumentoTarefa.Id)
                        {
                            listaToUpdate.Add(BuildComponenteCarregarDocumentosTarefaObject(componente, request.idTarefa));
                            existeComponente = true;
                        }
                    }
                    if (!existeComponente)
                    {
                        listaToAdd.Add(BuildComponenteCarregarDocumentosTarefaObject(componente, request.idTarefa));
                    }
                }
            }
            else
            {
                foreach (var componente in request.documentoTarefa)
                {
                    listaToAdd.Add(BuildComponenteCarregarDocumentosTarefaObject(componente, request.idTarefa));
                }
            }

            //update in DB
            try
            {
                if (listaToInactive != null && listaToInactive.Count > 0)
                {
                    foreach (var componenteInactivee in listaToInactive)
                    {
                        _unitOfWork.ComponenteCarregarDocumentoRepository.Update(componenteInactivee);
                    }
                }

                if (listaToUpdate != null && listaToUpdate.Count > 0)
                {
                    foreach (var componenteUpdate in listaToUpdate)
                    {
                        _unitOfWork.ComponenteCarregarDocumentoRepository.Update(componenteUpdate);
                    }
                }

                if (listaToAdd != null && listaToAdd.Count > 0)
                {
                    foreach (var componenteAdd in listaToAdd)
                    {
                        _unitOfWork.ComponenteCarregarDocumentoRepository.Add(componenteAdd);
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

        public ResponseBaseDataContract EditarComponenteClassificacaoSubClassifTarefa(ComponenteClassificacaoSubClassificTarefaRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };
            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.ConfigurarTarefas, _unitOfWork, CRUD.UPDATE);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            var listaClassificacaoSubClassificTarefa = _unitOfWork.ComponenteClassificacaoSubClassificRepository.GetComponenteByIdTarefa(request.idTarefa);

            List<Componenteclassificacaosubclassific> listaToUpdate = new List<Componenteclassificacaosubclassific>();
            List<Componenteclassificacaosubclassific> listaToAdd = new List<Componenteclassificacaosubclassific>();
            List<Componenteclassificacaosubclassific> listaToInactive = new List<Componenteclassificacaosubclassific>();


            if (listaClassificacaoSubClassificTarefa != null && listaClassificacaoSubClassificTarefa.Count > 0)
            {
                foreach (var componente in listaClassificacaoSubClassificTarefa)
                {
                    componente.IndActivo = false;
                    var componenteToInactive = componente;
                    componenteToInactive = _utils.UpdateDetailsToEntity(componenteToInactive);
                    listaToInactive.Add(componenteToInactive);
                }
            }

            if (listaClassificacaoSubClassificTarefa != null && listaClassificacaoSubClassificTarefa.Count > 0)
            {
                foreach (var componente in request.classificacaoSubClassific)
                {
                    var existeComponente = false;
                    foreach (var componenteClassificacaoSubClassific in listaClassificacaoSubClassificTarefa)
                    {
                        if (componente.id == componenteClassificacaoSubClassific.Id)
                        {
                            listaToUpdate.Add(BuildComponenteClassificacaoTarefaObject(componente, request.idTarefa));
                            existeComponente = true;
                        }
                    }
                    if (!existeComponente)
                    {
                        listaToAdd.Add(BuildComponenteClassificacaoTarefaObject(componente, request.idTarefa));
                    }
                }
            }
            else
            {
                foreach (var componente in request.classificacaoSubClassific)
                {
                    listaToAdd.Add(BuildComponenteClassificacaoTarefaObject(componente, request.idTarefa));
                }
            }

            //update in DB
            try
            {
                if (listaToInactive != null && listaToInactive.Count > 0)
                {
                    foreach (var componenteInactivee in listaToInactive)
                    {
                        _unitOfWork.ComponenteClassificacaoSubClassificRepository.Update(componenteInactivee);
                    }
                }

                if (listaToUpdate != null && listaToUpdate.Count > 0)
                {
                    foreach (var componenteUpdate in listaToUpdate)
                    {
                        _unitOfWork.ComponenteClassificacaoSubClassificRepository.Update(componenteUpdate);
                    }
                }

                if (listaToAdd != null && listaToAdd.Count > 0)
                {
                    foreach (var componenteAdd in listaToAdd)
                    {
                        _unitOfWork.ComponenteClassificacaoSubClassificRepository.Add(componenteAdd);
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

        public ResponseBaseDataContract EditarComponenteControloAcessoPerfilTarefa(ComponenteControloAcessoPerfilTarefaRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };
            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.ConfigurarTarefas, _unitOfWork, CRUD.UPDATE);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            var listaControloAcessoTarefa = _unitOfWork.ComponenteControloAcessoRepository.GetByIdTarefa(request.idTarefa);

            List<Componentecontroleacesso> listaToUpdate = new List<Componentecontroleacesso>();
            List<Componentecontroleacesso> listaToAdd = new List<Componentecontroleacesso>();
            List<Componentecontroleacesso> listaToInactive = new List<Componentecontroleacesso>();

            if (listaControloAcessoTarefa != null && listaControloAcessoTarefa.Count > 0)
            {
                foreach (var componente in listaControloAcessoTarefa)
                {
                    if (componente.PerfilFk != null && componente.PerfilFk > 0)
                    {
                        componente.IndActivo = false;
                        var componenteToInactive = componente;
                        componenteToInactive = _utils.UpdateDetailsToEntity(componenteToInactive);
                        listaToInactive.Add(componenteToInactive);
                    }
                }
            }

            if (listaControloAcessoTarefa != null && listaControloAcessoTarefa.Count > 0)
            {
                foreach (var componente in request.controloAcessoPerfil)
                {
                    var existeComponente = false;
                    foreach (var componenteControloAcesso in listaControloAcessoTarefa)
                    {
                        if (componente.id == componenteControloAcesso.Id)
                        {
                            listaToUpdate.Add(BuildControleAcessoPerfil(componente, request.idTarefa));
                            existeComponente = true;
                        }
                    }
                    if (!existeComponente)
                    {
                        listaToAdd.Add(BuildControleAcessoPerfil(componente, request.idTarefa));
                    }
                }
            }
            else
            {
                foreach (var componente in request.controloAcessoPerfil)
                {
                    listaToAdd.Add(BuildControleAcessoPerfil(componente, request.idTarefa));
                }
            }

            //update in DB
            try
            {
                if (listaToInactive != null && listaToInactive.Count > 0)
                {
                    foreach (var componenteInactive in listaToInactive)
                    {
                        _unitOfWork.ComponenteControloAcessoRepository.Update(componenteInactive);
                    }
                }

                if (listaToUpdate != null && listaToUpdate.Count > 0)
                {
                    foreach (var componenteUpdate in listaToUpdate)
                    {
                        _unitOfWork.ComponenteControloAcessoRepository.Update(componenteUpdate);
                    }
                }

                if (listaToAdd != null && listaToAdd.Count > 0)
                {
                    foreach (var componenteAdd in listaToAdd)
                    {
                        _unitOfWork.ComponenteControloAcessoRepository.Add(componenteAdd);
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

        public ResponseBaseDataContract EditarComponenteControloAcessoUtilizadorTarefa(ComponenteControloAcessoUtilizadorTarefaRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };
            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.ConfigurarTarefas, _unitOfWork, CRUD.UPDATE);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            var listaControloAcessoTarefa = _unitOfWork.ComponenteControloAcessoRepository.GetByIdTarefa(request.idTarefa);

            List<Componentecontroleacesso> listaToUpdate = new List<Componentecontroleacesso>();
            List<Componentecontroleacesso> listaToAdd = new List<Componentecontroleacesso>();
            List<Componentecontroleacesso> listaToInactive = new List<Componentecontroleacesso>();

            if (listaControloAcessoTarefa != null && listaControloAcessoTarefa.Count > 0)
            {
                foreach (var componente in listaControloAcessoTarefa)
                {
                    if (componente.UtilizadorFk != null && componente.UtilizadorFk > 0)
                    {
                        componente.IndActivo = false;
                        var componenteToInactive = componente;
                        componenteToInactive = _utils.UpdateDetailsToEntity(componenteToInactive);
                        listaToInactive.Add(componenteToInactive);
                    }
                }
            }

            if (listaControloAcessoTarefa != null && listaControloAcessoTarefa.Count > 0)
            {
                foreach (var componente in request.controloAcessoUtilizador)
                {
                    var existeComponente = false;
                    foreach (var componenteControloAcesso in listaControloAcessoTarefa)
                    {
                        if (componente.id == componenteControloAcesso.Id)
                        {
                            listaToUpdate.Add(BuildControleAcessoUtilizador(componente, request.idTarefa));
                            existeComponente = true;
                        }
                    }
                    if (!existeComponente)
                    {
                        listaToAdd.Add(BuildControleAcessoUtilizador(componente, request.idTarefa));
                    }
                }
            }
            else
            {
                foreach (var componente in request.controloAcessoUtilizador)
                {
                    listaToAdd.Add(BuildControleAcessoUtilizador(componente, request.idTarefa));
                }
            }

            //update in DB
            try
            {
                if (listaToInactive != null && listaToInactive.Count > 0)
                {
                    foreach (var componenteInactive in listaToInactive)
                    {
                        _unitOfWork.ComponenteControloAcessoRepository.Update(componenteInactive);
                    }
                }

                if (listaToUpdate != null && listaToUpdate.Count > 0)
                {
                    foreach (var componenteUpdate in listaToUpdate)
                    {
                        _unitOfWork.ComponenteControloAcessoRepository.Update(componenteUpdate);
                    }
                }

                if (listaToAdd != null && listaToAdd.Count > 0)
                {
                    foreach (var componenteAdd in listaToAdd)
                    {
                        _unitOfWork.ComponenteControloAcessoRepository.Add(componenteAdd);
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

        public ResponseBaseDataContract EditarComponenteOrcamento(ComponenteOrcamentoRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                // Validar se o utilizador tem as permissões necessárias
                bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.ConfigurarTarefas, _unitOfWork, CRUD.UPDATE);

                if (!permission)
                {
                    response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                    return response;
                }

                Componenteorcamento existeComponenteBD = new Componenteorcamento();
                existeComponenteBD = _unitOfWork.ComponenteOrcamentoRepository.GetByIdTarefa(request.ComponenteOrcamento.TarefaFk);

                Componenteorcamento componente = BuildComponenteOrcamentoObject(request.ComponenteOrcamento);

                if (componente.Id > 0)
                {
                    _unitOfWork.ComponenteOrcamentoRepository.Update(componente);
                }
                else
                {
                    if (componente.Id == 0 && existeComponenteBD != null && existeComponenteBD.Id > 0)
                    {
                        existeComponenteBD.IndActivo = false;
                        existeComponenteBD = _utils.UpdateDetailsToEntity(existeComponenteBD);
                        _unitOfWork.ComponenteOrcamentoRepository.Update(existeComponenteBD);
                    }
                    else
                    {
                        _unitOfWork.ComponenteOrcamentoRepository.Add(componente);
                    }
                }

                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }

        public ResponseBaseDataContract EditarComponenteDespesa(ComponenteDespesaRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                // Validar se o utilizador tem as permissões necessárias
                bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.ConfigurarTarefas, _unitOfWork, CRUD.UPDATE);

                if (!permission)
                {
                    response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                    return response;
                }
                Componentedespesa existeComponenteBD = new Componentedespesa();
                existeComponenteBD = _unitOfWork.ComponenteDespesaRepository.GetByIdTarefa(request.ComponenteDespesa.tarefaFk);

                Componentedespesa componente = BuildComponenteDespesaObject(request.ComponenteDespesa);

                if (componente.Id > 0)
                    _unitOfWork.ComponenteDespesaRepository.Update(componente);
                else if (componente.Id == 0)
                {
                    if (existeComponenteBD != null && existeComponenteBD.Id > 0)
                    {
                        existeComponenteBD.IndActivo = false;
                        existeComponenteBD = _utils.UpdateDetailsToEntity(existeComponenteBD);
                        _unitOfWork.ComponenteDespesaRepository.Update(existeComponenteBD);
                    }
                    else
                    {
                        _unitOfWork.ComponenteDespesaRepository.Add(componente);
                    }
                }

                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }

        public ResponseBaseDataContract EditarComponenteConciliacaoMovimentos(ComponenteConciliacaoMovimentosRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                // Validar se o utilizador tem as permissões necessárias
                bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.ConfigurarTarefas, _unitOfWork, CRUD.UPDATE);

                if (!permission)
                {
                    response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                    return response;
                }
                Componenteconciliacaomovimentos existeComponenteBD = new Componenteconciliacaomovimentos();
                existeComponenteBD = _unitOfWork.ComponenteConciliacaoMovimentosRepository.GetByIdTarefa(request.ComponenteConciliacaoMovimentos.tarefaFk);

                Componenteconciliacaomovimentos componente = BuildComponenteConciliacaoMovimentosObject(request.ComponenteConciliacaoMovimentos);

                if (componente.Id > 0)
                    _unitOfWork.ComponenteConciliacaoMovimentosRepository.Update(componente);
                else if (componente.Id == 0)
                {
                    if (existeComponenteBD != null && existeComponenteBD.Id > 0)
                    {
                        existeComponenteBD.IndActivo = false;
                        existeComponenteBD = _utils.UpdateDetailsToEntity(existeComponenteBD);
                        _unitOfWork.ComponenteConciliacaoMovimentosRepository.Update(existeComponenteBD);
                    }
                    else
                    {
                        _unitOfWork.ComponenteConciliacaoMovimentosRepository.Add(componente);
                    }
                }

                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }

        public ResponseBaseDataContract EditarComponenteReceita(ComponenteReceitaRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                // Validar se o utilizador tem as permissões necessárias
                bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.ConfigurarTarefas, _unitOfWork, CRUD.UPDATE);

                if (!permission)
                {
                    response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                    return response;
                }
                Componentereceita existeComponenteBD = new Componentereceita();
                existeComponenteBD = _unitOfWork.ComponenteReceitaRepository.GetByIdTarefa(request.ComponenteReceita.tarefaFk);

                Componentereceita componente = BuildComponenteReceitaObject(request.ComponenteReceita);

                if (componente.Id > 0)
                    _unitOfWork.ComponenteReceitaRepository.Update(componente);
                else if (componente.Id == 0)
                {
                    if (existeComponenteBD != null && existeComponenteBD.Id > 0)
                    {
                        existeComponenteBD.IndActivo = false;
                        existeComponenteBD = _utils.UpdateDetailsToEntity(existeComponenteBD);
                        _unitOfWork.ComponenteReceitaRepository.Update(existeComponenteBD);
                    }
                    else
                    {
                        _unitOfWork.ComponenteReceitaRepository.Add(componente);
                    }
                }

                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }

        private Componentetexto BuildComponenteTextoObject(ComponenteTexto request)
        {
            ComponenteTextoDto componenteTexto = new ComponenteTextoDto
            {
                Id = request.id,
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

            if (componenteTexto.Id > 0)
            {
                Componentetexto original = _unitOfWork.ComponenteTextoRepository.Get(componenteTexto.Id);
                componenteTexto.UtilizadorCriacao = original.UtilizadorCriacao;
                componenteTexto.DataCriacao = original.DataCriacao;
                componenteTexto = _utils.UpdateDetailsToEntity(componenteTexto);
            }
            else
                componenteTexto = _utils.SetDetailsToEntity(componenteTexto);
            return Utils.MappClassFromDto<ComponenteTextoDto, Componentetexto>(componenteTexto);
        }

        private Componenteaccoestarefa BuildComponenteAccoesTarefaObject(ComponenteAccoesTarefa request, int tarefaFk)
        {
            ComponenteAccoesTarefaDto componenteAccoesTarefa = new ComponenteAccoesTarefaDto
            {
                Id = request.id,
                TarefaFk = tarefaFk,
                TarefaSeguirFk = request.idTarefa,
                ApelidoDaTarefa = request.apelidoTarefa,
                IndActivo = true,
            };
            if (componenteAccoesTarefa.Id > 0)
            {
                Componenteaccoestarefa original = _unitOfWork.ComponenteAccoesTarefaRepository.Get(componenteAccoesTarefa.Id);
                componenteAccoesTarefa.TarefaSeguirFk = original.TarefaSeguirFk;
                componenteAccoesTarefa.UtilizadorCriacao = original.UtilizadorCriacao;
                componenteAccoesTarefa.DataCriacao = original.DataCriacao;
                componenteAccoesTarefa = _utils.UpdateDetailsToEntity(componenteAccoesTarefa);
            }
            else
                componenteAccoesTarefa = _utils.SetDetailsToEntity(componenteAccoesTarefa);
            return Utils.MappClassFromDto<ComponenteAccoesTarefaDto, Componenteaccoestarefa>(componenteAccoesTarefa);
        }

        private Componentecarregardocumento BuildComponenteCarregarDocumentosTarefaObject(ComponenteDocumentoTarefa request, int tarefaFk)
        {
            ComponenteCarregarDocumentoDto componenteCarregarDocumento = new ComponenteCarregarDocumentoDto
            {
                Id = request.id,
                TarefaFk = tarefaFk,
                DocumentoFk = request.idDocumento,
                Obrigatorio = request.obrigatorio,
                IndActivo = true,
            };

            if (componenteCarregarDocumento.Id > 0)
            {
                Componentecarregardocumento original = _unitOfWork.ComponenteCarregarDocumentoRepository.Get(componenteCarregarDocumento.Id);
                componenteCarregarDocumento.UtilizadorCriacao = original.UtilizadorCriacao;
                componenteCarregarDocumento.DataCriacao = original.DataCriacao;
                componenteCarregarDocumento = _utils.UpdateDetailsToEntity(componenteCarregarDocumento);
            }
            else
                componenteCarregarDocumento = _utils.SetDetailsToEntity(componenteCarregarDocumento);
            return Utils.MappClassFromDto<ComponenteCarregarDocumentoDto, Componentecarregardocumento>(componenteCarregarDocumento);
        }

        private Componenteclassificacaosubclassific BuildComponenteClassificacaoTarefaObject(ComponenteClassificacaoSubClassificTarefa request, int tarefaFk)
        {
            ComponenteClassificacaoSubClassificDto componenteClassificacaoTarefa = new ComponenteClassificacaoSubClassificDto
            {
                Id = request.id,
                TarefaFk = tarefaFk,
                SubclassificacaoFk = request.idSubClassificacao,
                IndActivo = true,
            };
            if (componenteClassificacaoTarefa.Id > 0)
            {
                Componenteclassificacaosubclassific original = _unitOfWork.ComponenteClassificacaoSubClassificRepository.Get(componenteClassificacaoTarefa.Id);
                componenteClassificacaoTarefa.UtilizadorCriacao = original.UtilizadorCriacao;
                componenteClassificacaoTarefa.DataCriacao = original.DataCriacao;
                componenteClassificacaoTarefa = _utils.UpdateDetailsToEntity(componenteClassificacaoTarefa);
            }
            else
                componenteClassificacaoTarefa = _utils.SetDetailsToEntity(componenteClassificacaoTarefa);

            return Utils.MappClassFromDto<ComponenteClassificacaoSubClassificDto, Componenteclassificacaosubclassific>(componenteClassificacaoTarefa);
        }

        private Componentecontroleacesso BuildControleAcessoPerfil(ComponenteAcessoPerfil request, int tarefaFk)
        {
            ComponenteControleAcessoDto newEntity = new ComponenteControleAcessoDto
            {
                Id = request.id,
                TarefaFk = tarefaFk,
                PerfilFk = request.idPerfil,
                IndActivo = true,
            };
            if (newEntity.Id > 0)
            {
                Componentecontroleacesso original = _unitOfWork.ComponenteControloAcessoRepository.Get(newEntity.Id);
                newEntity.UtilizadorCriacao = original.UtilizadorCriacao;
                newEntity.DataCriacao = original.DataCriacao;
                newEntity = _utils.UpdateDetailsToEntity(newEntity);
            }
            else
                newEntity = _utils.SetDetailsToEntity(newEntity);
            return Utils.MappClassFromDto<ComponenteControleAcessoDto, Componentecontroleacesso>(newEntity);
        }

        private Componentecontroleacesso BuildControleAcessoUtilizador(ComponenteAcessoUtilizador request, int tarefaFk)
        {
            ComponenteControleAcessoDto newEntity = new ComponenteControleAcessoDto
            {
                Id = request.id,
                TarefaFk = tarefaFk,
                UtilizadorFk = request.idUtilizador,
                IndActivo = true,
            };
            if (newEntity.Id > 0)
            {
                Componentecontroleacesso original = _unitOfWork.ComponenteControloAcessoRepository.Get(newEntity.Id);
                newEntity.UtilizadorCriacao = original.UtilizadorCriacao;
                newEntity.DataCriacao = original.DataCriacao;
                newEntity = _utils.UpdateDetailsToEntity(newEntity);
            }
            else
                newEntity = _utils.SetDetailsToEntity(newEntity);

            return Utils.MappClassFromDto<ComponenteControleAcessoDto, Componentecontroleacesso>(newEntity);
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
                VisualiazarDespesaC = componente.visualiazarDespesaC,
                VisualizarDespesaComCompromisso = componente.visualizarDespesaComCompromisso,
                ExecutarPagamentos = componente.executarPagamentos,
                VisualizarExecucaoDespesaCabimentada = componente.visualizarExecucaoDespesaCabimentada,
                EmitirOrdemPagamento = componente.emitirOrdemPagamento,
                TarefaFk = componente.tarefaFk,
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

        private Componenteconciliacaomovimentos BuildComponenteConciliacaoMovimentosObject(ComponenteConciliacaoMovimentosDataContract componente)
        {
            ComponenteconciliacaomovimentosDto componenteDto = new ComponenteconciliacaomovimentosDto
            {
                Id = componente.id,
                TarefaFk = componente.tarefaFk,
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
                TarefaFk = componente.tarefaFk,
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
    }
}