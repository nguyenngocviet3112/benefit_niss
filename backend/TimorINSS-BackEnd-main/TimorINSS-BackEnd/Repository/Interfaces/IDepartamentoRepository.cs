using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IDepartamentoRepository : IDataRepository<Departamento, DepartamentoDto>
    {
        public ValueCampoEditavelListagemResponse getAllActiveDepartamento(SearchFilter filter);

        public List<SelectDescription> GetAllDepartamentosAtivo();

        public List<SelectDescription> GetAllInstitutionAtivo();
    }
}