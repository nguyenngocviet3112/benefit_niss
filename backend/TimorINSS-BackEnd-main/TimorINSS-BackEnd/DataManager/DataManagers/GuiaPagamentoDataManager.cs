using System;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class GuiaPagamentoDataManager : IGuiaPagamentoDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public GuiaPagamentoDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        public GuiapagamentoDto GetDto(int id)
        {
            return _unitOfWork.GuiaPagamentoRepository.GetDto(id);
        }

        public ResponseBaseDataContract SaveGuiaPagamento(GuiaPagamentoRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };
            Guiapagamento guiaPagamento = BuildGuiaPagamentoObject(request);

            try
            {
                _unitOfWork.GuiaPagamentoRepository.Add(guiaPagamento);
                _unitOfWork.Commit();

                _unitOfWork.ContaCorrenteRepository.UpdateSituacaoPagamento(guiaPagamento.ContaCorrenteId);
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
                _unitOfWork.Rollback();
            }
            return response;
        }

        private Guiapagamento BuildGuiaPagamentoObject(GuiaPagamentoRequest request)
        {
            EntidadeEmpregadoraConsultaResponse entidade = _unitOfWork.EntidadeEmpregadoraRepository.GetByIdEntidade(request.GuiaPagamento.GuiaEntidadeFk);
            string numDoc = _unitOfWork.GuiaPagamentoRepository.GetNextNumDocumento();
            GuiapagamentoDto guiaPagamento = new GuiapagamentoDto
            {

                GuiaEntidadeFk = request.GuiaPagamento.GuiaEntidadeFk,
                NumDocumento = numDoc,
                DtEmissao = DateTime.Now,
                Descricao = request.GuiaPagamento.Descricao,
                Valor = request.GuiaPagamento.Valor - request.GuiaPagamento.Juros,
                DataCriacao = DateTime.Now,
                IndPago = _unitOfWork.DominioRepository.getIdDominio("INDPAGO", 2),
                ComprovativoPag = null,
                DataComprovPag = null,
                ValorComprovPag = null,
                DtValidade = request.GuiaPagamento.DtValidade,
                TipoGuia = request.GuiaPagamento.TipoGuia > 1 ? _unitOfWork.DominioRepository.getIdDominio("TIPOGUIA", 2) :
                                                                _unitOfWork.DominioRepository.getIdDominio("TIPOGUIA", 1),
                ContaCorrenteId = request.IdContaCorrente,
                MesAno = request.GuiaPagamento.MesAno,
                IndActivo = true,
                QrInvoice = GenerateRandomString(25),
                
                PaymentRef = entidade.Niss + request.GuiaPagamento.MesAno.ToString("MMyyyy") +
                int.Parse(numDoc[0..^5]).ToString(),
                BankCode = request.GuiaPagamento.BankCode
            };
            guiaPagamento = _utils.SetDetailsToEntity(guiaPagamento);
            return Utils.MappClassFromDto<GuiapagamentoDto, Guiapagamento>(guiaPagamento);
        }

        

        private string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            char[] stringChars = new char[length];

            for (int i = 0; i < length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new string(stringChars);
        }

        //private Contacorrente BuildContaCorrenteObject(Contacorrente contaCorrente)
        //{
        //    ContacorrenteDto contaCorrenteDto = new ContacorrenteDto
        //    {
        //        IdContaCorrente = contaCorrente.IdContaCorrente,
        //        ContaCorrenteEntidadeFk = contaCorrente.ContaCorrenteEntidadeFk,
        //        ContaCorrenteTrabalhadorFk = contaCorrente.ContaCorrenteTrabalhadorFk,
        //        TipoDivida = contaCorrente.TipoDivida,
        //        MesAno = contaCorrente.MesAno,
        //        DataVencimento = contaCorrente.DataVencimento,
        //        ValorEntidade = contaCorrente.ValorEntidade,
        //        ValorTrabalhador = contaCorrente.ValorTrabalhador,
        //        ValorTotal = contaCorrente.ValorTotal,
        //        DataCriacao = DateTime.Now,
        //        PagoEm = contaCorrente.PagoEm,
        //        SituacaoPagamento = _unitOfWork.DominioRepository.getIdDominio("SITUACAOPAGAMENTO", 2),
        //        ContaCorrenteTaxaJuroFk = contaCorrente.ContaCorrenteTaxaJuroFk,
        //        Juroapurado = contaCorrente.Juroapurado,
        //        //GuiaPagamentoFk = contaCorrente.GuiaPagamentoFk,
        //        IndActivo = contaCorrente.IndActivo
        //    };
        //    contaCorrenteDto = _utils.SetDetailsToEntity(contaCorrenteDto);
        //    return Utils.MappClassFromDto<ContacorrenteDto, Contacorrente>(contaCorrenteDto);
        //}

        public GuiaListagemResponse listGuiasByEntidade(GetAllGuiasStatesFromYearByFilterRequest request)
        {
            GuiaListagemResponse response = new GuiaListagemResponse();
            if (request.filter == null)
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.FilterDoesNotExist).ToString(),
                    ErrorMessage = ErrorsDataContract.FilterDoesNotExist.ToString()
                });
            else
            {
                response = _unitOfWork.GuiaPagamentoRepository.getGuiasByFilter(request);
                
            }

            return response;
        }

        public GuiaListagemResponse guiaPagamentoDetail(GetGuiaPagamentoRequest request)
        {
            GuiaListagemResponse response = new GuiaListagemResponse();
            if (request.filter == null)
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.FilterDoesNotExist).ToString(),
                    ErrorMessage = ErrorsDataContract.FilterDoesNotExist.ToString()
                });
            else
            {
                response = _unitOfWork.GuiaPagamentoRepository.getGuiasPagamentoByFilter(request);

            }

            return response;
        }

        public GuiaListagemResponse listGuiasByEntidadeApprove(GetAllGuiasStatesFromDateByFilterRequest request)
        {
            GuiaListagemResponse response = new GuiaListagemResponse();
            if (request.filter == null)
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.FilterDoesNotExist).ToString(),
                    ErrorMessage = ErrorsDataContract.FilterDoesNotExist.ToString()
                });
            else
            {
                response = _unitOfWork.GuiaPagamentoRepository.getGuiasAporoveByFilter(request);

            }

            return response;

        }

        public ResponseBaseDataContract useCreditInGuiaPagamento(UseCreditInGuiaPagamentoRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            var guiaPagamento = _unitOfWork.GuiaPagamentoRepository.Get(request.idGuia);

            if (guiaPagamento == null)
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.EntityDoesNotExist).ToString(),
                    ErrorMessage = ErrorsDataContract.EntityDoesNotExist.ToString()
                });

            var reservaCredito = _unitOfWork.ReservaCreditoRepository.GetActiveByEntidadeId(request.idEntidade);

            if (reservaCredito == null)
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.EntityDoesNotExist).ToString(),
                    ErrorMessage = ErrorsDataContract.EntityDoesNotExist.ToString()
                });

            if (response.Errors.Count > 0)
                return response;

            var pagamentoTypes = _unitOfWork.DominioRepository.getAllTiposDeDominio(TiposDominio.INDPAGO);
            var contaCorrentePagamentoTypes = _unitOfWork.DominioRepository.getAllTiposDeDominio(TiposDominio.SITUACAOPAGAMENTO);

            var contaCorrente = _unitOfWork.ContaCorrenteRepository.Get(guiaPagamento.ContaCorrenteId);

            var indPago = (int)pagamentoTypes.Find(x => x.value == 1).id;
            var indPagoParcial = (int)pagamentoTypes.Find(x => x.value == 5).id;
            var contaCorrenteGerada = (int)contaCorrentePagamentoTypes.Find(x => x.value == 2).id;

            var documentoComprovativo = _unitOfWork.GuiaPagamentoRepository.Get(reservaCredito.ReservaGuiaPagamentoFk.GetValueOrDefault())?.ComprovativoPag;

            var valorAPagar = guiaPagamento.Valor + guiaPagamento.ValorJuros - reservaCredito.Valor.Value;

            guiaPagamento.ValorJurosFixo = contaCorrente.ValorJuros;
            //Logic of use credit to pay the value of guia de pagamento
            if (valorAPagar >= 0)
            {
                guiaPagamento.DataComprovPag = DateTime.Now;
                guiaPagamento.ComprovativoPag = documentoComprovativo;
                if (valorAPagar == 0)
                {
                    guiaPagamento.ValorComprovPag = guiaPagamento.Valor + guiaPagamento.ValorJuros;
                    guiaPagamento.IndPago = indPago;
                }
                else
                {
                    guiaPagamento.ValorComprovPag = reservaCredito.Valor.Value;
                    guiaPagamento.IndPago = indPagoParcial;
                }
                guiaPagamento = _utils.UpdateDetailsToEntity(guiaPagamento);
                // Updates Reserva Credito
                reservaCredito.Valor = 0;
                reservaCredito.IndActivo = false;
                reservaCredito = _utils.UpdateDetailsToEntity(reservaCredito);
            }
            else
            {
                var valorDeSobra = -valorAPagar;
                guiaPagamento.DataComprovPag = DateTime.Now;
                guiaPagamento.ValorComprovPag = guiaPagamento.Valor + guiaPagamento.ValorJuros;
                guiaPagamento.ComprovativoPag = documentoComprovativo;
                guiaPagamento.IndPago = indPago;
                guiaPagamento = _utils.UpdateDetailsToEntity(guiaPagamento);
                reservaCredito.Valor = valorDeSobra;
            }

            try
            {
                _unitOfWork.GuiaPagamentoRepository.Update(guiaPagamento);
                contaCorrente.PagoEm = DateTime.Now;

                _unitOfWork.ReservaCreditoRepository.Update(reservaCredito);
                _unitOfWork.Commit();

                _unitOfWork.ContaCorrenteRepository.UpdateSituacaoPagamento(guiaPagamento.ContaCorrenteId);
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
                _unitOfWork.Rollback();
            }

            return response;
        }

        public ResponseBaseDataContract insertComprovativoPagamento(insertComprovativoPagamentoRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            var guiaPagamento = _unitOfWork.GuiaPagamentoRepository.Get(request.idGuia);
            Reservacredito newReservaCredito = null;

            if (guiaPagamento == null)
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.EntityDoesNotExist).ToString(),
                    ErrorMessage = ErrorsDataContract.EntityDoesNotExist.ToString()
                });

            var pagamentoTypes = _unitOfWork.DominioRepository.getAllTiposDeDominio(TiposDominio.INDPAGO);
            var contaCorrentePagamentoTypes = _unitOfWork.DominioRepository.getAllTiposDeDominio(TiposDominio.SITUACAOPAGAMENTO);

            byte[] doc = Convert.FromBase64String(request.comprovativoPag);

            var indValidacao = (int)pagamentoTypes.Find(x => x.value == 3).id;
            var indValidacaoParcial = (int)pagamentoTypes.Find(x => x.value == 4).id;

            //var valorAPagar = guiaPagamento.Valor + guiaPagamento.ValorJuros - request.valorComprovativoPag;

            //guiaPagamento.ValorJurosFixo = contaCorrente.ValorJuros;
            ////logica de pagar a guia de pagamento
            //if (valorAPagar >= 0)
            //{
            //    guiaPagamento.DataComprovPag = request.dataComprovativoPag;
            //    guiaPagamento.ComprovativoPag = doc;
            //    if (valorAPagar == 0)
            //    {
            //        guiaPagamento.ValorComprovPag = guiaPagamento.Valor + guiaPagamento.ValorJuros;
            //        guiaPagamento.IndPago = indValidacao;
            //    }
            //    else
            //    {
            //        guiaPagamento.ValorComprovPag = request.valorComprovativoPag;
            //        guiaPagamento.IndPago = indValidacaoParcial;
            //    }
            //    guiaPagamento = _utils.UpdateDetailsToEntity(guiaPagamento);
            //}
            //else
            //{
            //    var valorDeSobra = -valorAPagar;
            //    guiaPagamento.DataComprovPag = request.dataComprovativoPag;
            //    guiaPagamento.ValorComprovPag = guiaPagamento.Valor + guiaPagamento.ValorJuros;
            //    guiaPagamento.ComprovativoPag = doc;
            //    guiaPagamento.IndPago = indValidacao;
            //    guiaPagamento = _utils.UpdateDetailsToEntity(guiaPagamento);
            //    newReservaCredito = new Reservacredito
            //    {
            //        ReservaEntidadeFk = request.idEntidade,
            //        Valor = valorDeSobra,
            //        IndActivo = false,
            //        ReservaGuiaPagamentoFk = guiaPagamento.IdGuia
            //    };
            //    newReservaCredito = _utils.SetDetailsToEntity(newReservaCredito);
            //}

            try
            {
                guiaPagamento.ValorComprovPag = Math.Min(guiaPagamento.Valor, request.valorComprovativoPag);
                guiaPagamento.DataComprovPag = request.dataComprovativoPag;
                guiaPagamento.ComprovativoPag = doc;

                _unitOfWork.Commit();

                var contaCorrente = _unitOfWork.ContaCorrenteRepository.Get(guiaPagamento.ContaCorrenteId);

                var valorAPagar = guiaPagamento.Valor + contaCorrente.ValorJuros - request.valorComprovativoPag;

                if (valorAPagar == 0)
                {
                    guiaPagamento.IndPago = indValidacao;
                }
                else if (valorAPagar > 0)
                {
                    guiaPagamento.IndPago = indValidacaoParcial;
                }
                else if (valorAPagar < 0)
                {
                    guiaPagamento.IndPago = indValidacao;
                    newReservaCredito = new Reservacredito
                    {
                        ReservaEntidadeFk = request.idEntidade,
                        Valor = -valorAPagar,
                        IndActivo = false,
                        ReservaGuiaPagamentoFk = guiaPagamento.IdGuia
                    };
                    newReservaCredito = _utils.SetDetailsToEntity(newReservaCredito);
                }
                guiaPagamento = _utils.UpdateDetailsToEntity(guiaPagamento);
                guiaPagamento.ValorComprovPag = Math.Min(guiaPagamento.Valor + (contaCorrente.ValorJuros ?? 0), request.valorComprovativoPag);
                guiaPagamento.ValorJurosFixo = contaCorrente.ValorJuros;
                guiaPagamento.BankCode = request.bankCode;


                _unitOfWork.GuiaPagamentoRepository.Update(guiaPagamento);

                contaCorrente.PagoEm = request.dataComprovativoPag;

                if (newReservaCredito != null)
                    _unitOfWork.ReservaCreditoRepository.Add(newReservaCredito);
                _unitOfWork.Commit();

                _unitOfWork.ContaCorrenteRepository.UpdateSituacaoPagamento(guiaPagamento.ContaCorrenteId);
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
                _unitOfWork.Rollback();
            }

            return response;
        }

        public ResponseBaseDataContract approveComprovativoPagamento(approveComprovativoPagamentoRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            var guiaPagamento = _unitOfWork.GuiaPagamentoRepository.Get(request.idGuia);
            Reservacredito newReservaCredito = null;

            if (guiaPagamento == null)
                response.Errors.Add(new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.EntityDoesNotExist).ToString(),
                    ErrorMessage = ErrorsDataContract.EntityDoesNotExist.ToString()
                });

            var pagamentoTypes = _unitOfWork.DominioRepository.getAllTiposDeDominio(TiposDominio.INDPAGO);
            var contaCorrentePagamentoTypes = _unitOfWork.DominioRepository.getAllTiposDeDominio(TiposDominio.SITUACAOPAGAMENTO);

            byte[] doc = Convert.FromBase64String(request.comprovativoPag);

            var indValidacao = (int)pagamentoTypes.Find(x => x.value == 1).id;
            var indValidacaoParcial = (int)pagamentoTypes.Find(x => x.value == 5).id;

            if (request.rejectStatus == 0)
            {
                guiaPagamento.RejectReason = request.rejectReason;
                indValidacao = (int)pagamentoTypes.Find(x => x.value == 6).id;
                indValidacaoParcial = (int)pagamentoTypes.Find(x => x.value == 6).id;
            }

            try
            {
                guiaPagamento.ValorComprovPag = Math.Min(guiaPagamento.Valor, request.valorComprovativoPag);
                guiaPagamento.DataComprovPag = request.dataComprovativoPag;
                guiaPagamento.ApproveFile = doc;

                _unitOfWork.Commit();

                var contaCorrente = _unitOfWork.ContaCorrenteRepository.Get(guiaPagamento.ContaCorrenteId);

                var valorAPagar = guiaPagamento.Valor + contaCorrente.ValorJuros - request.valorComprovativoPag;

                if (valorAPagar == 0)
                {
                    guiaPagamento.IndPago = indValidacao;
                }
                else if (valorAPagar > 0)
                {
                    guiaPagamento.IndPago = indValidacaoParcial;
                }
                else if (valorAPagar < 0)
                {
                    guiaPagamento.IndPago = indValidacao;
                    newReservaCredito = new Reservacredito
                    {
                        ReservaEntidadeFk = request.idEntidade,
                        Valor = -valorAPagar,
                        IndActivo = false,
                        ReservaGuiaPagamentoFk = guiaPagamento.IdGuia
                    };
                    newReservaCredito = _utils.SetDetailsToEntity(newReservaCredito);
                }
                guiaPagamento = _utils.UpdateDetailsToEntity(guiaPagamento);
                guiaPagamento.ValorComprovPag = Math.Min(guiaPagamento.Valor + (contaCorrente.ValorJuros ?? 0), request.valorComprovativoPag);
                guiaPagamento.ValorJurosFixo = contaCorrente.ValorJuros;


                _unitOfWork.GuiaPagamentoRepository.Update(guiaPagamento);

                contaCorrente.PagoEm = request.dataComprovativoPag;

                if (newReservaCredito != null)
                    _unitOfWork.ReservaCreditoRepository.Add(newReservaCredito);
                _unitOfWork.Commit();

                _unitOfWork.ContaCorrenteRepository.UpdateSituacaoPagamento(guiaPagamento.ContaCorrenteId);
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
                _unitOfWork.Rollback();
            }

            return response;
        }

        public RelatorioGuiaPagamentoListagemResponse GetGuiasPagamentoRelatorios(RelatorioGuiaPagamentoListagemRequest request)
        {
            RelatorioGuiaPagamentoListagemResponse response = new RelatorioGuiaPagamentoListagemResponse();

            // Validar se o utilizador tem as permissões necessárias
            bool permission = _utils.ValidatePermission((int)request.UserId, (int)ModuleRelatorios.Consultas, _unitOfWork);

            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            try
            {
                response = _unitOfWork.GuiaPagamentoRepository.GetGuiasPagamentoRelatorios(request);
            }
            catch (Exception e)
            {
                response.Errors = new List<Error> { new Error { ErrorCode = "-1", ErrorMessage = e.Message } };
            }

            return response;
        }
    }
}