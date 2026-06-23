using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class RelTarefaComponenteRepository : IRelTarefaComponenteRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public RelTarefaComponenteRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Reltarefacomponente> GetAll()
        {
            return _moduloContribuicoesContext.Reltarefacomponente.Where(u => u.IndActivo).ToList();
        }

        public Reltarefacomponente Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var relTarefaComponente = _moduloContribuicoesContext.Reltarefacomponente
                .SingleOrDefault(u => u.Id == id);

            return relTarefaComponente;
        }

        public RelTarefaComponenteDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var relTarefaComponente = _moduloContribuicoesContext.Reltarefacomponente
                .SingleOrDefault(u => u.Id == id);

            RelTarefaComponenteDto relTarefaComponenteDto = Utils.MappClassToDto<Reltarefacomponente, RelTarefaComponenteDto>(relTarefaComponente);
            return relTarefaComponenteDto;
        }

        public void Add(Reltarefacomponente entity)
        {
            _moduloContribuicoesContext.Reltarefacomponente.Add(entity);
        }

        public void Update(Reltarefacomponente entity)
        {
            Reltarefacomponente entityToUpdate = _moduloContribuicoesContext.Reltarefacomponente
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Reltarefacomponente entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public List<Componentes> GetRelTarefaComponenteByIdTarefa(int idTarefa)
        {
            var componente = _moduloContribuicoesContext.Reltarefacomponente
                .Include(u => u.ComponenteFkNavigation)
                .Where(u => u.TarefaFk == idTarefa && u.IndActivo)
                .Select(relContaCorrente => new Componentes
                {
                    id = relContaCorrente.ComponenteFk,
                    descricao = relContaCorrente.ComponenteFkNavigation.Descricao,
                    expandir = relContaCorrente.Expandir,
                    ordem = relContaCorrente.Ordem
                });

            List<Componentes> listaComponente = componente
              .ToList();

            return listaComponente;
        }

        public Reltarefacomponente GetRelTarefaComponenteByIdTarefaIdComponente(int idComponente, int idTarefa)
        {
            var relTarefaComponenteQuery = _moduloContribuicoesContext.Reltarefacomponente
                .Include(u => u.ComponenteFkNavigation)
                .Where(u => u.ComponenteFk == idComponente && u.TarefaFk == idTarefa && u.IndActivo)
                .Select(relTarefaComponente => new Reltarefacomponente
                {
                    Id = relTarefaComponente.Id,
                    TarefaFk = relTarefaComponente.TarefaFk,
                    ComponenteFk = relTarefaComponente.ComponenteFk,
                    UtilizadorCriacao = relTarefaComponente.UtilizadorCriacao,
                    DataCriacao = relTarefaComponente.DataCriacao,
                    IndActivo = relTarefaComponente.IndActivo
                });

            Reltarefacomponente relTarefaComponente = relTarefaComponenteQuery
              .SingleOrDefault();

            return relTarefaComponente;
        }
    }
}