using System;
using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IRelEntidadeTrabalhadorRepository : IDataRepository<Relentidadetrabalhador, RelentidadetrabalhadorDto>
    {
        public bool IsTrabalhadorVinculado(DateTime begin, DateTime end, int trabalhador, int empresa, int idRel = 0);

        public bool IsTrabalhadorAssociadoEntidadeEmpregadora(int idTrabalhador, int idEntidadeEmpregadora);

        public bool NumFuncPublicoExists(string numFuncPublico, int trabalhadorId = 0);

        public List<int> GetAllRelTrabalhadorByEntidadeAndMonth(int entidadeId, DateTime date, GetDeclaracaoByEntidadeAndFilterRequest request);

        public List<int> GetAllRelTrabalhadorByEntidadeAndMonthAll(int entidadeId, DateTime date);

        public int GetTrabalhadorIdByRelId(int relId);

        public DateTime? GetFirstContratoDateByEntidade(int entidadeId);

        public int GetEntidadeINSSIdByTrabalhadorInterno(int trabalhadorId);

        public bool EntidadeTemConflitoComTipoRegime(int entidadeId, int regimeId);

        public List<Relentidadetrabalhador> GetRelEntidadeTrabalhadorINSS(string Niss);

        // Versão em lote de GetDto -- 1 query para todos os ids em vez de 1 query por id.
        public Dictionary<int, RelentidadetrabalhadorDto> GetDtoBatch(List<int> ids);
    }
}