using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class RelProcessoConfigTarefaRepository : IRelProcessoConfigTarefaRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public RelProcessoConfigTarefaRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Relprocessoconfigtarefa> GetAll()
        {
            return _moduloContribuicoesContext.Relprocessoconfigtarefa.Where(u => u.IndActivo).ToList();
        }

        public Relprocessoconfigtarefa Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var Relprocessoconfigtarefa = _moduloContribuicoesContext.Relprocessoconfigtarefa
                .SingleOrDefault(u => u.Id == id);

            return Relprocessoconfigtarefa;
        }

        public Relprocessoconfigtarefadto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var Relprocessoconfigtarefa = _moduloContribuicoesContext.Relprocessoconfigtarefa
                .SingleOrDefault(u => u.Id == id);

            Relprocessoconfigtarefadto Relprocessoconfigtarefadto = Utils.MappClassToDto<Relprocessoconfigtarefa, Relprocessoconfigtarefadto>(Relprocessoconfigtarefa);
            return Relprocessoconfigtarefadto;
        }

        public void Add(Relprocessoconfigtarefa entity)
        {
            _moduloContribuicoesContext.Relprocessoconfigtarefa.Add(entity);
        }

        public void Update(Relprocessoconfigtarefa entity)
        {
            Relprocessoconfigtarefa entityToUpdate = _moduloContribuicoesContext.Relprocessoconfigtarefa
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Relprocessoconfigtarefa entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public List<Relprocessoconfigtarefa> GetRelTarefaByProcesso(long processoId)
        {
            var response = new List<Relprocessoconfigtarefa>();

            response = _moduloContribuicoesContext.Relprocessoconfigtarefa
                .Where(x => x.ProcessoConfigFk == processoId).ToList();

            return response;
        }
    }
}