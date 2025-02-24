using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IMoradaRepository : IDataRepository<Morada, MoradaDto>
    {
        public MoradaListagemResponse GetMoradasByFilter(MoradaListagemRequest request);

        public void UpdateMoradaPrincipal(Morada morada);

        public Morada GetMoradasIguais(Morada morada);

        public Morada GetMoradasPrincipalByEntidadeFk(int entidadeFk);

        public Morada GetMoradasPrincipalByTrabalhadorFk(int trabalhadorFk);
    }
}