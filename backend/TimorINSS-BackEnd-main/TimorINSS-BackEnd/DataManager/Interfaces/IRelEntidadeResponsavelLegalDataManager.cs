using System.Collections.Generic;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IRelEntidadeResponsavelLegalDataManager
    {
        public IEnumerable<Relentidaderesplegal> GetAll();

        public Relentidaderesplegal Get(long id);

        public RelentidaderesplegalDto GetDto(long id);

        public void Add(Relentidaderesplegal entity);

        public void Update(Relentidaderesplegal entity);

        public void Delete(Relentidaderesplegal entity);
    }
}