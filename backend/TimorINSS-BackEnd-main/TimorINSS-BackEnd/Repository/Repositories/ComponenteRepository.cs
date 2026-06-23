using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class ComponenteRepository : IComponenteRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public ComponenteRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Componente> GetAll()
        {
            return _moduloContribuicoesContext.Componente.Where(u => u.IndActivo).ToList();
        }

        public Componente Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var componetne = _moduloContribuicoesContext.Componente
                .SingleOrDefault(u => u.Id == id);

            return componetne;
        }

        public ComponenteDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var componente = _moduloContribuicoesContext.Componente
                .SingleOrDefault(u => u.Id == id);

            ComponenteDto componenteDto = Utils.MappClassToDto<Componente, ComponenteDto>(componente);
            return componenteDto;
        }

        public void Add(Componente entity)
        {
            _moduloContribuicoesContext.Componente.Add(entity);
        }

        public void Update(Componente entity)
        {
            Componente entityToUpdate = _moduloContribuicoesContext.Componente
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Componente entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public ComponentesListagemResponse GetAllComponentes()
        {
            ComponentesListagemResponse response = new ComponentesListagemResponse();

            var listaComponente = _moduloContribuicoesContext.Componente
                .Where(u => u.IndActivo)
               .Select(u => new Componentes
               {
                   id = u.Id,
                   descricao = u.Descricao,
                   expandir = false,
                   select = false
               });

            var componentes = listaComponente
               .ToList();

            response.componentes = componentes;

            return response;
        }
    }
}