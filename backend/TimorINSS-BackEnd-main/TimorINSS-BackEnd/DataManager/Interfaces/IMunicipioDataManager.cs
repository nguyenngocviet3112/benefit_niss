using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IMunicipioDataManager
    {
        public IEnumerable<Municipio> GetAll();

        public Municipio Get(long id);

        public MunicipioDto GetDto(long id);

        public void Add(Municipio entity);

        public void Update(Municipio entity);

        public void Delete(Municipio entity);

        public SelectDescriptionResponse getAllMunicipio();
    }
}