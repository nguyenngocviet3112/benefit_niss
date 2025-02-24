using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IPostoAdministrativoDataManager
    {
        public IEnumerable<Postoadministrativo> GetAll();

        public Postoadministrativo Get(long id);

        public PostoadministrativoDto GetDto(long id);

        public void Add(Postoadministrativo entity);

        public void Update(Postoadministrativo entity);

        public void Delete(Postoadministrativo entity);

        public SelectDescriptionResponse getAllPostoAdministrativo();

        public SelectDescriptionResponse getPostoByIdMunicipio(int id);
    }
}