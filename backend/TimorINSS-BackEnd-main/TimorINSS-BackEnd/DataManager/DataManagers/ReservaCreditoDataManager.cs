using System;
using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class ReservaCreditoDataManager : IReservaCreditoDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public ReservaCreditoDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        public IEnumerable<Reservacredito> GetAll()
        {
            return _unitOfWork.ReservaCreditoRepository.GetAll();
        }

        public Reservacredito Get(long id)
        {
            return _unitOfWork.ReservaCreditoRepository.Get(id);
        }

        public ReservaCreditoDto GetDto(long id)
        {
            return _unitOfWork.ReservaCreditoRepository.GetDto(id);
        }

        public void Add(Reservacredito entity)
        {
            _unitOfWork.ReservaCreditoRepository.Add(entity);
            _unitOfWork.Commit();
        }

        public void Update(Reservacredito entity)
        {
            _unitOfWork.ReservaCreditoRepository.Update(entity);
            _unitOfWork.Commit();
        }

        public void Delete(Reservacredito entity)
        {
            _unitOfWork.ReservaCreditoRepository.Delete(entity);
            _unitOfWork.Commit();
        }

        public ReservaCreditoListagemResponse GetReservaCreditoByIdEntidade(ReservaCreditoListagemRequest request)
        {
            ReservaCreditoListagemResponse response = new ReservaCreditoListagemResponse();
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
                        response = _unitOfWork.ReservaCreditoRepository.GetReservaCreditoByIdEntidade(request);
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
    }
}