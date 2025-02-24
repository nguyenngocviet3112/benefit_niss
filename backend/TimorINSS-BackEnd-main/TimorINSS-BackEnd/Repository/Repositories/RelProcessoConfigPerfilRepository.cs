using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class RelProcessoConfigPerfilRepository : IRelProcessoConfigPerfilRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public RelProcessoConfigPerfilRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Relprocessoconfigperfil> GetAll()
        {
            return _moduloContribuicoesContext.Relprocessoconfigperfil.Where(u => u.IndActivo).ToList();
        }

        public Relprocessoconfigperfil Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var relProcessoConfigPerfil = _moduloContribuicoesContext.Relprocessoconfigperfil
                .SingleOrDefault(u => u.Id == id);

            return relProcessoConfigPerfil;
        }

        public RelprocessoconfigperfilDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var relProcessoConfigPerfil = _moduloContribuicoesContext.Relprocessoconfigperfil
                .SingleOrDefault(u => u.Id == id);

            RelprocessoconfigperfilDto relProcessoConfigPerfilDto = Utils.MappClassToDto<Relprocessoconfigperfil, RelprocessoconfigperfilDto>(relProcessoConfigPerfil);
            return relProcessoConfigPerfilDto;
        }

        public void Add(Relprocessoconfigperfil entity)
        {
            _moduloContribuicoesContext.Relprocessoconfigperfil.Add(entity);
        }

        public void Update(Relprocessoconfigperfil entity)
        {
            Relprocessoconfigperfil entityToUpdate = _moduloContribuicoesContext.Relprocessoconfigperfil
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Relprocessoconfigperfil entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public List<Relprocessoconfigperfil> GetRelPerfilByProcesso(long processoId)
        {
            var response = new List<Relprocessoconfigperfil>();

            response = _moduloContribuicoesContext.Relprocessoconfigperfil
                .Where(x => x.ProcessoConfigFk == processoId).ToList();

            return response;
        }

        public List<int> GetProcessoIdsByPerfis(List<int> perfilIds)
        {
            var response = new List<int>();

            response = _moduloContribuicoesContext.Relprocessoconfigperfil
                .Where(x => perfilIds.Contains(x.PerfilFk) && x.IndActivo)
                .Select(x => x.ProcessoConfigFk)
                .ToList();

            return response;
        }
    }
}