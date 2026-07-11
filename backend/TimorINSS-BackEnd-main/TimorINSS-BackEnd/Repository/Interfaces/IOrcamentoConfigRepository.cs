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

        // Added 2026-07-11 for the new Kỳ ngân sách (System Settings) admin CRUD —
        // see OrcamentoConfigController/OrcamentoConfigDataManager (new files, separate
        // from the pre-existing OrcamentoController which manages OrcamentoLinha/M2).
        public bool IsAnoTipoValid(Orcamentoconfig orcamento);

        public bool HasOrcamentoBatch(int id);
    }
}