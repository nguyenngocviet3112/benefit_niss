using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IUserPermissionRepository
    {
        List<PermissionPreset> GetAllPresets();
        PermissionPreset GetPreset(int id);

        List<UserPermission> GetByUser(int utilizadorFk);
        List<UserPermission> GetAll();
        void Add(UserPermission entity);
        void Update(UserPermission entity);

        // Reads Utilizador directly (any IndActivo status, unlike
        // IUtilizadoresRepository.GetAll() which only returns active rows) —
        // this admin screen must also show/reactivate disabled accounts.
        List<Utilizador> GetAllInternalUsers();
        Utilizador GetInternalUser(int id);
        bool UsernameExists(string username);

        List<UserProfile> GetAllProfiles();
        UserProfile GetProfile(int utilizadorFk);
        void AddProfile(UserProfile entity);
        void UpdateProfile(UserProfile entity);
    }
}
