using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IEntidadeEmpregadoraRepository : IDataRepository<Entidadeempregadora, EntidadeempregadoraDto>
    {
        public EntidadeEmpregadoraConsultaResponse GetByIdEntidade(int id);
        public Entidadeempregadora GetByTin(string tin);

        public Entidadeempregadora GetByNiss(string niss);

        public EntidadeEmpregadoraDeclaracaoViewResponse GetEntidadeInfoForDeclaracao(EntidadeEmpregadoraIdRequest request);

        public EntidadeEmpregadoraConsultaResponse GetEntidadeByNiss(string Niss);

        public DestinatarioDataContract GetDestinatarioByNiss(string niss);

        public DestinatarioDataContract GetDestinatarioByTin(string tin);

        public Destinatario GetDestinatarioByIdEntidade(int id);

        public void Create(EntidadeEmpregadoraUpsertRequest entity);

        public void Update(EntidadeEmpregadoraUpsertRequest entity);

        public List<Entidadeempregadora> GetEntidadesByNissOrTin(List<string> niss, List<string> tin);
    }
}