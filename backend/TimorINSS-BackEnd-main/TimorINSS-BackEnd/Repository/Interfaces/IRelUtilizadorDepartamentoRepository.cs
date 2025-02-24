using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IRelUtilizadorDepartamentoRepository : IDataRepository<Relutilizadordepartamento, RelUtilizadorDepartamentoDto>
    {
        public List<Relutilizadordepartamento> GetRelUtilizadorDepartamentoByUserId(int userId);

        public DepartamentoListagemResponse GetDepartamentosByUserId(int id);

        public Relutilizadordepartamento GetRelByDepartamentoId(int idDepartamento);
    }
}