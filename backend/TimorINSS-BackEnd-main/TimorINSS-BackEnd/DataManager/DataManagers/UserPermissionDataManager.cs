using System;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    // Backs the single combined "Quản lý User & Phân quyền" screen: list +
    // create/edit user with permission/preset assignment saved in one action.
    // Additive only — does not touch Perfil/Funcionalidade/UserModeAccess, and
    // deliberately does NOT enforce any of these tokens on business controllers
    // yet (that's a separate follow-up once the Chi tiêu modules settle).
    public class UserPermissionDataManager : IUserPermissionDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

        public UserPermissionDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
            _configuration = configuration;
        }

        public PermissionCatalogResponse GetCatalog()
        {
            PermissionCatalogResponse response = new PermissionCatalogResponse();
            try
            {
                response.Groups = PermissionCatalog.Groups;
                response.Presets = _unitOfWork.UserPermissionRepository.GetAllPresets()
                    .Select(MapPreset)
                    .ToList();
                response.Departamentos = _unitOfWork.DepartamentoRepository.GetAllDepartamentosAtivo();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public UserPermissionListResponse GetUsers()
        {
            UserPermissionListResponse response = new UserPermissionListResponse();
            try
            {
                List<Utilizador> users = _unitOfWork.UserPermissionRepository.GetAllInternalUsers();
                List<UserPermission> allPerms = _unitOfWork.UserPermissionRepository.GetAll();
                List<PermissionPreset> presets = _unitOfWork.UserPermissionRepository.GetAllPresets();
                List<UserProfile> profiles = _unitOfWork.UserPermissionRepository.GetAllProfiles();

                response.Items = users
                    .Select(u => MapUser(
                        u,
                        allPerms.Where(p => p.UtilizadorFk == u.IdUtilizador).ToList(),
                        presets,
                        profiles.SingleOrDefault(p => p.UtilizadorFk == u.IdUtilizador)))
                    .ToList();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public UserPermissionDetailResponse GetUser(GetUserPermissionRequest request)
        {
            UserPermissionDetailResponse response = new UserPermissionDetailResponse { RequestId = request.RequestId };
            try
            {
                Utilizador user = _unitOfWork.UserPermissionRepository.GetInternalUser(request.Id);
                if (user == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "USERPERM-NOT-FOUND", ErrorMessage = "Không tìm thấy user." });
                    return response;
                }

                List<UserPermission> perms = _unitOfWork.UserPermissionRepository.GetByUser(user.IdUtilizador);
                List<PermissionPreset> presets = _unitOfWork.UserPermissionRepository.GetAllPresets();
                UserProfile profile = _unitOfWork.UserPermissionRepository.GetProfile(user.IdUtilizador);
                response.Item = MapUser(user, perms, presets, profile);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public SaveUserPermissionResponse SaveUser(SaveUserPermissionRequest request)
        {
            SaveUserPermissionResponse response = new SaveUserPermissionResponse { RequestId = request.RequestId };
            try
            {
                List<string> explicitTokens = (request.Tokens ?? new List<string>()).Distinct().ToList();
                foreach (string token in explicitTokens)
                {
                    if (!PermissionCatalog.IsValidToken(token))
                    {
                        response.Errors.Add(new Error { ErrorCode = "USERPERM-INVALID-TOKEN", ErrorMessage = $"Permission token không hợp lệ: {token}" });
                        return response;
                    }
                }

                List<PermissionPreset> allPresets = _unitOfWork.UserPermissionRepository.GetAllPresets();
                List<int> presetIds = (request.PresetIds ?? new List<int>()).Distinct().ToList();
                List<PermissionPreset> chosenPresets = allPresets.Where(p => presetIds.Contains(p.Id)).ToList();
                if (chosenPresets.Count != presetIds.Count)
                {
                    response.Errors.Add(new Error { ErrorCode = "USERPERM-PRESET-NOT-FOUND", ErrorMessage = "Có preset không tồn tại hoặc đã bị vô hiệu hoá." });
                    return response;
                }

                Utilizador user;
                if (request.Id > 0)
                {
                    user = _unitOfWork.UserPermissionRepository.GetInternalUser(request.Id);
                    if (user == null)
                    {
                        response.Errors.Add(new Error { ErrorCode = "USERPERM-NOT-FOUND", ErrorMessage = "Không tìm thấy user." });
                        return response;
                    }

                    if (!string.Equals(user.Username, request.Username, StringComparison.Ordinal)
                        && _unitOfWork.UserPermissionRepository.UsernameExists(request.Username))
                    {
                        response.Errors.Add(new Error { ErrorCode = "USERPERM-DUP-USERNAME", ErrorMessage = "Username đã tồn tại." });
                        return response;
                    }

                    user.Username = request.Username;
                    user.IndActivo = request.IndActivo;
                    if (!string.IsNullOrEmpty(request.Password))
                    {
                        user.Salt = Guid.NewGuid().ToString();
                        user.Password = _utils.CreateHashPassword(request.Password, user.Salt, _configuration["AppSettings:InternalSalt"]);
                        user.LoginAttempts = 0;
                        user.Locked = false;
                    }
                    user = _utils.UpdateDetailsToEntity(user);
                    _unitOfWork.UtilizadoresRepository.Update(user);
                    _unitOfWork.Commit();
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                    {
                        response.Errors.Add(new Error { ErrorCode = "USERPERM-MISSING-FIELDS", ErrorMessage = "Username và Password là bắt buộc khi tạo user mới." });
                        return response;
                    }
                    if (_unitOfWork.UserPermissionRepository.UsernameExists(request.Username))
                    {
                        response.Errors.Add(new Error { ErrorCode = "USERPERM-DUP-USERNAME", ErrorMessage = "Username đã tồn tại." });
                        return response;
                    }

                    user = new Utilizador
                    {
                        Username = request.Username,
                        Salt = Guid.NewGuid().ToString(),
                        IndActivo = request.IndActivo,
                        Interno = true,
                        LoginAttempts = 0,
                        Locked = false
                    };
                    user.Password = _utils.CreateHashPassword(request.Password, user.Salt, _configuration["AppSettings:InternalSalt"]);
                    user = _utils.SetDetailsToEntity(user);
                    _unitOfWork.UtilizadoresRepository.Add(user);
                    _unitOfWork.Commit();

                    AssignPlaceholderPerfil(user.IdUtilizador);
                }

                SyncPermissions(user.IdUtilizador, explicitTokens, chosenPresets);
                SaveProfile(user.IdUtilizador, request.Nome, request.Email, request.DepartamentoFk);

                response.Id = user.IdUtilizador;
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        private void SyncPermissions(int utilizadorId, List<string> explicitTokens, List<PermissionPreset> chosenPresets)
        {
            // token -> source preset id (null = granted individually)
            Dictionary<string, int?> desired = new Dictionary<string, int?>();
            foreach (string token in explicitTokens)
            {
                desired[token] = null;
            }
            foreach (PermissionPreset preset in chosenPresets)
            {
                foreach (PermissionPresetItem item in preset.PermissionPresetItem)
                {
                    desired[item.PermissionToken] = preset.Id;
                }
            }

            List<UserPermission> existing = _unitOfWork.UserPermissionRepository.GetByUser(utilizadorId);
            Dictionary<string, UserPermission> existingByToken = existing.ToDictionary(p => p.PermissionToken);

            foreach (UserPermission current in existing)
            {
                if (!desired.ContainsKey(current.PermissionToken))
                {
                    current.IndActivo = false;
                    UserPermission toUpdate = _utils.UpdateDetailsToEntity(current);
                    _unitOfWork.UserPermissionRepository.Update(toUpdate);
                }
                else if (current.SourcePresetFk != desired[current.PermissionToken])
                {
                    current.SourcePresetFk = desired[current.PermissionToken];
                    UserPermission toUpdate = _utils.UpdateDetailsToEntity(current);
                    _unitOfWork.UserPermissionRepository.Update(toUpdate);
                }
            }

            foreach (KeyValuePair<string, int?> pair in desired)
            {
                if (existingByToken.ContainsKey(pair.Key))
                {
                    continue;
                }

                UserPermission entity = new UserPermission
                {
                    UtilizadorFk = utilizadorId,
                    PermissionToken = pair.Key,
                    SourcePresetFk = pair.Value,
                    IndActivo = true
                };
                entity = _utils.SetDetailsToEntity(entity);
                _unitOfWork.UserPermissionRepository.Add(entity);
            }

            _unitOfWork.Commit();
        }

        // Nome/Email/Departamento — added on top of the original username/
        // password/permissions scope (user request, 2026-07-11). Email is
        // captured to back a future password-reset flow; that flow itself
        // isn't built yet, this only stores the data.
        private void SaveProfile(int utilizadorId, string nome, string email, int? departamentoFk)
        {
            UserProfile profile = _unitOfWork.UserPermissionRepository.GetProfile(utilizadorId);
            if (profile == null)
            {
                profile = new UserProfile
                {
                    UtilizadorFk = utilizadorId,
                    Nome = nome,
                    Email = email,
                    DepartamentoFk = departamentoFk,
                    IndActivo = true
                };
                profile = _utils.SetDetailsToEntity(profile);
                _unitOfWork.UserPermissionRepository.AddProfile(profile);
            }
            else
            {
                profile.Nome = nome;
                profile.Email = email;
                profile.DepartamentoFk = departamentoFk;
                profile = _utils.UpdateDetailsToEntity(profile);
                _unitOfWork.UserPermissionRepository.UpdateProfile(profile);
            }
            _unitOfWork.Commit();
        }

        // InternalLoginManager (old login path, untouched) rejects any non-"admin"
        // login with zero active Relutilizadorperfil rows ("CurrentUserHasNoProfile").
        // That check belongs to the old Perfil/Funcionalidade system, unrelated to
        // this new UserPermission RBAC — accounts created here would otherwise be
        // unable to log in at all. Link every new account to a fixed placeholder
        // Perfil (db_migrations/2026-07-12d_placeholder_perfil.sql, zero
        // Relperfilfuncionalidade rows so it grants no old-system capability) purely
        // to satisfy that legacy gate.
        private const string PlaceholderPerfilDescricao = "Conta Módulo Contabilidade (sem Perfil legado)";

        private void AssignPlaceholderPerfil(int utilizadorId)
        {
            Perfil placeholder = _unitOfWork.PerfilRepository.GetAll()
                .SingleOrDefault(p => p.Descricao == PlaceholderPerfilDescricao);
            if (placeholder == null)
            {
                return;
            }

            Relutilizadorperfil rel = new Relutilizadorperfil
            {
                UtilizadorFk = utilizadorId,
                PerfilFk = placeholder.Id,
                IndActivo = true
            };
            rel = _utils.SetDetailsToEntity(rel);
            _unitOfWork.RelUtilizadorPerfilRepository.Add(rel);
            _unitOfWork.Commit();
        }

        private UserPermissionListItemDataContract MapUser(Utilizador user, List<UserPermission> perms, List<PermissionPreset> allPresets, UserProfile profile)
        {
            List<int> presetIds = perms.Where(p => p.SourcePresetFk.HasValue).Select(p => p.SourcePresetFk.Value).Distinct().ToList();

            return new UserPermissionListItemDataContract
            {
                Id = user.IdUtilizador,
                Username = user.Username,
                IndActivo = user.IndActivo,
                Locked = user.Locked,
                Nome = profile?.Nome,
                Email = profile?.Email,
                DepartamentoFk = profile?.DepartamentoFk,
                DepartamentoNome = profile?.DepartamentoFkNavigation?.Nome,
                Tokens = perms.Select(p => p.PermissionToken).Distinct().ToList(),
                PresetCodigos = allPresets.Where(p => presetIds.Contains(p.Id)).Select(p => p.Codigo).ToList()
            };
        }

        private PermissionPresetDataContract MapPreset(PermissionPreset preset)
        {
            return new PermissionPresetDataContract
            {
                Id = preset.Id,
                Codigo = preset.Codigo,
                Nome = preset.Nome,
                Tokens = preset.PermissionPresetItem.Select(i => i.PermissionToken).ToList()
            };
        }
    }
}
