using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IContaCorrenteDataManager
    {
        public IEnumerable<Contacorrente> GetAll();

        public Contacorrente Get(long id);

        public ContacorrenteDto GetDto(long id);

        public ContaCorrenteListagemResponse GetContaCorrenteByIdEntidade(ContaCorrenteListagemRequest request);

        public ResumoContaCorrenteListagemResponse GetResumoContaCorrenteByIdEntidade(ResumoContaCorrenteListagemRequest request);

        public GetAllContasStatesFromYearByFilterResponse GetAllContasStatesFromYearByFilter(GetAllContasStatesFromYearByFilterRequest request);
    }
}