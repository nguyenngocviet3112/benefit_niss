using System.Collections.Generic;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IComponenteControloAcessoRepository : IDataRepository<Componentecontroleacesso, ComponenteControleAcessoDto>
    {
        public List<Componentecontroleacesso> GetByIdTarefa(int idTarefa);

        public List<int> GetAllowedTarefaIdsByPerfilAndUser(List<int> perfilIds, int userId);
    }
}