using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;
using TimorINSSBackEnd.Resources;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class MovimentosPorConciliarRepository : IMovimentosPorConciliarRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public MovimentosPorConciliarRepository(TimorINSSModuloContribuicoesContext storeContext, IStringLocalizer<SharedResource> localizer)
        {
            _moduloContribuicoesContext = storeContext;
            _localizer = localizer;
        }

        public IEnumerable<Movimentosporconciliar> GetAll()
        {
            return _moduloContribuicoesContext.Movimentosporconciliar
                .Include(u => u.MovimentoBancarioFkNavigation)
                .Include(u => u.TarefaAtivoFkNavigation)
                .Where(u => u.IndActivo.Value)
                .ToList();
        }

        public Movimentosporconciliar Get(long id)
        {
            var movimento = _moduloContribuicoesContext.Movimentosporconciliar
                .Include(u => u.MovimentoBancarioFkNavigation)
                .Include(u => u.TarefaAtivoFkNavigation)
                .SingleOrDefault(u => u.Id == id);

            return movimento;
        }

        public MovimentosPorConciliarDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;
            var movimento = _moduloContribuicoesContext.Movimentosporconciliar
                .Include(u => u.MovimentoBancarioFkNavigation)
                .Include(u => u.TarefaAtivoFkNavigation)
                .SingleOrDefault(u => u.Id == id && u.IndActivo.Value);

            MovimentosPorConciliarDto movimentoDto = Utils.MappClassToDto<Movimentosporconciliar, MovimentosPorConciliarDto>(movimento);
            return movimentoDto;
        }

        public void Add(Movimentosporconciliar entity)
        {
            _moduloContribuicoesContext.Movimentosporconciliar.Add(entity);
        }

        public void AddRelation(RelMovimentosporconciliarMovimentos entity)
        {
            _moduloContribuicoesContext.RelMovimentosporconciliarMovimentos.Add(entity);
        }

        public RelMovimentosporconciliarMovimentos CreateRelationObject(int movimentoBancario, MovimentosAConciliar movimentoAConciliar, int idDominio)
        {
            return new RelMovimentosporconciliarMovimentos()
            {
                MovimentosBancariosFk = movimentoBancario,
                GuiaPagamentoFk = movimentoAConciliar.Type == MovimentosPorConciliarListagemType.GuiaPagamento ? movimentoAConciliar.Id : (int?)null,
                PagamentosExecutadosFk = movimentoAConciliar.Type == MovimentosPorConciliarListagemType.PagamentoExecutado ? movimentoAConciliar.Id : (int?)null,
                MovimentoPorConciliarFk = movimentoAConciliar.Type == MovimentosPorConciliarListagemType.MovimentoAConciliar ? movimentoAConciliar.Id : (int?)null,
                ReservaCreditoFk = movimentoAConciliar.Type == MovimentosPorConciliarListagemType.ReservaCredito ? movimentoAConciliar.Id : (int?)null,
                Estado = idDominio
            };
        }

        public void Update(Movimentosporconciliar entity)
        {
            Movimentosporconciliar entityToUpdate = _moduloContribuicoesContext.Movimentosporconciliar
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Movimentosporconciliar entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public MovimentosPorConciliar GetListagem(MovimentosPorConciliarListagemRequest request)
        {
            MovimentosPorConciliar response = new MovimentosPorConciliar { RequestId = request.RequestId };

            int index = 0;
            if (request.filter.index.HasValue)
                index = request.filter.index.Value;

            int rows = 5;
            if (request.filter.rows.HasValue)
                rows = request.filter.rows.Value;

            var beginDate = request.filter.dateFilterBegin;
            var endDate = request.filter.dateFilterEnd;

            var listaMovimentosPorConciliar = _moduloContribuicoesContext.Movimentosporconciliar
               .Include(e => e.RelMovimentosporconciliarMovimentos)
               .Include(e => e.MovimentoBancarioFkNavigation)
               .Where(e => e.IndActivo.Value && e.IsReceita == request.IsReceita && e.MovimentoBancarioFk != null &&
                    (request.TarefaAtivoId == null || e.TarefaAtivoFk == request.TarefaAtivoId) &&
                    // Filtrar por descrição
                    (request.filter.filterBy == null || e.MovimentoBancarioFkNavigation.Descricao.Contains(request.filter.filterBy)) &&
                    // Filtrar por Todos / Conciliados / Não conciliados
                    (request.FiltroConciliado == FiltroConciliadoEnum.Todos || (request.FiltroConciliado == FiltroConciliadoEnum.Conciliados && e.RelMovimentosporconciliarMovimentos.Any(a => a.IndActivo.Value)) || (request.FiltroConciliado == FiltroConciliadoEnum.NaoConciliados && !e.RelMovimentosporconciliarMovimentos.Any(a => a.IndActivo.Value))) &&
                    // Filtrar por datas
                    (beginDate.HasValue && endDate.HasValue ? e.DataCriacao >= beginDate && e.DataCriacao <= endDate :
                     beginDate.HasValue && !endDate.HasValue ? e.DataCriacao == beginDate :
                     true) &&
                    // Movimento manual, không có Bank riêng — chỉ hiện khi không lọc theo Bank
                    request.BankCode == null
               )
               .Select(e => new MovimentosPorConciliarListagem
               {
                   id = e.Id,
                   descricao = new MovimentosPorConciliarDescricao()
                   {
                       id = e.MovimentoBancarioFkNavigation.Id,
                       descricao = e.MovimentoBancarioFkNavigation.Descricao,
                   },
                   comprovativo = e.Comprovativo,
                   numeroDocumento = e.NumeroDocumento,
                   valor = e.Valor,
                   conciliado = e.RelMovimentosporconciliarMovimentos.Any(),
                   type = MovimentosPorConciliarListagemType.MovimentoAConciliar,
                   editavel = true,
                   isClassificada = e.CodigoContaCreditoFk != null,
               });

            if (request.IsReceita)
            {
                listaMovimentosPorConciliar = listaMovimentosPorConciliar.Concat(
                    _moduloContribuicoesContext.Guiapagamento
                    .Include(e => e.RelMovimentosporconciliarMovimentos)
                    .Include(e => e.Movimentosporconciliar)
                    .Where(e => e.IndActivo &&
                        e.ValorComprovPag.HasValue &&
                        // Filtrar por descrição
                        (request.filter.filterBy == null || e.Descricao.Contains(request.filter.filterBy)) &&
                        // Filtrar por Todos / Conciliados / Não conciliados
                        (request.FiltroConciliado == FiltroConciliadoEnum.Todos || (request.FiltroConciliado == FiltroConciliadoEnum.Conciliados && e.RelMovimentosporconciliarMovimentos.Any(a => a.IndActivo.Value)) || (request.FiltroConciliado == FiltroConciliadoEnum.NaoConciliados && !e.RelMovimentosporconciliarMovimentos.Any(a => a.IndActivo.Value))) &&
                        // Filtrar por datas
                        (beginDate.HasValue && endDate.HasValue ? e.DataCriacao >= beginDate && e.DataCriacao <= endDate :
                         beginDate.HasValue && !endDate.HasValue ? e.DataCriacao == beginDate :
                         true) &&
                        // Filtrar por Bank — phải khớp/đồng bộ với Guia Pagamento/Invoice
                        (request.BankCode == null || e.BankCode == request.BankCode)
                    )
                    .Select(e => new MovimentosPorConciliarListagem
                    {
                        id = e.IdGuia,
                        descricao = new MovimentosPorConciliarDescricao()
                        {
                            id = null,
                            descricao = e.Descricao,
                        },
                        comprovativo = e.ComprovativoPag,
                        numeroDocumento = e.NumDocumento,
                        valor = e.ValorComprovPag.Value,
                        conciliado = e.RelMovimentosporconciliarMovimentos.Any(),
                        type = MovimentosPorConciliarListagemType.GuiaPagamento,
                        editavel = false,
                        isClassificada = e.Movimentosporconciliar.Any(a => a.IndActivo == true && a.CodigoContaCreditoFk != null),
                        bankCode = e.BankCode,
                    })
               );
                listaMovimentosPorConciliar = listaMovimentosPorConciliar.Concat(
                    _moduloContribuicoesContext.Reservacredito
                    .Include(e => e.ReservaGuiaPagamentoFkNavigation)
                    .Include(e => e.RelMovimentosporconciliarMovimentos)
                    .Include(e => e.Movimentosporconciliar)
                    .Where(e => !e.IndActivo &&
                        // Filtrar por descrição
                        (request.filter.filterBy == null || e.ReservaGuiaPagamentoFkNavigation.Descricao.Contains(request.filter.filterBy)) &&
                        // Filtrar por Todos / Conciliados / Não conciliados
                        (request.FiltroConciliado == FiltroConciliadoEnum.Todos || (request.FiltroConciliado == FiltroConciliadoEnum.Conciliados && e.RelMovimentosporconciliarMovimentos.Any(a => a.IndActivo.Value)) || (request.FiltroConciliado == FiltroConciliadoEnum.NaoConciliados && !e.RelMovimentosporconciliarMovimentos.Any(a => a.IndActivo.Value))) &&
                        // Filtrar por datas
                        (beginDate.HasValue && endDate.HasValue ? e.DataCriacao >= beginDate && e.DataCriacao <= endDate :
                         beginDate.HasValue && !endDate.HasValue ? e.DataCriacao == beginDate :
                         true) &&
                        // Nota de crédito thừa kế Bank từ Guia gốc
                        (request.BankCode == null || e.ReservaGuiaPagamentoFkNavigation.BankCode == request.BankCode)
                    )
                    .Select(e => new MovimentosPorConciliarListagem
                    {
                        id = e.IdReserva,
                        descricao = new MovimentosPorConciliarDescricao()
                        {
                            id = null,
                            descricao = e.ReservaGuiaPagamentoFkNavigation.Descricao + " - " + _localizer["notaCredito"].Value,
                        },
                        comprovativo = e.ReservaGuiaPagamentoFkNavigation.ComprovativoPag,
                        numeroDocumento = e.ReservaGuiaPagamentoFkNavigation.NumDocumento,
                        valor = e.Valor.Value,
                        conciliado = e.RelMovimentosporconciliarMovimentos.Any(),
                        type = MovimentosPorConciliarListagemType.ReservaCredito,
                        editavel = false,
                        isClassificada = e.Movimentosporconciliar.Any(a => a.IndActivo == true && a.CodigoContaCreditoFk != null),
                        bankCode = e.ReservaGuiaPagamentoFkNavigation.BankCode,
                    })
               );
            }
            else if (!request.IsReceita)
            {
                listaMovimentosPorConciliar = listaMovimentosPorConciliar.Concat(
                    _moduloContribuicoesContext.Pagamentosexecutados
                    .Include(e => e.CompromissoFkNavigation)
                    .ThenInclude(e => e.ComponenteDespesaRegistoFkNavigation)
                    .Include(e => e.RelMovimentosporconciliarMovimentos)
                    .Where(e => e.IndActivo && e.EstadoNavigation.Dominio1 == TiposDominio.ESTADOPAGAMENTO.ToString() && e.EstadoNavigation.Valor == 2 &&
                        (request.TarefaAtivoId == null || e.ProcessoAtivoFkNavigation.Tarefaativo.Any(a => a.Id == request.TarefaAtivoId)) &&
                        // Filtrar por descrição
                        (request.filter.filterBy == null || e.CompromissoFkNavigation.ComponenteDespesaRegistoFkNavigation.Descricao.Contains(request.filter.filterBy)) &&
                        // Filtrar por Todos / Conciliados / Não conciliados
                        (request.FiltroConciliado == FiltroConciliadoEnum.Todos || (request.FiltroConciliado == FiltroConciliadoEnum.Conciliados && e.RelMovimentosporconciliarMovimentos.Any(a => a.IndActivo.Value)) || (request.FiltroConciliado == FiltroConciliadoEnum.NaoConciliados && !e.RelMovimentosporconciliarMovimentos.Any(a => a.IndActivo.Value))) &&
                        // Filtrar por datas
                        (beginDate.HasValue && endDate.HasValue ? e.DataCriacao >= beginDate && e.DataCriacao <= endDate :
                         beginDate.HasValue && !endDate.HasValue ? e.DataCriacao == beginDate :
                         true)
                    )
                    .Select(e => new MovimentosPorConciliarListagem
                    {
                        id = e.Id,
                        descricao = new MovimentosPorConciliarDescricao()
                        {
                            id = null,
                            descricao = e.CompromissoFkNavigation.ComponenteDespesaRegistoFkNavigation.Descricao,
                        },
                        comprovativo = null,
                        numeroDocumento = e.NumeroPagamento,
                        valor = -e.ValorExecutado,
                        conciliado = e.RelMovimentosporconciliarMovimentos.Any(),
                        type = MovimentosPorConciliarListagemType.PagamentoExecutado,
                        editavel = false,
                        isClassificada = e.CodigoContaCreditoFk != null,
                    })
                );
            }

            var movimentosPorConciliar = listaMovimentosPorConciliar
                .OrderBy(request.filter.orderBy, request.filter.orderDirection)
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = listaMovimentosPorConciliar.Count();
            response.rows = totalNumber;
            response.movimentos = movimentosPorConciliar;

            return response;
        }

        public MovimentosPorConciliar GetListagemConciliacao(MovimentosPorConciliarConciliacaoListagemRequest request, int idEstadoPagamentoEmitido)
        {
            MovimentosPorConciliar response = new MovimentosPorConciliar();

            int index = 0;
            if (request.filter.index.HasValue)
                index = request.filter.index.Value;

            int rows = 5;
            if (request.filter.rows.HasValue)
                rows = request.filter.rows.Value;

            var beginDate = request.filter.dateFilterBegin;
            var endDate = request.filter.dateFilterEnd;

            var listaMovimentosPorConciliar = _moduloContribuicoesContext.Movimentosporconciliar
               .Include(e => e.RelMovimentosporconciliarMovimentos)
               .Include(e => e.MovimentoBancarioFkNavigation)
               .Where(e => e.IndActivo.Value &&
                    // Filtrar por descrição
                    (request.filter.filterBy == null || e.MovimentoBancarioFkNavigation.Descricao.Contains(request.filter.filterBy)) &&
                    // Só podem aparecer movimentos que não estejam com o estado de pagamento emitido
                    !e.RelMovimentosporconciliarMovimentos.Any(a => a.IndActivo.Value && a.Estado == idEstadoPagamentoEmitido) &&
                    // Filtrar por movimentos conciliados a um movimento bancário específico
                    (request.ConciliadoCom.HasValue ? e.RelMovimentosporconciliarMovimentos.Any(a => a.IndActivo.Value && request.ConciliadoCom == a.MovimentosBancariosFk) :
                    // Se o ConciliadoCom for null, só se devolve os movimentos que não têm outros movimentos relacionados com o movimento bancário
                    (e.RelMovimentosporconciliarMovimentos.Count > 1 || e.RelMovimentosporconciliarMovimentos.Any(a => a.IndActivo.Value && a.MovimentosBancariosFkNavigation.RelMovimentosporconciliarMovimentos.Count == 1))) &&
                    // Filtrar por datas
                    (beginDate.HasValue && endDate.HasValue ? e.DataCriacao >= beginDate && e.DataCriacao <= endDate :
                     beginDate.HasValue && !endDate.HasValue ? e.DataCriacao == beginDate :
                     true)
               )
               .Select(e => new MovimentosPorConciliarListagem
               {
                   id = e.Id,
                   descricao = new MovimentosPorConciliarDescricao()
                   {
                       id = e.MovimentoBancarioFkNavigation.Id,
                       descricao = e.MovimentoBancarioFkNavigation.Descricao,
                   },
                   comprovativo = e.Comprovativo,
                   numeroDocumento = e.NumeroDocumento,
                   valor = e.Valor,
                   conciliado = e.RelMovimentosporconciliarMovimentos.Any(),
                   type = MovimentosPorConciliarListagemType.MovimentoAConciliar
               })
               //.Concat(
               //     _moduloContribuicoesContext.Guiapagamento
               //     .Include(e => e.RelMovimentosporconciliarMovimentos)
               //     .Where(e => e.IndActivo &&
               //         (request.filter.filterBy == null || e.Descricao.Contains(request.filter.filterBy)) &&
               //         (request.ConciliadoCom.HasValue ? e.RelMovimentosporconciliarMovimentos.Any(a => a.IndActivo.Value && request.ConciliadoCom == a.MovimentosBancariosFk) :
               //         (e.RelMovimentosporconciliarMovimentos.Count > 1 || e.RelMovimentosporconciliarMovimentos.Any(a => a.MovimentosBancariosFkNavigation.RelMovimentosporconciliarMovimentos.Count == 1))) &&
               //         (beginDate.HasValue && endDate.HasValue ? e.DataCriacao >= beginDate && e.DataCriacao <= endDate :
               //          beginDate.HasValue && !endDate.HasValue ? e.DataCriacao == beginDate :
               //          true)
               //     )
               //     .Select(e => new MovimentosPorConciliarListagem
               //     {
               //         id = e.IdGuia,
               //         descricao = new MovimentosPorConciliarDescricao()
               //         {
               //             id = null,
               //             descricao = e.Descricao,
               //         },
               //         comprovativo = e.ComprovativoPag,
               //         numeroDocumento = e.NumDocumento,
               //         valor = e.Valor,
               //         conciliado = e.RelMovimentosporconciliarMovimentos.Any(),
               //         type = MovimentosPorConciliarListagemType.GuiaPagamento
               //     })
               //)
               .Concat(
                    _moduloContribuicoesContext.Pagamentosexecutados
                    .Include(e => e.CompromissoFkNavigation)
                    .ThenInclude(e => e.ComponenteDespesaRegistoFkNavigation)
                    .Include(e => e.RelMovimentosporconciliarMovimentos)
                    .Where(e => e.IndActivo &&
                        // Filtrar por descrição
                        (request.filter.filterBy == null || e.CompromissoFkNavigation.ComponenteDespesaRegistoFkNavigation.Descricao.Contains(request.filter.filterBy)) &&
                        // Só podem aparecer movimentos que não estejam com o estado de pagamento emitido
                        !e.RelMovimentosporconciliarMovimentos.Any(a => a.IndActivo.Value && a.Estado == idEstadoPagamentoEmitido) &&
                        // Filtrar por movimentos conciliados a um movimento bancário específico
                        (request.ConciliadoCom.HasValue ? e.RelMovimentosporconciliarMovimentos.Any(a => a.IndActivo.Value && request.ConciliadoCom == a.MovimentosBancariosFk) :
                        // Se o ConciliadoCom for null, só se devolve os movimentos que não têm outros movimentos relacionados com o movimento bancário
                        (e.RelMovimentosporconciliarMovimentos.Count > 1 || e.RelMovimentosporconciliarMovimentos.Any(a => a.MovimentosBancariosFkNavigation.RelMovimentosporconciliarMovimentos.Count == 1))) &&
                        // Filtrar por datas
                        (beginDate.HasValue && endDate.HasValue ? e.DataCriacao >= beginDate && e.DataCriacao <= endDate :
                         beginDate.HasValue && !endDate.HasValue ? e.DataCriacao == beginDate :
                         true)
                    )
                    .Select(e => new MovimentosPorConciliarListagem
                    {
                        id = e.Id,
                        descricao = new MovimentosPorConciliarDescricao()
                        {
                            id = null,
                            descricao = e.CompromissoFkNavigation.ComponenteDespesaRegistoFkNavigation.Descricao,
                        },
                        comprovativo = null,
                        numeroDocumento = e.NumeroPagamento,
                        valor = e.ValorExecutado,
                        conciliado = e.RelMovimentosporconciliarMovimentos.Any(),
                        type = MovimentosPorConciliarListagemType.PagamentoExecutado
                    })
                );

            var movimentosPorConciliar = listaMovimentosPorConciliar
                .OrderBy(request.filter.orderBy, request.filter.orderDirection)
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = listaMovimentosPorConciliar.Count();
            response.rows = totalNumber;
            response.movimentos = movimentosPorConciliar;

            return response;
        }

        public bool TemConciliados(List<MovimentosAConciliar> items)
        {
            if (items == null || items.Count == 0) return false;

            var guiasPagamentos = items.Where(e => e.Type == MovimentosPorConciliarListagemType.GuiaPagamento).Select(x => x.Id).ToList();
            var pagamentosExecutados = items.Where(e => e.Type == MovimentosPorConciliarListagemType.PagamentoExecutado).Select(x => x.Id).ToList();
            var movimentosAConciliar = items.Where(e => e.Type == MovimentosPorConciliarListagemType.MovimentoAConciliar).Select(x => x.Id).ToList();

            var result = _moduloContribuicoesContext.RelMovimentosporconciliarMovimentos.Where(e =>
                e.IndActivo.Value && (
                    (guiasPagamentos.Count() != 0 && guiasPagamentos.Any(a => a == e.GuiaPagamentoFk.Value)) ||
                    (pagamentosExecutados.Count() != 0 && pagamentosExecutados.Any(a => a == e.PagamentosExecutadosFk.Value)) ||
                    (movimentosAConciliar.Count() != 0 && movimentosAConciliar.Any(a => a == e.MovimentoPorConciliarFk.Value))
                )
            ).ToList().Any();

            return result;
        }

        public List<decimal> GetValores(List<MovimentosAConciliar> items)
        {
            var result = new List<decimal>();

            if (items == null || items.Count == 0) return result;

            var guiasPagamentos = items.Where(e => e.Type == MovimentosPorConciliarListagemType.GuiaPagamento).Select(x => x.Id).ToList();
            var pagamentosExecutados = items.Where(e => e.Type == MovimentosPorConciliarListagemType.PagamentoExecutado).Select(x => x.Id).ToList();
            var movimentosAConciliar = items.Where(e => e.Type == MovimentosPorConciliarListagemType.MovimentoAConciliar).Select(x => x.Id).ToList();

            var reservaCredito = items.Where(e => e.Type == MovimentosPorConciliarListagemType.ReservaCredito).Select(x => x.Id).ToList();


            if (guiasPagamentos.Any())
            {
                var guiasValores = _moduloContribuicoesContext.Guiapagamento.Where(e => e.IndActivo && guiasPagamentos.Any(a => a == e.IdGuia)).Select(e => e.ValorComprovPag.Value).ToList();
                result.AddRange(guiasValores);
            }

            if (pagamentosExecutados.Any())
            {
                var pagamentosValores = _moduloContribuicoesContext.Pagamentosexecutados.Where(e => e.IndActivo && pagamentosExecutados.Any(a => a == e.Id)).Select(e => e.ValorExecutado).ToList();
                result.AddRange(pagamentosValores);
            }

            if (movimentosAConciliar.Any())
            {
                var movimentosValores = _moduloContribuicoesContext.Movimentosporconciliar.Where(e => e.IndActivo.Value && movimentosAConciliar.Any(a => a == e.Id)).Select(e => e.Valor).ToList();
                result.AddRange(movimentosValores);
            }

            if (reservaCredito.Any())
            {   
               
                var reservaCreditoValores = _moduloContribuicoesContext.Reservacredito.Where(e => e.Valor != null && reservaCredito.Any(a => a == e.IdReserva)).Select(e => e.Valor.Value).ToList();
                result.AddRange(reservaCreditoValores);

            }


            return result;
        }

        public MovimentosPorConciliar GetMovimentosConciliados(SearchFilterRequest request, int estado)
        {
            MovimentosPorConciliar response = new MovimentosPorConciliar();

            int index = 0;
            if (request.filter.index.HasValue)
                index = request.filter.index.Value;

            int rows = 5;
            if (request.filter.rows.HasValue)
                rows = request.filter.rows.Value;

            var listaMovimentosPorConciliar = _moduloContribuicoesContext.RelMovimentosporconciliarMovimentos
                .Include(e => e.GuiaPagamentoFkNavigation)
               .Include(e => e.MovimentoPorConciliarFkNavigation)
               .ThenInclude(e => e.MovimentoBancarioFkNavigation)
               .Where(e => e.IndActivo.Value &&
                           e.MovimentoPorConciliarFkNavigation.IsReceita &&
                           e.Estado == estado &&
                           // Filtrar por descrição
                           (request.filter.filterBy == null || e.MovimentoPorConciliarFkNavigation.MovimentoBancarioFkNavigation.Descricao.Contains(request.filter.filterBy))
                )
               .Select(e => new MovimentosPorConciliarListagem
               {
                   id = e.Id,
                   descricao = new MovimentosPorConciliarDescricao()
                   {
                       id = e.MovimentoPorConciliarFkNavigation.MovimentoBancarioFkNavigation.Id,
                       descricao = e.MovimentoPorConciliarFkNavigation.MovimentoBancarioFkNavigation.Descricao,
                   },
                   comprovativo = e.MovimentoPorConciliarFkNavigation.Comprovativo,
                   numeroDocumento = e.MovimentoPorConciliarFkNavigation.NumeroDocumento + " " + e.MovimentoPorConciliarFkNavigation.TipoDocumento,
                   valor = e.MovimentoPorConciliarFkNavigation.Valor,
                   type = MovimentosPorConciliarListagemType.MovimentoAConciliar,
               })
               .Concat(
                    _moduloContribuicoesContext.RelMovimentosporconciliarMovimentos
                    .Include(e => e.GuiaPagamentoFkNavigation)
                    .Where(e => e.GuiaPagamentoFk != null && e.IndActivo == true && e.Estado == estado
                    && (request.filter.filterBy == null || e.GuiaPagamentoFkNavigation.Descricao.Contains(request.filter.filterBy)))
                    .Select(e => new MovimentosPorConciliarListagem
                    {
                        id = e.Id,
                        descricao = new MovimentosPorConciliarDescricao()
                        {
                            id = null,
                            descricao = e.GuiaPagamentoFkNavigation.Descricao,
                        },
                        comprovativo = e.GuiaPagamentoFkNavigation.ComprovativoPag,
                        numeroDocumento = e.GuiaPagamentoFkNavigation.NumDocumento,
                        valor = e.GuiaPagamentoFkNavigation.ValorComprovPag.Value,
                        type = MovimentosPorConciliarListagemType.GuiaPagamento,
                    })

                );

            var movimentosPorConciliar = listaMovimentosPorConciliar
                .OrderBy(request.filter.orderBy, request.filter.orderDirection)
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = listaMovimentosPorConciliar.Count();
            response.rows = totalNumber;
            response.movimentos = movimentosPorConciliar;

            return response;
        }

        public List<MovimentosPorConciliarListagem> GetExtraDataByListIds(List<int> ids)
        {
            return _moduloContribuicoesContext.Movimentosporconciliar
                .Where(u => u.IndActivo.Value && ids.Contains(u.Id))
                .Select(x => new MovimentosPorConciliarListagem
                {
                    id = x.Id,
                    movimentoBancarioId = x.MovimentoBancarioFk.GetValueOrDefault(),
                    tipoDocumento = x.TipoDocumento,
                    nomeComprovativo = x.NomeComprovativo,
                    contabilidadeCredito = x.CodigoContaCreditoFk,
                    contabilidadeDebito = x.CodigoContaDebitoFk,
                    departamentoINSS = x.DepartamentoFk,
                    centroCusto = x.CentroCustoFk,
                    tipoConta = x.TipoContaFk,
                    contaOSS = x.AgrupamentoConfigFk
                })
                .ToList();
        }

        public List<MovimentosPorConciliarListagem> GetMovimentosConciliadosByIdEstado(IEnumerable<int> ids, int estado)
        {

            var listaMovimentosPorConciliar = _moduloContribuicoesContext.RelMovimentosporconciliarMovimentos
                .Include(e => e.GuiaPagamentoFkNavigation)
               .Include(e => e.MovimentoPorConciliarFkNavigation)
               .ThenInclude(e => e.MovimentoBancarioFkNavigation)
               .Where(e => e.IndActivo.Value &&
                           e.MovimentoPorConciliarFkNavigation.IsReceita &&
                           e.Estado == estado &&
                           ids.Contains(e.Id)
                )
               .Select(e => new MovimentosPorConciliarListagem
               {
                   id = e.Id,
                   descricao = new MovimentosPorConciliarDescricao()
                   {
                       id = e.MovimentoPorConciliarFkNavigation.MovimentoBancarioFkNavigation.Id,
                       descricao = e.MovimentoPorConciliarFkNavigation.MovimentoBancarioFkNavigation.Descricao,
                   },
                   comprovativo = e.MovimentoPorConciliarFkNavigation.Comprovativo,
                   numeroDocumento = e.MovimentoPorConciliarFkNavigation.NumeroDocumento + " " + e.MovimentoPorConciliarFkNavigation.TipoDocumento,
                   valor = e.MovimentoPorConciliarFkNavigation.Valor,
                   type = MovimentosPorConciliarListagemType.MovimentoAConciliar,
               })
               .Concat(
                    _moduloContribuicoesContext.RelMovimentosporconciliarMovimentos
                    .Include(e => e.GuiaPagamentoFkNavigation)
                    .Where(e => e.GuiaPagamentoFk != null && e.IndActivo == true && e.Estado == estado && ids.Contains(e.Id))
                    .Select(e => new MovimentosPorConciliarListagem
                    {
                        id = e.Id,
                        descricao = new MovimentosPorConciliarDescricao()
                        {
                            id = null,
                            descricao = e.GuiaPagamentoFkNavigation.Descricao,
                        },
                        comprovativo = e.GuiaPagamentoFkNavigation.ComprovativoPag,
                        numeroDocumento = e.GuiaPagamentoFkNavigation.NumDocumento,
                        valor = e.GuiaPagamentoFkNavigation.ValorComprovPag.Value,
                        type = MovimentosPorConciliarListagemType.GuiaPagamento,
                    })

                );

            return listaMovimentosPorConciliar.ToList();
        }

        public Movimentosporconciliar GuiaExistenteNosMovimentos(int idGuia)
        {
            Movimentosporconciliar response = new Movimentosporconciliar();

            response = _moduloContribuicoesContext.Movimentosporconciliar
                .Where(e =>
                e.IndActivo.Value && e.GuiapagamentoId == idGuia
            ).FirstOrDefault();

            return response;
        }
        public Movimentosporconciliar ReservaCreditoExistenteNosMovimentos(int idReserva)
        {
            Movimentosporconciliar response = new Movimentosporconciliar();

            response = _moduloContribuicoesContext.Movimentosporconciliar
                .Where(e =>
                e.IndActivo.Value && e.ReservaCreditoId == idReserva
            ).FirstOrDefault();

            return response;
        }

    }
}