using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IInstitutionRepository : IDataRepository<Institution, InstitutionDto>
    {
       public List<SelectDescription> GetAllInstitutionAtivo();
    }
}