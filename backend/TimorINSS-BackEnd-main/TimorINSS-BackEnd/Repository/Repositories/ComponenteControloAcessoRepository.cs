using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class ComponenteControloAcessoRepository : IComponenteControloAcessoRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public ComponenteControloAcessoRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Componentecontroleacesso> GetAll()
        {
            return _moduloContribuicoesContext.Componentecontroleacesso.Where(u => u.IndActivo).ToList();
        }

        public Componentecontroleacesso Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var componenteControleAcesso = _moduloContribuicoesContext.Componentecontroleacesso
                .SingleOrDefault(u => u.Id == id);

            return componenteControleAcesso;
        }

        public ComponenteControleAcessoDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var componenteControleAcesso = _moduloContribuicoesContext.Componentecontroleacesso
                .SingleOrDefault(u => u.Id == id);

            ComponenteControleAcessoDto componenteControleAcessoDto = Utils.MappClassToDto<Componentecontroleacesso, ComponenteControleAcessoDto>(componenteControleAcesso);
            return componenteControleAcessoDto;
        }

        public void Add(Componentecontroleacesso entity)
        {
            _moduloContribuicoesContext.Componentecontroleacesso.Add(entity);
        }

        public void Update(Componentecontroleacesso entity)
        {
            Componentecontroleacesso entityToUpdate = _moduloContribuicoesContext.Componentecontroleacesso
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Componentecontroleacesso entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public List<Componentecontroleacesso> GetByIdTarefa(int idTarefa)
        {
            var componenteControloAcesso = _moduloContribuicoesContext.Componentecontroleacesso
                    .Include(o => o.UtilizadorFkNavigation)
                   .Where(u => u.TarefaFk == idTarefa && u.IndActivo)
                   .Select(componente => new Componentecontroleacesso
                   {
                       Id = componente.Id,
                       TarefaFk = componente.TarefaFk,
                       PerfilFk = componente.PerfilFk,
                       UtilizadorFk = componente.UtilizadorFk,
                       PerfilFkNavigation = componente.PerfilFkNavigation,
                       UtilizadorFkNavigation = componente.UtilizadorFkNavigation,
                       UtilizadorCriacao = componente.UtilizadorCriacao,
                       DataCriacao = componente.DataCriacao,
                       IndActivo = componente.IndActivo
                   });

            List<Componentecontroleacesso> listaComponenteControloAcessoc = componenteControloAcesso
              .ToList();

            return listaComponenteControloAcessoc;
        }

        public List<int> GetAllowedTarefaIdsByPerfilAndUser(List<int> perfilIds, int userId)
        {
            List<int> response = new List<int>();

            response = _moduloContribuicoesContext.Componentecontroleacesso
                .Where(x => (x.UtilizadorFk == userId || perfilIds.Contains(x.PerfilFk.Value)) && x.IndActivo)
                .Select(x => x.TarefaFk)
                .Distinct()
                .ToList();

            return response;
        }
    }
}