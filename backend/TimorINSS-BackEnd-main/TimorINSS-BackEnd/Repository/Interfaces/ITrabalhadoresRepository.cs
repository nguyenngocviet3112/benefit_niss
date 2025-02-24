using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface ITrabalhadoresRepository : IDataRepository<Trabalhador, TrabalhadorDto>
    {
        public TrabalhadorListagemResponse GetTrabalhadoresByIdEntidadeEmpregadora(TrabalhadorListagemRequest request);

        public VincularTrabalhadorListagemResponse getTrabalhadoresByFilter(SearchFilterRequest request);

        public Trabalhador GetByNiss(string niss);

        public TrabalhadorListagemResponse GetTrabalhadoresByNiss(TrabalhadorListagemRequest request);

        public Trabalhador GetTrabalhadorViewById(long id);

        public string GetNextNumInscProvisoria();

        bool NissExists(string niss, int idTrabalhador = 0);

        bool TinExists(string tin, int idTrabalhador = 0);

        public Trabalhador GetInternalByNiss(string niss);

        public DestinatarioDataContract GetDestinatarioByNiss(string niss);

        public DestinatarioDataContract GetDestinatarioByTin(string tin);

        public Destinatario GetDestinatarioByIdTrabalhador(int id);

        public List<Trabalhador> GetTrabalhadoresByNissOrTin(List<string> niss, List<string> tin);
    }
}