using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IComponenteOrcamentoValorRepository : IDataRepository<Componenteorcamentovalor, ComponenteOrcamentoValorDto>
    {
        public List<Componenteorcamentovalor> SearchComponenteOrcamentoValor(ComponenteOrcamentoValorSearch filter);

        public Componenteorcamentovalor getOrcamentoValorByAgrupamentoAndOrcamentoRegisto(int agrupamentoId, int orcamentoRegistoID);

        public List<Componenteorcamentovalor> getAllComponentesOrcamentoValorByRegisto(int orcamentoRegistoID);

        public List<Componenteorcamentovalor> getAllByRegistoId(int id);

        public List<Componenteorcamentovalor> getOrcamentoValorByAgrupamentoFkOrcamentoRegistoFk(int agrupamentoId, int orcamentoRegistoID);

        public List<Componenteorcamentovalor> getOrcamentoValorByAgrupamentoFkOrcamentoRegistoFk(int agrupamentoId, int orcamentoRegistoID,
            int institutionId, int actidadeId,  int funcionalId);


        public Componenteorcamentovalor GetComponenteOrcamentoValorSameForeignKeys(Componenteorcamentovalor componente);

        public List<ExcTractOrcamentoValor> SearchComponenteOrcamentoValorExtraction(ComponenteOrcamentoValorSearch filter);

        public Componenteorcamentovalor getOrcamentoValorByAgrupamentoAndOrcamentoRegistoInactivo(int agrupamentoId, int orcamentoRegistoID);

        // Get() normal desliga lazy loading e não faz Include -- esta variante traz
        // AgrupamentoFkNavigation carregado, para ecrãs que precisam de mostrar a designação
        // da rubrica (ex: lista de Ajustes de Orçamento pendentes/histórico).
        public Componenteorcamentovalor GetWithAgrupamento(int id);
    }
}