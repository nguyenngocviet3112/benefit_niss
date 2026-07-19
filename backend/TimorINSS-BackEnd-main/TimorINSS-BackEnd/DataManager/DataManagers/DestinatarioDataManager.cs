using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class DestinatarioDataManager : IDestinatarioDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration Configuration;
        private readonly IUtilsDataManager _utils;

        public DestinatarioDataManager(IUnitOfWork unitOfWork, IConfiguration configuration, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            Configuration = configuration;
            _utils = utils;
        }

        public GetDestinatarioResponse GetDestinatarioByNissTin(GetDestinatarioRequest request)
        {
            // um destinatário pode ser uma Entidade Empregadora, um Trabalhador já registados no sistema, ou simplesmente 
            //um destinatário novo que portanto só existe na tabela dos destinatarios
            // por isso o Get do destinatário vai primeiro à tabela dos destinatários, caso não existir na tabela dos destinatários
            //vai procurar na tabela das entidades empregadoras e caso não encontra neste tabela vai à dos trabalhadores

            GetDestinatarioResponse response = new GetDestinatarioResponse();
            DestinatarioDataContract destinatario = null;
            try
            {
                // Tabela Destinatario
                if (!string.IsNullOrEmpty(request.Niss))
                {
                    destinatario = _unitOfWork.DestinatarioRepository.GetDestinatarioByNiss(request.Niss);
                }
                else if (!string.IsNullOrEmpty(request.Tin))
                {
                    destinatario = _unitOfWork.DestinatarioRepository.GetDestinatarioByTin(request.Tin);
                }

                //Entidade Empregadora
                if (destinatario == null)
                {
                    if (!string.IsNullOrEmpty(request.Niss))
                    {
                        destinatario = _unitOfWork.EntidadeEmpregadoraRepository.GetDestinatarioByNiss(request.Niss);
                    }
                    else if (!string.IsNullOrEmpty(request.Tin))
                    {
                        destinatario = _unitOfWork.EntidadeEmpregadoraRepository.GetDestinatarioByTin(request.Tin);
                    }

                    if (destinatario != null)
                    {
                        var morada = _unitOfWork.MoradaRepository.GetMoradasPrincipalByEntidadeFk((int)destinatario.EntidadeFk);

                        if (morada != null)
                        {
                            destinatario.Morada = BuildMoradaString(morada);
                        }
                    }
                }

                //Trabalhador
                if (destinatario == null)
                {
                    if (!string.IsNullOrEmpty(request.Niss))
                    {
                        destinatario = _unitOfWork.TrabalhadoresRepository.GetDestinatarioByNiss(request.Niss);
                    }
                    else if (!string.IsNullOrEmpty(request.Tin))
                    {
                        destinatario = _unitOfWork.TrabalhadoresRepository.GetDestinatarioByTin(request.Tin);
                    }

                    if (destinatario != null)
                    {
                        var morada = _unitOfWork.MoradaRepository.GetMoradasPrincipalByTrabalhadorFk((int)destinatario.TrabalhadorFk);

                        if (morada != null)
                        {
                            destinatario.Morada = BuildMoradaString(morada);
                        }
                    }
                }

                if (destinatario == null)
                {
                    response.Errors.Add(new Error
                    {
                        ErrorCode = ((int)ErrorsDataContract.DestinatarioNaoExiste).ToString(),
                        ErrorMessage = ErrorsDataContract.DestinatarioNaoExiste.ToString()
                    });
                    return response;
                }
                response.Destinatario = destinatario;
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }

        public ResponseBaseDataContract SaveDestinatario(SaveDestinatarioRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.PreenchimentoTarefa, _unitOfWork, CRUD.UPDATE);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            Destinatario destinatario = BuildDestinatarioObject(request.destinatario);

            Destinatario entidadeEmpregadora = null;
            if (destinatario.EntidadeFk != null && destinatario.EntidadeFk > 0)
            {
                entidadeEmpregadora = _unitOfWork.DestinatarioRepository.GetDestinatarioByIdEntidade((int)destinatario.EntidadeFk);
            }
            Destinatario trabalhador = null;
            if (destinatario.TrabalhadorFk != null && destinatario.TrabalhadorFk > 0)
            {
                trabalhador = _unitOfWork.DestinatarioRepository.GetDestinatarioByIdTrabalhador((int)destinatario.TrabalhadorFk);
            }

            try
            {
                if (destinatario.Id > 0)
                {
                    _unitOfWork.DestinatarioRepository.Update(destinatario);
                }
                else
                {
                    if (entidadeEmpregadora == null && trabalhador == null)
                    {
                        _unitOfWork.DestinatarioRepository.Add(destinatario);
                    }
                }
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }

        public ExcelImportReponse ImportDestinatarios(ExcelImporterRequest request)
        {
            var response = new ExcelImportReponse { RequestId = request.RequestId };
            try
            {
                // Validar se o utilizador tem as permissões necessárias
                bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.PreenchimentoTarefa, _unitOfWork, CRUD.UPDATE);

                if (!permission)
                {
                    response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                    return response;
                }

                var ip = _utils.GetIPV6();
                List<ExcelReaderService.Models.Destinatario> destinatarios;

                try
                {
                    destinatarios = ExcelReaderService.ExcelReader.Read<ExcelReaderService.Models.Destinatario>(request.File.OpenReadStream());
                }
                catch (Exception ex)
                {
                    response.Errors = new List<Error> { new Error { ErrorCode = ((int)ErrorsDataContract.ExcelNotValidOrMissingColumns).ToString(), ErrorMessage = ErrorsDataContract.ExcelNotValidOrMissingColumns.ToString() } };
                    return response;
                }

                var guid = Guid.NewGuid();

                Task.Run(async () =>
                {
                    using (var dataContext = new TimorINSSModuloContribuicoesContext(Configuration.GetConnectionString("sqlserverconnection")))
                    {
                        var nissList = destinatarios.Select(e => e.Niss).Where(e => !string.IsNullOrEmpty(e)).Distinct().ToList();
                        var tinList = destinatarios.Select(e => e.Tin).Where(e => !string.IsNullOrEmpty(e)).Distinct().ToList();

                        var destinatariosDb = dataContext.Destinatario
                            .ToList()
                            .Where(u => u.IndActivo && (nissList.Contains(u.Niss) || tinList.Contains(u.Tin)));

                        var trabalhadoresDb = dataContext.Trabalhador
                            .ToList()
                            .Where(u => nissList.Contains(u.Niss) || tinList.Contains(u.Tin));

                        var entidadesDb = dataContext.Entidadeempregadora
                            .ToList()
                            .Where(u => nissList.Contains(u.Niss) || tinList.Contains(u.Tin));

                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"Imports", $"{guid}.json");

                        File.Create(filePath).Close();

                        dynamic progressJson = new ExpandoObject();

                        progressJson.Total = destinatarios.Count;
                        progressJson.Success = 0;
                        progressJson.Fail = 0;
                        progressJson.Existing = 0;
                        progressJson.InvalidIBAN = new List<string>();
                        progressJson.MissingFields = new List<string>();
                        progressJson.InvalidAccountNumber = new List<string>();
                        progressJson.DiffNissAndTinEntities = new List<string>();
                        progressJson.DiffNissAndTinWorkers = new List<string>();
                        progressJson.AmountHigherThanZero = new List<string>();
                        progressJson.TotalAmount = 0;


                        for (int i = 0; i < destinatarios.Count; i++)
                        {
                            try
                            {
                                var destinatario = destinatarios[i];

                                Destinatario existing = null;

                                if (!string.IsNullOrEmpty(destinatario.Niss) || !string.IsNullOrEmpty(destinatario.Tin))
                                {
                                    var existingDests = destinatariosDb.Where(e => e.Niss == destinatario.Niss || e.Tin == destinatario.Tin);
                                    if (existingDests.Count() > 1) existing = existingDests.FirstOrDefault(e => e.Niss == destinatario.Niss);
                                    if (existing == null) existing = existingDests.FirstOrDefault();
                                }

                                var pagamentoInfoIsFilled = (!string.IsNullOrEmpty(destinatario.AccountNumber) || !string.IsNullOrEmpty(destinatario.IBAN)) && destinatario.Amount.HasValue;
                                var newDestinatarioIsFilled = (!string.IsNullOrEmpty(destinatario.Niss) || !string.IsNullOrEmpty(destinatario.Tin)) && !string.IsNullOrEmpty(destinatario.Name) && !string.IsNullOrEmpty(destinatario.Address);
                                var isNewDestinatario = existing == null;

                                if ((isNewDestinatario && !newDestinatarioIsFilled) || !pagamentoInfoIsFilled)
                                {
                                    // json error - campos obrigatórios em falta
                                    progressJson.Fail++;
                                    progressJson.MissingFields.Add((i + 2) + " - " + destinatario.Name);
                                    File.WriteAllText(filePath, JsonConvert.SerializeObject(progressJson, Formatting.Indented));
                                    continue;
                                }

                                if (destinatario.Amount.Value <= 0)
                                {
                                    // json error - Valor a registar tem que ser superior a 0
                                    progressJson.Fail++;
                                    progressJson.AmountHigherThanZero.Add((i + 2) + " - " + destinatario.Name);
                                    File.WriteAllText(filePath, JsonConvert.SerializeObject(progressJson, Formatting.Indented));
                                    continue;
                                }

                                Destinatario row = null;

                                var isTimorIban = destinatario.IBAN?.ToLower().StartsWith("tl") ?? false;
                                if (!string.IsNullOrEmpty(destinatario.IBAN) && !((!isTimorIban && Regex.Match(destinatario.IBAN, "^[a-zA-Z]{2}[0-9]{21,25}$").Success) || (isTimorIban && Regex.Match(destinatario.IBAN, "^(tl|TL)38[0-9]{19}$").Success)))
                                {
                                    // json error - IBAN não é válido
                                    progressJson.Fail++;
                                    progressJson.InvalidIBAN.Add((i + 2) + " - " + destinatario.Name);
                                    File.WriteAllText(filePath, JsonConvert.SerializeObject(progressJson, Formatting.Indented));
                                    continue;
                                }

                                if (!string.IsNullOrEmpty(destinatario.IBAN) && !isTimorIban && string.IsNullOrEmpty(destinatario.Swift))
                                {
                                    // json error - SWIFT é obrigatório (apenas quando existe um IBAN estrangeiro)
                                    progressJson.Fail++;
                                    progressJson.MissingFields.Add((i + 2) + " - " + destinatario.Name);
                                    File.WriteAllText(filePath, JsonConvert.SerializeObject(progressJson, Formatting.Indented));
                                    continue;
                                }

                                if (!string.IsNullOrEmpty(destinatario.IBAN) && !Regex.Match(destinatario.IBAN, "^[a-zA-Z]{2}[0-9]{21,25}$").Success)
                                {
                                    // json error - IBAN não é válido
                                    progressJson.Fail++;
                                    progressJson.InvalidIBAN.Add((i + 2) + " - " + destinatario.Name);
                                    File.WriteAllText(filePath, JsonConvert.SerializeObject(progressJson, Formatting.Indented));
                                    continue;
                                }

                                if (!string.IsNullOrEmpty(destinatario.AccountNumber) && string.IsNullOrEmpty(destinatario.IBAN) && !Regex.Match(destinatario.AccountNumber, "^[0-9]{1,21}$").Success)
                                {
                                    // json error - Número de conta não é válido
                                    progressJson.Fail++;
                                    progressJson.InvalidAccountNumber.Add((i + 2) + " - " + destinatario.Name);
                                    File.WriteAllText(filePath, JsonConvert.SerializeObject(progressJson, Formatting.Indented));
                                    continue;
                                }

                                if (isNewDestinatario)
                                {
                                    var entidadeWithNiss = string.IsNullOrEmpty(destinatario.Niss) ? null : entidadesDb.FirstOrDefault(e => e.Niss == destinatario.Niss);
                                    var entidadeWithTin = string.IsNullOrEmpty(destinatario.Tin) ? null : entidadesDb.FirstOrDefault(e => e.Tin == destinatario.Tin);

                                    if (entidadeWithNiss != null && entidadeWithTin != null && entidadeWithNiss.IdEntidadeEmpreg != entidadeWithTin.IdEntidadeEmpreg)
                                    {
                                        // json error - entidades com niss e tin são diferentes
                                        progressJson.Fail++;
                                        progressJson.DiffNissAndTinEntities.Add((i + 2) + " - " + destinatario.Name);
                                        File.WriteAllText(filePath, JsonConvert.SerializeObject(progressJson, Formatting.Indented));
                                        continue;
                                    }

                                    var trabalhadorWithNiss = string.IsNullOrEmpty(destinatario.Niss) ? null : trabalhadoresDb.FirstOrDefault(e => e.Niss == destinatario.Niss);
                                    var trabalhadorWithTin = string.IsNullOrEmpty(destinatario.Tin) ? null : trabalhadoresDb.FirstOrDefault(e => e.Tin == destinatario.Tin);

                                    if (trabalhadorWithNiss != null && trabalhadorWithTin != null && trabalhadorWithNiss.IdTrabalhador != trabalhadorWithTin.IdTrabalhador)
                                    {
                                        // json error - trabalhadores com niss e tin são diferentes
                                        progressJson.Fail++;
                                        progressJson.DiffNissAndTinWorkers.Add((i + 2) + " - " + destinatario.Name);
                                        File.WriteAllText(filePath, JsonConvert.SerializeObject(progressJson, Formatting.Indented));
                                        continue;
                                    }

                                    row = new Destinatario()
                                    {
                                        Niss = string.IsNullOrEmpty(destinatario.Niss) ? null : destinatario.Niss,
                                        Tin = string.IsNullOrEmpty(destinatario.Tin) ? null : destinatario.Tin,
                                        EntidadeFk = entidadeWithNiss?.IdEntidadeEmpreg ?? entidadeWithTin?.IdEntidadeEmpreg,
                                        TrabalhadorFk = trabalhadorWithNiss?.IdTrabalhador ?? trabalhadorWithTin?.IdTrabalhador,
                                        Morada = destinatario.Address,
                                        Nome = destinatario.Name,
                                        IndActivo = true,
                                        UtilizadorCriacao = request.UserId,
                                        DataCriacao = DateTime.Now,
                                        Ipv6 = ip,
                                    };

                                    dataContext.Destinatario.Add(row);
                                    dataContext.SaveChanges();
                                    progressJson.Success++;
                                }
                                else
                                {
                                    progressJson.Existing++;
                                }

                                dataContext.ExcelImporterRel.Add(new ExcelImporterRel()
                                {
                                    ImportId = guid,
                                    RelId = row?.Id ?? existing.Id,
                                    Date = DateTime.Now,
                                    Data = JsonConvert.SerializeObject(new ExcelReaderService.Models.Pagamento()
                                    {
                                        IBAN = destinatario.IBAN,
                                        Swift = destinatario.Swift,
                                        AccountNumber = destinatario.AccountNumber,
                                        Amount = destinatario.Amount,
                                    })
                                });

                                dataContext.SaveChanges();

                                progressJson.TotalAmount += destinatario.Amount.Value;

                                File.WriteAllText(filePath, JsonConvert.SerializeObject(progressJson, Formatting.Indented));

                                // update json counts

                                //await Task.Delay(300);
                            }
                            catch (Exception ex)
                            {
                                progressJson.Fail++;
                            }
                        }

                        await Task.Delay(5000);

                        File.Delete(filePath);
                    }

                });

                response.Id = guid.ToString();
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }

        private Destinatario BuildDestinatarioObject(DestinatarioDataContract request)
        {
            Destinatario destinatario = new Destinatario
            {
                Id = request.Id,
                EntidadeFk = request.EntidadeFk,
                TrabalhadorFk = request.TrabalhadorFk,
                Niss = request.Niss,
                Tin = request.Tin,
                Morada = request.Morada,
                Nome = request.Nome,
                IndActivo = true,
            };
            if (destinatario.EntidadeFk != null && destinatario.EntidadeFk > 0)
            {
                destinatario = new Destinatario
                {
                    EntidadeFk = request.EntidadeFk,
                    IndActivo = true
                };
            }

            if (destinatario.TrabalhadorFk != null && destinatario.TrabalhadorFk > 0)
            {
                destinatario = new Destinatario
                {
                    TrabalhadorFk = request.TrabalhadorFk,
                    IndActivo = true
                };
            }

            if (destinatario.Id > 0)
            {
                Destinatario original = _unitOfWork.DestinatarioRepository.Get(destinatario.Id);
                destinatario.UtilizadorCriacao = original.UtilizadorCriacao;
                destinatario.DataCriacao = original.DataCriacao;
                destinatario = _utils.UpdateDetailsToEntity(destinatario);
            }
            else
                destinatario = _utils.SetDetailsToEntity(destinatario);
            return destinatario;
        }

        private string BuildMoradaString(Morada morada)
        {
            var moradaFinal = !string.IsNullOrEmpty(morada.Rua) ? morada.Rua + "," : "";
            moradaFinal += !string.IsNullOrEmpty(morada.NumPorta) ? morada.NumPorta + " - " : "";

            //Existe uma adição de todas as strings pois para a morada final tem de constar tudo: aldeia, posto, suco etc... caso existam
            if (morada.MoradaAldeiaFkNavigation != null && morada.MoradaAldeiaFkNavigation.Nome != null)
            {
                moradaFinal += morada.MoradaAldeiaFkNavigation.Nome + " ";

                if (morada.MoradaAldeiaFkNavigation.AldeiaSucoFkNavigation != null && morada.MoradaAldeiaFkNavigation.AldeiaSucoFkNavigation.Nome != null)
                {
                    moradaFinal += morada.MoradaAldeiaFkNavigation.AldeiaSucoFkNavigation.Nome + " ";

                    if (morada.MoradaAldeiaFkNavigation.AldeiaSucoFkNavigation.SucoPostoAdminFkNavigation != null && morada.MoradaAldeiaFkNavigation.AldeiaSucoFkNavigation.SucoPostoAdminFkNavigation.Nome != null)
                    {
                        moradaFinal += morada.MoradaAldeiaFkNavigation.AldeiaSucoFkNavigation.SucoPostoAdminFkNavigation.Nome + " ";

                        if (morada.MoradaAldeiaFkNavigation.AldeiaSucoFkNavigation.SucoPostoAdminFkNavigation.PostoAdminMunicipioFkNavigation != null && morada.MoradaAldeiaFkNavigation.AldeiaSucoFkNavigation.SucoPostoAdminFkNavigation.PostoAdminMunicipioFkNavigation.Nome != null)
                        {
                            moradaFinal += morada.MoradaAldeiaFkNavigation.AldeiaSucoFkNavigation.SucoPostoAdminFkNavigation.PostoAdminMunicipioFkNavigation.Nome + " ";
                        }
                    }
                }
            }

            if (morada.MoradaPaisFkNavigation != null && morada.MoradaPaisFkNavigation.Nome != null)
            {
                moradaFinal += morada.MoradaPaisFkNavigation.Nome;
            }

            return moradaFinal;
        }
    }
}