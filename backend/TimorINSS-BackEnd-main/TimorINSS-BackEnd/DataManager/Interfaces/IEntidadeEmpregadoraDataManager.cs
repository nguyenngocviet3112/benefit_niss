using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IEntidadeEmpregadoraDataManager
    {
        public IEnumerable<Entidadeempregadora> GetAll();

        public Entidadeempregadora Get(long id);

        public EntidadeempregadoraDto GetDto(long id);

        public void Add(Entidadeempregadora entity);

        public void Update(Entidadeempregadora entity);

        public void Delete(Entidadeempregadora entity);

        public EntidadeEmpregadoraConsultaResponse GetByIdEntidade(int id);

        public EntidadeEmpregadoraConsultaResponse UpdateEntidade(EntidadeEmpregadoraRequest morada);

        public ResponseBaseDataContract UpsertEntidade(EntidadeEmpregadoraUpsertRequest data);

        public EntidadeEmpregadoraDeclaracaoViewResponse GetEntidadeInfoForDeclaracao(EntidadeEmpregadoraIdRequest request);

        public EntidadeEmpregadoraConsultaResponse GetEntidadeByNiss(EntidadeEmpregadoraNissRequest request);
    }
}