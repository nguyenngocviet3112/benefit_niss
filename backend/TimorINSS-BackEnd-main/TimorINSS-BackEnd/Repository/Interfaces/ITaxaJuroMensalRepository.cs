using System;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface ITaxaJuroMensalRepository : IDataRepository<Taxajuromensal, TaxajuromensalDto>
    {
        public Taxajuromensal GetTaxaJuroMensalByData(DateTime mesAno);

        public ValueCampoEditavelListagemResponse GetAllActiveTaxas(SearchFilter filter);

        public bool IsTaxaDatesValid(Taxajuromensal taxa);
    }
}