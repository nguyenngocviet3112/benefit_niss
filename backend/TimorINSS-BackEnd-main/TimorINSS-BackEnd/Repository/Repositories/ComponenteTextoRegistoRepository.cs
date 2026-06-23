using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class ComponenteTextoRegistoRepository : IComponenteTextoRegistoRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public ComponenteTextoRegistoRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<ComponentetextoRegisto> GetAll()
        {
            return _moduloContribuicoesContext.ComponentetextoRegisto.Where(u => u.IndActivo).ToList();
        }

        public ComponentetextoRegisto Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var componenteTextoRegisto = _moduloContribuicoesContext.ComponentetextoRegisto
                .SingleOrDefault(u => u.Id == id);

            return componenteTextoRegisto;
        }

        public ComponentetextoRegistoDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var componenteTextoRegisto = _moduloContribuicoesContext.ComponentetextoRegisto
                .SingleOrDefault(u => u.Id == id);

            ComponentetextoRegistoDto componenteTextoRegistoDto = Utils.MappClassToDto<ComponentetextoRegisto, ComponentetextoRegistoDto>(componenteTextoRegisto);
            return componenteTextoRegistoDto;
        }

        public void Add(ComponentetextoRegisto entity)
        {
            _moduloContribuicoesContext.ComponentetextoRegisto.Add(entity);
        }

        public void Update(ComponentetextoRegisto entity)
        {
            ComponentetextoRegisto entityToUpdate = _moduloContribuicoesContext.ComponentetextoRegisto
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(ComponentetextoRegisto entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public List<HistoryText> GetHistoryTextsByTarefaAtivoIds(List<int> tarefaIds)
        {
            var response = new List<HistoryText>();

            response = _moduloContribuicoesContext.ComponentetextoRegisto
                .Where(m => m.IndActivo && tarefaIds.Contains(m.TarefaAtivoFk))
               .Select(u => new HistoryText
               {
                   user = u.UtilizadorAlteracao == null ? _moduloContribuicoesContext.Utilizador.FirstOrDefault(x => x.IdUtilizador == u.UtilizadorCriacao).Username : _moduloContribuicoesContext.Utilizador.FirstOrDefault(x => x.IdUtilizador == u.UtilizadorAlteracao).Username,
                   tarefa = u.TarefaAtivoFkNavigation.TarefaconfigFkNavigation.Nome,
                   titulo = u.TituloTexto,
                   texto = u.Texto,
                   data = u.DataAlteracao ?? u.DataCriacao
               })
               .OrderByDescending(x => x.data)
               .ToList();

            return response;
        }

        public IEnumerable<ComponentetextoRegisto> GetAllByTarefaAtivoId(int tarefaAtivoId)
        {
            return _moduloContribuicoesContext.ComponentetextoRegisto.Where(u => u.TarefaAtivoFk == tarefaAtivoId && u.IndActivo).ToList();
        }
    }
}