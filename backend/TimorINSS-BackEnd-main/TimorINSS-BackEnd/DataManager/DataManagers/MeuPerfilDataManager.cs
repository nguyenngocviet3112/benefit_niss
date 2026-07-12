using Microsoft.Extensions.Configuration;
using System;
using System.Net.Mail;
using System.Text.RegularExpressions;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    // "Meu Perfil" — tự phục vụ (self-service): mỗi user chỉ xem/sửa được
    // đúng hồ sơ của CHÍNH MÌNH, luôn lấy Id từ request.UserId (header
    // User-Id do interceptor gắn tự động theo user đang đăng nhập), KHÔNG
    // nhận Id từ client — khác với UserPermissionDataManager (màn admin
    // "Quản lý User & Phân quyền", quản lý user KHÁC, có RequirePerm riêng).
    public class MeuPerfilDataManager : IMeuPerfilDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;
        private readonly IConfiguration _configuration;

        // Cùng chính sách mật khẩu đã dùng ở UtilizadorDataManager: tối thiểu 8 ký tự,
        // có chữ hoa + chữ thường + số + ký tự đặc biệt.
        private const string PasswordPattern = "(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[$@#!%*?&])[A-Za-z\\d$@#!%*?&].{8,}";

        public MeuPerfilDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
            _configuration = configuration;
        }

        public MeuPerfilResponse GetMeuPerfil(GetMeuPerfilRequest request)
        {
            MeuPerfilResponse response = new MeuPerfilResponse();
            try
            {
                Utilizador user = _unitOfWork.UtilizadoresRepository.Get(request.UserId);
                if (user == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "MP-NOT-FOUND", ErrorMessage = "Không tìm thấy tài khoản." });
                    return response;
                }

                UserProfile profile = _unitOfWork.UserPermissionRepository.GetProfile(request.UserId);

                response.Username = user.Username;
                response.Nome = profile?.Nome;
                response.Email = profile?.Email;
                response.DepartamentoFk = profile?.DepartamentoFk;
                response.DepartamentoNome = profile?.DepartamentoFkNavigation?.Nome;
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract AlterarEmail(AlterarEmailRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                if (!IsValidEmail(request.Email))
                {
                    response.Errors.Add(new Error { ErrorCode = "MP-EMAIL-INVALID", ErrorMessage = "Email không hợp lệ." });
                    return response;
                }

                UserProfile profile = _unitOfWork.UserPermissionRepository.GetProfile(request.UserId);
                if (profile == null)
                {
                    profile = new UserProfile
                    {
                        UtilizadorFk = request.UserId,
                        Email = request.Email,
                        IndActivo = true
                    };
                    profile = _utils.SetDetailsToEntity(profile);
                    _unitOfWork.UserPermissionRepository.AddProfile(profile);
                }
                else
                {
                    profile.Email = request.Email;
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

        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            try
            {
                _ = new MailAddress(email);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public ResponseBaseDataContract AlterarSenha(AlterarSenhaRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                Utilizador user = _unitOfWork.UtilizadoresRepository.Get(request.UserId);
                if (user == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "MP-NOT-FOUND", ErrorMessage = "Không tìm thấy tài khoản." });
                    return response;
                }

                string internalSalt = _configuration["AppSettings:InternalSalt"];
                string senhaAtualHash = _utils.CreateHashPassword(request.SenhaAtual, user.Salt, internalSalt);
                if (senhaAtualHash != user.Password)
                {
                    response.Errors.Add(new Error { ErrorCode = "MP-WRONG-PASSWORD", ErrorMessage = "Mật khẩu hiện tại không đúng." });
                    return response;
                }

                if (string.IsNullOrEmpty(request.SenhaNova) || request.SenhaNova != request.ConfirmarSenhaNova)
                {
                    response.Errors.Add(new Error { ErrorCode = "MP-CONFIRM-MISMATCH", ErrorMessage = "Mật khẩu mới và xác nhận không khớp." });
                    return response;
                }

                if (!Regex.IsMatch(request.SenhaNova, PasswordPattern))
                {
                    response.Errors.Add(new Error
                    {
                        ErrorCode = "MP-WEAK-PASSWORD",
                        ErrorMessage = "Mật khẩu mới phải có tối thiểu 8 ký tự, gồm chữ hoa, chữ thường, số và ký tự đặc biệt."
                    });
                    return response;
                }

                string newSalt = Guid.NewGuid().ToString();
                user.Salt = newSalt;
                user.Password = _utils.CreateHashPassword(request.SenhaNova, newSalt, internalSalt);
                user = _utils.UpdateDetailsToEntity(user);
                _unitOfWork.UtilizadoresRepository.Update(user);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }
    }
}
