using System;
using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class InstitutionDataManager : IInstitutionDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public InstitutionDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }


        public SelectDescriptionResponse GetAllInstitutionAtivo()
        {
            SelectDescriptionResponse response = new SelectDescriptionResponse();
            try
            {
                List<SelectDescription> selects = _unitOfWork.InstitutionRepository.GetAllInstitutionAtivo();
                response.selects = selects;
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }
    }
}