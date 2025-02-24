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
    public class SuspensaoDataManager : ISuspensaoDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public SuspensaoDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        public IEnumerable<Suspensoes> GetAll()
        {
            return _unitOfWork.SuspensaoRepository.GetAll();
        }

        public Suspensoes Get(long id)
        {
            return _unitOfWork.SuspensaoRepository.Get(id);
        }

        public SuspensoesDto GetDto(long id)
        {
            return _unitOfWork.SuspensaoRepository.GetDto(id);
        }

        public void Add(Suspensoes entity)
        {
            _unitOfWork.SuspensaoRepository.Add(entity);
            _unitOfWork.Commit();
        }

        public void Update(Suspensoes entity)
        {
            _unitOfWork.SuspensaoRepository.Update(entity);
            _unitOfWork.Commit();
        }

        public void Delete(Suspensoes entity)
        {
            _unitOfWork.SuspensaoRepository.Delete(entity);
            _unitOfWork.Commit();
        }

        public SuspensaoResponse SaveSuspensao(SuspensaoRequest request)
        {
            var response = new SuspensaoResponse { RequestId = request.RequestId };

            DateTime begin = request.Suspensao.DataInicioSuspensao;
            DateTime end = request.Suspensao.DataFimSuspensao.HasValue ? request.Suspensao.DataFimSuspensao.Value : System.Data.SqlTypes.SqlDateTime.MaxValue.Value;

            //validações

            //Build Objects
            Suspensoes suspensao = BuildSuspensaoObject(request.Suspensao);

            //Insert in DB
            if (_unitOfWork.SuspensaoRepository.ExistSuspensao(begin, end, suspensao.EntidadeSuspensaoFk, suspensao.TrabalhadorSuspensaoFk))
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.SuspensaoAlreadyExists).ToString(),
                    ErrorMessage = ErrorsDataContract.SuspensaoAlreadyExists.ToString()
                });
                return response;
            }
            try
            {
                _unitOfWork.SuspensaoRepository.Add(suspensao);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                _unitOfWork.Rollback();
            }
            return response;
        }

        public SuspensaoListagemResponse GetSuspensaoByIdEntidadeEmpregadora(SuspensaoListagemRequest request)
        {
            SuspensaoListagemResponse response = new SuspensaoListagemResponse();
            try
            {
                if (request.filter != null)
                {
                    bool valid = false;
                    var userId = request.UserId;
                    var user = _unitOfWork.UtilizadoresRepository.Get(userId);
                    var entidadeEmpregadoraId = user.UtilizadorEntidadeFk;

                    // Validar se o utilizador tem as permissões necessárias
                    if (entidadeEmpregadoraId.HasValue || user.Interno == true)
                        valid = true;

                    if (valid)
                    {
                        request.filter.filterField = "ENTIDADEEMPREGADORA";
                        response = _unitOfWork.SuspensaoRepository.GetSuspensaoByFilter(request);
                    }
                    else
                    {
                        response.Errors.Add(new Error
                        {
                            ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(),
                            ErrorMessage = ErrorsDataContract.InvalidPermission.ToString()
                        });
                    }
                }
                else
                {
                    response.Errors.Add(new Error
                    {
                        ErrorCode = ((int)ErrorsDataContract.FilterDoesNotExist).ToString(),
                        ErrorMessage = ErrorsDataContract.FilterDoesNotExist.ToString()
                    });
                }
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }

        public SuspensaoListagemResponse GetSuspensaoByIdTrabalhador(SuspensaoListagemRequest request)
        {
            SuspensaoListagemResponse response = new SuspensaoListagemResponse();
            try
            {
                if (request.filter != null)
                {
                    bool valid = false;
                    var userId = request.UserId;
                    var user = _unitOfWork.UtilizadoresRepository.Get(userId);
                    var entidadeEmpregadoraId = user.UtilizadorEntidadeFk;

                    // Validar se o utilizador tem as permissões necessárias
                    if (entidadeEmpregadoraId.HasValue || user.Interno == true)
                        valid = true;

                    if (valid)
                    {
                        request.filter.filterField = "TRABALHADOR";
                        response = _unitOfWork.SuspensaoRepository.GetSuspensaoByFilter(request);
                    }
                    else
                    {
                        response.Errors.Add(new Error
                        {
                            ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(),
                            ErrorMessage = ErrorsDataContract.InvalidPermission.ToString()
                        });
                    }
                }
                else
                {
                    response.Errors.Add(new Error
                    {
                        ErrorCode = ((int)ErrorsDataContract.FilterDoesNotExist).ToString(),
                        ErrorMessage = ErrorsDataContract.FilterDoesNotExist.ToString()
                    });
                }
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }

        public ResponseBaseDataContract DeleteSuspensao(SuspensaoDeleteRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            //validações
            Suspensoes domainSuspensao = _unitOfWork.SuspensaoRepository.Get(request.Id);

            if (domainSuspensao == null)
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.EntityDoesNotExist).ToString(), ErrorMessage = ErrorsDataContract.EntityDoesNotExist.ToString() });

            if (response.Errors.Count > 0)
            {
                return response;
            }

            domainSuspensao.IndActivo = false;

            //Update in DB
            try
            {
                _unitOfWork.SuspensaoRepository.Update(domainSuspensao);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                _unitOfWork.Rollback();
            }
            return response;
        }

        private Suspensoes BuildSuspensaoObject(SuspensaoDataContract request)
        {
            SuspensoesDto suspensao = new SuspensoesDto
            {
                EntidadeSuspensaoFk = request.IdEntidade,
                TrabalhadorSuspensaoFk = request.IdTrabalhador > 0 ? request.IdTrabalhador : null,
                DataInicioSuspensao = request.DataInicioSuspensao,
                DataFimSuspensao = request.DataFimSuspensao,
                IndActivo = true
            };
            suspensao = _utils.SetDetailsToEntity<SuspensoesDto>(suspensao);
            return Utils.MappClassFromDto<SuspensoesDto, Suspensoes>(suspensao);
        }
    }
}