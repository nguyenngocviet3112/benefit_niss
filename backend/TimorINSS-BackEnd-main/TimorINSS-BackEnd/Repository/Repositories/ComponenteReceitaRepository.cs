using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class ComponenteReceitaRepository : IComponenteReceitaRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public ComponenteReceitaRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Componentereceita> GetAll()
        {
            return _moduloContribuicoesContext.Componentereceita.Where(u => u.IndActivo).ToList();
        }

        public Componentereceita Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var componenteReceita = _moduloContribuicoesContext.Componentereceita
                .SingleOrDefault(u => u.Id == id);

            return componenteReceita;
        }

        public ComponenteReceitaDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var componenteReceita = _moduloContribuicoesContext.Componentereceita
                .SingleOrDefault(u => u.Id == id);

            ComponenteReceitaDto componenteReceitaDto = Utils.MappClassToDto<Componentereceita, ComponenteReceitaDto>(componenteReceita);
            return componenteReceitaDto;
        }

        public void Add(Componentereceita entity)
        {
            _moduloContribuicoesContext.Componentereceita.Add(entity);
        }

        public void Update(Componentereceita entity)
        {
            Componentereceita entityToUpdate = _moduloContribuicoesContext.Componentereceita
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Componentereceita entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public Componentereceita? GetByIdTarefa(int id)
        {
            return _moduloContribuicoesContext.Componentereceita
                .Where(c => c.TarefaFk == id && c.IndActivo).FirstOrDefault();
        }
    }
}