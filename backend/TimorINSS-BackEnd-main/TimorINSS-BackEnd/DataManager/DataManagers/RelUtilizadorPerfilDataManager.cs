using System;
using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class RelUtilizadorPerfilDataManager : IRelUtilizadorPerfilDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public RelUtilizadorPerfilDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        public PerfilListagemResponse GetPerfisByUserId(int id)
        {
            PerfilListagemResponse response = new PerfilListagemResponse();
            try
            {
                response = _unitOfWork.RelUtilizadorPerfilRepository.GetPerfisByUserId(id);
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }
    }
}