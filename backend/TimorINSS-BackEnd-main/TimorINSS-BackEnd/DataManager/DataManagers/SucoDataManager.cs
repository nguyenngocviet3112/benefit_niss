using System;
using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class SucoDataManager : ISucoDataManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public SucoDataManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Suco> GetAll()
        {
            return _unitOfWork.SucoRepository.GetAll();
        }

        public Suco Get(long id)
        {
            return _unitOfWork.SucoRepository.Get(id);
        }

        public SucoDto GetDto(long id)
        {
            return _unitOfWork.SucoRepository.GetDto(id);
        }

        public void Add(Suco entity)
        {
            _unitOfWork.SucoRepository.Add(entity);
            _unitOfWork.Commit();
        }

        public void Update(Suco entity)
        {
            _unitOfWork.SucoRepository.Update(entity);
            _unitOfWork.Commit();
        }

        public void Delete(Suco entity)
        {
            _unitOfWork.SucoRepository.Delete(entity);
            _unitOfWork.Commit();
        }

        public SelectDescriptionResponse getAllSuco()
        {
            SelectDescriptionResponse response = new SelectDescriptionResponse();
            try
            {
                List<SelectDescription> dropdowns = _unitOfWork.SucoRepository.getAllSuco();
                response.selects = dropdowns;
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }

        public SelectDescriptionResponse getSucoByIdPosto(int id)
        {
            SelectDescriptionResponse response = new SelectDescriptionResponse();
            try
            {
                List<SelectDescription> dropdowns = _unitOfWork.SucoRepository.getSucoByIdPosto(id);
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