using System;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class RelTarefaComponenteDataManager : IRelTarefaComponenteDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public RelTarefaComponenteDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        public ResponseBaseDataContract UpdateRelTarefaComponente(UpdateRelTarefaComponenteRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.ConfigurarTarefas, _unitOfWork, CRUD.UPDATE);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            // validações

            Reltarefacomponente rel = new Reltarefacomponente();
            List<Reltarefacomponente> relUpdate = new List<Reltarefacomponente>();
            List<Reltarefacomponente> relAdicionar = new List<Reltarefacomponente>();

            int ordem = 1;

            if (request.componenteListagem != null && request.componenteListagem.Count > 0)
            {
                foreach (var tarefaComponente in request.componenteListagem)
                {
                    rel = _unitOfWork.RelTarefaComponenteRepository.GetRelTarefaComponenteByIdTarefaIdComponente(tarefaComponente.id, request.idTarefa);

                    //Update
                    if (rel != null && rel.Id > 0)
                    {
                        //Se não estiver selecionado e existir, é para inativar
                        if (!tarefaComponente.select)
                        {
                            rel.IndActivo = false;
                            rel.Ordem = 0;

                            if (ordem > 1)
                                ordem = ordem + 1;
                        }
                        else
                        {
                            rel.Ordem = ordem;
                            ordem = ordem + 1;
                        }
                        rel.Expandir = tarefaComponente.expandir;
                        rel = _utils.UpdateDetailsToEntity(rel);
                        relUpdate.Add(rel);
                    }
                    //Adiciona um novo
                    else
                    {
                        if (tarefaComponente.select)
                        {
                            relAdicionar.Add(BuildInsertRelTarefaComponenteObject(tarefaComponente, ordem));
                            ordem = ordem + 1;
                        }
                    }
                }
            }

            try
            {
                if (relUpdate != null && relUpdate.Count > 0)
                {
                    foreach (var update in relUpdate)
                    {
                        _unitOfWork.RelTarefaComponenteRepository.Update(update);
                    }
                }

                if (relAdicionar != null && relAdicionar.Count > 0)
                {
                    foreach (var add in relAdicionar)
                    {
                        add.TarefaFk = request.idTarefa;
                        _unitOfWork.RelTarefaComponenteRepository.Add(add);
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

        public ComponentesListagemResponse GetAllRelTarefaComponenteByIdTarefaActivo(GetAllRelTarefaComponenteByIdTarefaActivoRequest request)
        {
            ComponentesListagemResponse response = new ComponentesListagemResponse();

            Tarefaativo tarefaActivo = _unitOfWork.TarefaAtivoRepository.Get(request.IdTarefaActivo);

            if (tarefaActivo != null)
            {
                response.componentes = _unitOfWork.RelTarefaComponenteRepository.GetRelTarefaComponenteByIdTarefa(tarefaActivo.TarefaconfigFk)
                    .OrderBy(c => c.ordem).ToList();
            }

            return response;
        }

        private Reltarefacomponente BuildInsertRelTarefaComponenteObject(Componentes request, int ordem)
        {
            Reltarefacomponente tarefaComponente = new Reltarefacomponente()
            {
                ComponenteFk = request.id,
                IndActivo = true,
                Expandir = request.expandir,
                Ordem = ordem
            };
            tarefaComponente = _utils.SetDetailsToEntity(tarefaComponente);
            return tarefaComponente;
        }
    }
}