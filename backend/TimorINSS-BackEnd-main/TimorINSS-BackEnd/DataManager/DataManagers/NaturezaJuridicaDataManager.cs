using System;
using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class NaturezaJuridicaDataManager : INaturezaJuridicaDataManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public NaturezaJuridicaDataManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public SelectDescriptionResponse GetAllNaturezaJuridica()
        {
            SelectDescriptionResponse response = new SelectDescriptionResponse();
            try
            {
                List<SelectDescription> selects = _unitOfWork.NaturezaJuridicaRepository.GetAllNaturezaJuridica();
                response.selects = selects;
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }

        public NaturezajuridicaDto GetDto(int id)
        {
            return _unitOfWork.NaturezaJuridicaRepository.GetDto(id);
        }
    }
}