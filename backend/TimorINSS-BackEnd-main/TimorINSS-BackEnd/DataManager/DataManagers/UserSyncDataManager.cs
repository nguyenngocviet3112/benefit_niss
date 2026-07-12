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
    // Màn "Đồng bộ User từ hệ thống cũ" (Quản lý User & Phân quyền) — đưa
    // Utilizador cũ (Interno=true) vào danh sách quản lý của mode mới.
    // Additive only: không đụng Utilizador/Perfil cũ, chỉ tạo thêm
    // UserModeAccess (cổng vào mode mới) + UserProfile (Nome/Email do admin
    // gõ tay lúc đồng bộ, vì Utilizador cũ không có tên đáng tin cậy).
    // KHÔNG tự gán UserPermission — 2 hệ thống quyền khác ngữ nghĩa nhau,
    // admin phải tự gán tay ở màn Quản lý User sau khi đồng bộ (quyết định
    // 2026-07-12, tránh đoán nhầm quyền). External chỉ đọc, không ghi gì cả.
    public class UserSyncDataManager : IUserSyncDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public UserSyncDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        public UserSyncListResponse GetInternalList()
        {
            UserSyncListResponse response = new UserSyncListResponse();
            try
            {
                List<Utilizador> users = _unitOfWork.UserPermissionRepository.GetAllInternalUsers();
                List<int> activeModeAccessIds = _unitOfWork.UserModeAccessRepository.GetActiveUtilizadorFks();
                List<UserProfile> profiles = _unitOfWork.UserPermissionRepository.GetAllProfiles();

                response.Items = users
                    .Select(u => MapItem(u, activeModeAccessIds.Contains(u.IdUtilizador), profiles.SingleOrDefault(p => p.UtilizadorFk == u.IdUtilizador)))
                    .ToList();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public UserSyncListResponse GetExternalList()
        {
            UserSyncListResponse response = new UserSyncListResponse();
            try
            {
                List<Utilizador> users = _unitOfWork.UserPermissionRepository.GetAllExternalUsers();
                response.Items = users
                    .Select(u => MapItem(u, hasNewModeAccess: false, profile: null))
                    .ToList();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public SyncInternalUserResponse SyncInternal(SyncInternalUserRequest request)
        {
            SyncInternalUserResponse response = new SyncInternalUserResponse { RequestId = request.RequestId };
            try
            {
                Utilizador user = _unitOfWork.UserPermissionRepository.GetInternalUser(request.Id);
                if (user == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "USERSYNC-NOT-FOUND", ErrorMessage = "Không tìm thấy user internal này." });
                    return response;
                }

                if (!_unitOfWork.UserModeAccessRepository.HasAccess(user.IdUtilizador))
                {
                    UserModeAccess access = new UserModeAccess
                    {
                        UtilizadorFk = user.IdUtilizador,
                        IndActivo = true
                    };
                    access = _utils.SetDetailsToEntity(access);
                    _unitOfWork.UserModeAccessRepository.Add(access);
                }

                UserProfile profile = _unitOfWork.UserPermissionRepository.GetProfile(user.IdUtilizador);
                if (profile == null)
                {
                    profile = new UserProfile
                    {
                        UtilizadorFk = user.IdUtilizador,
                        Nome = request.Nome,
                        Email = request.Email,
                        DepartamentoFk = request.DepartamentoFk,
                        IndActivo = true
                    };
                    profile = _utils.SetDetailsToEntity(profile);
                    _unitOfWork.UserPermissionRepository.AddProfile(profile);
                }
                else
                {
                    profile.Nome = request.Nome;
                    profile.Email = request.Email;
                    profile.DepartamentoFk = request.DepartamentoFk;
                    profile = _utils.UpdateDetailsToEntity(profile);
                    _unitOfWork.UserPermissionRepository.UpdateProfile(profile);
                }

                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        private UserSyncListItemDataContract MapItem(Utilizador user, bool hasNewModeAccess, UserProfile profile)
        {
            return new UserSyncListItemDataContract
            {
                Id = user.IdUtilizador,
                Username = user.Username,
                IndActivo = user.IndActivo,
                Locked = user.Locked,
                HasNewModeAccess = hasNewModeAccess,
                Nome = profile?.Nome,
                Email = profile?.Email,
                DepartamentoNome = profile?.DepartamentoFkNavigation?.Nome
            };
        }
    }
}
