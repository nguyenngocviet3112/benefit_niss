using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IMovimentoBancarioRepository : IDataRepository<Movimentobancario, MovimentoBancarioDto>
    {
        public ValueCampoEditavelListagemResponse GetAllMovimentosBancarios(SearchFilter filter, List<SelectDescription> dropDownSelectionDescription);

        public bool IsMovimentoValid(Movimentobancario movimento);

        public List<DominioDescricaoString> GetMovimentosBancariosDominioByFilter(int domainFilterId);
    }
}