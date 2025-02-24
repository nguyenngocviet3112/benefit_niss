using System;
using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class PostoAdministrativoDataManager : IPostoAdministrativoDataManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public PostoAdministrativoDataManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Postoadministrativo> GetAll()
        {
            return _unitOfWork.PostoAdministrativoRepository.GetAll();
        }

        public Postoadministrativo Get(long id)
        {
            return _unitOfWork.PostoAdministrativoRepository.Get(id);
        }

        public PostoadministrativoDto GetDto(long id)
        {
            return _unitOfWork.PostoAdministrativoRepository.GetDto(id);
        }

        public void Add(Postoadministrativo entity)
        {
            _unitOfWork.PostoAdministrativoRepository.Add(entity);
            _unitOfWork.Commit();
        }

        public void Update(Postoadministrativo entity)
        {
            _unitOfWork.PostoAdministrativoRepository.Update(entity);
            _unitOfWork.Commit();
        }

        public void Delete(Postoadministrativo entity)
        {
            _unitOfWork.PostoAdministrativoRepository.Delete(entity);
            _unitOfWork.Commit();
        }

        public SelectDescriptionResponse getAllPostoAdministrativo()
        {
            SelectDescriptionResponse response = new SelectDescriptionResponse();
            try
            {
                List<SelectDescription> selects = _unitOfWork.PostoAdministrativoRepository.getAllPostoAdministrativo();
                response.selects = selects;
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }

        public SelectDescriptionResponse getPostoByIdMunicipio(int id)
        {
            SelectDescriptionResponse response = new SelectDescriptionResponse();
            try
            {
                List<SelectDescription> selects = _unitOfWork.PostoAdministrativoRepository.getPostoByIdMunicipio(id);
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