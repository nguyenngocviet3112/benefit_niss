using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class ComponenteClassificacaoSubClassificRegistoRepository : IComponenteClassificacaoSubClassificRegistoRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public ComponenteClassificacaoSubClassificRegistoRepository(TimorINSSModuloContribuicoesContext storeContext) 
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<ComponenteclassificacaosubRegisto> GetAll()
        {
            return _moduloContribuicoesContext.ComponenteclassificacaosubRegisto.Where(u => u.IndActivo).ToList();
        }

        public ComponenteclassificacaosubRegisto Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var componenteClassificacaoSubClassificRegisto = _moduloContribuicoesContext.ComponenteclassificacaosubRegisto
                .SingleOrDefault(u => u.Id == id);

            return componenteClassificacaoSubClassificRegisto;
        }

        public ComponenteclassificacaosubRegistoDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var componenteClassificacaoSubClassificRegisto = _moduloContribuicoesContext.ComponenteclassificacaosubRegisto
                .SingleOrDefault(u => u.Id == id);

            ComponenteclassificacaosubRegistoDto componenteClassificacaoSubClassificRegistoDto = Utils.MappClassToDto<ComponenteclassificacaosubRegisto, ComponenteclassificacaosubRegistoDto>(componenteClassificacaoSubClassificRegisto);
            return componenteClassificacaoSubClassificRegistoDto;
        }

        public void Add(ComponenteclassificacaosubRegisto entity)
        {
            _moduloContribuicoesContext.ComponenteclassificacaosubRegisto.Add(entity);
        }

        public void Update(ComponenteclassificacaosubRegisto entity)
        {
            ComponenteclassificacaosubRegisto entityToUpdate = _moduloContribuicoesContext.ComponenteclassificacaosubRegisto
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(ComponenteclassificacaosubRegisto entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public ComponenteclassificacaosubRegisto GetByTarefaAtivoId(int tarefaAtivoId)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var componenteClassificacaoSubClassificRegisto = _moduloContribuicoesContext.ComponenteclassificacaosubRegisto
                .SingleOrDefault(u => u.TarefaAtivoFk == tarefaAtivoId && u.IndActivo);

            return componenteClassificacaoSubClassificRegisto;
        }
    }
}