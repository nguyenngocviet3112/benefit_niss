using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.ExcelDocumentService;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;
using TimorINSSBackEnd.Resources;
using static TimorINSSBackEnd.ExcelDocumentService.Models;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class PagamentoExecutadoDataManager : IPagamentoExecutadoDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public PagamentoExecutadoDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils, IStringLocalizer<SharedResource> localizer)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
            _localizer = localizer;
        }

        public ResponseBaseDataContract SavePagamentoExecutadoImport(SavePagamentoExecutadoRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.PreenchimentoTarefa, _unitOfWork, CRUD.UPDATE);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            // Pagamento executado

            try
            {
                var importRows = _unitOfWork.DestinatarioRepository.GetImportRows(request.importId.Value);

                foreach (var importRow in importRows)
                {
                    Pagamentosexecutados pagamentoExecutado = BuildPagamentoExecutadoImportObject(importRow, request.pagamento);

                    _unitOfWork.PagamentosExecutadosRepository.Add(pagamentoExecutado);
                }
                

                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }

        public ResponseBaseDataContract SavePagamentoExecutado(SavePagamentoExecutadoRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.PreenchimentoTarefa, _unitOfWork, CRUD.UPDATE);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            // validações

            //destinatario
            Destinatario destinatario = BuildDestinatarioObject(request.destinatario);

            Destinatario entidadeEmpregadora = null;
            if (destinatario.EntidadeFk != null && destinatario.EntidadeFk > 0)
            {
                entidadeEmpregadora = _unitOfWork.DestinatarioRepository.GetDestinatarioByIdEntidade((int)destinatario.EntidadeFk);
                if (entidadeEmpregadora != null)
                {
                    destinatario.Id = entidadeEmpregadora.Id;
                }
            }
            Destinatario trabalhador = null;
            if (destinatario.TrabalhadorFk != null && destinatario.TrabalhadorFk > 0)
            {
                trabalhador = _unitOfWork.DestinatarioRepository.GetDestinatarioByIdTrabalhador((int)destinatario.TrabalhadorFk);
                if (trabalhador != null)
                {
                    destinatario.Id = trabalhador.Id;
                }
            }

            // Pagamento executado
            Pagamentosexecutados pagamentoExecutado = BuildPagamentoExecutadoObject(request.pagamento);

            try
            {
                if (destinatario.Id > 0)
                {
                    _unitOfWork.DestinatarioRepository.Update(destinatario);
                    pagamentoExecutado.DestinatarioFk = destinatario.Id;
                }
                else
                {
                    if (entidadeEmpregadora == null && trabalhador == null)
                    {
                        _unitOfWork.DestinatarioRepository.Add(destinatario);
                        pagamentoExecutado.DestinatarioFkNavigation = destinatario;
                    }
                }

                if (pagamentoExecutado.Id > 0)
                {
                    _unitOfWork.PagamentosExecutadosRepository.Update(pagamentoExecutado);
                }
                else
                {
                    _unitOfWork.PagamentosExecutadosRepository.Add(pagamentoExecutado);
                }

                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }

        public DestinatarioPagamentoExecutadosResponse GetDestinatariosPagamentoByNumPagamento(GetDestinatarioPagamentoRequest request)
        {
            var response = new DestinatarioPagamentoExecutadosResponse { RequestId = request.RequestId };

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.PreenchimentoTarefa, _unitOfWork, CRUD.UPDATE);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            Morada moradaDestinatario = new Morada();
            int idDestinatario = 0;
            List<PagamentoExecutadoDestinatrioDataContract> listaDestinatarioPagamento = new List<PagamentoExecutadoDestinatrioDataContract>();
            if (!string.IsNullOrEmpty(request.NumPagamento))
            {
                listaDestinatarioPagamento = _unitOfWork.PagamentosExecutadosRepository.GetPagamentosexecutadosByNumPagamento(request.NumPagamento);

                if (listaDestinatarioPagamento != null && listaDestinatarioPagamento.Count > 0)
                {
                    foreach (var pagamentoDestinatario in listaDestinatarioPagamento)
                    {
                        idDestinatario = pagamentoDestinatario.Destinatario.Id;

                        //Logica consoante se temos uma relação com entidade ou com trabalhador, e vai buscar as informações consoante o que temos disponível
                        if (pagamentoDestinatario.Destinatario != null && pagamentoDestinatario.Destinatario.EntidadeFk != null
                            && pagamentoDestinatario.Destinatario.EntidadeFk > 0)
                        {
                            pagamentoDestinatario.Destinatario = _unitOfWork.EntidadeEmpregadoraRepository.GetDestinatarioByIdEntidade((int)pagamentoDestinatario.Destinatario.EntidadeFk);

                            if (pagamentoDestinatario.Destinatario != null)
                            {
                                pagamentoDestinatario.Destinatario.Id = idDestinatario;
                                moradaDestinatario = _unitOfWork.MoradaRepository.GetMoradasPrincipalByEntidadeFk((int)pagamentoDestinatario.Destinatario.EntidadeFk);

                                if (moradaDestinatario != null)
                                {
                                    pagamentoDestinatario.Destinatario.Morada = BuildMoradaString(moradaDestinatario);
                                }
                            }
                        }

                        if (pagamentoDestinatario.Destinatario != null && pagamentoDestinatario.Destinatario.TrabalhadorFk != null
                            && pagamentoDestinatario.Destinatario.TrabalhadorFk > 0)
                        {
                            pagamentoDestinatario.Destinatario = _unitOfWork.TrabalhadoresRepository.GetDestinatarioByIdTrabalhador((int)pagamentoDestinatario.Destinatario.TrabalhadorFk);
                            if (pagamentoDestinatario.Destinatario != null)
                            {
                                pagamentoDestinatario.Destinatario.Id = idDestinatario;

                                moradaDestinatario = _unitOfWork.MoradaRepository.GetMoradasPrincipalByEntidadeFk((int)pagamentoDestinatario.Destinatario.TrabalhadorFk);

                                if (moradaDestinatario != null)
                                {
                                    pagamentoDestinatario.Destinatario.Morada = BuildMoradaString(moradaDestinatario);
                                }
                            }
                        }
                    }

                    response.DestinatarioPagamentosExecutados = listaDestinatarioPagamento;
                }
            }
            return response;
        }

        public ResponseBaseDataContract DeletePagamento(DeletePagamentoRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.PreenchimentoTarefa, _unitOfWork, CRUD.UPDATE);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            // validações
            Pagamentosexecutados pagamento = _unitOfWork.PagamentosExecutadosRepository.Get(request.Id);
            if (pagamento != null)
            {
                pagamento.IndActivo = false;
                pagamento = _utils.UpdateDetailsToEntity(pagamento);
            }

            try
            {
                if (pagamento != null)
                {
                    _unitOfWork.PagamentosExecutadosRepository.Update(pagamento);
                }
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }

        public DestinatarioPagamentoExecutadosResponse GetPagamentosExecutadosByIdDestinatario(GetPagamentoExecutadoRequest request)
        {
            var response = new DestinatarioPagamentoExecutadosResponse { RequestId = request.RequestId };

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.PreenchimentoTarefa, _unitOfWork, CRUD.UPDATE);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            List<PagamentoExecutadoDestinatrioDataContract> listaDestinatarioPagamento = new List<PagamentoExecutadoDestinatrioDataContract>();
            int idDestinatario = 0;
            listaDestinatarioPagamento = _unitOfWork.PagamentosExecutadosRepository.GetPagamentosexecutadosByIdDestinatario(request.IdDestinatario);
            if (listaDestinatarioPagamento != null && listaDestinatarioPagamento.Count > 0)
            {
                foreach (var pagamentoDestinatario in listaDestinatarioPagamento)
                {
                    idDestinatario = pagamentoDestinatario.Destinatario.Id;

                    //Logica consoante se temos uma relação com entidade ou com trabalhador, e vai buscar as informações consoante o que temos disponível
                    if (pagamentoDestinatario.Destinatario != null && pagamentoDestinatario.Destinatario.EntidadeFk != null
                        && pagamentoDestinatario.Destinatario.EntidadeFk > 0)
                    {
                        pagamentoDestinatario.Destinatario = _unitOfWork.EntidadeEmpregadoraRepository.GetDestinatarioByIdEntidade((int)pagamentoDestinatario.Destinatario.EntidadeFk);

                        if (pagamentoDestinatario.Destinatario != null)
                        {
                            pagamentoDestinatario.Destinatario.Id = idDestinatario;
                        }
                    }

                    if (pagamentoDestinatario.Destinatario != null && pagamentoDestinatario.Destinatario.TrabalhadorFk != null
                        && pagamentoDestinatario.Destinatario.TrabalhadorFk > 0)
                    {
                        pagamentoDestinatario.Destinatario = _unitOfWork.TrabalhadoresRepository.GetDestinatarioByIdTrabalhador((int)pagamentoDestinatario.Destinatario.TrabalhadorFk);
                        if (pagamentoDestinatario.Destinatario != null)
                        {
                            pagamentoDestinatario.Destinatario.Id = idDestinatario;
                        }
                    }
                }

                response.DestinatarioPagamentosExecutados = listaDestinatarioPagamento;
            }
            response.DestinatarioPagamentosExecutados = listaDestinatarioPagamento;

            if (response.DestinatarioPagamentosExecutados != null && response.DestinatarioPagamentosExecutados.Count > 0)
            {
                response.ExcelExtraido = ExtractToExcel(response.DestinatarioPagamentosExecutados, false);
            }

            return response;
        }

        public string ExtractToExcel(List<PagamentoExecutadoDestinatrioDataContract> destinatarioPagamentosExecutados, bool numPagamentoCell)
        {

            // Inicialização do documento excel
            var excelDocument = new ExcelDocument(_localizer["pagamentos"].Value, new ExcelDocumentOptions()
            {
                TextStyles = Extensions.ServiceExtensions.ExcelDocumentTextStyles
            });

            var header = numPagamentoCell ? _localizer["listaPagamentosProcesso"].Value : _localizer["listaPagamentosDestinatario"].Value;

            // Adição dos títulos
            excelDocument.Pages[0].AddText(header, new ExcelDocumentTextPosition(1, 1), "Header", new ExcelDocumentTextPosition(7, 1));

            excelDocument.Pages[0].AddTable(new ExcelDocumentTextPosition(1, 3), destinatarioPagamentosExecutados, new List<ColumnOption<PagamentoExecutadoDestinatrioDataContract>>()
            {
                new ColumnOption<PagamentoExecutadoDestinatrioDataContract>()
                {
                    Name = numPagamentoCell ? _localizer["numeroPagamento"].Value : _localizer["destinatario"].Value,
                    ColumnTextStyleKey = "TableColumn",
                    DataTextStyleKey = "TableCell",
                    Value = (data) => numPagamentoCell ? data.NumeroPagamento : data.Destinatario.Nome
                },
                new ColumnOption<PagamentoExecutadoDestinatrioDataContract>()
                {
                    Name = numPagamentoCell ? _localizer["destinatario"].Value : _localizer["tin"].Value,
                    ColumnTextStyleKey = "TableColumn",
                    DataTextStyleKey = "TableCell",
                    Value = (data) => numPagamentoCell ? data.Destinatario.Nome : data.Destinatario.Tin
                },
                new ColumnOption<PagamentoExecutadoDestinatrioDataContract>()
                {
                    Name = numPagamentoCell ? _localizer["tin"].Value : _localizer["niss"].Value,
                    ColumnTextStyleKey = "TableColumn",
                    DataTextStyleKey = "TableCell",
                    Value = (data) => numPagamentoCell ? data.Destinatario.Tin : data.Destinatario.Niss
                },
                new ColumnOption<PagamentoExecutadoDestinatrioDataContract>()
                {
                    Name = numPagamentoCell ? _localizer["niss"].Value : _localizer["numeroPagamento"].Value,
                    ColumnTextStyleKey = "TableColumn",
                    DataTextStyleKey = "TableCell",
                    Value = (data) => numPagamentoCell ? data.Destinatario.Niss : data.NumeroPagamento
                },
                new ColumnOption<PagamentoExecutadoDestinatrioDataContract>()
                {
                    Name = _localizer["estadoPagamento"].Value,
                    ColumnTextStyleKey = "TableColumn",
                    DataTextStyleKey = "TableCell",
                    Value = (data) => data.Estado
                },
                new ColumnOption<PagamentoExecutadoDestinatrioDataContract>()
                {
                    Name = _localizer["despesa"].Value,
                    ColumnTextStyleKey = "TableColumn",
                    DataTextStyleKey = "TableCell",
                    Value = (data) => data.DescricaoDespesa
                },
                new ColumnOption<PagamentoExecutadoDestinatrioDataContract>()
                {
                    Name = _localizer["valorExecutado"].Value,
                    ColumnTextStyleKey = "TableColumn",
                    DataTextStyleKey = "TableCellMoney",
                    Value = (data) => data.ValorExecutado
                },
            });

            // Adição da linha de Total como footer da tabela
            excelDocument.Pages[0].AddText(_localizer["total"].Value + ":", new ExcelDocumentTextPosition(1, 4 + destinatarioPagamentosExecutados.Count), "HeaderAlignRight", new ExcelDocumentTextPosition(6, 4 + destinatarioPagamentosExecutados.Count));

            var sumRange = new ExcelDocumentTextPositionRange(new ExcelDocumentTextPosition(7, 4), new ExcelDocumentTextPosition(7, 4 + destinatarioPagamentosExecutados.Count - 1));
            excelDocument.Pages[0].AddSum(new ExcelDocumentTextPosition(7, 4 + destinatarioPagamentosExecutados.Count), sumRange, "HeaderAlignLeftMoney");

            // Conversão do documento excel em base64
            return excelDocument.GetFileString();

        }

        public DestinatarioPagamentoExecutadosResponse GetListaPagamentoExcel(ListagemPagamentosProcessoRequest request)
        {
            var response = new DestinatarioPagamentoExecutadosResponse { RequestId = request.RequestId };

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.PreenchimentoTarefa, _unitOfWork, CRUD.UPDATE);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            Morada moradaDestinatario = new Morada();
            int idDestinatario = 0;
            List<PagamentoExecutadoDestinatrioDataContract> listaDestinatarioPagamento = new List<PagamentoExecutadoDestinatrioDataContract>();
            if (request.ProcessoAtivo > 0)
            {
                listaDestinatarioPagamento = _unitOfWork.PagamentosExecutadosRepository.GetPagamentosexecutadosByProcessoAtivoID(request.ProcessoAtivo);

                if (listaDestinatarioPagamento != null && listaDestinatarioPagamento.Count > 0)
                {
                    foreach (var pagamentoDestinatario in listaDestinatarioPagamento)
                    {
                        idDestinatario = pagamentoDestinatario.Destinatario.Id;

                        //Logica consoante se temos uma relação com entidade ou com trabalhador, e vai buscar as informações consoante o que temos disponível
                        if (pagamentoDestinatario.Destinatario != null && pagamentoDestinatario.Destinatario.EntidadeFk != null
                            && pagamentoDestinatario.Destinatario.EntidadeFk > 0)
                        {
                            pagamentoDestinatario.Destinatario = _unitOfWork.EntidadeEmpregadoraRepository.GetDestinatarioByIdEntidade((int)pagamentoDestinatario.Destinatario.EntidadeFk);

                            if (pagamentoDestinatario.Destinatario != null)
                            {
                                pagamentoDestinatario.Destinatario.Id = idDestinatario;
                                moradaDestinatario = _unitOfWork.MoradaRepository.GetMoradasPrincipalByEntidadeFk((int)pagamentoDestinatario.Destinatario.EntidadeFk);

                                if (moradaDestinatario != null)
                                {
                                    pagamentoDestinatario.Destinatario.Morada = BuildMoradaString(moradaDestinatario);
                                }
                            }
                        }

                        if (pagamentoDestinatario.Destinatario != null && pagamentoDestinatario.Destinatario.TrabalhadorFk != null
                            && pagamentoDestinatario.Destinatario.TrabalhadorFk > 0)
                        {
                            pagamentoDestinatario.Destinatario = _unitOfWork.TrabalhadoresRepository.GetDestinatarioByIdTrabalhador((int)pagamentoDestinatario.Destinatario.TrabalhadorFk);
                            if (pagamentoDestinatario.Destinatario != null)
                            {
                                pagamentoDestinatario.Destinatario.Id = idDestinatario;

                                moradaDestinatario = _unitOfWork.MoradaRepository.GetMoradasPrincipalByEntidadeFk((int)pagamentoDestinatario.Destinatario.TrabalhadorFk);

                                if (moradaDestinatario != null)
                                {
                                    pagamentoDestinatario.Destinatario.Morada = BuildMoradaString(moradaDestinatario);
                                }
                            }
                        }
                    }

                    response.DestinatarioPagamentosExecutados = listaDestinatarioPagamento;

                    if (response.DestinatarioPagamentosExecutados != null && response.DestinatarioPagamentosExecutados.Count > 0)
                    {
                        response.ExcelExtraido = ExtractToExcel(response.DestinatarioPagamentosExecutados, true);
                    }
                }
            }
            return response;
        }

        public ResponseBaseDataContract EditPagamentoExecutado(EditPagamentoExecutadoRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.PreenchimentoTarefa, _unitOfWork, CRUD.UPDATE);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            // validações
            Pagamentosexecutados pagamentosExecutados = null;
            List<Pagamentosexecutados> listaPagamentosToUpdate = new List<Pagamentosexecutados>();

            if (request.listaIdPagamento != null && request.listaIdPagamento.Count > 0)
            {
                foreach (var id in request.listaIdPagamento)
                {
                    pagamentosExecutados = _unitOfWork.PagamentosExecutadosRepository.Get(id);

                    if (pagamentosExecutados != null)
                    {
                        pagamentosExecutados.Estado = _unitOfWork.DominioRepository.getIdDominio("ESTADOPAGAMENTO", 2);
                        pagamentosExecutados = _utils.UpdateDetailsToEntity(pagamentosExecutados);
                        listaPagamentosToUpdate.Add(pagamentosExecutados);
                    }
                }
            }

            try
            {
                if (listaPagamentosToUpdate != null && listaPagamentosToUpdate.Count > 0)
                {
                    foreach (var pagamento in listaPagamentosToUpdate)
                    {
                        _unitOfWork.PagamentosExecutadosRepository.Update(pagamento);
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

        public ListagemPagamentosProcessoResponse GetListaPagamentosProcesso(ListagemPagamentosProcessoRequest request)
        {
            var response = new ListagemPagamentosProcessoResponse { RequestId = request.RequestId };

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.PreenchimentoTarefa, _unitOfWork, CRUD.UPDATE);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            List<ListaPagamentosDoProcessoDataContract> listaPagamentosProcessoresponse = new List<ListaPagamentosDoProcessoDataContract>();

            List<ListaPagamentosDoProcessoDataContract> listaPagamentosProcesso = _unitOfWork.PagamentosExecutadosRepository.GetListaPagamentosByProcessoAtivoID(request.ProcessoAtivo);
            if (listaPagamentosProcesso != null && listaPagamentosProcesso.Count > 0)
            {
                foreach (var pagamento in listaPagamentosProcesso ) {
                    listaPagamentosProcessoresponse = _unitOfWork.PagamentosExecutadosRepository.GetListaPagamentoByNumPagamento(pagamento.NumeroPagamento);
                    
                    if (listaPagamentosProcessoresponse != null && listaPagamentosProcessoresponse.Count > 0) {

                        foreach (var pagamentoProcesso in listaPagamentosProcessoresponse) {
                            pagamento.Valor = pagamento.Valor + pagamentoProcesso.Valor;

                        }
                    }

                }
                response.Pagamentos = listaPagamentosProcesso;
            }
            return response;
        }

        public ListagemPagamentosProcessoResponse GetPagamentoDetails(GetDestinatarioPagamentoRequest request) {

            var response = new ListagemPagamentosProcessoResponse { RequestId = request.RequestId };

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.PreenchimentoTarefa, _unitOfWork, CRUD.UPDATE);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            List<ListaPagamentosDoProcessoDataContract> listaPagamentosProcesso = _unitOfWork.PagamentosExecutadosRepository.GetListaPagamentoByNumPagamento(request.NumPagamento);
            if (listaPagamentosProcesso != null && listaPagamentosProcesso.Count > 0)
            {
                foreach (var pagamento in listaPagamentosProcesso)
                {
                    pagamento.ContaOGE = _unitOfWork.AgrupamentoConfigRepository.GetFullCodigo(pagamento.idContaOGE) + "-" + _unitOfWork.AgrupamentoConfigRepository.GetFullDesignacao(pagamento.idContaOGE);

                }
                response.Pagamentos = listaPagamentosProcesso;
            }
            return response;
        }

        private Pagamentosexecutados BuildPagamentoExecutadoImportObject(ExcelImporterRel importRow, PagamentoExecutadoDataContract request)
        {
            var pag = JsonConvert.DeserializeObject<ExcelReaderService.Models.Pagamento>(importRow.Data);

            Pagamentosexecutados pagamentoExecutado = new Pagamentosexecutados
            {
                CompromissoFk = request.CompromissoFk,
                DestinatarioFk = importRow.RelId,
                NumeroPagamento = request.NumeroPagamento,
                ValorExecutado = pag.Amount.Value,
                Estado = _unitOfWork.DominioRepository.getIdDominio("ESTADOPAGAMENTO", 1),
                Iban = pag.IBAN ?? null,
                Swift = pag.Swift ?? null,
                ProcessoAtivoFk = request.ProcessoId,
                IndActivo = true,
                NumeroConta = string.IsNullOrEmpty(pag.IBAN) ? pag.AccountNumber : null,
                CodigoContaCreditoFk = request.CodigoContaCredito,
                CodigoContaDebitoFk = request.CodigoContaDebito,
                DataObrigacao = request.DataObrigacao,
            };

            pagamentoExecutado = _utils.SetDetailsToEntity(pagamentoExecutado);

            return pagamentoExecutado;
        }

        private Pagamentosexecutados BuildPagamentoExecutadoObject(PagamentoExecutadoDataContract request)
        {
            Pagamentosexecutados pagamentoExecutado = new Pagamentosexecutados
            {
                Id = request.Id,
                CompromissoFk = request.CompromissoFk,
                DestinatarioFk = request.DestinatarioFk,
                NumeroPagamento = request.NumeroPagamento,
                ValorExecutado = request.ValorExecutado,
                Estado = _unitOfWork.DominioRepository.getIdDominio("ESTADOPAGAMENTO", 1),
                Iban = request.Iban,
                Swift = request.Swift,
                BankCode = request.BankCode,
                ProcessoAtivoFk = request.ProcessoId,
                IndActivo = true,
                NumeroConta = request.NumeroConta,
                CodigoContaCreditoFk = request.CodigoContaCredito,
                CodigoContaDebitoFk = request.CodigoContaDebito,
                DataObrigacao = request.DataObrigacao,
            };

            if (pagamentoExecutado.Id > 0)
            {
                Pagamentosexecutados original = _unitOfWork.PagamentosExecutadosRepository.Get(pagamentoExecutado.Id);
                pagamentoExecutado.UtilizadorCriacao = original.UtilizadorCriacao;
                pagamentoExecutado.DataCriacao = original.DataCriacao;
                pagamentoExecutado = _utils.UpdateDetailsToEntity(pagamentoExecutado);
            }
            else
                pagamentoExecutado = _utils.SetDetailsToEntity(pagamentoExecutado);
            return pagamentoExecutado;
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
                destinatario = new Destinatario();
                destinatario.EntidadeFk = request.EntidadeFk;
                destinatario.IndActivo = true;
            }

            if (destinatario.TrabalhadorFk != null && destinatario.TrabalhadorFk > 0)
            {
                destinatario = new Destinatario();
                destinatario.TrabalhadorFk = request.TrabalhadorFk;
                destinatario.IndActivo = true;
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
            string moradaFinal = "";
            moradaFinal = !string.IsNullOrEmpty(morada.Rua) ? morada.Rua + "," : "";
            moradaFinal += !string.IsNullOrEmpty(morada.NumPorta) ? morada.NumPorta + " - " : "";

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

        public RelatorioPagamentosListagemResponse GetPagamentosRelatoriosGrouped(RelatorioPagamentosListagemRequest request)
        {
            RelatorioPagamentosListagemResponse response = new RelatorioPagamentosListagemResponse();

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleRelatorios.Consultas, _unitOfWork);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            try
            {
                response = _unitOfWork.PagamentosExecutadosRepository.GetPagamentosRelatoriosGrouped(request);
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }

        public SelectDescriptionResponse GetDropdownContasOGE(RelatorioContaOGEDropdownListagemRequest request)
        {
            SelectDescriptionResponse response = new SelectDescriptionResponse();

            try
            {
                response = _unitOfWork.PagamentosExecutadosRepository.GetDropdownContasOGE(request);
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }

        public SelectDescriptionResponse GetCentrosCusto(SearchFilterRequest request)
        {
            SelectDescriptionResponse response = new SelectDescriptionResponse();

            try
            {
                response.selects = _unitOfWork.CentroCustoRepository.GetAllActive(request.filter.dateFilterBegin).Select(e => new SelectDescription()
                {
                    id = e.Id,
                    nome = e.Descricao
                })
                .ToList();
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }

        public StringFileReponse ExtractToExcelRelatorios(RelatorioPagamentosListagemRequest request)
        {
            StringFileReponse response = new StringFileReponse();

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleRelatorios.Consultas, _unitOfWork);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            var pagamentos = _unitOfWork.PagamentosExecutadosRepository.GetPagamentosRelatoriosExcel(request);

            // Inicialização do documento excel
            var excelDocument = new ExcelDocument(_localizer["ordensPagamento"].Value, new ExcelDocumentOptions()
            {
                TextStyles = Extensions.ServiceExtensions.ExcelDocumentTextStyles
            });

            // Adição dos títulos
            excelDocument.Pages[0].AddText(_localizer["consultaOrdensPagamento"].Value, new ExcelDocumentTextPosition(1, 1), "Header", new ExcelDocumentTextPosition(6, 1));

            // Adição da tabela
            excelDocument.Pages[0].AddTable(new ExcelDocumentTextPosition(1, 3), pagamentos.pagamentos, new List<ColumnOption<RelatorioPagamentoDataContract>>()
            {
                new ColumnOption<RelatorioPagamentoDataContract>()
                {
                    Name = _localizer["nOrdemPagamento"].Value,
                    ColumnTextStyleKey = "TableColumn",
                    DataTextStyleKey = "TableCell",
                    Value = (data) => data.numeroPagamento
                },
                new ColumnOption<RelatorioPagamentoDataContract>()
                {
                    Name = _localizer["destinatario"].Value,
                    ColumnTextStyleKey = "TableColumn",
                    DataTextStyleKey = "TableCell",
                    Value = (data) => data.destinatario
                },
                new ColumnOption<RelatorioPagamentoDataContract>()
                {
                    Name = _localizer["contaOGE"].Value,
                    ColumnTextStyleKey = "TableColumn",
                    DataTextStyleKey = "TableCell",
                    Value = (data) => data.conta
                },
                new ColumnOption<RelatorioPagamentoDataContract>()
                {
                    Name = _localizer["dataEmissao"].Value,
                    ColumnTextStyleKey = "TableColumn",
                    DataTextStyleKey = "TableCell",
                    Value = (data) => data.dataEmissao.ToString("dd/MM/yyyy")
                },
                new ColumnOption<RelatorioPagamentoDataContract>()
                {
                    Name = _localizer["estado"].Value,
                    ColumnTextStyleKey = "TableColumn",
                    DataTextStyleKey = "TableCell",
                    Value = (data) => data.estado
                },
                new ColumnOption<RelatorioPagamentoDataContract>()
                {
                    Name = _localizer["valor"].Value,
                    ColumnTextStyleKey = "TableColumn",
                    DataTextStyleKey = "TableCellMoney",
                    Value = (data) => data.valor
                }
            });

            // Adição da linha de Total como footer da tabela
            excelDocument.Pages[0].AddText(_localizer["total"].Value + ":", new ExcelDocumentTextPosition(1, 4 + pagamentos.pagamentos.Count), "HeaderAlignRight", new ExcelDocumentTextPosition(5, 4 + pagamentos.pagamentos.Count));

            var sumRange = new ExcelDocumentTextPositionRange(new ExcelDocumentTextPosition(6, 4), new ExcelDocumentTextPosition(6, 4 + pagamentos.pagamentos.Count - 1));
            excelDocument.Pages[0].AddSum(new ExcelDocumentTextPosition(6, 4 + pagamentos.pagamentos.Count), sumRange, "HeaderAlignLeftMoney");

            // Conversão do documento excel em base64
            response.File = excelDocument.GetFileString();
            return response;
        }

        public RelatorioPagamentosListagemResponse GetPagamentosRelatorios(RelatorioPagamentosListagemRequest request)
        {
            RelatorioPagamentosListagemResponse response = new RelatorioPagamentosListagemResponse();

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleRelatorios.Relatorios, _unitOfWork);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            try
            {
                response = _unitOfWork.PagamentosExecutadosRepository.GetPagamentosRelatorios(request);
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }

        public ExecucaoOrcamentalListagemResponse GetExecucaoOrcamental(RelatorioExecucaoOrcamentalListagemRequest request)
        {
            ExecucaoOrcamentalListagemResponse response = new ExecucaoOrcamentalListagemResponse();

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleRelatorios.Relatorios, _unitOfWork);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            try
            {
                response = _unitOfWork.PagamentosExecutadosRepository.GetExecucaoOrcamental(request);
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }

        public StringFileReponse GetExecucaoOrcamentalExcel(RelatorioExecucaoOrcamentalListagemRequest request)
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

            var data = GetExecucaoOrcamental(request);

            response.File = _unitOfWork.AgrupamentoConfigRepository.ExecucaoOrcamentalExcel(request, data.lista);

            return response;
        }

        public ClassificacaoContabilisticaRelatorios ClassificacaoContabilisticaRelatorios(SearchFilterRequest request)
        {
            ClassificacaoContabilisticaRelatorios response = new ClassificacaoContabilisticaRelatorios();

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleRelatorios.Consultas, _unitOfWork);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            try
            {
                response = _unitOfWork.PagamentosExecutadosRepository.ClassificacaoContabilisticaRelatorios(request);
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }


        public StringFileReponse ClassificacaoContabilisticaRelatoriosExcel(SearchFilterRequest request)
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

            var data = ClassificacaoContabilisticaRelatorios(request);

            response.File = _unitOfWork.PagamentosExecutadosRepository.ClassificacaoContabilisticaRelatoriosExcel(request, data.movimentos);

            return response;
        }

        public FornecedoresRelatorios FornecedoresRelatorios(FornecedoresRelatoriosRequest request)
        {
            FornecedoresRelatorios response = new FornecedoresRelatorios();

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleRelatorios.Consultas, _unitOfWork);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            try
            {
                response = _unitOfWork.PagamentosExecutadosRepository.FornecedoresRelatorios(request);
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }

        public StringFileReponse FornecedoresRelatoriosExcel(FornecedoresRelatoriosRequest request)
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

            var data = FornecedoresRelatorios(request);

            response.File = _unitOfWork.PagamentosExecutadosRepository.FornecedoresRelatoriosExcel(request, data.movimentos);

            return response;
        }


        public BalancoRelatorios BalancoRelatorios(BalancoRelatoriosRequest request)
        {
            BalancoRelatorios response = new BalancoRelatorios();

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleRelatorios.Consultas, _unitOfWork);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            try
            {
                response = _unitOfWork.PagamentosExecutadosRepository.BalancoRelatorios(request);

                if (request.CompararAnoAnterior)
                {
                    request.filter.dateFilterBegin = request.filter.dateFilterBegin.Value.AddYears(-1);
                    request.filter.dateFilterEnd = request.filter.dateFilterEnd.Value.AddYears(-1);
                    var anoAnterior = _unitOfWork.PagamentosExecutadosRepository.BalancoRelatorios(request);

                    for (int i = 0; i < response.lista.Count; i++)
                    {
                        response.lista[i].creditoAntes = anoAnterior.lista[i].credito;
                        response.lista[i].debitoAntes = anoAnterior.lista[i].debito;
                    }

                }
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }

        public StringFileReponse BalancoRelatoriosExcel(BalancoRelatoriosRequest request)
        {
            StringFileReponse response = new StringFileReponse();

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleRelatorios.Consultas, _unitOfWork);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            var data = BalancoRelatorios(request);

            if (request.CompararAnoAnterior)
            {
                request.filter.dateFilterBegin = request.filter.dateFilterBegin.Value.AddYears(-1);
                request.filter.dateFilterEnd = request.filter.dateFilterEnd.Value.AddYears(-1);
                var anoAnterior = _unitOfWork.PagamentosExecutadosRepository.BalancoRelatorios(request);

                for (int i = 0; i < data.lista.Count; i++)
                {
                    data.lista[i].creditoAntes = anoAnterior.lista[i].credito;
                    data.lista[i].debitoAntes = anoAnterior.lista[i].debito;
                }

            }

            response.File = _unitOfWork.PagamentosExecutadosRepository.BalancoRelatoriosExcel(request, data.lista);

            return response;
        }

        public ResponseBaseDataContract SaveClassificacaoExecucao(SaveClassificacaoContabilisticaExecucaoRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleGestao.PreenchimentoTarefa, _unitOfWork, CRUD.UPDATE);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            // validações

            // Pagamentos executado
            var pagamentoExecutados = _unitOfWork.PagamentosExecutadosRepository.GetPagamentosexecutadosById(request.DespesasIds);

            try
            {
                foreach(var pagamentoExecutado in pagamentoExecutados)
                {
                    pagamentoExecutado.CreditoExecucaoFk = request.Classificacao.CodigoContaCredito;
                    pagamentoExecutado.DebitoExecucaoFk = request.Classificacao.CodigoContaDebito;
                    pagamentoExecutado.DataExecucao = request.Classificacao.DataExecucao;
                    _utils.UpdateDetailsToEntity(pagamentoExecutado);
                    _unitOfWork.PagamentosExecutadosRepository.Update(pagamentoExecutado);
                }


                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }
            return response;
        }
    }
}