using System;
using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class RelUtilizadorDepartamentoDataManager : IRelUtilizadorDepartamentoDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public RelUtilizadorDepartamentoDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        public DepartamentoListagemResponse GetDepartamentosByUserId(int id)
        {
            DepartamentoListagemResponse response = new DepartamentoListagemResponse();
            try
            {
                response = _unitOfWork.RelUtilizadorDepartamentoRepository.GetDepartamentosByUserId(id);
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }
    }
}