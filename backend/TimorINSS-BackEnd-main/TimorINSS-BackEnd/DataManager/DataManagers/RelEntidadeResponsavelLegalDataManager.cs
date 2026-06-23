using System.Collections.Generic;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class RelEntidadeResponsavelLegalDataManager : IRelEntidadeResponsavelLegalDataManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public RelEntidadeResponsavelLegalDataManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Relentidaderesplegal> GetAll()
        {
            return _unitOfWork.RelEntidadeResponsavelLegalRepository.GetAll();
        }

        public Relentidaderesplegal Get(long id)
        {
            return _unitOfWork.RelEntidadeResponsavelLegalRepository.Get(id);
        }

        public RelentidaderesplegalDto GetDto(long id)
        {
            return _unitOfWork.RelEntidadeResponsavelLegalRepository.GetDto(id);
        }

        public void Add(Relentidaderesplegal entity)
        {
            _unitOfWork.RelEntidadeResponsavelLegalRepository.Add(entity);
            _unitOfWork.Commit();
        }

        public void Update(Relentidaderesplegal entity)
        {
            _unitOfWork.RelEntidadeResponsavelLegalRepository.Update(entity);
            _unitOfWork.Commit();
        }

        public void Delete(Relentidaderesplegal entity)
        {
            _unitOfWork.RelEntidadeResponsavelLegalRepository.Delete(entity);
            _unitOfWork.Commit();
        }
    }
}