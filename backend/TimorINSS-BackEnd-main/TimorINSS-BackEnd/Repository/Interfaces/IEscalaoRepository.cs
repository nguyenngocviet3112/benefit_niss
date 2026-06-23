using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IEscalaoRepository : IDataRepository<Escalao, EscalaoDto>
    {
        public List<SelectDescription> GetAllEscaloes();

        public ValueCampoEditavelListagemResponse GetAllActiveEscaloes(SearchFilter filter);

        public List<Escalao> GetAllEscaloesByParent(int idParent);

        public bool IsEscalaoDeclared(Escalao escalao);
    }
}