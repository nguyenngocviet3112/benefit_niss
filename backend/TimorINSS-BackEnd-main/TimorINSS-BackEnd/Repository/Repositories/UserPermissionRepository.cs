using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class UserPermissionRepository : IUserPermissionRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public UserPermissionRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public List<PermissionPreset> GetAllPresets()
        {
            return _moduloContribuicoesContext.PermissionPreset
                .Include(p => p.PermissionPresetItem)
                .Where(p => p.IndActivo)
                .OrderBy(p => p.Nome)
                .ToList();
        }

        public PermissionPreset GetPreset(int id)
        {
            return _moduloContribuicoesContext.PermissionPreset
                .Include(p => p.PermissionPresetItem)
                .SingleOrDefault(p => p.Id == id);
        }

        public List<UserPermission> GetByUser(int utilizadorFk)
        {
            return _moduloContribuicoesContext.UserPermission
                .Where(u => u.IndActivo && u.UtilizadorFk == utilizadorFk)
                .ToList();
        }

        public List<UserPermission> GetAll()
        {
            return _moduloContribuicoesContext.UserPermission
                .Where(u => u.IndActivo)
                .ToList();
        }

        public void Add(UserPermission entity)
        {
            _moduloContribuicoesContext.UserPermission.Add(entity);
        }

        public void Update(UserPermission entity)
        {
            UserPermission entityToUpdate = _moduloContribuicoesContext.UserPermission
                .Single(u => u.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public List<Utilizador> GetAllInternalUsers()
        {
            return _moduloContribuicoesContext.Utilizador
                .Where(u => u.Interno == true)
                .OrderBy(u => u.Username)
                .ToList();
        }

        public Utilizador GetInternalUser(int id)
        {
            return _moduloContribuicoesContext.Utilizador
                .SingleOrDefault(u => u.IdUtilizador == id && u.Interno == true);
        }

        public bool UsernameExists(string username)
        {
            return _moduloContribuicoesContext.Utilizador.Any(u => u.Username == username);
        }

        public List<UserProfile> GetAllProfiles()
        {
            return _moduloContribuicoesContext.UserProfile
                .Include(p => p.DepartamentoFkNavigation)
                .Where(p => p.IndActivo)
                .ToList();
        }

        public UserProfile GetProfile(int utilizadorFk)
        {
            return _moduloContribuicoesContext.UserProfile
                .Include(p => p.DepartamentoFkNavigation)
                .SingleOrDefault(p => p.IndActivo && p.UtilizadorFk == utilizadorFk);
        }

        public void AddProfile(UserProfile entity)
        {
            _moduloContribuicoesContext.UserProfile.Add(entity);
        }

        public void UpdateProfile(UserProfile entity)
        {
            UserProfile entityToUpdate = _moduloContribuicoesContext.UserProfile
                .Single(p => p.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }
    }
}
