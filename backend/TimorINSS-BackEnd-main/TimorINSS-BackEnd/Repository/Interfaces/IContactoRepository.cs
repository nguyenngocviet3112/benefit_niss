using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IContactoRepository : IDataRepository<Contacto, ContactoDto>
    {
        public ContatoListagemResponse GetContactosByFilter(ContatoListagemRequest request);

        public ContactoDto GetDtoByEmail(string email, long trabalhadorId);

        public ContactoDto GetInternalDtoByEmailAndWorkerId(string email, long workerId);

        public List<Contacto> GetByTrabalhadorFkEntidadeFk(int? trabalhadorFk, int? entidadeFk);
    }
}