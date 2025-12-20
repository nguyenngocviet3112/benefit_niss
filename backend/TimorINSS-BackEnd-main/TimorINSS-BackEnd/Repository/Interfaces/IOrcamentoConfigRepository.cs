using System;
using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IOrcamentoConfigRepository : IDataRepository<Orcamentoconfig, OrcamentoConfigDto>
    {
        public ValueCampoEditavelListagemResponse getAllActiveOrcamentoConfig(SearchFilter filter);

        public List<SelectDescription> GetAllOrcamentoConfig();

        public Error IsOrcamentoValid(Orcamentoconfig orcamento);
        public Error IsComponenteOrcamentoValid(DateTime startDate, DateTime? endDate);

        public Orcamentoconfig GetOrcamentoConfigByDates(DateTime start, DateTime end);

        public Orcamentoconfig GetOrcamentoConfigCurrentDate();

        public bool IsOrcamentoDeleteValid(Orcamentoconfig orcamento);
    }
}