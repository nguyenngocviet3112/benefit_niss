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

        // Versão em lote de GetDeclaracaoTrabalhadorInfo -- mesma lógica/regras por item, mas faz um punhado de
        // queries para toda a lista em vez de uma query (multi-join) por trabalhador. Devolve um dicionário
        // chaveado por declaracaoRelEntidadeTrabalhadorFk, na mesma ordem de processamento não é garantida --
        // o chamador deve iterar pela sua própria lista/ordem.
        public Dictionary<int, DeclaracaoListagem> GetDeclaracaoTrabalhadorInfoBatch(List<DeclaracaoRemuneracaoDataContract> declaracoes, DateTime mesAno);

        public decimal getTemDecimoTerceiroMes(DateTime mesAno, int idRel);

        public List<Tuple<int, DateTime>> GetEntidadesAndFirstDateComDeclaracaoNaoRegistadasPastDay(DateTime day);

        public bool ExistsDeclaracaoWithRegime(int idRegime);

        public RelatorioDeclaracaoRenumeracaoListagemResponse GetDeclaracoesRelatorios(RelatorioDeclaracaoRenumeracaoListagemRequest request);

        public SituacaoContributivaEmpresasRelatorioResponse GetSituacaoContributivaEmpresasRelatorio(SituacaoContributivaEmpresasRelatorioRequest request);

        public List<(DateTime mesAno, decimal valorPago, decimal valorDivida)> GetContribuicoesTrendsPorMes(DateTime? beginDate, DateTime? endDate);
    }
}