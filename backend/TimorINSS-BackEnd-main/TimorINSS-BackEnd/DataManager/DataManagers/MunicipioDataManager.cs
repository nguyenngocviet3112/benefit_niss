using System;
using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class MunicipioDataManager : IMunicipioDataManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public MunicipioDataManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Municipio> GetAll()
        {
            return _unitOfWork.MunicipioRepository.GetAll();
        }

        public Municipio Get(long id)
        {
            return _unitOfWork.MunicipioRepository.Get(id);
        }

        public MunicipioDto GetDto(long id)
        {
            return _unitOfWork.MunicipioRepository.GetDto(id);
        }

        public void Add(Municipio entity)
        {
            _unitOfWork.MunicipioRepository.Add(entity);
            _unitOfWork.Commit();
        }

        public void Update(Municipio entity)
        {
            _unitOfWork.MunicipioRepository.Update(entity);
            _unitOfWork.Commit();
        }

        public void Delete(Municipio entity)
        {
            _unitOfWork.MunicipioRepository.Delete(entity);
            _unitOfWork.Commit();
        }

        public SelectDescriptionResponse getAllMunicipio()
        {
            SelectDescriptionResponse response = new SelectDescriptionResponse();
            try
            {
                List<SelectDescription> dropdowns = _unitOfWork.MunicipioRepository.getAllMunicipio();
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