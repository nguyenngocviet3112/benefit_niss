using System;
using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class ImporterDataManager : IImporterDataManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public ImporterDataManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void DeleteOld()
        {
            _unitOfWork.ImporterRepository.DeleteOld();
        }
    }
}