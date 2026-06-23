using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class ComponenteOrcamentoRepository : IComponenteOrcamentoRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public ComponenteOrcamentoRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Componenteorcamento> GetAll()
        {
            return _moduloContribuicoesContext.Componenteorcamento.Where(u => u.IndActivo).ToList();
        }

        public Componenteorcamento Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var componenteOrcamento = _moduloContribuicoesContext.Componenteorcamento
                .SingleOrDefault(u => u.Id == id);

            return componenteOrcamento;
        }

        public ComponenteOrcamentoDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var componenteOrcamento = _moduloContribuicoesContext.Componenteorcamento
                .SingleOrDefault(u => u.Id == id);

            ComponenteOrcamentoDto componenteOrcamentoDto = Utils.MappClassToDto<Componenteorcamento, ComponenteOrcamentoDto>(componenteOrcamento);
            return componenteOrcamentoDto;
        }

        public void Add(Componenteorcamento entity)
        {
            _moduloContribuicoesContext.Componenteorcamento.Add(entity);
        }

        public void Update(Componenteorcamento entity)
        {
            Componenteorcamento entityToUpdate = _moduloContribuicoesContext.Componenteorcamento
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Componenteorcamento entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public Componenteorcamento? GetByIdTarefa(int id)
        {
            return _moduloContribuicoesContext.Componenteorcamento
                .Where(c => c.TarefaFk == id && c.IndActivo).FirstOrDefault();
        }
    }
}