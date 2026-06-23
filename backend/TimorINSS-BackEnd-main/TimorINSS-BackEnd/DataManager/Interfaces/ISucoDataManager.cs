using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface ISucoDataManager
    {
        public IEnumerable<Suco> GetAll();

        public Suco Get(long id);

        public SucoDto GetDto(long id);

        public void Add(Suco entity);

        public void Update(Suco entity);

        public void Delete(Suco entity);

        public SelectDescriptionResponse getAllSuco();

        public SelectDescriptionResponse getSucoByIdPosto(int id);
    }
}