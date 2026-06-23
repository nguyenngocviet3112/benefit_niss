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
    public class PerfilDataManager : IPerfilDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public PerfilDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        public PerfilListagemResponse GetAllPerfis(SearchFilterRequest request)
        {
            PerfilListagemResponse response = new PerfilListagemResponse();

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.GestaoPerfil, _unitOfWork, CRUD.READ);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            try
            {
                response = _unitOfWork.PerfilRepository.GetAllPerfis(request);
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }

        public ResponseBaseDataContract UpdatePerfil(PerfilRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.GestaoPerfil, _unitOfWork, CRUD.UPDATE);
            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            //validações
            Perfil perfil = _unitOfWork.PerfilRepository.Get(request.Id);

            List<int> listaPerfis = new List<int>();
            listaPerfis.Add(request.Id);

            List<Relperfilfuncionalidade> relPerfilFuncionalidade = _unitOfWork.RelPerfilFuncionalidadeRepository.GetFuncionalidadeByPerfil(listaPerfis);

            if (perfil == null)
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.EntityDoesNotExist).ToString(), ErrorMessage = ErrorsDataContract.EntityDoesNotExist.ToString() });

            if (response.Errors.Count > 0)
            {
                return response;
            }

            if (relPerfilFuncionalidade != null && relPerfilFuncionalidade.Count > 0)
            {
                foreach (var rel in relPerfilFuncionalidade)
                {
                    rel.IndActivo = perfil.IndActivo ? false : true;
                }
            }

            if (perfil.IndActivo)
            {
                perfil.IndActivo = false;
            }
            else
                perfil.IndActivo = true;

            //update in DB
            try
            {
                _unitOfWork.PerfilRepository.Update(perfil);

                if (relPerfilFuncionalidade != null && relPerfilFuncionalidade.Count > 0)
                {
                    foreach (var rel in relPerfilFuncionalidade)
                    {
                        _unitOfWork.RelPerfilFuncionalidadeRepository.Update(rel);
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

        public ResponseBaseDataContract AddPerfil(AddPerfilRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };
            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.GestaoPerfil, _unitOfWork, CRUD.CREATE);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            Perfil perfil = BuildPerfilObject(request);

            List<Relperfilfuncionalidade> ListRelPerfilFuncionalidade = new List<Relperfilfuncionalidade>();

            if (request.Funcionalidade != null && request.Funcionalidade.Count > 0)
            {
                foreach (var funcionalidade in request.Funcionalidade)
                {
                    ListRelPerfilFuncionalidade.Add(BuildRelacaoPerfilFuncionalidadeObject(funcionalidade));
                }
            }

            try
            {
                _unitOfWork.PerfilRepository.Add(perfil);

                if (ListRelPerfilFuncionalidade != null && ListRelPerfilFuncionalidade.Count > 0)
                {
                    foreach (var relPerfilFuncionalidade in ListRelPerfilFuncionalidade)
                    {
                        relPerfilFuncionalidade.PerfilFkNavigation = perfil;
                        _unitOfWork.RelPerfilFuncionalidadeRepository.Add(relPerfilFuncionalidade);
                    }
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

        public ResponseBaseDataContract EditPerfil(AddPerfilRequest request)
        {
            var response = new ResponseBaseDataContract();
            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.GestaoPerfil, _unitOfWork, CRUD.UPDATE);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            //validações
            List<Relperfilfuncionalidade> listaRelacaoPerfilFuncionalidade = new List<Relperfilfuncionalidade>();

            Perfil perfil = new Perfil();
            if (request.idPerfil != null && request.idPerfil > 0)
            {
                List<int> listaIdPerfil = new List<int>();
                listaIdPerfil.Add((int)request.idPerfil);
                listaRelacaoPerfilFuncionalidade = _unitOfWork.RelPerfilFuncionalidadeRepository.GetFuncionalidadeByPerfil(listaIdPerfil);

                perfil = BuildPerfilObject(request);
            }
            else
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.FilterDoesNotExist).ToString(),
                    ErrorMessage = ErrorsDataContract.FilterDoesNotExist.ToString()
                });

                return response;
            }

            List<Relperfilfuncionalidade> listaRelPerfilFuncionalidade = new List<Relperfilfuncionalidade>();

            if (request.Funcionalidade != null && request.Funcionalidade.Count > 0)
            {
                foreach (var funcionalidade in request.Funcionalidade)
                {
                    listaRelPerfilFuncionalidade.Add(BuildRelacaoPerfilFuncionalidadeObject(funcionalidade));
                }
            }

            try
            {
                //remover os esxistentes
                if (listaRelacaoPerfilFuncionalidade != null && listaRelacaoPerfilFuncionalidade.Count > 0)
                {
                    foreach (var relPerfilFuncionalidade in listaRelacaoPerfilFuncionalidade)
                    {
                        _unitOfWork.RelPerfilFuncionalidadeRepository.Delete(relPerfilFuncionalidade);
                    }
                }

                //adicionar as relações
                if (listaRelPerfilFuncionalidade != null && listaRelPerfilFuncionalidade.Count > 0)
                {
                    foreach (var relPerfilFuncionalidade in listaRelPerfilFuncionalidade)
                    {
                        relPerfilFuncionalidade.PerfilFk = (int)request.idPerfil;
                        _unitOfWork.RelPerfilFuncionalidadeRepository.Add(relPerfilFuncionalidade);
                    }
                }

                //update do perfil
                _unitOfWork.PerfilRepository.Update(perfil);

                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
                _unitOfWork.Rollback();
            }

            return response;
        }

        public SelectDescriptionResponse GetAllPerfisAtivo()
        {
            SelectDescriptionResponse response = new SelectDescriptionResponse();
            try
            {
                List<SelectDescription> selects = _unitOfWork.PerfilRepository.GetAllPerfisAtivo();
                response.selects = selects;
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }

        private Perfil BuildPerfilObject(AddPerfilRequest request)
        {
            PerfilDto perfil = new PerfilDto
            {
                Id = request.idPerfil != null ? (int)request.idPerfil : 0,
                Descricao = request.Descricao,
                DataCriacao = DateTime.Now,
                IndActivo = true,
            };

            if (perfil.Id > 0)
            {
                Perfil original = _unitOfWork.PerfilRepository.Get(perfil.Id);
                perfil.UtilizadorCriacao = original.UtilizadorCriacao;
                perfil.DataCriacao = original.DataCriacao;
                perfil = _utils.UpdateDetailsToEntity(perfil);
            }
            else
                perfil = _utils.SetDetailsToEntity(perfil);

            return Utils.MappClassFromDto<PerfilDto, Perfil>(perfil);
        }

        private Relperfilfuncionalidade BuildRelacaoPerfilFuncionalidadeObject(FuncionalidadeDataContract request)
        {
            RelPerfilFuncionalidadeDto relPerfilFuncionalidade = new RelPerfilFuncionalidadeDto
            {
                FuncionalidadeFk = request.id,
                Create = request.create,
                Read = request.read,
                Update = request.update,
                Delete = request.delete,
                DataCriacao = DateTime.Now,
                IndActivo = true,
            };
            if (relPerfilFuncionalidade.Id > 0)
            {
                Relperfilfuncionalidade original = _unitOfWork.RelPerfilFuncionalidadeRepository.Get(relPerfilFuncionalidade.Id);
                relPerfilFuncionalidade.UtilizadorCriacao = original.UtilizadorCriacao;
                relPerfilFuncionalidade.DataCriacao = original.DataCriacao;
                relPerfilFuncionalidade = _utils.UpdateDetailsToEntity(relPerfilFuncionalidade);
            }
            else
                relPerfilFuncionalidade = _utils.SetDetailsToEntity(relPerfilFuncionalidade);
            return Utils.MappClassFromDto<RelPerfilFuncionalidadeDto, Relperfilfuncionalidade>(relPerfilFuncionalidade);
        }
    }
}