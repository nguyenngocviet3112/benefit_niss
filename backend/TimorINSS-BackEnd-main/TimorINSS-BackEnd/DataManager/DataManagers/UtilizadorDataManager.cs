using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;
using System.Text.RegularExpressions;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class UtilizadorDataManager : IUtilizadorDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration Configuration;
        private readonly IEmailSenderDataManager _emailSenderDataManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUtilsDataManager _utils;

        private string regexPattern = "(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[$@#!%*?&])[A-Za-z\\d$@#!%*?&].{8,}";

        public UtilizadorDataManager(IUnitOfWork unitOfWork, IConfiguration configuration, IEmailSenderDataManager emailSenderDataManager, IHttpContextAccessor httpContextAccessor, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            Configuration = configuration;
            _emailSenderDataManager = emailSenderDataManager;
            _httpContextAccessor = httpContextAccessor;
            _utils = utils;
        }

        public IEnumerable<Utilizador> GetAll()
        {
            return _unitOfWork.UtilizadoresRepository.GetAll();
        }

        public Utilizador Get(long id)
        {
            return _unitOfWork.UtilizadoresRepository.Get(id);
        }

        public UtilizadorDto GetDto(long id)
        {
            return _unitOfWork.UtilizadoresRepository.GetDto(id);
        }

        public void Add(Utilizador entity)
        {
            entity.Salt = Guid.NewGuid().ToString();
            entity.Password = _utils.CreateHashPassword(entity.Password, entity.Salt, Configuration["AppSettings:InternalSalt"]);
            entity.IndActivo = true;
            entity.UtilizadorCriacao = 2;
            entity.DataCriacao = DateTime.Now;
            _unitOfWork.UtilizadoresRepository.Add(entity);
            _unitOfWork.Commit();
        }

        public void Update(Utilizador entity)
        {
            _unitOfWork.UtilizadoresRepository.Update(entity);
            _unitOfWork.Commit();
        }

        public void Delete(Utilizador entity)
        {
            _unitOfWork.UtilizadoresRepository.Delete(entity);
            _unitOfWork.Commit();
        }

        public LoginResponse LoginManager(LoginRequest request)
        {
            var response = new LoginResponse { RequestId = request.RequestId };
            var user = _unitOfWork.UtilizadoresRepository.GetByUsername(request.Username);

            // Returns if user is locked
            if (user.Locked)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.UserIsBlocked).ToString(),
                    ErrorMessage = ErrorsDataContract.UserIsBlocked.ToString()
                });
                return response;
            }

            if (user == null || !VerifyPassword(request.Password, user?.Salt, Configuration["AppSettings:InternalSalt"], user.Password))
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.InvalidUserNameOrPassword).ToString(),
                    ErrorMessage = ErrorsDataContract.InvalidUserNameOrPassword.ToString()
                });
                //Logic of increment login attempts and lock if limit is exceeded
                if (user != null)
                {
                    user.LoginAttempts++;
                    if (user.LoginAttempts >= int.Parse(Configuration["AppSettings:loginLimitAttempts"]))
                    {
                        user.Locked = true;
                    }
                    _unitOfWork.UtilizadoresRepository.Update(user);
                    _unitOfWork.Commit();
                }
            }

            //mais validações se forem precisas

            if (response.Errors.Count > 0)
            {
                return response;
            }

            if (user.LoginAttempts > 0)
            {
                user.LoginAttempts = 0;
                _unitOfWork.UtilizadoresRepository.Update(user);
            }

            var tokenString = GenerateToken(user.IdUtilizador, int.Parse(Configuration["AppSettings:expirationTimeMinutes"]));

            response.user = new User
            {
                Id = user.IdUtilizador,
                Username = user.Username,
                NISS = user.UtilizadorEntidadeFkNavigation == null ? "" : user.UtilizadorEntidadeFkNavigation.Niss,
                IdEntidade = user.UtilizadorEntidadeFk.GetValueOrDefault(),
                isInternal = user.Interno ?? false
            };
            response.Token = tokenString;

            try
            {
                var newTokenSalt = Guid.NewGuid().ToString();
                // Fetch current logged sessions
                var loggedSessions = _unitOfWork.UtilizadorTokenRepository.GetByUserId(user.IdUtilizador);

                //creates new unique logged session
                Utilizadortoken newSession = new Utilizadortoken
                {
                    IdUtilizador = user.IdUtilizador,
                    Salt = newTokenSalt,
                    TokenString = _utils.CreateHashPassword(tokenString, newTokenSalt, Configuration["AppSettings:TokenSalt"])
                };

                if (loggedSessions.Count > 0)
                    loggedSessions.ForEach(session => { _unitOfWork.UtilizadorTokenRepository.Delete(session); });

                _unitOfWork.UtilizadorTokenRepository.Add(newSession);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Token = null;
                response.user = null;
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            return response;
        }

        public ResponseBaseDataContract RecoverManager(RecoverRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            //Validate if Entidade empregadora exists for this niss
            var entidadeEmpregadora = _unitOfWork.EntidadeEmpregadoraRepository.GetByNiss(request.Niss);
            if (entidadeEmpregadora == null)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = "-1",ErrorMessage = "Entidade doesn't exist"
                });
                return response;
            }

            //Validate if there is an existing email for this Entidade Empregadora
            var contacto = _unitOfWork.ContactoRepository.GetDtoByEmail(request.Email, entidadeEmpregadora.IdEntidadeEmpreg);
            if (contacto == null)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = "-1",
                    ErrorMessage = "Email doesn't exist"
                });
                return response;
            }

            //Validate if user exists for this Entidade Empregadora
            var user = _unitOfWork.UtilizadoresRepository.GetByEntidadeEmpregadora(entidadeEmpregadora.IdEntidadeEmpreg);
            if (user == null)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = "-1",
                    ErrorMessage = "User doesn't exist for this entidade empregadora"
                });
                return response;
            }

            var tokenString = GenerateToken(user.IdUtilizador, 15);

            try
            {
                _emailSenderDataManager.SendEmailAsync(contacto.Email, "Recuperação de palavra passe", tokenString, user.Username).GetAwaiter();

                //Lógica de criar token com uma só utilização
                var newTokenSalt = Guid.NewGuid().ToString();

                //Elimina tokens anteriores
                var previousTokens = _unitOfWork.UtilizadorTokenRepository.GetUniqueTokensByUserId(user.IdUtilizador);

                if (previousTokens.Count > 0)
                    previousTokens.ForEach(session => { _unitOfWork.UtilizadorTokenRepository.Delete(session); });

                //cria token para depois ser de uma só utilização
                Utilizadortoken recoverToken = new Utilizadortoken
                {
                    IdUtilizador = user.IdUtilizador,
                    Salt = newTokenSalt,
                    TokenString = _utils.CreateHashPassword(tokenString, newTokenSalt, Configuration["AppSettings:TokenSalt"]),
                    IsRecover = true
                };                

                _unitOfWork.UtilizadorTokenRepository.Add(recoverToken);
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = "-1",
                    ErrorMessage = ex.Message
                });
            }

            return response;
        }

        public ResponseBaseDataContract FirstAcessManager(RecoverRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            //Validate if Entidade empregadora exists for this niss
            var entidadeEmpregadora = _unitOfWork.EntidadeEmpregadoraRepository.GetByNiss(request.Niss);
            if (entidadeEmpregadora == null)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = "-1",
                    ErrorMessage = "Entidade doesn't exist"
                });
                return response;
            }

            //Validate if there is an existing email for this Entidade Empregadora
            var contacto = _unitOfWork.ContactoRepository.GetDtoByEmail(request.Email, entidadeEmpregadora.IdEntidadeEmpreg);
            if (contacto == null)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = "-1",
                    ErrorMessage = "Email doesn't exist"
                });
                return response;
            }

            //Validate if user exists for this Entidade Empregadora
            var user = _unitOfWork.UtilizadoresRepository.GetByEntidadeEmpregadora(entidadeEmpregadora.IdEntidadeEmpreg);
            if (user != null)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = "-1",
                    ErrorMessage = "User already exists for this entidade empregadora"
                });
                return response;
            }

            var tokenString = GenerateToken(entidadeEmpregadora.IdEntidadeEmpreg, 15);

            try
            {
                _emailSenderDataManager.SendEmailAsync(contacto.Email, "Primeiro acesso", tokenString).GetAwaiter();

                //Lógica de criar token com uma só utilização
                var newTokenSalt = Guid.NewGuid().ToString();

                //Elimina tokens anteriores
                var previousTokens = _unitOfWork.UtilizadorTokenRepository.GetUniqueTokensByEntidadeId(entidadeEmpregadora.IdEntidadeEmpreg);

                if (previousTokens.Count > 0)
                    previousTokens.ForEach(session => { _unitOfWork.UtilizadorTokenRepository.Delete(session); });

                //cria token para depois ser de uma só utilização
                Utilizadortoken recoverToken = new Utilizadortoken
                {
                    Salt = newTokenSalt,
                    TokenString = _utils.CreateHashPassword(tokenString, newTokenSalt, Configuration["AppSettings:TokenSalt"]),
                    EntidadeId = entidadeEmpregadora.IdEntidadeEmpreg
                };

                _unitOfWork.UtilizadorTokenRepository.Add(recoverToken);
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                response.Errors.Add(new Error
                {
                    ErrorMessage = ex.Message
                });
            }

            return response;
        }

        public ResponseBaseDataContract SetUpPasswordManager(RecoverSetPasswordRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            if (request.Password != request.ConfirmPassword)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = "-1",
                    ErrorMessage = "Password and Confirm password do not match"
                });
                return response;
            }

            //Valida o formato da password que deve conter pelo menos 8 caracteres, um número, um caractere maiúsculo, um caractere minúsculo e um caractere especial
            if (!Regex.Match(request.Password, regexPattern).Success)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.WrongPasswordFormat).ToString(),
                    ErrorMessage = ErrorsDataContract.WrongPasswordFormat.ToString()
                });
                return response;
            }

            //Valida o token e retorna o id de utilizador
            var userId = Authenticate(request.Token);

            if (userId == null)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.ExpiredToken).ToString(),
                    ErrorMessage = ErrorsDataContract.ExpiredToken.ToString()
                });
                return response;
            }

            //Valida token que é enviado com token que está na base de dados
            var activeToken = _unitOfWork.UtilizadorTokenRepository.GetUniqueTokensByUserId(userId.Value).FirstOrDefault();
            if (activeToken == null || activeToken.TokenString != _utils.CreateHashPassword(request.Token, activeToken.Salt, Configuration["AppSettings:TokenSalt"]))
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.ExpiredToken).ToString(),
                    ErrorMessage = ErrorsDataContract.ExpiredToken.ToString()
                });
                return response;
            }

            var user = _unitOfWork.UtilizadoresRepository.Get(userId.Value);

            if (user == null)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = "-1",
                    ErrorMessage = "User doesn't exist"
                });
                return response;
            }

            //Verifica se é admin
            var isAdmin = userId == _unitOfWork.UtilizadoresRepository.GetAdminUser().IdUtilizador;

            //indicador de querer mudar nome de utilizador e mudar consoante o que vem do request 
            if (request.UsernameChange && !isAdmin)
            {
                var existingUsername = _unitOfWork.UtilizadoresRepository.GetByUsername(request.Username);
                if (existingUsername != null && (existingUsername.IdUtilizador != user.IdUtilizador))
                {
                    response.Errors.Add(new Error
                    {
                        ErrorCode = ((int)ErrorsDataContract.UsernameAlreadyExists).ToString(),
                        ErrorMessage = ErrorsDataContract.UsernameAlreadyExists.ToString()
                    });
                    return response;
                }
            }

            user = UpdateUserAttributes(user, request, isAdmin);

            try
            {
                _unitOfWork.UtilizadoresRepository.Update(user);
                _unitOfWork.UtilizadorTokenRepository.Delete(activeToken);
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = ex.Message } };
            }

            return response;
        }

        public ResponseBaseDataContract CreateUserManager(RecoverSetPasswordRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            if (request.Username.ToLower() == "admin")
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.InvalidUserName).ToString(),
                    ErrorMessage = ErrorsDataContract.InvalidUserName.ToString()
                });
                return response;
            }

            if (request.Password != request.ConfirmPassword)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = "-1",
                    ErrorMessage = "Password and Confirm password do not match"
                });
                return response;
            }

            //Valida o formato da password que deve conter pelo menos 8 caracteres, um número, um caractere maiúsculo, um caractere minúsculo e um caractere especial
            if (!Regex.Match(request.Password, regexPattern).Success)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.WrongPasswordFormat).ToString(),
                    ErrorMessage = ErrorsDataContract.WrongPasswordFormat.ToString()
                });
                return response;
            }

            //Valida se o token ainda está valido ou se já expirou
            var entidadeId = Authenticate(request.Token);

            if (entidadeId == null)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.ExpiredToken).ToString(),
                    ErrorMessage = ErrorsDataContract.ExpiredToken.ToString()
                });
                return response;
            }

            //Valida se já existe este nome de utilizador
            var existingUsername = _unitOfWork.UtilizadoresRepository.GetByUsername(request.Username);
            if (existingUsername != null)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.UsernameAlreadyExists).ToString(),
                    ErrorMessage = ErrorsDataContract.UsernameAlreadyExists.ToString()
                });
                return response;
            }

            // Valida token que é enviado com token que está na base de dados
            var activeToken = _unitOfWork.UtilizadorTokenRepository.GetUniqueTokensByEntidadeId(entidadeId.Value).FirstOrDefault();
            if (activeToken == null || activeToken.TokenString != _utils.CreateHashPassword(request.Token, activeToken.Salt, Configuration["AppSettings:TokenSalt"]))
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.ExpiredToken).ToString(),
                    ErrorMessage = ErrorsDataContract.ExpiredToken.ToString()
                });
                return response;
            }

            var user = CreateNewUser(request, entidadeId);

            try
            {
                _unitOfWork.UtilizadoresRepository.Add(user);
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = ex.Message } };
            }

            return response;
        }

        private int? Authenticate(string token)
        {
            var key = Encoding.ASCII.GetBytes(Configuration["AppSettings:Secret"]);
            var signingKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key);
            SecurityToken validatedToken;
            var validator = new JwtSecurityTokenHandler();

            // These need to match the values used to generate the token
            TokenValidationParameters validationParameters = new TokenValidationParameters();
            validationParameters.ValidIssuer = "INSSTimor";
            validationParameters.IssuerSigningKey = signingKey;
            validationParameters.ValidateIssuerSigningKey = true;
            validationParameters.ValidateIssuer = true;
            validationParameters.ValidateAudience = false;
            validationParameters.ValidateLifetime = true;
            validationParameters.ClockSkew = TimeSpan.FromMinutes(0);

            if (validator.CanReadToken(token))
            {
                ClaimsPrincipal principal;
                try
                {
                    // This line throws if invalid
                    principal = validator.ValidateToken(token, validationParameters, out validatedToken);
                    // If we got here then the token is valid
                    if (principal.HasClaim(c => c.Type == ClaimTypes.Name))
                    {
                        return int.Parse(principal.Claims.Where(c => c.Type == ClaimTypes.Name).First().Value);
                    }
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }

        private string GenerateToken(int entityId, int? expireTime = null)
        {
            //Metodo para criar o token de autenticação
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Configuration["AppSettings:Secret"]);
            var signingKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, entityId.ToString())
                }),
                Issuer = "INSSTimor",
                Expires = DateTime.UtcNow.AddMinutes(expireTime ?? 60),
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private bool VerifyPassword(string password, string? userSalt, string internalSalt, string userHashPassord)
        {
            return userHashPassord == _utils.CreateHashPassword(password, userSalt, internalSalt);
        }

        private Utilizador UpdateUserAttributes(Utilizador user, RecoverSetPasswordRequest request, bool isAdmin)
        {
            var newSalt = Guid.NewGuid().ToString();

            user.Salt = newSalt;
            user.Password = _utils.CreateHashPassword(request.Password, newSalt, Configuration["AppSettings:InternalSalt"]);
            if (request.UsernameChange && !isAdmin)
                user.Username = request.Username;

            return user;
        }

        private Utilizador CreateNewUser(RecoverSetPasswordRequest request, int? entidadeId)
        {
            var user = new Utilizador
            {
                Salt = Guid.NewGuid().ToString(),
                Username = request.Username,
                UtilizadorEntidadeFk = entidadeId,
                IndActivo = true,
                UtilizadorCriacao = 0,
                DataCriacao = DateTime.Now,
                Ipv6 = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString()
            };
            user.Password = _utils.CreateHashPassword(request.Password, user.Salt, Configuration["AppSettings:InternalSalt"]);

            return user;
        }

        public LoginResponse InternalLoginManager(LoginRequest request)
        {
            var response = new LoginResponse { RequestId = request.RequestId };
            var user = _unitOfWork.UtilizadoresRepository.GetInternalByUsername(request.Username);
            List<int> perfilIds = null;
            bool isAdmin = false;

            // Returns if user is locked
            if (user != null && user.Locked)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.UserIsBlocked).ToString(),
                    ErrorMessage = ErrorsDataContract.UserIsBlocked.ToString()
                });
                return response;
            }

            if (user == null || !VerifyPassword(request.Password, user?.Salt, Configuration["AppSettings:InternalSalt"], user.Password))
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.InvalidUserNameOrPassword).ToString(),
                    ErrorMessage = ErrorsDataContract.InvalidUserNameOrPassword.ToString()
                });
                //Logic of increment login attempts and lock if limit is exceeded
                if (user != null)
                {
                    user.LoginAttempts++;
                    if (user.LoginAttempts >= int.Parse(Configuration["AppSettings:loginLimitAttempts"]))
                    {
                        user.Locked = true;
                    }
                    _unitOfWork.UtilizadoresRepository.Update(user);
                    _unitOfWork.Commit();
                }
            }
            else if (user.Username.ToLower() != "admin")
            {
                var associatedPerfilIds = _unitOfWork.RelUtilizadorPerfilRepository.GetPerfisIdsByUtilizador(user.IdUtilizador);
                perfilIds = _unitOfWork.PerfilRepository.GetActivePerfisByIds(associatedPerfilIds);

                if (perfilIds.Count == 0)
                {
                    response.Errors.Add(new Error
                    {
                        ErrorCode = ((int)ErrorsDataContract.CurrentUserHasNoProfile).ToString(),
                        ErrorMessage = ErrorsDataContract.CurrentUserHasNoProfile.ToString()
                    });
                }
            }
            else
            {
                isAdmin = true;
            }

            if (response.Errors.Count > 0)
            {
                return response;
            }

            List<Funcionalidade> funcionalidades = new List<Funcionalidade>();
            List<Relperfilfuncionalidade> permissoesFuncionalidades = new List<Relperfilfuncionalidade>();
            List<Perfil> perfis = new List<Perfil>();

            if (isAdmin)
            {
                funcionalidades = _unitOfWork.FuncionalidadeRepository.GetAll().ToList();
            }
            else
            {
                permissoesFuncionalidades = _unitOfWork.RelPerfilFuncionalidadeRepository.GetFuncionalidadeByPerfil(perfilIds);
                perfis = _unitOfWork.PerfilRepository.GetPerfisByIds(perfilIds);
            }

            var tokenString = GenerateToken(user.IdUtilizador, int.Parse(Configuration["AppSettings:expirationTimeMinutes"]));

            response.user = new User
            {
                Id = user.IdUtilizador,
                Username = user.Username,
                NISS = user.UtilizadorEntidadeFkNavigation == null ? "" : user.UtilizadorEntidadeFkNavigation.Niss,
                IdEntidade = user.UtilizadorEntidadeFk.GetValueOrDefault(),
                isInternal = user.Interno ?? false,
                Permissions = new List<Permission>(),
                Perfil = isAdmin ? "Admin" : string.Join(", ", perfis.Select(x => x.Descricao))
            };

            //Se for utilizador admin ele adiciona todas as permissões em termos de funcionalidades
            if (isAdmin)
            {
                foreach (var funcionalidade in funcionalidades)
                {
                    response.user.Permissions.Add(
                        new Permission
                        {
                            Module = CalculateModule(funcionalidade.Id),
                            IdFuncionalidade = funcionalidade.Id,
                            Create = true,
                            Read = true,
                            Update = true,
                            Delete = true
                        });
                }
            }
            else
            {
                //Adiciona as permissões consoante as permissões definidas para este utilziador
                foreach (var permission in permissoesFuncionalidades)
                {
                    if (!response.user.Permissions.Any(x => x.IdFuncionalidade == permission.FuncionalidadeFk))
                    {
                        response.user.Permissions.Add(
                            new Permission
                            {
                                Module = CalculateModule(permission.FuncionalidadeFk),
                                IdFuncionalidade = permission.FuncionalidadeFk,
                                Create = permission.Create,
                                Read = permission.Read,
                                Update = permission.Update,
                                Delete = permission.Delete
                            });
                    }
                }
            }

            response.user.Permissions = response.user.Permissions.OrderBy(x => x.Module).ToList();

            response.Token = tokenString;

            if (user.LoginAttempts > 0)
            {
                user.LoginAttempts = 0;
                _unitOfWork.UtilizadoresRepository.Update(user);
            }

            try
            {
                var newTokenSalt = Guid.NewGuid().ToString();
                // Fetch current logged sessions
                var loggedSessions = _unitOfWork.UtilizadorTokenRepository.GetByUserId(user.IdUtilizador);

                //creates new unique logged session
                Utilizadortoken newSession = new Utilizadortoken
                {
                    IdUtilizador = user.IdUtilizador,
                    Salt = newTokenSalt,
                    TokenString = _utils.CreateHashPassword(tokenString, newTokenSalt, Configuration["AppSettings:TokenSalt"])
                };

                if (loggedSessions.Count > 0)
                    loggedSessions.ForEach(session => { _unitOfWork.UtilizadorTokenRepository.Delete(session); });

                _unitOfWork.UtilizadorTokenRepository.Add(newSession);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Token = null;
                response.user = null;
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            return response;
        }

        private long? CalculateModule(int funcionalidadeId)
        {
            if (Enum.IsDefined(typeof(ModuleGestao), funcionalidadeId))
                return (long?)Module.GESTAO;
            if (Enum.IsDefined(typeof(ModuleContribuicoes), funcionalidadeId))
                return (long?)Module.CONTRIBUICOES;
            if (Enum.IsDefined(typeof(ModuleRelatorios), funcionalidadeId))
                return (long?)Module.RELATORIOS;
            else
                return 1000 + funcionalidadeId;
        }

        public UtilizadorListagemResponse GetAllUtilizadoresInterno(SearchFilterRequest request)
        {
            UtilizadorListagemResponse response = new UtilizadorListagemResponse();

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.GestaoUtilizador, _unitOfWork, CRUD.READ);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            try
            {
                response = _unitOfWork.UtilizadoresRepository.GetAllUtilizadoresInterno(request);
                List<Relutilizadorperfil> relUtilizadorPerfil = new List<Relutilizadorperfil>();
                List<Relutilizadordepartamento> relUtilizadorDepartamento = new List<Relutilizadordepartamento>();

                if (response != null && response.utilizador != null && response.utilizador.Count > 0)
                {
                    foreach (var res in response.utilizador)
                    {
                        res.idPerfil = new List<int>();
                        relUtilizadorPerfil = _unitOfWork.RelUtilizadorPerfilRepository.GetRelUtilizadorPerfilByUserId(res.id);

                        if (relUtilizadorPerfil != null && relUtilizadorPerfil.Count > 0)
                        {
                            foreach (var perfil in relUtilizadorPerfil)
                            {
                                if (perfil.PerfilFkNavigation != null)
                                {
                                    //Caso haja mais que um perfil ele concatena as descrições, caso contrário mete uma string vazia
                                    if (res.perfil == null || res.perfil == "")
                                    {
                                        res.perfil = perfil.PerfilFkNavigation.Descricao;
                                    }
                                    else
                                        res.perfil = res.perfil + " / " + perfil.PerfilFkNavigation.Descricao;

                                    res.idPerfil.Add(perfil.PerfilFk);
                                }
                            }
                        }

                        relUtilizadorDepartamento = _unitOfWork.RelUtilizadorDepartamentoRepository.GetRelUtilizadorDepartamentoByUserId(res.id);
                        if (relUtilizadorDepartamento != null && relUtilizadorDepartamento.Count > 0)
                        {
                            foreach (var departamento in relUtilizadorDepartamento)
                            {
                                if (departamento.DepartamentoFkNavigation != null)
                                {
                                    if (res.departamento == null || res.departamento == "")
                                    {
                                        res.departamento = departamento.DepartamentoFkNavigation.Nome;
                                    }
                                    else
                                        res.departamento = res.departamento + " / " + departamento.DepartamentoFkNavigation.Nome;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }

        public DadosUtilizadorResponse GetAllDadosUtilizador(DadosUtilizadorRequest request)
        {
            DadosUtilizadorResponse response = new DadosUtilizadorResponse();
            Trabalhador trabalhador = new Trabalhador();

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.GestaoUtilizador, _unitOfWork, CRUD.UPDATE);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            try
            {
                trabalhador = _unitOfWork.TrabalhadoresRepository.GetTrabalhadorViewById(request.idTrabalhador);

                if (trabalhador != null)
                {
                    response = BuildPerfilObject(trabalhador);
                }
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }

        public ResponseBaseDataContract AddUtilizador(UtilizadorRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.GestaoUtilizador, _unitOfWork, CRUD.CREATE);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            //validações
            List<Relutilizadordepartamento> relUtilizadorDepartamento = new List<Relutilizadordepartamento>();
            List<Relutilizadordepartamento> relUtilizadorDepartamentoToDelete = _unitOfWork.RelUtilizadorDepartamentoRepository.GetRelUtilizadorDepartamentoByUserId(request.id);

            List<Relutilizadorperfil> relUtilizadorPerfil = new List<Relutilizadorperfil>();
            List<Relutilizadorperfil> relUtilizadorPerfilToDelete = _unitOfWork.RelUtilizadorPerfilRepository.GetRelUtilizadorPerfilByUserId(request.id);

            if (request.departamento != null && request.departamento.Count() > 0)
            {
                foreach (var departamento in request.departamento)
                {
                    relUtilizadorDepartamento.Add(BuildRelUtilizadorDepartamento(departamento));
                }
            }

            if (request.perfil != null && request.perfil.Count() > 0)
            {
                foreach (var perfil in request.perfil)
                {
                    relUtilizadorPerfil.Add(BuildRelUtilizadorPerfil(perfil));
                }
            }

            try
            {
                //Métodos para apagar relações de departamentos ou de perfis caso seja para apagar
                if (relUtilizadorDepartamentoToDelete != null && relUtilizadorDepartamentoToDelete.Count > 0)
                {
                    foreach (var rel in relUtilizadorDepartamentoToDelete)
                    {
                        _unitOfWork.RelUtilizadorDepartamentoRepository.Delete(rel);
                    }
                }

                if (relUtilizadorPerfilToDelete != null && relUtilizadorPerfilToDelete.Count > 0)
                {
                    foreach (var rel in relUtilizadorPerfilToDelete)
                    {
                        _unitOfWork.RelUtilizadorPerfilRepository.Delete(rel);
                    }
                }

                if (relUtilizadorDepartamento != null && relUtilizadorDepartamento.Count > 0)
                {
                    foreach (var rel in relUtilizadorDepartamento)
                    {
                        rel.UtilizadorFk = request.id;
                        _unitOfWork.RelUtilizadorDepartamentoRepository.Add(rel);
                    }
                }

                if (relUtilizadorPerfil != null && relUtilizadorPerfil.Count > 0)
                {
                    foreach (var rel in relUtilizadorPerfil)
                    {
                        rel.UtilizadorFk = request.id;
                        _unitOfWork.RelUtilizadorPerfilRepository.Add(rel);
                    }
                }

                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
                _unitOfWork.Rollback();
            }

            return response;
        }

        private DadosUtilizadorResponse BuildPerfilObject(Trabalhador trabalhador)
        {
            int docIdentificacao = 0;
            string numero = "";
            string dataValidade = "";

            if (trabalhador != null && trabalhador.Documentoidentificacao != null && trabalhador.Documentoidentificacao.Count() > 0)
            {
                foreach (var documento in trabalhador.Documentoidentificacao)
                {
                    if (documento.IndActivo)
                    {
                        docIdentificacao = documento.TpDocIdentificacao;
                        numero = documento.Numero;
                        dataValidade = documento.DataValidade.ToString();
                        break;
                    }
                }
            }

            Dominio descricaoTpDocumento = _unitOfWork.DominioRepository.Get(docIdentificacao);
            string email = "";
            string telefone = "";
            if (trabalhador != null && trabalhador.Contacto != null && trabalhador.Contacto.Count() > 0)
            {
                foreach (var contacto in trabalhador.Contacto)
                {
                    if (contacto.IndActivo)
                    {
                        email = contacto.Email;
                        telefone = contacto.Telemovel;
                        break;
                    }
                }
            }

            DadosUtilizadorResponse utilizador = new DadosUtilizadorResponse
            {
                idTrabalhador = trabalhador.IdTrabalhador,
                nomeTrabalhador = trabalhador.Nome,
                dataNascimento = trabalhador.DataNasc.ToString().Substring(0, 8),
                documentoIdentificacao = descricaoTpDocumento != null ? descricaoTpDocumento.Descricao : "",
                numeroDocumento = numero != null ? numero : "",
                dataValidade = dataValidade != null && dataValidade != "" ? dataValidade.Substring(0, 8) : "",
                email = email != null ? email : "",
                telefone = telefone != null ? telefone : ""
            };
            return utilizador;
        }

        private Relutilizadordepartamento BuildRelUtilizadorDepartamento(DepartamentoDataContract departamento)
        {
            RelUtilizadorDepartamentoDto newEntity = new RelUtilizadorDepartamentoDto();
            newEntity.DepartamentoFk = departamento.id;
            newEntity.IndActivo = true;
            newEntity.DataCriacao = DateTime.Now;

            if (newEntity.Id > 0)
            {
                Relutilizadordepartamento original = _unitOfWork.RelUtilizadorDepartamentoRepository.GetRelByDepartamentoId(newEntity.Id);
                newEntity.UtilizadorCriacao = original.UtilizadorCriacao;
                newEntity.DataCriacao = original.DataCriacao;
                newEntity = _utils.UpdateDetailsToEntity(newEntity);
            }
            else
                newEntity = _utils.SetDetailsToEntity(newEntity);
            return Utils.MappClassFromDto<RelUtilizadorDepartamentoDto, Relutilizadordepartamento>(newEntity);
        }

        private Relutilizadorperfil BuildRelUtilizadorPerfil(PerfilDataContract perfil)
        {
            RelUtilizadorPerfilDto newEntity = new RelUtilizadorPerfilDto();
            newEntity.PerfilFk = perfil.id;
            newEntity.IndActivo = true;
            newEntity.DataCriacao = DateTime.Now;

            if (newEntity.Id > 0)
            {
                Relutilizadorperfil original = _unitOfWork.RelUtilizadorPerfilRepository.GetRelByPerfilId(newEntity.Id);
                newEntity.UtilizadorCriacao = original.UtilizadorCriacao;
                newEntity.DataCriacao = original.DataCriacao;
                newEntity = _utils.UpdateDetailsToEntity(newEntity);
            }
            else
                newEntity = _utils.SetDetailsToEntity(newEntity);
            return Utils.MappClassFromDto<RelUtilizadorPerfilDto, Relutilizadorperfil>(newEntity);
        }

        public ResponseBaseDataContract InternalFirstAcessManager(RecoverRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            //Validate if internal trabalhador exists for this niss
            var trabalhadorInterno = _unitOfWork.TrabalhadoresRepository.GetInternalByNiss(request.Niss);
            if (trabalhadorInterno == null)
            {
                response.Errors.Add(new Error
                {
                    ErrorMessage = "Worker doesn't exist"
                });
                return response;
            }

            //Validate if there is an existing email for this Entidade Empregadora
            var contacto = _unitOfWork.ContactoRepository.GetInternalDtoByEmailAndWorkerId(request.Email, trabalhadorInterno.IdTrabalhador);
            if (contacto == null)
            {
                response.Errors.Add(new Error
                {
                    ErrorMessage = "Email doesn't exist"
                });
                return response;
            }

            //Validate if user exists for this Entidade Empregadora
            var user = _unitOfWork.UtilizadoresRepository.GetByTrabalhadorFk(trabalhadorInterno.IdTrabalhador);
            if (user != null)
            {
                response.Errors.Add(new Error
                {
                    ErrorMessage = "User already exists for this Intern Worker"
                });
                return response;
            }

            var tokenString = GenerateToken(trabalhadorInterno.IdTrabalhador, 15);

            try
            {
                _emailSenderDataManager.SendEmailAsync(contacto.Email, "Primeiro acesso", tokenString, null, true).GetAwaiter();

                //Lógica de criar token com uma só utilização
                var newTokenSalt = Guid.NewGuid().ToString();

                //Elimina tokens anteriores
                var previousTokens = _unitOfWork.UtilizadorTokenRepository.GetUniqueTokensByEntidadeId(trabalhadorInterno.IdTrabalhador);

                if (previousTokens.Count > 0)
                    previousTokens.ForEach(session => { _unitOfWork.UtilizadorTokenRepository.Delete(session); });

                //cria token para depois ser de uma só utilização
                Utilizadortoken recoverToken = new Utilizadortoken
                {
                    Salt = newTokenSalt,
                    TokenString = _utils.CreateHashPassword(tokenString, newTokenSalt, Configuration["AppSettings:TokenSalt"]),
                    EntidadeId = trabalhadorInterno.IdTrabalhador
                };

                _unitOfWork.UtilizadorTokenRepository.Add(recoverToken);
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                response.Errors.Add(new Error
                {
                    ErrorMessage = ex.Message
                });
            }

            return response;
        }

        public ResponseBaseDataContract InternalRecoverManager(RecoverRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            //Valida se é o utiilizador admin
            Boolean isAdmin = false;
            if (request.Niss == _unitOfWork.DominioRepository.getTipoDeDominio(TiposDominio.NISSSEGURANCASOCIAL).descricao
                && request.Email == Configuration["EmailSettings:UsernameEmail"])
                isAdmin = true;

            Utilizador user = null;
            ContactoDto contacto = new ContactoDto();

            if (!isAdmin)
            {
                //Validate if internal trabalhador exists for this niss
                var trabalhadorInterno = _unitOfWork.TrabalhadoresRepository.GetInternalByNiss(request.Niss);
                if (trabalhadorInterno == null)
                {
                    response.Errors.Add(new Error
                    {
                        ErrorMessage = "Worker doesn't exist"
                    });
                    return response;
                }

                //Validate if there is an existing email for this Entidade Empregadora
                contacto = _unitOfWork.ContactoRepository.GetInternalDtoByEmailAndWorkerId(request.Email, trabalhadorInterno.IdTrabalhador);
                if (contacto == null)
                {
                    response.Errors.Add(new Error
                    {
                        ErrorMessage = "Email doesn't exist"
                    });
                    return response;
                }

                //Validate if user exists for this Entidade Empregadora
                user = _unitOfWork.UtilizadoresRepository.GetByTrabalhadorFk(trabalhadorInterno.IdTrabalhador);
                if (user == null)
                {
                    response.Errors.Add(new Error
                    {
                        ErrorMessage = "User doesn't exist"
                    });
                    return response;
                }
            }
            else
            {
                contacto.Email = Configuration["EmailSettings:UsernameEmail"];
                user = _unitOfWork.UtilizadoresRepository.GetAdminUser();
            }
            

            var tokenString = GenerateToken(user.IdUtilizador, 15);

            try
            {
                _emailSenderDataManager.SendEmailAsync(contacto.Email, "Recuperação de palavra passe", tokenString, user.Username, true).GetAwaiter();

                //Lógica de criar token com uma só utilização
                var newTokenSalt = Guid.NewGuid().ToString();

                //Elimina tokens anteriores
                var previousTokens = _unitOfWork.UtilizadorTokenRepository.GetUniqueTokensByUserId(user.IdUtilizador);

                if (previousTokens.Count > 0)
                    previousTokens.ForEach(session => { _unitOfWork.UtilizadorTokenRepository.Delete(session); });

                //cria token para depois ser de uma só utilização
                Utilizadortoken recoverToken = new Utilizadortoken
                {
                    IdUtilizador = user.IdUtilizador,
                    Salt = newTokenSalt,
                    TokenString = _utils.CreateHashPassword(tokenString, newTokenSalt, Configuration["AppSettings:TokenSalt"]),
                    IsRecover = true
                };

                _unitOfWork.UtilizadorTokenRepository.Add(recoverToken);
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                response.Errors.Add(new Error
                {
                    ErrorMessage = ex.Message
                });
            }

            return response;
        }

        public ResponseBaseDataContract CreateInternalUserManager(RecoverSetPasswordRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            if (request.Username.ToLower() == "admin")
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.InvalidUserName).ToString(),
                    ErrorMessage = ErrorsDataContract.InvalidUserName.ToString()
                });
                return response;
            }

            if (request.Password != request.ConfirmPassword)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = "-1",
                    ErrorMessage = "Password and Confirm password do not match"
                });
                return response;
            }

            //Valida o formato da password que deve conter pelo menos 8 caracteres, um número, um caractere maiúsculo, um caractere minúsculo e um caractere especial
            if (!Regex.Match(request.Password, regexPattern).Success)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.WrongPasswordFormat).ToString(),
                    ErrorMessage = ErrorsDataContract.WrongPasswordFormat.ToString()
                });
                return response;
            }

            // valida se o token está expirado
            var trabalhadorId = Authenticate(request.Token);

            if (trabalhadorId == null)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.ExpiredToken).ToString(),
                    ErrorMessage = ErrorsDataContract.ExpiredToken.ToString()
                });
                return response;
            }

            //Valida se este nome de utilizador já existe
            var existingUsername = _unitOfWork.UtilizadoresRepository.GetByUsername(request.Username);
            if (existingUsername != null)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.UsernameAlreadyExists).ToString(),
                    ErrorMessage = ErrorsDataContract.UsernameAlreadyExists.ToString()
                });
                return response;
            }

            var user = CreateNewInternalUser(request, trabalhadorId);

            try
            {
                _unitOfWork.UtilizadoresRepository.Add(user);
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = ex.Message } };
            }

            return response;
        }

        private Utilizador CreateNewInternalUser(RecoverSetPasswordRequest request, int? trabalhadorId)
        {
            var user = new Utilizador
            {
                Salt = Guid.NewGuid().ToString(),
                Username = request.Username,
                TrabalhadorFk = trabalhadorId,
                IndActivo = true,
                UtilizadorCriacao = 0,
                DataCriacao = DateTime.Now,
                Ipv6 = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString(),
                Interno = true
            };
            user.Password = _utils.CreateHashPassword(request.Password, user.Salt, Configuration["AppSettings:InternalSalt"]);

            return user;
        }

        public UtilizadoresAcessoListagemResponse GetAllAcessoUtilizadores(SearchFilterRequest request)
        {
            UtilizadoresAcessoListagemResponse response = new UtilizadoresAcessoListagemResponse();

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.ControlodeAcessodeUtilizadores, _unitOfWork, CRUD.READ);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            try
            {
                response = _unitOfWork.UtilizadoresRepository.GetAllAcessoUtilizadores(request);
                List<Relutilizadorperfil> relUtilizadorPerfil = new List<Relutilizadorperfil>();
                List<Relutilizadordepartamento> relUtilizadorDepartamento = new List<Relutilizadordepartamento>();

                if (response != null && response.utilizador != null && response.utilizador.Count > 0)
                {
                    foreach (var res in response.utilizador)
                    {
                        relUtilizadorPerfil = _unitOfWork.RelUtilizadorPerfilRepository.GetRelUtilizadorPerfilByUserId(res.id);

                        if (relUtilizadorPerfil != null && relUtilizadorPerfil.Count > 0)
                        {
                            foreach (var perfil in relUtilizadorPerfil)
                            {
                                //Caso haja mais que um perfil ele concatena as descrições, caso contrário mete uma string vazia
                                if (perfil.PerfilFkNavigation != null)
                                {
                                    if (res.perfil == null || res.perfil == "")
                                    {
                                        res.perfil = perfil.PerfilFkNavigation.Descricao;
                                    }
                                    else
                                        res.perfil = res.perfil + " / " + perfil.PerfilFkNavigation.Descricao;
                                }
                            }
                        }

                        relUtilizadorDepartamento = _unitOfWork.RelUtilizadorDepartamentoRepository.GetRelUtilizadorDepartamentoByUserId(res.id);
                        if (relUtilizadorDepartamento != null && relUtilizadorDepartamento.Count > 0)
                        {
                            foreach (var departamento in relUtilizadorDepartamento)
                            {
                                if (departamento.DepartamentoFkNavigation != null)
                                {
                                    if (res.departamento == null || res.departamento == "")
                                    {
                                        res.departamento = departamento.DepartamentoFkNavigation.Nome;
                                    }
                                    else
                                        res.departamento = res.departamento + " / " + departamento.DepartamentoFkNavigation.Nome;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }

        public ResponseBaseDataContract SwitchUserBlockState(UserUpdateRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.ControlodeAcessodeUtilizadores, _unitOfWork, CRUD.UPDATE);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            try
            {
                var user = _unitOfWork.UtilizadoresRepository.Get(request.id);
                user.LoginAttempts = 0;
                user.Locked = !user.Locked;
                _unitOfWork.UtilizadoresRepository.Update(user);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }

        public ResponseBaseDataContract ValidToken(ValidTokenRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            //Apanha o id de utilizador ou id de entidade do token
            var Id = Authenticate(request.token);

            if (Id == null)
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.ExpiredToken).ToString(),
                    ErrorMessage = ErrorsDataContract.ExpiredToken.ToString()
                });
            }

            //Valida se o token enviado coincide com o token na base de dados para tokens de recuperação de password ou de primeiro acesso
            var token = request.isRecover ? _unitOfWork.UtilizadorTokenRepository.GetUniqueTokensByUserId(Id.Value).FirstOrDefault() : _unitOfWork.UtilizadorTokenRepository.GetUniqueTokensByEntidadeId(Id.Value).FirstOrDefault();

            if (token == null || token.TokenString != _utils.CreateHashPassword(request.token, token.Salt, Configuration["AppSettings:TokenSalt"]))
            {
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.ExpiredToken).ToString(),
                    ErrorMessage = ErrorsDataContract.ExpiredToken.ToString()
                });
            }

            return response;
        }
    }
}