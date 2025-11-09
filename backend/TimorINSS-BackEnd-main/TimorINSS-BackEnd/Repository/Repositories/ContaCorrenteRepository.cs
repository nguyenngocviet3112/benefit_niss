using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class ContaCorrenteRepository : IContaCorrenteRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ContaCorrenteRepository(TimorINSSModuloContribuicoesContext storeContext, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Contacorrente> GetAll()
        {
            _moduloContribuicoesContext.Contacorrente
               .Include(u => u.TipoDividaNavigation)
               .Include(u => u.SituacaoPagamentoNavigation)
               .ToList();

            return _moduloContribuicoesContext.Contacorrente;
        }

        public Contacorrente Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var contaCorrente = _moduloContribuicoesContext.Contacorrente
                .Include(u => u.SituacaoPagamentoNavigation)
                .Include(u => u.TipoDividaNavigation)
                .SingleOrDefault(u => u.IdContaCorrente == id);

            return contaCorrente;
        }

        public ContacorrenteDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var contaCorrente = _moduloContribuicoesContext.Contacorrente
                .SingleOrDefault(u => u.IdContaCorrente == id);

            ContacorrenteDto contaCorrenteDto = Utils.MappClassToDto<Contacorrente, ContacorrenteDto>(contaCorrente);
            return contaCorrenteDto;
        }

        public void Add(Contacorrente entity)
        {
            _moduloContribuicoesContext.Contacorrente.Add(entity);
        }

        public void Update(Contacorrente entity)
        {
            Contacorrente entityToUpdate = _moduloContribuicoesContext.Contacorrente
                .Single(d => d.IdContaCorrente == entity.IdContaCorrente);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Contacorrente entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public ContaCorrenteListagemResponse GetContaCorrenteByFilter(ContaCorrenteListagemRequest request)
        {
            ContaCorrenteListagemResponse result = new ContaCorrenteListagemResponse();

            DateTime begin = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
            if (request.filter.dateFilterBegin.HasValue)
            {
                var beginFullDate = request.filter.dateFilterBegin.Value;
                begin = new DateTime(beginFullDate.Year, beginFullDate.Month, 1);
            }

            int index = 0;
            if (request.filter.index.HasValue)
                index = request.filter.index.Value;

            int rows = 5;
            if (request.filter.rows.HasValue)
                rows = request.filter.rows.Value;
            const int SECRET_A = 511;
            const int SECRET_B = 2025;
            int decodedId = (request.IdEntidade - SECRET_B) / SECRET_A;

            IQueryable<Contacorrente> queryContaCorrenteConditional = _moduloContribuicoesContext.Contacorrente
                .Include(u => u.TipoDividaNavigation)
                .Include(u => u.SituacaoPagamentoNavigation)
                .Include(u => u.Guiapagamento);
                //.Include(u => u.GuiaPagamentoFkNavigation.IndPagoNavigation);

            switch (request.filter.filterField)
            {
                case "ENTIDADEEMPREGADORA":
                    if (request.filter.dateFilterBegin != null)
                    {
                        queryContaCorrenteConditional = queryContaCorrenteConditional.Where(u => (u.ContaCorrenteEntidadeFk == decodedId &&
                         u.ContaCorrenteTrabalhadorFk == null && begin.Year == u.MesAno.Year && begin.Month == u.MesAno.Month && u.IndActivo));
                        break;
                    }
                    else
                    {
                        queryContaCorrenteConditional = queryContaCorrenteConditional.Where(u => (u.ContaCorrenteEntidadeFk == decodedId &&
                        u.ContaCorrenteTrabalhadorFk == null && u.IndActivo));
                        break;
                    }

                case "TRABALHADOR":

                    if (request.filter.dateFilterBegin != null)
                    {
                        queryContaCorrenteConditional = queryContaCorrenteConditional.Where(u => (u.ContaCorrenteEntidadeFk == decodedId &&
                        u.ContaCorrenteTrabalhadorFk == request.IdTrabalhador && begin.Year == u.MesAno.Year && begin.Month == u.MesAno.Month && u.IndActivo));
                        break;
                    }
                    else
                    {
                        queryContaCorrenteConditional = queryContaCorrenteConditional.Where(u => (u.ContaCorrenteEntidadeFk == decodedId &&
                        u.ContaCorrenteTrabalhadorFk == request.IdTrabalhador && DateTime.Now.Year == u.MesAno.Year && u.IndActivo));
                        break;
                    }
                default:
                    result.Errors.Add(new Error
                    {
                        ErrorCode = ((int)ErrorsDataContract.InvalidFilter).ToString(),
                        ErrorMessage = ErrorsDataContract.InvalidFilter.ToString()
                    });
                    return result;
            }

            var estadosGerarGuia = new List<int>()
            {
                4, 5, 6
            };

            var contaCorrentes = queryContaCorrenteConditional
               .OrderBy("IdContaCorrente", OrderDirectionEnum.descending)
               //.OrderBy("MesAno", OrderDirectionEnum.descending)
               .Skip(index * rows)
               .Take(rows)
               .ToList()
               .Select(contaCorrente => new ContaCorrenteListagem
               {
                   IdContaCorrente = contaCorrente.IdContaCorrente,
                   IdEntidade = contaCorrente.ContaCorrenteEntidadeFk,
                   IdTrabalhador = contaCorrente.ContaCorrenteTrabalhadorFk,
                   TipoDivida = contaCorrente.TipoDividaNavigation.Descricao,
                   DataVencimento = contaCorrente.DataVencimento,
                   DataCriacao = contaCorrente.DataCriacao,
                   ValorEntidade = contaCorrente.ValorEntidade,
                   ValorTrabalhador = contaCorrente.ValorTrabalhador,
                   ValorTotal = contaCorrente.ValorTotal + (contaCorrente.ValorJuros ?? 0),
                   PagoEm = contaCorrente.PagoEm,
                   SituacaoPagamento = contaCorrente.SituacaoPagamentoNavigation.Valor,
                   MesAno = contaCorrente.MesAno,
                   ValorPago = contaCorrente.Guiapagamento.Sum(e => e.ValorComprovPag ?? 0),
                   //ValorPago = contaCorrente.GuiaPagamentoFkNavigation.ValorComprovPag,
                   //NumDocumento = contaCorrente.GuiaPagamentoFkNavigation.NumDocumento,
                   //Niss = contaCorrente.GuiaPagamentoFkNavigation.GuiaEntidadeFkNavigation.Niss,
                   JuroApurado = contaCorrente.ValorJuros,
                   GerarGuia = estadosGerarGuia.Contains(contaCorrente.SituacaoPagamentoNavigation.Valor) && !contaCorrente.Guiapagamento.Any(e => e.ValorComprovPag == null),
                   //TipoGuia = contaCorrente.GuiaPagamentoFkNavigation.TipoGuia,
                   //IndActivoGuiaPagamento = contaCorrente.GuiaPagamentoFkNavigation.IndActivo
               })
               .ToList();

            var totalNumber = queryContaCorrenteConditional.Count();

            ContaCorrenteListagemResponse response = new ContaCorrenteListagemResponse
            {
                contaCorrente = contaCorrentes,
                rows = totalNumber
            };
            return response;
        }

        public ResumoContaCorrenteListagemResponse GetResumoContaCorrente(ResumoContaCorrenteListagemRequest request)
        {
            ResumoContaCorrenteListagemResponse result = new ResumoContaCorrenteListagemResponse();

            var data = _moduloContribuicoesContext.Contacorrente
                .Include(u => u.TipoDividaNavigation)
                .Include(u => u.SituacaoPagamentoNavigation)
                //.Include(u => u.GuiaPagamentoFkNavigation.InverseGuiaPagamentoPaiNavigation)
                //.Include(u => u.GuiaPagamentoFkNavigation.TipoGuiaNavigation)
                .Where(e => e.ContaCorrenteEntidadeFk == request.IdEntidade)
                .Select(c => new ResumoContaCorrenteListagem
                {
                    Ano = c.MesAno.Year,                    // đảm bảo MesAno không null. Nếu DateTime? thì dùng c.MesAno!.Value.Year hoặc c.MesAno.HasValue ? c.MesAno.Value.Year : 0
                    Contribuicoes = c.ValorEntidade,             // nếu là decimal?
                    Quotizacoes = c.ValorTrabalhador,          // nếu là decimal?
                    ValorAPagar = c.ValorTotal,                // nếu là decimal?
                    TotalJuros = c.ValorJuros ?? 0,                // ✅ không dùng .Value
                    TotalPago = c.Guiapagamento.Sum(g => g.ValorComprovPag ?? 0)
                })
                .ToList()
                .GroupBy(e => e.Ano)
                .Select(a => new ResumoContaCorrenteListagem
                {
                    Ano = a.Key,
                    Contribuicoes = a.Sum(e => e.Contribuicoes),
                    Quotizacoes = a.Sum(e => e.Quotizacoes),
                    ValorAPagar = a.Sum(e => e.ValorAPagar),
                    TotalJuros = a.Sum(e => e.TotalJuros),
                    TotalPago = a.Sum(e => e.TotalPago),
                })
                .OrderByDescending(e => e.Ano)
                .ToList();

            result.data = data;
            return result;
        }

        public List<Contacorrente> GetContaCorrenteByData(DateTime dataVencimento)
        {
            DateTime begin = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
            if (dataVencimento != null)
            {
                var beginFullDate = dataVencimento;
                begin = new DateTime(beginFullDate.Year, beginFullDate.Month, 1);
            }

            var queryContaCorrente = _moduloContribuicoesContext.Contacorrente
                .Where(u => u.ContaCorrenteTrabalhadorFk == null && u.DataVencimento < dataVencimento && u.IndActivo)
                .Include(u => u.TipoDividaNavigation)
                .Include(u => u.SituacaoPagamentoNavigation)
                .Select(contaCorrente => new Contacorrente
                {
                    IdContaCorrente = contaCorrente.IdContaCorrente,
                    ContaCorrenteEntidadeFk = contaCorrente.ContaCorrenteEntidadeFk,
                    ContaCorrenteTrabalhadorFk = contaCorrente.ContaCorrenteTrabalhadorFk,
                    TipoDivida = contaCorrente.TipoDivida,
                    DataVencimento = contaCorrente.DataVencimento,
                    ValorEntidade = contaCorrente.ValorEntidade,
                    ValorTrabalhador = contaCorrente.ValorTrabalhador,
                    ValorTotal = contaCorrente.ValorTotal,
                    PagoEm = contaCorrente.PagoEm,
                    SituacaoPagamento = contaCorrente.SituacaoPagamento,
                    MesAno = contaCorrente.MesAno,
                    ValorJuros = contaCorrente.ValorJuros,
                    ContaCorrenteTaxaJuroFk = contaCorrente.ContaCorrenteTaxaJuroFk,
                    //GuiaPagamentoFk = contaCorrente.GuiaPagamentoFk,
                    DataCriacao = contaCorrente.DataCriacao,
                    UtilizadorCriacao = contaCorrente.UtilizadorCriacao
                });
            List<Contacorrente> contaCorrentes = queryContaCorrente
               .ToList();
            return contaCorrentes;
        }

        public bool IsEntidadeRegularizada(int entidade)
        {
            bool result = false;

            var pagoTarde = _moduloContribuicoesContext.Contacorrente
                .Include(c => c.SituacaoPagamentoNavigation)
                .Where(c => c.IndActivo && !c.SituacaoPagamentoNavigation.Descricao.Equals("Guia Paga")
                        && c.DataVencimento <= DateTime.Now.Date).Count();

            result = pagoTarde == 0;

            return result;
        }

        public List<ContasState> GetAllContasStatesFromYearByIdEntidade(int idEntidade, DateTime dateInicial, DateTime dateFinal)
        {
            var contas = _moduloContribuicoesContext.Contacorrente
                        .Include(c => c.SituacaoPagamentoNavigation)
                        //.Include(c => c.GuiaPagamentoFkNavigation.IndPagoNavigation)
                        .Where(c => c.ContaCorrenteEntidadeFk == idEntidade 
                        && c.MesAno.Year == dateInicial.Year && c.IndActivo
                        && c.MesAno.Month <= dateFinal.Month)
                        .Select(c => new ContasState
                        {
                            id = c.IdContaCorrente,
                            month = c.MesAno.Month,
                            state = c.SituacaoPagamentoNavigation.Descricao,
                            stateId = c.SituacaoPagamentoNavigation.Valor
                        })
                        .ToList();

            return contas;
        }

        public Contacorrente GetContaCorrenteByMesAnoAndEntidadeId(DateTime mesAno, int entidadeEmpregadoraID)
        {
            return _moduloContribuicoesContext.Declaracaoremuneracao
                  .Include(d => d.DeclaracaoRelEntidadeTrabalhadorFkNavigation)
                  .Include(d => d.ContaCorrenteFkNavigation)
                  .Where(d => d.DeclaracaoRelEntidadeTrabalhadorFkNavigation.EntidadeFk == entidadeEmpregadoraID
                  && d.MesAno == mesAno && d.IndActivo)
                  .Select(d => d.ContaCorrenteFkNavigation).FirstOrDefault();
        }

       
        public void UpdateCorrente(int contaCorrenteId)
        {
            var contaCorrente = _moduloContribuicoesContext.Contacorrente.FirstOrDefault(e => e.IdContaCorrente == contaCorrenteId);

            if (contaCorrente == null) return;
            contaCorrente.DataCriacao = DateTime.Now.Date;
            _moduloContribuicoesContext.SaveChanges();
        }


        public void UpdateSituacaoPagamento(int contaCorrenteId)
        {
            var contaCorrente = _moduloContribuicoesContext.Contacorrente.FirstOrDefault(e => e.IdContaCorrente == contaCorrenteId);

            if (contaCorrente == null) return;

            var contaPagStatus = _moduloContribuicoesContext.Dominio.Where(e => e.Dominio1 == TiposDominio.SITUACAOPAGAMENTO.ToString());

            // Estado de Guia Paga
            var guiaPagaStatus = contaPagStatus.First(e => e.Valor == 1).IdDominio;

            if (contaCorrente.SituacaoPagamento == guiaPagaStatus) return;


            var guias = _moduloContribuicoesContext.Guiapagamento.Include(e => e.Movimentosporconciliar.Where(a => a.IndActivo == true))
                                                                 .Where(e => e.ContaCorrenteId == contaCorrenteId)
                                                                 .ToList();

            var guiasPagStatus = _moduloContribuicoesContext.Dominio.Where(e => e.Dominio1 == TiposDominio.INDPAGO.ToString());

            var todosConciliados = !guias.Any(e => !e.Movimentosporconciliar.Any());
            
            var tudoPago = contaCorrente.ValorTotal + (contaCorrente.ValorJuros ?? 0) == guias.Sum(e => e.ValorComprovPag);

            if (guias.Count == 0)
            {
                // Estado de Sem Guia Gerada
                var semGuiaStatus = contaPagStatus.First(e => e.Valor == 4).IdDominio;

                if (contaCorrente.SituacaoPagamento != semGuiaStatus)
                {
                    contaCorrente.SituacaoPagamento = semGuiaStatus;
                }
                else return;
            }
            else if (todosConciliados)
            {
                if (tudoPago)
                {
                    if (contaCorrente.SituacaoPagamento != guiaPagaStatus)
                    {
                        contaCorrente.SituacaoPagamento = guiaPagaStatus;
                    }
                    else return;
                }
                else
                {
                    // Estado de Guia Parcialmente Paga
                    var guiaParcialPagaStatus = contaPagStatus.First(e => e.Valor == 6).IdDominio;

                    if (contaCorrente.SituacaoPagamento != guiaParcialPagaStatus)
                    {
                        contaCorrente.SituacaoPagamento = guiaParcialPagaStatus;
                    }
                    else return;
                }
            }
            else
            {
                if (tudoPago)
                {
                    // Estado de Comprovativo em Validação
                    var guiaValidacaoPagaStatus = contaPagStatus.First(e => e.Valor == 3).IdDominio;

                    if (contaCorrente.SituacaoPagamento != guiaValidacaoPagaStatus)
                    {
                        contaCorrente.SituacaoPagamento = guiaValidacaoPagaStatus;
                    }
                    else return;
                }
                else if (guias.Any(e => e.ValorComprovPag != null))
                {
                    // Estado de Comprovativo Parcial em Validação
                    var guiaParcialPagaStatus = contaPagStatus.First(e => e.Valor == 5).IdDominio;

                    if (contaCorrente.SituacaoPagamento != guiaParcialPagaStatus)
                    {
                        contaCorrente.SituacaoPagamento = guiaParcialPagaStatus;
                    }
                    else return;
                }
                else
                {
                    // Estado de Guia Gerada
                    var guiaGeradaStatus = contaPagStatus.First(e => e.Valor == 2).IdDominio;

                    if (contaCorrente.SituacaoPagamento != guiaGeradaStatus)
                    {
                        contaCorrente.SituacaoPagamento = guiaGeradaStatus;
                    }
                    else return;

                }
            }

            _moduloContribuicoesContext.SaveChanges();
        }
    }
}