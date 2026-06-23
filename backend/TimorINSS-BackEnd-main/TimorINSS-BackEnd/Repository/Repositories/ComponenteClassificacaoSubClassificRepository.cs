using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class ComponenteClassificacaoSubClassificRepository : IComponenteClassificacaoSubClassificRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public ComponenteClassificacaoSubClassificRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Componenteclassificacaosubclassific> GetAll()
        {
            return _moduloContribuicoesContext.Componenteclassificacaosubclassific.Where(u => u.IndActivo).ToList();
        }

        public Componenteclassificacaosubclassific Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var componenteClassificacaoSubClassific = _moduloContribuicoesContext.Componenteclassificacaosubclassific
                .SingleOrDefault(u => u.Id == id);

            return componenteClassificacaoSubClassific;
        }

        public ComponenteClassificacaoSubClassificDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var componenteClassificacaoSubClassific = _moduloContribuicoesContext.Componenteclassificacaosubclassific
                .SingleOrDefault(u => u.Id == id);

            ComponenteClassificacaoSubClassificDto componenteClassificacaoSubClassificDto = Utils.MappClassToDto<Componenteclassificacaosubclassific, ComponenteClassificacaoSubClassificDto>(componenteClassificacaoSubClassific);
            return componenteClassificacaoSubClassificDto;
        }

        public void Add(Componenteclassificacaosubclassific entity)
        {
            _moduloContribuicoesContext.Componenteclassificacaosubclassific.Add(entity);
        }

        public void Update(Componenteclassificacaosubclassific entity)
        {
            Componenteclassificacaosubclassific entityToUpdate = _moduloContribuicoesContext.Componenteclassificacaosubclassific
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Componenteclassificacaosubclassific entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public List<ComponenteClassificacaoSubClassificTarefa> GetByIdTarefa(int idTarefa)
        {
            var componenteClassificacaoSubClassific = _moduloContribuicoesContext.Componenteclassificacaosubclassific
                   .Where(u => u.TarefaFk == idTarefa && u.IndActivo)
                   .Select(componente => new ComponenteClassificacaoSubClassificTarefa
                   {
                       id = componente.Id,
                       idClassificacao = componente.SubclassificacaoFkNavigation.ClassificacaoFk,
                       nomeClassificacao = componente.SubclassificacaoFkNavigation.ClassificacaoFkNavigation.Nome,
                       idSubClassificacao = componente.SubclassificacaoFk,
                       nomeSubClassificacao = componente.SubclassificacaoFkNavigation.Nome
                   });

            List<ComponenteClassificacaoSubClassificTarefa> listaComponenteClassificacaoSubClassific = componenteClassificacaoSubClassific
              .ToList();

            return listaComponenteClassificacaoSubClassific;
        }

        public List<Componenteclassificacaosubclassific> GetComponenteByIdTarefa(int idTarefa)
        {
            var componenteClassificacaoSubClassific = _moduloContribuicoesContext.Componenteclassificacaosubclassific
                   .Where(u => u.TarefaFk == idTarefa && u.IndActivo)
                   .Select(componente => new Componenteclassificacaosubclassific
                   {
                       Id = componente.Id,
                       SubclassificacaoFk = componente.SubclassificacaoFk,
                       TarefaFk = componente.TarefaFk,
                       UtilizadorCriacao = componente.UtilizadorCriacao,
                       DataCriacao = componente.DataCriacao,
                       IndActivo = componente.IndActivo
                   });

            List<Componenteclassificacaosubclassific> listaComponenteClassificacaoSubClassific = componenteClassificacaoSubClassific
              .ToList();

            return listaComponenteClassificacaoSubClassific;
        }
    }
}