using System;
using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class PaisDataManager : IPaisDataManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public PaisDataManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public PaisDto Get(int id)
        {
            return _unitOfWork.PaisRepository.GetDto(id);
        }

        public SelectDescriptionResponse getPais()
        {
            SelectDescriptionResponse response = new SelectDescriptionResponse();
            try
            {
                List<SelectDescription> dropdowns = _unitOfWork.PaisRepository.getAllPais();
                response.selects = dropdowns;
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }
    }
}