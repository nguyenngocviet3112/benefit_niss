using System;
using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface ICentroCustoRepository : IDataRepository<Centrocusto, CentroCustoDto>
    {
        public ValueCampoEditavelListagemResponse GetAllActiveCentroCusto(SearchFilter filter);

        public List<SelectDescription> GetAllActiveCentroCustoByOrcamentoRegisto(int orcamentoId);

        public bool IsCentroCustoDeleteValid(Centrocusto centro);

        public List<Centrocusto> GetAllActive(DateTime? data = null);
    }
}