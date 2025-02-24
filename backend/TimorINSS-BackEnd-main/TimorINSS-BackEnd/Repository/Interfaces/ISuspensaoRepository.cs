using System;
using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface ISuspensaoRepository : IDataRepository<Suspensoes, SuspensoesDto>
    {
        public SuspensaoListagemResponse GetSuspensaoByFilter(SuspensaoListagemRequest request);

        public bool GetSuspensaoByData(SuspensaoListagemRequest request);

        public bool ExistSuspensao(DateTime begin, DateTime end, int? idEntidade, int? idTrabalhador);

        public List<int> GetAllRelEntidadeTrabalhadorSuspensosByDate(int idEntidade, DateTime begin, DateTime end);
    }
}