using System.Collections.Generic;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IRelProcessoConfigTarefaRepository : IDataRepository<Relprocessoconfigtarefa, Relprocessoconfigtarefadto>
    {
        public List<Relprocessoconfigtarefa> GetRelTarefaByProcesso(long processoId);
    }
}