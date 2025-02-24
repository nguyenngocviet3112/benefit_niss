using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class UtilsDataManager : IUtilsDataManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration Configuration;
        private readonly IUnitOfWork _unitOfWork;

        public UtilsDataManager(IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _httpContextAccessor = httpContextAccessor;
            Configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        public string GetIPV6() => _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();

        public T SetDetailsToEntity<T>(T entity)
        {
            string Ipv6 = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
            _httpContextAccessor.HttpContext.Request.Headers.TryGetValue("User-Id", out var userId);
            int userIdInt = userId.ToString().Length != 0 ? int.Parse(userId) : 0;
            var type = entity.GetType();
            var propertyCreationDate = type.GetProperty("DataCriacao");
            var propertyUtilizdor = type.GetProperty("UtilizadorCriacao");
            var propertyIpv6 = type.GetProperty("Ipv6");
            propertyUtilizdor.SetValue(entity, userIdInt);
            propertyIpv6.SetValue(entity, Ipv6);
            propertyCreationDate.SetValue(entity, DateTime.Now);
            return entity;
        }

        public T UpdateDetailsToEntity<T>(T entity)
        {
            string Ipv6 = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
            _httpContextAccessor.HttpContext.Request.Headers.TryGetValue("User-Id", out var userId);
            int userIdInt = userId.ToString().Length != 0 ? int.Parse(userId) : 0;
            var type = entity.GetType();
            var propertyAlterationDate = type.GetProperty("DataAlteracao");
            var propertyUtilizdor = type.GetProperty("UtilizadorAlteracao");
            var propertyIpv6 = type.GetProperty("Ipv6");
            propertyUtilizdor.SetValue(entity, userIdInt);
            propertyIpv6.SetValue(entity, Ipv6);
            propertyAlterationDate.SetValue(entity, DateTime.Now);
            return entity;
        }

        public bool IsSuspenso(int idEntidade, int idTrabalhador, IUnitOfWork _unitOfWork)
        {
            SuspensaoListagemRequest request = new SuspensaoListagemRequest();
            bool response = false;

            request.IdEntidade = idEntidade;
            request.IdTrabalhador = idTrabalhador;

            SearchFilter filter = new SearchFilter();
            request.filter = filter;

            if (idTrabalhador > 0)
            {
                request.filter.filterField = "TRABALHADOR";
            }
            else
            {
                request.filter.filterField = "ENTIDADEEMPREGADORA";
            }
            response = _unitOfWork.SuspensaoRepository.GetSuspensaoByData(request);

            if (response)
                return true;

            return false;
        }

        public bool ValidateRelentidadetrabalhador(Relentidadetrabalhador relEntidadeTrabalhador, IUnitOfWork _unitOfWork, out List<Error> error)
        {
            error = new List<Error>();
            DateTime end = relEntidadeTrabalhador.DtIniFimTrabalhador ?? System.Data.SqlTypes.SqlDateTime.MaxValue.Value;
            if (relEntidadeTrabalhador.DtIniVincTrabalhador > end)
            {
                error.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.DateFromSuperiorThanDateTo).ToString(),
                    ErrorMessage = ErrorsDataContract.DateFromSuperiorThanDateTo.ToString()
                });
            }

            if (_unitOfWork.RelEntidadeTrabalhadorRepository.IsTrabalhadorVinculado(relEntidadeTrabalhador.DtIniVincTrabalhador, end, relEntidadeTrabalhador.TrabalhadorFk, relEntidadeTrabalhador.EntidadeFk, relEntidadeTrabalhador.IdRel))
            {
                error.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.TrabalhadorVinculado).ToString(),
                    ErrorMessage = ErrorsDataContract.TrabalhadorVinculado.ToString()
                });
            }

            if (relEntidadeTrabalhador.FuncPublico)
            {
                if (string.IsNullOrWhiteSpace(relEntidadeTrabalhador.NumFuncPublico))
                    error.Add(new Error
                    {
                        ErrorCode = ((int)ErrorsDataContract.NumFuncPublicoObrigatorio).ToString(),
                        ErrorMessage = ErrorsDataContract.NumFuncPublicoObrigatorio.ToString()
                    });
                else
                    if (_unitOfWork.RelEntidadeTrabalhadorRepository.NumFuncPublicoExists(relEntidadeTrabalhador.NumFuncPublico, relEntidadeTrabalhador.TrabalhadorFk))
                    error.Add(new Error
                    {
                        ErrorCode = ((int)ErrorsDataContract.NumFuncPublicoAlreadyExists).ToString(),
                        ErrorMessage = ErrorsDataContract.NumFuncPublicoAlreadyExists.ToString()
                    });
            }

            if (_unitOfWork.RelEntidadeTrabalhadorRepository.EntidadeTemConflitoComTipoRegime(relEntidadeTrabalhador.EntidadeFk, relEntidadeTrabalhador.RegimeFk))
                error.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.TipoRegimeErrado).ToString(),
                    ErrorMessage = ErrorsDataContract.TipoRegimeErrado.ToString()
                });

            return error.Count == 0;
        }

        public bool ValidatePermission(int userId, int funcionalidadeId, IUnitOfWork _unitOfWork, CRUD? permissionType = null)
        {
            var user = _unitOfWork.UtilizadoresRepository.Get(userId);

            if (user.Username.ToLower() != "admin")
            {
                //Validate permissions of create/read/update/delete of a certain funcionality that the user has access

                var perfilIds = _unitOfWork.RelUtilizadorPerfilRepository.GetPerfisIdsByUtilizador(userId);

                var funcionalidades = _unitOfWork.RelPerfilFuncionalidadeRepository.GetFuncionalidadeByPerfil(perfilIds);

                if (!funcionalidades.Select(x => x.FuncionalidadeFk == funcionalidadeId).Any())
                    return false;

                var funcionalidade = funcionalidades.FirstOrDefault(x => x.FuncionalidadeFk == funcionalidadeId);

                switch (permissionType)
                {
                    case CRUD.CREATE:
                        if (!funcionalidade.Create)
                            return false;
                        break;

                    case CRUD.READ:
                        if (!funcionalidade.Read)
                            return false;
                        break;

                    case CRUD.UPDATE:
                        if (!funcionalidade.Update)
                            return false;
                        break;

                    case CRUD.DELETE:
                        if (!funcionalidade.Delete)
                            return false;
                        break;

                    default:
                        break;
                }
            }
            else if (user == null)
            {
                return false;
            }

            return true;
        }

        public LogResponse Compress(RequestBaseDataContract request)
        {
            //TODO: add permission

            LogResponse response = new LogResponse();

            bool permission = ValidatePermission((int)request.UserId, (int)ExternalModule.Auditoria, _unitOfWork);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            string directoryPath = Configuration["AppSettings:logsFolderPath"];

            var FilePaths = Directory.GetFiles(directoryPath);
            using (var zipFileMemoryStream = new MemoryStream())
            {
                using (ZipArchive archive = new ZipArchive(zipFileMemoryStream, ZipArchiveMode.Update, leaveOpen: true))
                {
                    foreach (var FilePath in FilePaths)
                    {
                        var FileName = Path.GetFileName(FilePath);
                        var entry = archive.CreateEntry(FileName);
                        using (var entryStream = entry.Open())
                        using (var fileStream = File.Open(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            fileStream.CopyTo(entryStream);
                        }
                    }
                }
                zipFileMemoryStream.Seek(0, SeekOrigin.Begin);
                // use stream as needed
                var bitArray = zipFileMemoryStream.ToArray();
                var base64String = Convert.ToBase64String(bitArray);
                response.logString = base64String;

                return response;
            }
        }

        public string CreateHashPassword(string password, string userSalt, string internalSalt)
        {
            var valueBytes = KeyDerivation.Pbkdf2(
                             password: password,
                             salt: Encoding.UTF8.GetBytes(userSalt + internalSalt),
                             prf: KeyDerivationPrf.HMACSHA512,
                             iterationCount: 10000,
                             numBytesRequested: 256 / 8);

            var hashedPassword = Convert.ToBase64String(valueBytes);

            return hashedPassword;
        }
    }
}