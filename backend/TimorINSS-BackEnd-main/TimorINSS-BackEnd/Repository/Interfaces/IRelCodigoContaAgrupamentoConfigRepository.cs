using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IRelCodigoContaAgrupamentoConfigRepository : IDataRepository<Relcodigocontaagrupamentoconfig, RelCodigoContaAgrupamentoConfigDto>
    {
        public List<Relcodigocontaagrupamentoconfig> GetAllByCodigoConta(int id);

        public List<AgrupamentoConfigDataContract> GetAgrupamentoConfigByIdCodigoContaTipoConta(int codigoContaFK, int tipoContaFk);

        public bool IsRelBeingUsed(Relcodigocontaagrupamentoconfig rel);
    }
}