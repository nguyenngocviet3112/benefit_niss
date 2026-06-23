using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class ComponenteAccoesTarefaRepository : IComponenteAccoesTarefaRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public ComponenteAccoesTarefaRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Componenteaccoestarefa> GetAll()
        {
            return _moduloContribuicoesContext.Componenteaccoestarefa.Where(u => u.IndActivo).ToList();
        }

        public Componenteaccoestarefa Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var componenteAccoesTarefa = _moduloContribuicoesContext.Componenteaccoestarefa
                .SingleOrDefault(u => u.Id == id);

            return componenteAccoesTarefa;
        }

        public ComponenteAccoesTarefaDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var componenteAccoesTarefa = _moduloContribuicoesContext.Componenteaccoestarefa
                .SingleOrDefault(u => u.Id == id);

            ComponenteAccoesTarefaDto componenteAccoesTarefaDto = Utils.MappClassToDto<Componenteaccoestarefa, ComponenteAccoesTarefaDto>(componenteAccoesTarefa);
            return componenteAccoesTarefaDto;
        }

        public void Add(Componenteaccoestarefa entity)
        {
            _moduloContribuicoesContext.Componenteaccoestarefa.Add(entity);
        }

        public void Update(Componenteaccoestarefa entity)
        {
            Componenteaccoestarefa entityToUpdate = _moduloContribuicoesContext.Componenteaccoestarefa
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Componenteaccoestarefa entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public List<ComponenteAccoesTarefa> GetByIdTarefa(int idTarefa)
        {
            var componenteAccoesTarefa = _moduloContribuicoesContext.Componenteaccoestarefa
                    .Where(u => u.TarefaFk == idTarefa && u.IndActivo)
                    .Select(componente => new ComponenteAccoesTarefa
                    {
                        id = componente.Id,
                        nomeTarefa = componente.TarefaSeguirFkNavigation.Nome,
                        apelidoTarefa = componente.ApelidoDaTarefa
                    });

            List<ComponenteAccoesTarefa> listaComponenteAccoesTarefa = componenteAccoesTarefa
              .ToList();

            return listaComponenteAccoesTarefa;
        }

        public List<Componenteaccoestarefa> GetComponenteByIdTarefa(int idTarefa)
        {
            var componenteAccoesTarefa = _moduloContribuicoesContext.Componenteaccoestarefa
                   .Where(u => u.TarefaFk == idTarefa && u.IndActivo)
                   .Select(componente => new Componenteaccoestarefa
                   {
                       Id = componente.Id,
                       TarefaFk = componente.TarefaFk,
                       TarefaSeguirFk = componente.TarefaSeguirFk,
                       ApelidoDaTarefa = componente.ApelidoDaTarefa,
                       UtilizadorCriacao = componente.UtilizadorCriacao,
                       DataCriacao = componente.DataCriacao,
                       IndActivo = componente.IndActivo
                   });

            List<Componenteaccoestarefa> listaComponenteAccoesTarefa = componenteAccoesTarefa
              .ToList();

            return listaComponenteAccoesTarefa;
        }

        public List<SelectDescription> GetAllTarefasASeguir(int idTarefa)
        {
            var tarefasASeguir = _moduloContribuicoesContext.Componenteaccoestarefa
                    .Where(u => u.TarefaFk == idTarefa && u.IndActivo)
                    .Select(componente => new SelectDescription
                    {
                        id = componente.TarefaSeguirFk,
                        indActivo = componente.IndActivo,
                        nome = componente.ApelidoDaTarefa
                    });

            List<SelectDescription> listaComponenteAccoesTarefa = tarefasASeguir
              .ToList();

            return listaComponenteAccoesTarefa;
        }
    }
}