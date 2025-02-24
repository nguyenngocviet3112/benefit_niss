using System;
using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class SectorActividadeDataManager : ISectorActividadeDataManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public SectorActividadeDataManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public SelectDescriptionResponse GetAllSectorActividade()
        {
            SelectDescriptionResponse response = new SelectDescriptionResponse();
            try
            {
                List<SelectDescription> selects = _unitOfWork.SectorActividadeRepository.GetAllSectorActividade();
                response.selects = selects;
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }

        public SectoractividadeDto GetDto(int id)
        {
            return _unitOfWork.SectorActividadeRepository.GetDto(id);
        }
    }
}