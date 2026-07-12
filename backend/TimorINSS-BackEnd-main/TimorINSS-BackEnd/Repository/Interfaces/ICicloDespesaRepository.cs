using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface ICicloDespesaRepository
    {
        List<CicloDespesaDataContract> GetByAno(int ano, int? institution);
    }
}
