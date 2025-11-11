using System;
using System.Collections.Generic;
using System.Text;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class MoradaDataManager : IMoradaDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public MoradaDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        public IEnumerable<Morada> GetAll()
        {
            return _unitOfWork.MoradaRepository.GetAll();
        }

        public Morada Get(long id)
        {
            return _unitOfWork.MoradaRepository.Get(id);
        }

        public MoradaDto GetDto(long id)
        {
            return _unitOfWork.MoradaRepository.GetDto(id);
        }

        public void Add(Morada entity)
        {
            _unitOfWork.MoradaRepository.Add(entity);
            _unitOfWork.Commit();
        }

        public void Update(Morada entity)
        {
            _unitOfWork.MoradaRepository.Update(entity);
            _unitOfWork.Commit();
        }

        public void Delete(Morada entity)
        {
            _unitOfWork.MoradaRepository.Delete(entity);
            _unitOfWork.Commit();
        }

        public MoradaListagemResponse GetMoradasByIdEntidadeEmpregadora(MoradaListagemRequest request)
        {
            MoradaListagemResponse response = new MoradaListagemResponse();

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
                        response = _unitOfWork.MoradaRepository.GetMoradasByFilter(request);
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

        public MoradaListagemResponse GetMoradasByIdTrabalhador(MoradaListagemRequest request)
        {
            MoradaListagemResponse response = new MoradaListagemResponse();
            try
            {
                if (request.filter != null)
                {
                    bool valid = false;
                    var userId = request.UserId;
                    var user = _unitOfWork.UtilizadoresRepository.Get(userId);
                    //var trabalhadorId = request.Id;
                    var entidadeEmpregadoraId = user.UtilizadorEntidadeFk;

                    int trabalhadorId = IdDecoder.DecodeId(request.IdStr); ;


                    // Validar se o utilizador tem as permissões necessárias
                    if (user.Interno == true)
                        valid = true;
                    else if (entidadeEmpregadoraId.HasValue)
                        valid = _unitOfWork.RelEntidadeTrabalhadorRepository.IsTrabalhadorAssociadoEntidadeEmpregadora(trabalhadorId, entidadeEmpregadoraId.Value);

                    if (valid)
                    {
                        request.filter.filterField = "TRABALHADOR";
                        response = _unitOfWork.MoradaRepository.GetMoradasByFilter(request);
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

        public MoradaListagemResponse SaveMorada(MoradaRequest request)
        {
            var response = new MoradaListagemResponse { RequestId = request.RequestId };

            //Build Objects
            Morada morada = BuildMoradaObject(request);

            bool saveMorada = true;
            Morada moradaBD = _unitOfWork.MoradaRepository.GetMoradasIguais(morada);

            if (moradaBD != null)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.MoradaDuplicada).ToString(),
                    ErrorMessage = ErrorsDataContract.MoradaDuplicada.ToString()
                });
                saveMorada = false;
            }

            //Insert in DB
            try
            {
                if (saveMorada)
                {
                    //caso exista morada principal
                    if (morada.MoradaPrincipal)
                    {
                        //atualizar as outras moradas com morada Principal a false
                        _unitOfWork.MoradaRepository.UpdateMoradaPrincipal(morada);
                    }
                    _unitOfWork.MoradaRepository.Add(morada);
                    _unitOfWork.Commit();
                }
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                _unitOfWork.Rollback();
            }
            return response;
        }

        public MoradaListagemResponse UpdateMorada(MoradaRequest request)
        {
            var response = new MoradaListagemResponse { RequestId = request.RequestId };

            //Build Objects
            Morada morada = BuildMoradaObject(request);

            bool updateMorada = true;
            Morada moradaBD = _unitOfWork.MoradaRepository.GetMoradasIguais(morada);

            if (moradaBD != null && moradaBD.IdMorada != morada.IdMorada)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.MoradaDuplicada).ToString(),
                    ErrorMessage = ErrorsDataContract.MoradaDuplicada.ToString()
                });
                updateMorada = false;
            }

            //Insert in DB
            try
            {
                if (updateMorada)
                {
                    //caso exista morada principal
                    if (morada.MoradaPrincipal)
                    {
                        //atualizar as outras moradas com morada Principal a false
                        _unitOfWork.MoradaRepository.UpdateMoradaPrincipal(morada);
                    }
                    _unitOfWork.MoradaRepository.Update(morada);
                    _unitOfWork.Commit();
                }
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                _unitOfWork.Rollback();
            }
            return response;
        }

        public ResponseBaseDataContract DeleteMorada(MoradaDeleteRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            Morada morada = _unitOfWork.MoradaRepository.Get(request.Id);

            if (morada == null)
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.EntityDoesNotExist).ToString(), ErrorMessage = ErrorsDataContract.EntityDoesNotExist.ToString() });

            if (response.Errors.Count > 0)
            {
                return response;
            }

            //Delete in DB
            try
            {
                _unitOfWork.MoradaRepository.Delete(morada);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                _unitOfWork.Rollback();
            }
            return response;
        }

        private Morada BuildMoradaObject(MoradaRequest request)
        {
            MoradaDto morada = new MoradaDto
            {
                IdMorada = request.Morada.IdMorada,
                MoradaAldeiaFk = request.Morada.MoradaAldeiaFk == 0 ? null : request.Morada.MoradaAldeiaFk,
                Rua = request.Morada.Rua,
                NumPorta = request.Morada.NumPorta,
                MoradaPaisFk = request.Morada.MoradaPaisFk,
                MoradaPrincipal = request.Morada.MoradaPrincipal,
                EntidadeMoradaFk = request.Morada.IdEntidadeEmpreg,
                TrabalhadorMoradaFk = request.Morada.IdTrabalhador,
                FlagImportado = false,
                DataCriacao = DateTime.Now,
            };
            morada = _utils.SetDetailsToEntity<MoradaDto>(morada);
            return Utils.MappClassFromDto<MoradaDto, Morada>(morada);
        }
    }
}