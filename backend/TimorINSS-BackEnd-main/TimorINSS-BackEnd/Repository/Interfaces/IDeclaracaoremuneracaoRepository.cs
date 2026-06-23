using System;
using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IDeclaracaoremuneracaoRepository : IDataRepository<Declaracaoremuneracao, DeclaracaoremuneracaoDto>
    {
        public List<Declaracaoremuneracao> GetByEntidadeAndDate(int entidadeId, DateTime date);

        public List<Declaracaoremuneracao> GetLastDeclaracaoOficiosa(int entidadeId, DateTime date);

        public DeclaracaoListagem GetDeclaracaoTrabalhadorInfo(DeclaracaoRemuneracaoDataContract declaracao);

        public decimal getTemDecimoTerceiroMes(DateTime mesAno, int idRel);

        public List<Tuple<int, DateTime>> GetEntidadesAndFirstDateComDeclaracaoNaoRegistadasPastDay(DateTime day);

        public bool ExistsDeclaracaoWithRegime(int idRegime);

        public RelatorioDeclaracaoRenumeracaoListagemResponse GetDeclaracoesRelatorios(RelatorioDeclaracaoRenumeracaoListagemRequest request);
    }
}