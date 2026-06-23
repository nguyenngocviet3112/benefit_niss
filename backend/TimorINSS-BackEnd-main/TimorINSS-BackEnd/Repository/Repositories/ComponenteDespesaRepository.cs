using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class ComponenteDespesaRepository : IComponenteDespesaRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public ComponenteDespesaRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Componentedespesa> GetAll()
        {
            return _moduloContribuicoesContext.Componentedespesa.Where(u => u.IndActivo).ToList();
        }

        public Componentedespesa Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var componenteDespesa = _moduloContribuicoesContext.Componentedespesa
                .SingleOrDefault(u => u.Id == id);

            return componenteDespesa;
        }

        public ComponenteDespesaDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var componenteDespesa = _moduloContribuicoesContext.Componentedespesa
                .SingleOrDefault(u => u.Id == id);

            ComponenteDespesaDto componenteDespesaDto = Utils.MappClassToDto<Componentedespesa, ComponenteDespesaDto>(componenteDespesa);
            return componenteDespesaDto;
        }

        public void Add(Componentedespesa entity)
        {
            _moduloContribuicoesContext.Componentedespesa.Add(entity);
        }

        public void Update(Componentedespesa entity)
        {
            Componentedespesa entityToUpdate = _moduloContribuicoesContext.Componentedespesa
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Componentedespesa entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public Componentedespesa? GetByIdTarefa(int id)
        {
            return _moduloContribuicoesContext.Componentedespesa
                .Where(c => c.TarefaFk == id && c.IndActivo).FirstOrDefault();
        }
    }
}