using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface ICamposEditaveisRepository : IDataRepository<Camposeditaveis, CamposEditaveisDto>
    {
        public List<CamposEditaveisListagem> getAllCamposEditaveis();
    }
}