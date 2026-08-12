using System.Collections.Generic;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IComponenteOrcamentoAjusteRepository
    {
        public ComponenteOrcamentoAjuste Get(int id);

        public void Add(ComponenteOrcamentoAjuste entity);

        public void Update(ComponenteOrcamentoAjuste entity);

        public List<ComponenteOrcamentoAjuste> GetPendentesByComponenteOrcamentoRegistoFk(int componenteOrcamentoRegistoFk);

        public List<ComponenteOrcamentoAjuste> GetHistoricoByComponenteOrcamentoRegistoFk(int componenteOrcamentoRegistoFk);
    }
}
