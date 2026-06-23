using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class ComponenteConciliacaoMovimentosRepository : IComponenteConciliacaoMovimentosRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public ComponenteConciliacaoMovimentosRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Componenteconciliacaomovimentos> GetAll()
        {
            return _moduloContribuicoesContext.Componenteconciliacaomovimentos.Where(u => u.IndActivo).ToList();
        }

        public Componenteconciliacaomovimentos Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var ComponenteConciliacaoMovimentos = _moduloContribuicoesContext.Componenteconciliacaomovimentos
                .SingleOrDefault(u => u.Id == id);

            return ComponenteConciliacaoMovimentos;
        }

        public ComponenteconciliacaomovimentosDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var ComponenteConciliacaoMovimentos = _moduloContribuicoesContext.Componenteconciliacaomovimentos
                .SingleOrDefault(u => u.Id == id);

            ComponenteconciliacaomovimentosDto ComponenteConciliacaoMovimentosDto = Utils.MappClassToDto<Componenteconciliacaomovimentos, ComponenteconciliacaomovimentosDto>(ComponenteConciliacaoMovimentos);
            return ComponenteConciliacaoMovimentosDto;
        }

        public void Add(Componenteconciliacaomovimentos entity)
        {
            _moduloContribuicoesContext.Componenteconciliacaomovimentos.Add(entity);
        }

        public void Update(Componenteconciliacaomovimentos entity)
        {
            Componenteconciliacaomovimentos entityToUpdate = _moduloContribuicoesContext.Componenteconciliacaomovimentos
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Componenteconciliacaomovimentos entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public Componenteconciliacaomovimentos? GetByIdTarefa(int id)
        {
            return _moduloContribuicoesContext.Componenteconciliacaomovimentos
                .Where(c => c.TarefaFk == id && c.IndActivo).FirstOrDefault();
        }
    }
}