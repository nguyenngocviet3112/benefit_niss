using System;
using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IRegimeRepository : IDataRepository<Regime, RegimeDto>
    {
        public List<SelectDescription> GetAllRegimes();

        public Regime GetRegimeByParentAndDate(int parent, DateTime data);

        public Dominio GetTipoRegimeFromRegimePai(long regimePai);

        public List<Regime> GetAllRegimesByParent(int parent);

        public List<Regime> GetAllActiveRegimesByParent(int parent);

        public ValueCampoEditavelListagemResponse GetAllActiveRegimes(SearchFilter filter);
    }
}