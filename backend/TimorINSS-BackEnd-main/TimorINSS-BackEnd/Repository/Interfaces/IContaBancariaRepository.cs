using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IContaBancariaRepository : IDataRepository<Contabancaria, ContaBancariaDto>
    {
        public ValueCampoEditavelListagemResponse GetAllContasBancarias(SearchFilter filter);

        public bool IsIbanValid(Contabancaria conta);

        public List<ContaBancariaDto> GetAllDto(bool incluirSaldo);
    }
}