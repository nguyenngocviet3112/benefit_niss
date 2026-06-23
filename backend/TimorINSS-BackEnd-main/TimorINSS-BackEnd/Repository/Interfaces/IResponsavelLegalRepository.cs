using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IResponsavelLegalRepository : IDataRepository<Responsavellegal, ResponsavellegalDto>
    {
        public ResponsavelLegalListagemResponse GetByIdEntidadeEmpregadora(ResponsavelLegalListagemRequest request, List<DominioDescricaoString> funcaoDominioList);

        public Responsavellegal GetByTrabalhadorId(long trabalhadorId);

        public Responsavellegal GetByTin(string tin);
    }
}