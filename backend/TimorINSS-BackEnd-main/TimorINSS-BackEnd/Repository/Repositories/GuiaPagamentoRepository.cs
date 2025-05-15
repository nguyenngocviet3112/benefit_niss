using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class GuiaPagamentoRepository : IGuiaPagamentoRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public GuiaPagamentoRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Guiapagamento> GetAll()
        {
            return _moduloContribuicoesContext.Guiapagamento.ToList();
        }

        public Guiapagamento Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var guiaPagamento = _moduloContribuicoesContext.Guiapagamento
                .SingleOrDefault(u => u.IdGuia == id);

            return guiaPagamento;
        }

        public GuiapagamentoDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var guiaPagamento = _moduloContribuicoesContext.Guiapagamento
                .SingleOrDefault(u => u.IdGuia == id);

            GuiapagamentoDto guiaPagamentoDto = Utils.MappClassToDto<Guiapagamento, GuiapagamentoDto>(guiaPagamento);
            return guiaPagamentoDto;
        }

        public void Add(Guiapagamento entity)
        {
            _moduloContribuicoesContext.Guiapagamento.Add(entity);
        }

        public void Update(Guiapagamento entity)
        {
            Guiapagamento entityToUpdate = _moduloContribuicoesContext.Guiapagamento
                .Single(d => d.IdGuia == entity.IdGuia);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Guiapagamento entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public string GetNextNumDocumento()
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            int numDocumento = 0;
            int anoDocumento = 0;
            Guiapagamento guiaPagamento = _moduloContribuicoesContext.Guiapagamento.OrderByDescending(p => p.IdGuia).FirstOrDefault();

            if (guiaPagamento != null)
            {
                // O formato do documento é numero/ano. ex: 1/2020
                // Substring para obter apenas o número do documento
                numDocumento = int.Parse(guiaPagamento.NumDocumento[0..^5]) + 1;
                // Substring para obter apenas o ano do documento
                anoDocumento = int.Parse(guiaPagamento.NumDocumento.Substring(guiaPagamento.NumDocumento.Length - 4, 4));
            }

            string numDocumentoFinal = numDocumento.ToString() + "/" + DateTime.Now.Year.ToString();

            if (anoDocumento != DateTime.Now.Year)
            {
                numDocumentoFinal = 1 + "/" + DateTime.Now.Year.ToString();
            }

            return numDocumentoFinal;
        }

       

        public GuiaListagemResponse getGuiasByFilter(GetAllGuiasStatesFromYearByFilterRequest request)
        {
            var utilizador = _moduloContribuicoesContext.Utilizador
                .SingleOrDefault(u => u.UtilizadorEntidadeFk == request.idEntidade);
            if (utilizador == null)
            {
                GuiaListagemResponse result1 = new GuiaListagemResponse
                {
                    guias = null,
                    rows = 0
                };
                return result1;
            }    
                //throw new Exception(ErrorsDataContract.EntityDoesNotExist.ToString());

            var trabalhador = _moduloContribuicoesContext.Trabalhador
                .SingleOrDefault(u => u.IdTrabalhador == utilizador.TrabalhadorFk);
            //if (trabalhador == null)
                if (trabalhador == null)
                {
                    GuiaListagemResponse result1 = new GuiaListagemResponse
                    {
                        guias = null,
                        rows = 0
                    };
                    return result1;
                }
            //throw new Exception(ErrorsDataContract.TrabalhadorDoesNotExist.ToString());

            var entidadeempregadora = _moduloContribuicoesContext.Entidadeempregadora
            .SingleOrDefault(u => u.IdEntidadeEmpreg == request.idEntidade);
            if (entidadeempregadora == null)
            {
                GuiaListagemResponse result1 = new GuiaListagemResponse
                {
                    guias = null,
                    rows = 0
                };
                return result1;
            }

            var filter = request.filter;
            if (filter == null || string.IsNullOrWhiteSpace(filter.filterBy) && filter.filterField != null)
                throw new Exception(ErrorsDataContract.FilterDoesNotExist.ToString());


            int index = 0;
            if (filter.index.HasValue)
                index = filter.index.Value;

            int rows = 5;
            if (filter.rows.HasValue)
                rows = filter.rows.Value;

            IQueryable<Guiapagamento> query = _moduloContribuicoesContext.Guiapagamento
                .Include(e => e.GuiaEntidadeFkNavigation)
                .Where(u => u.GuiaEntidadeFk == request.idEntidade && u.IndActivo == true);

            // Filtrar por data
            if (filter.dateFilterBegin != null)
            {
                query = query.Where(u => u.MesAno.Month == filter.dateFilterBegin.Value.Month && u.MesAno.Year == filter.dateFilterBegin.Value.Year);
            }
            //else
            //{
            //    query = query.Where(u => u.MesAno.Year == DateTime.Now.Year);
            //}

            if (filter.filterField != null)
            {
                query = filter.filterField switch
                {
                    // Filtrar por estado
                    "Estado" => query.Where(u => u.IndPagoNavigation.Valor.Equals(int.Parse(filter.filterBy))),
                    // Filtrar por tipo
                    "Tipo" => query.Where(u => u.TipoGuiaNavigation.Valor.Equals(int.Parse(filter.filterBy))),
                    _ => throw new Exception(ErrorsDataContract.FilterDoesNotExist.ToString()),
                };
            }

            if (filter.filter != null && (filter.filter?.filterField != null))
            {
                query = filter.filter.filterField switch
                {
                    // Filtrar por estado
                    "Estado" => query.Where(u => u.IndPagoNavigation.Valor.Equals(int.Parse(filter.filter.filterBy))),
                    // Filtrar por tipo
                    "Tipo" => query.Where(u => u.TipoGuiaNavigation.Valor.Equals(int.Parse(filter.filter.filterBy))),
                    _ => throw new Exception(ErrorsDataContract.FilterDoesNotExist.ToString()),
                };
            }

            var guias = new List<GuiaListagem>();
            var totalNumber = 0;
            if (query != null)
            {
                totalNumber = query.Count();
                guias = query
                    .Select(e => new GuiaListagem
                    {
                        idGuia = e.IdGuia,
                        numDocumento = e.NumDocumento,
                        mesAno = e.MesAno,
                        descricao = e.Descricao,
                        valor = e.Valor,
                        juros = (decimal)(e.ValorJurosFixo != null ? e.ValorJurosFixo : (e.ValorJuros ?? 0)),
                        total = e.Valor + (decimal)(e.ValorJurosFixo != null ? e.ValorJurosFixo : (e.ValorJuros ?? 0)),
                        dtValidade = e.DtValidade,
                        tipo = e.TipoGuiaNavigation.Valor,
                        valorPago = e.ValorComprovPag ?? 0,
                        dtValorPago = e.DataComprovPag,
                        //comprovativoPagamento = e.ComprovativoPag,
                        niss = e.GuiaEntidadeFkNavigation.Niss,
                        estadoPagamento = e.IndPagoNavigation.Valor,
                        userName = entidadeempregadora.Nome,
                        tin = trabalhador.Tin,
                        qrInvoice = e.QrInvoice,
                        //paymentRef = e.GuiaEntidadeFkNavigation.Niss + DateTime.Now.ToString("MMyyyy") + "01",
                        paymentRef = e.PaymentRef,
                        bankCode = e.BankCode,
                        dataCriacao = e.DataCriacao
                    })
                    .OrderBy("idGuia", OrderDirectionEnum.descending)
                    //.OrderBy("mesAno", OrderDirectionEnum.descending)
                    .Skip(index * rows)
                    .Take(rows)
                    .ToList();
            }
            GuiaListagemResponse result = new GuiaListagemResponse
            {
                guias = guias,
                rows = totalNumber
            };
            return result;
        }

        public GuiaListagemResponse getGuiasPagamentoByFilter(GetGuiaPagamentoRequest request)
        {
            

         

            IQueryable<Guiapagamento> query = _moduloContribuicoesContext.Guiapagamento
                .Include(e => e.GuiaEntidadeFkNavigation)
                .Where(u => u.IdGuia == request.idGuiaPagamento);


            var guias = new List<GuiaListagem>();
            var totalNumber = 0;
            if (query != null)
            {
                totalNumber = query.Count();
                guias = query
                    .Select(e => new GuiaListagem
                    {
                        idGuia = e.IdGuia,
                        numDocumento = e.NumDocumento,
                        mesAno = e.MesAno,
                        descricao = e.Descricao,
                        valor = e.Valor,
                        juros = (decimal)(e.ValorJurosFixo != null ? e.ValorJurosFixo : (e.ValorJuros ?? 0)),
                        total = e.Valor + (decimal)(e.ValorJurosFixo != null ? e.ValorJurosFixo : (e.ValorJuros ?? 0)),
                        dtValidade = e.DtValidade,
                        tipo = e.TipoGuiaNavigation.Valor,
                        valorPago = e.ValorComprovPag ?? 0,
                        dtValorPago = e.DataComprovPag,
                        niss = e.GuiaEntidadeFkNavigation.Niss,
                        estadoPagamento = e.IndPagoNavigation.Valor,
                        comprovativoPagamento = e.ComprovativoPag,
                        aprpoveFile = e.ApproveFile,
                        qrInvoice = e.QrInvoice,
                        paymentRef = e.PaymentRef,
                        bankCode = e.BankCode,
                        dataCriacao = e.DataCriacao
                    })
                    .OrderBy("idGuia", OrderDirectionEnum.descending)
                    .ToList();
            }
            GuiaListagemResponse result = new GuiaListagemResponse
            {
                guias = guias,
                rows = totalNumber
            };
            return result;
        }

        public GuiaListagemResponse getGuiasAporoveByFilter(GetAllGuiasStatesFromDateByFilterRequest request)
        {


            var filter = request.filter;
            if (filter == null || string.IsNullOrWhiteSpace(filter.filterBy) && filter.filterField != null)
                throw new Exception(ErrorsDataContract.FilterDoesNotExist.ToString());


            int index = 0;
            if (filter.index.HasValue)
                index = filter.index.Value;

            int rows = 5;
            if (filter.rows.HasValue)
                rows = filter.rows.Value;

            IQueryable<Guiapagamento> query = _moduloContribuicoesContext.Guiapagamento
                .Include(e => e.GuiaEntidadeFkNavigation)
                .Where(u => u.IndActivo == true);
            //.Where(u => u.GuiaEntidadeFk == request.idEntidade && u.IndActivo == true);

            // Filtrar por data
            if (filter.dateFilterBegin != null)
            {
                query = query.Where(u => u.MesAno.Month == filter.dateFilterBegin.Value.Month && u.MesAno.Year == filter.dateFilterBegin.Value.Year);
            }
            if (request.niss != null && !string.IsNullOrEmpty(request.niss))
            {
                query = query.Where(u => u.GuiaEntidadeFkNavigation.Niss == request.niss);
            }
            if (request.bankCode != null && !string.IsNullOrEmpty(request.bankCode))
            {
                query = query.Where(u => u.BankCode == request.bankCode);
            }
            if (request.paymentRef != null && !string.IsNullOrEmpty(request.paymentRef))
            {
                query = query.Where(u => u.PaymentRef == request.paymentRef);
            }
            //else
            //{
            //    query = query.Where(u => u.MesAno.Year == DateTime.Now.Year);
            //}

            if (filter.filterField != null)
            {
                query = filter.filterField switch
                {
                    // Filtrar por estado
                    "Estado" => query.Where(u => u.IndPagoNavigation.Valor.Equals(int.Parse(filter.filterBy))),
                    // Filtrar por tipo
                    "Tipo" => query.Where(u => u.TipoGuiaNavigation.Valor.Equals(int.Parse(filter.filterBy))),
                    _ => throw new Exception(ErrorsDataContract.FilterDoesNotExist.ToString()),
                };
            }  else {
                query = query.Where(u => !u.IndPagoNavigation.Valor.Equals(2));
            }

            if (filter.filter != null && (filter.filter?.filterField != null))
            {
                query = filter.filter.filterField switch
                {
                    // Filtrar por estado
                    "Estado" => query.Where(u => u.IndPagoNavigation.Valor.Equals(int.Parse(filter.filter.filterBy))),
                    // Filtrar por tipo
                    "Tipo" => query.Where(u => u.TipoGuiaNavigation.Valor.Equals(int.Parse(filter.filter.filterBy))),
                    _ => throw new Exception(ErrorsDataContract.FilterDoesNotExist.ToString()),
                };
            } else {
                query = query.Where(u => !u.IndPagoNavigation.Valor.Equals(2));
            }
            var guias = new List<GuiaListagem>();
            var totalNumber = 0;
            if (query != null)
            {
                totalNumber = query.Count();
                guias = query
                    .Select(e => new GuiaListagem
                    {
                        idGuia = e.IdGuia,
                        numDocumento = e.NumDocumento,
                        mesAno = e.MesAno,
                        descricao = e.Descricao,
                        valor = e.Valor,
                        juros = (decimal)(e.ValorJurosFixo != null ? e.ValorJurosFixo : (e.ValorJuros ?? 0)),
                        total = e.Valor + (decimal)(e.ValorJurosFixo != null ? e.ValorJurosFixo : (e.ValorJuros ?? 0)),
                        dtValidade = e.DtValidade,
                        tipo = e.TipoGuiaNavigation.Valor,
                        valorPago = e.ValorComprovPag ?? 0,
                        dtValorPago = e.DataComprovPag,
                        //comprovativoPagamento = e.ComprovativoPag,
                        niss = e.GuiaEntidadeFkNavigation.Niss,
                        estadoPagamento = e.IndPagoNavigation.Valor,
                        //userName = utilizador.Username,
                        //tin = trabalhador.Tin,
                        qrInvoice = e.QrInvoice,
                        dataCriacao = e.DataCriacao,
                        //paymentRef = e.GuiaEntidadeFkNavigation.Niss + DateTime.Now.ToString("MMyyyy") + "01",
                        paymentRef = e.PaymentRef,
                        bankCode = e.BankCode
                    })
                    .OrderBy("idGuia", OrderDirectionEnum.descending)
                    //.OrderBy("mesAno", OrderDirectionEnum.descending)
                    .Skip(index * rows)
                    .Take(rows)
                    .ToList();
            }
            GuiaListagemResponse result = new GuiaListagemResponse
            {
                guias = guias,
                rows = totalNumber
            };
            return result;
        }

        public List<Guiapagamento> GetAllGuiasAtrasadas(long geradaStateId)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var guiasPagamento = _moduloContribuicoesContext.Guiapagamento
                .Where(u => u.DtValidade < DateTime.Now && u.IndActivo && u.IndPago == geradaStateId)
                .ToList();

            return guiasPagamento;
        }

        public List<Guiapagamento> GetGuiasExpiradasPorMesAno(int mes, int ano)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var guiasPagamento = _moduloContribuicoesContext.Guiapagamento
                .Where(u => u.DtEmissao.Month == mes && u.DtEmissao.Year == ano && !u.IndActivo)
                .ToList();

            return guiasPagamento;
        }

        public decimal SumValorPago(int idContaCorrente)
        {
            return _moduloContribuicoesContext.Guiapagamento.Where(e => e.ContaCorrenteId == idContaCorrente && e.IndActivo)
                                                            .Sum(e => e.ValorComprovPag ?? 0);
        }

        //public Guiapagamento GetUltimoGuiaFilho(int? idGuiaPai)
        //{
        //    if (idGuiaPai != null && idGuiaPai > 0)
        //    {
        //        var guiaPagamento = _moduloContribuicoesContext.Guiapagamento
        //            .Where(u => u.GuiaPagamentoPai == idGuiaPai)
        //            .OrderByDescending(o => o.IdGuia)
        //            .FirstOrDefault();

        //        if (guiaPagamento != null)
        //        {
        //            return guiaPagamento;
        //        }
        //    }
        //    return null;
        //}

        public List<Guiapagamento> GetGuiasByIds(List<int> idGuias)
        {
            return _moduloContribuicoesContext.Guiapagamento.Where(u => idGuias.Contains(u.IdGuia) && u.IndActivo).ToList();
        }

        public RelatorioGuiaPagamentoListagemResponse GetGuiasPagamentoRelatorios(RelatorioGuiaPagamentoListagemRequest request)
        {
            RelatorioGuiaPagamentoListagemResponse response = new RelatorioGuiaPagamentoListagemResponse();

            int index = 0;
            if (request.filter.index.HasValue)
                index = request.filter.index.Value;

            int rows = 5;
            if (request.filter.rows.HasValue)
                rows = request.filter.rows.Value;

            var beginDate = request.filter.dateFilterBegin;
            var endDate = request.filter.dateFilterEnd;

            var listGuias = _moduloContribuicoesContext.Guiapagamento
                .Include(e => e.GuiaEntidadeFkNavigation)
               .Where(e => e.IndActivo &&
                           // Filtrar por número do documento
                           (request.numeroGuia == null || request.numeroGuia == e.NumDocumento) &&
                           // Filtrar por NISS
                           (request.niss == null || request.niss == e.GuiaEntidadeFkNavigation.Niss) &&
                           // Filtrar por estado
                           (!request.estado.HasValue || e.IndPago == request.estado) &&
                           // Filtrar por datas
                           (beginDate.HasValue && endDate.HasValue ? e.DtEmissao.Date >= beginDate && e.DtEmissao.Date <= endDate :
                            beginDate.HasValue && !endDate.HasValue ? e.DtEmissao.Date == beginDate :
                            true)
                )
               .Select(e => new RelatorioGuiaPagamentoDataContract
               {
                   numeroGuia = e.NumDocumento,
                   empregador = e.GuiaEntidadeFkNavigation.Nome,
                   periodo = e.DtEmissao,
                   estado = e.IndPagoNavigation.Descricao,
                   valorGuia = e.Valor + (e.ValorJuros ?? 0),
                   valorComprovativo = e.ValorComprovPag ?? 0,
                   pdf = e.ComprovativoPag,
                   valorDivida = e.Valor - (e.ValorComprovPag ?? 0)
               });

            var guias = listGuias
                .OrderBy(request.filter.orderBy, request.filter.orderDirection)
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = listGuias.Count();
            response.rows = totalNumber;
            response.guias = guias;

            return response;
        }
    }
}