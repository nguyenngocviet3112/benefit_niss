using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IProcessoAtivoRepository : IDataRepository<Processoativo, ProcessoativoDto>
    {
        public ProcessoativoDto GetLastDto();

        public ProcessosArquivadosListagemResponse GetAllProcessosArquivados(SearchFilterRequest request);

        public RelatorioProcessosListagemResponse GetProcessosRelatorios(RelatorioProcessosListagemRequest request);

        public GetTipoProcessosRelatoriosResponse GetTipoProcessosRelatorios();
    }
}