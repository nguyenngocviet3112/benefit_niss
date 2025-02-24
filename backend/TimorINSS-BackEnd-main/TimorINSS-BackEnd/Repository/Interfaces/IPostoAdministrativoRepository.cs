using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IPostoAdministrativoRepository : IDataRepository<Postoadministrativo, PostoadministrativoDto>
    {
        public List<SelectDescription> getAllPostoAdministrativo();

        public List<SelectDescription> getAllActivePostoAdministrativo();

        public List<SelectDescription> getPostoByIdMunicipio(int id);

        public ValueCampoEditavelListagemResponse getAllActivePostoAdministrativo(SearchFilter filter);

        public Postoadministrativo GetWithActiveChilds(int id);
    }
}