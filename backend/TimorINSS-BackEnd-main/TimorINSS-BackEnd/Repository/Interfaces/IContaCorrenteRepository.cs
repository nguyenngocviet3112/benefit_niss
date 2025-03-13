using System;
using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IContaCorrenteRepository : IDataRepository<Contacorrente, ContacorrenteDto>
    {
        public ContaCorrenteListagemResponse GetContaCorrenteByFilter(ContaCorrenteListagemRequest request);
        public ResumoContaCorrenteListagemResponse GetResumoContaCorrente(ResumoContaCorrenteListagemRequest request);

        public List<Contacorrente> GetContaCorrenteByData(DateTime data);

        public List<ContasState> GetAllContasStatesFromYearByIdEntidade(int idEntidade, DateTime dateInicial, DateTime dateFinal);

        public bool IsEntidadeRegularizada(int entidade);

        public Contacorrente GetContaCorrenteByMesAnoAndEntidadeId(DateTime mesAno, int entidadeEmpregadoraID);

        public void UpdateCorrente(int contraId);

        public void UpdateSituacaoPagamento(int contaCorrenteId);


    }
}