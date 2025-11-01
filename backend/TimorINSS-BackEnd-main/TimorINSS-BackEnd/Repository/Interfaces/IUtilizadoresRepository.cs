using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IUtilizadoresRepository : IDataRepository<Utilizador, UtilizadorDto>
    {
        public Utilizador GetByUsername(string username);

        public Utilizador GetByEntidadeEmpregadora(long entidadeId);

        public Utilizador GetInternalByUsername(string username);

        public UtilizadorListagemResponse GetAllUtilizadoresInterno(SearchFilterRequest request);

        public UtilizadorListagemResponse GetUtilizadoresInternoByPerfilId(SearchFilterRequest request);
        
        public Utilizador GetByTrabalhadorFk(long trabalhadorId);

        public UtilizadoresAcessoListagemResponse GetAllAcessoUtilizadores(SearchFilterRequest request);

        public Utilizador GetAdminUser();
    }
}