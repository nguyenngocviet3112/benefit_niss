using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class MovimentosBancariosDataManager : IMovimentosBancariosDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration Configuration;
        private readonly IUtilsDataManager _utils;

        public MovimentosBancariosDataManager(IUnitOfWork unitOfWork, IConfiguration configuration, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            Configuration = configuration;
            _utils = utils;
        }

        public ResponseBaseDataContract InsertMovimento(MovimentosUpsertDataRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            var tarefaConfig = _unitOfWork.TarefaAtivoRepository.GetTarefaAtivoById(request.data.TarefaAtivoId).TarefaconfigFkNavigation;

            // Validar se a tarefa tem permissões para o componente
            if (tarefaConfig.Componenteconciliacaomovimentos.FirstOrDefault(x => x.IndActivo).PermissaoMovimentosBancarios != 2)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            try
            {
                _unitOfWork.MovimentosbancariosRepository.Add(CreateObject(request));
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }

        public ResponseBaseDataContract UpdateMovimento(MovimentosUpsertDataRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            var tarefaConfig = _unitOfWork.TarefaAtivoRepository.GetTarefaAtivoById(request.data.TarefaAtivoId).TarefaconfigFkNavigation;

            // Validar se a tarefa tem permissões para o componente
            if (tarefaConfig.Componenteconciliacaomovimentos.FirstOrDefault(x => x.IndActivo).PermissaoMovimentosBancarios != 2)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            if (request.data.Id == null)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidConfiguration).ToString(), ErrorMessage = ErrorsDataContract.InvalidConfiguration.ToString() });
                return response;
            }

            try
            {
                _unitOfWork.MovimentosbancariosRepository.Update(UpdateObject(request));
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }

        public ResponseBaseDataContract DeleteMovimento(MovimentosDeleteRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            var tarefaConfig = _unitOfWork.TarefaAtivoRepository.GetTarefaAtivoById(request.TarefaAtivoId).TarefaconfigFkNavigation;

            // Validar se a tarefa tem permissões para o componente
            if (tarefaConfig.Componenteconciliacaomovimentos.FirstOrDefault(x => x.IndActivo).PermissaoMovimentosBancarios != 2)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            try
            {
                Movimentosbancarios objToDelete = _unitOfWork.MovimentosbancariosRepository.Get(request.Id);
                objToDelete.IndActivo = false;
                _unitOfWork.MovimentosbancariosRepository.Update(objToDelete);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }

        public MovimentosListagemResponse ListMovimentos(MovimentosListagemRequest request)
        {
            MovimentosListagemResponse response = new MovimentosListagemResponse();

            try
            {
                response = _unitOfWork.MovimentosbancariosRepository.GetMovimentosByFilter(request);
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }

        public ContasBancariasListagemResponse ListContasBancarias(bool incluirSaldo)
        {
            ContasBancariasListagemResponse response = new ContasBancariasListagemResponse();

            try
            {
                response.contas = _unitOfWork.ContaBancariaRepository.GetAllDto(incluirSaldo);
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }

        public DominioDescricaoStringResponse getMovimentoBancarioDropList(int domainFilterId)
        {
            DominioDescricaoStringResponse response = new DominioDescricaoStringResponse();

            List<DominioDescricaoString> dominios = _unitOfWork.MovimentoBancarioRepository.GetMovimentosBancariosDominioByFilter(domainFilterId);
            response.dominios = dominios;

            return response;
        }

        public SaldoMovimentosResponse ListSaldoMovimentos(int? contaId, int? caixaId)
        {
            SaldoMovimentosResponse response = new SaldoMovimentosResponse();

            try
            {
                // Making sure the website is sending one of the Ids
                if (!contaId.HasValue && !caixaId.HasValue) throw new Exception("An Id is required");

                var creditoConciliados = _unitOfWork.MovimentosbancariosRepository.GetCreditoConciliados(contaId, caixaId);
                var debitoConciliados = _unitOfWork.MovimentosbancariosRepository.GetDebitoConciliados(contaId, caixaId);
                var creditoPorConciliar = _unitOfWork.MovimentosbancariosRepository.GetCreditoPorConciliar(contaId, caixaId);
                var debitoPorConciliar = _unitOfWork.MovimentosbancariosRepository.GetDebitoPorConciliar(contaId, caixaId);

                response = BuildSaldoResponse(creditoConciliados, debitoConciliados, creditoPorConciliar, debitoPorConciliar);
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }

        public ConciliarMovimentosPermissionsListResponse ListPermissions(ConciliarMovimentosPermissionsListRequest request)
        {
            ConciliarMovimentosPermissionsListResponse response = new ConciliarMovimentosPermissionsListResponse();

            var tarefaConfig = _unitOfWork.TarefaAtivoRepository.GetTarefaAtivoById(request.TarefaAtivoId).TarefaconfigFkNavigation;

            response.selectMovimentosTypePermission = tarefaConfig.Componenteconciliacaomovimentos.FirstOrDefault(x => x.IndActivo).PermissaoSelecionarMovimentos;
            response.addEditMovimentosPermission = tarefaConfig.Componenteconciliacaomovimentos.FirstOrDefault(x => x.IndActivo).PermissaoMovimentosConciliar;
            response.addEditMovimentosBancariosPermission = tarefaConfig.Componenteconciliacaomovimentos.FirstOrDefault(x => x.IndActivo).PermissaoMovimentosBancarios;
            response.viewSelectedToConciliatePermission = tarefaConfig.Componenteconciliacaomovimentos.FirstOrDefault(x => x.IndActivo).PermissaoVerMovimentosAconciliar;
            response.conciliatePermission = tarefaConfig.Componenteconciliacaomovimentos.FirstOrDefault(x => x.IndActivo).PermissaoConciliar;
            response.undoConciliationPermission = tarefaConfig.Componenteconciliacaomovimentos.FirstOrDefault(x => x.IndActivo).PermissaoDesfazerConciliar;

            return response;
        }

        public MovimentosListagemResponse GetListagemConciliacao(MovimentosBancarioConciliacaoListagemRequest request)
        {
            MovimentosListagemResponse response = new MovimentosListagemResponse();

            try
            {
                response = _unitOfWork.MovimentosbancariosRepository.GetListagemConciliacao(request, _unitOfWork.DominioRepository.getIdDominio("ESTADOMOVIMENTO", 2));
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }

        public SaldoMovimentosResponse BuildSaldoResponse(decimal? creditoConciliados, decimal? debitoConciliados, decimal? creditoPorConciliar, decimal? debitoPorConciliar)
        {
            var response = new SaldoMovimentosResponse();

            //Saldo Conciliado
            response.SaldoConciliadoCredit += creditoConciliados.GetValueOrDefault();
            response.SaldoConciliadoDebit += debitoConciliados.GetValueOrDefault();
            response.SaldoConciliadoTotal = response.SaldoConciliadoCredit + response.SaldoConciliadoDebit;

            //Saldo por Conciliar
            response.SaldoPorConciliarCredit += creditoPorConciliar.GetValueOrDefault();
            response.SaldoPorConciliarDebit += debitoPorConciliar.GetValueOrDefault();
            response.SaldoPorConciliarTotal = response.SaldoPorConciliarCredit + response.SaldoPorConciliarDebit;

            //Saldo Total
            response.SaldoTotalCredit = response.SaldoConciliadoCredit + response.SaldoPorConciliarCredit;
            response.SaldoTotalDebit = response.SaldoConciliadoDebit + response.SaldoPorConciliarDebit;
            response.SaldoTotal = response.SaldoConciliadoTotal + response.SaldoPorConciliarTotal;

            return response;
        }

        public StringFileReponse ListMovimentosExcel(MovimentosListagemRequest request)
        {
            StringFileReponse response = new StringFileReponse();

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleRelatorios.Consultas, _unitOfWork);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            request.filter.index = 0;
            request.filter.rows = 999999;

            var data = ListMovimentos(request);

            response.File = _unitOfWork.MovimentosbancariosRepository.ListMovimentosExcel(request, data.movimentos);

            return response;
        }

        public ExcelImportReponse ImportMovimentos(ExcelImporterRequest request, int tarefaAtivoId, int? caixaId, int? bancoId)
        {
            var response = new ExcelImportReponse { RequestId = request.RequestId };
            try
            {
                var tarefaConfig = _unitOfWork.TarefaAtivoRepository.GetTarefaAtivoById(tarefaAtivoId).TarefaconfigFkNavigation;

                // Validar se a tarefa tem permissões para o componente
                if (tarefaConfig.Componenteconciliacaomovimentos.FirstOrDefault(x => x.IndActivo).PermissaoMovimentosBancarios != 2)
                {
                    response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                    return response;
                }

                var ip = _utils.GetIPV6();
                List<ExcelReaderService.Models.MovimentoBancario> movimentos;

                try
                {
                    movimentos = ExcelReaderService.ExcelReader.Read<ExcelReaderService.Models.MovimentoBancario>(request.File.OpenReadStream());
                }
                catch (Exception ex)
                {
                    response.Errors = new List<Error> { new Error { ErrorCode = ((int)ErrorsDataContract.ExcelNotValidOrMissingColumns).ToString(), ErrorMessage = ErrorsDataContract.ExcelNotValidOrMissingColumns.ToString() } };
                    return response;
                }

                var guid = Guid.NewGuid().ToString();

                Task.Run(async () =>
                {
                    using (var dataContext = new TimorINSSModuloContribuicoesContext(Configuration.GetConnectionString("sqlserverconnection")))
                    {

                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"Imports", $"{guid}.json");

                        File.Create(filePath).Close();

                        dynamic progressJson = new ExpandoObject();

                        progressJson.Total = movimentos.Count;
                        progressJson.Success = 0;
                        progressJson.Fail = 0;
                        progressJson.MissingFields = new List<int>();


                        for (int i = 0; i < movimentos.Count; i++)
                        {
                            try
                            {
                                var movimento = movimentos[i];

                                if (string.IsNullOrEmpty(movimento.Description) || !movimento._Date.HasValue || !movimento.Amount.HasValue || movimento.Amount == 0) // validar o resto
                                {
                                    // json error - campos obrigatórios em falta
                                    progressJson.Fail++;
                                    progressJson.MissingFields.Add(i + 2);
                                    File.WriteAllText(filePath, JsonConvert.SerializeObject(progressJson, Formatting.Indented));
                                    continue;
                                }

                                var row = new Movimentosbancarios()
                                {
                                    TarefaFk = tarefaAtivoId,
                                    CaixaFk = caixaId,
                                    ContaFk = bancoId,
                                    Descricao = movimento.Description,
                                    DataValor = movimento._Date.Value,
                                    Ipv6 = ip,
                                    DataCriacao = DateTime.Now,
                                    UtilizadorCriacao = request.UserId,
                                    IndActivo = true
                                };

                                if (movimento.Amount >= 0)
                                    row.Credito = movimento.Amount;
                                else
                                    row.Debito = movimento.Amount;

                                dataContext.Movimentosbancarios.Add(row);

                                dataContext.SaveChanges();

                                progressJson.Success++;
                                File.WriteAllText(filePath, JsonConvert.SerializeObject(progressJson, Formatting.Indented));

                                // update json counts

                                //await Task.Delay(300);
                            }
                            catch (Exception ex)
                            {

                            }
                        }

                        await Task.Delay(5000);

                        File.Delete(filePath);
                    }

                });

                response.Id = guid;
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }

        #region Private methods

        private Movimentosbancarios CreateObject(MovimentosUpsertDataRequest request)
        {
            Movimentosbancarios newObj = new Movimentosbancarios()
            {
                TarefaFk = request.data.TarefaAtivoId,
                CaixaFk = request.data.CaixaId,
                ContaFk = request.data.BancoId,
                Descricao = request.data.Descricao,
                DataValor = request.data.Data,
                IndActivo = true
            };

            if (request.data.Valor >= 0)
                newObj.Credito = request.data.Valor;
            else
                newObj.Debito = request.data.Valor;

            newObj = _utils.SetDetailsToEntity(newObj);
            return newObj;
        }

        private Movimentosbancarios UpdateObject(MovimentosUpsertDataRequest request)
        {
            Movimentosbancarios ObjToUpdate = _unitOfWork.MovimentosbancariosRepository.Get(request.data.Id.Value);

            ObjToUpdate.Descricao = request.data.Descricao;
            ObjToUpdate.DataValor = request.data.Data;

            if (request.data.Valor >= 0)
            {
                ObjToUpdate.Credito = request.data.Valor;
                ObjToUpdate.Debito = null;
            }
            else
            {
                ObjToUpdate.Debito = request.data.Valor;
                ObjToUpdate.Credito = null;
            }

            ObjToUpdate = _utils.UpdateDetailsToEntity(ObjToUpdate);
            return ObjToUpdate;
        }

        #endregion Private methods
    }
}