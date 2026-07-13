using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.ExcelDocumentService;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;
using TimorINSSBackEnd.Resources;
using static TimorINSSBackEnd.ExcelDocumentService.Models;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class MovimentosbancariosRepository : IMovimentosbancariosRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public MovimentosbancariosRepository(TimorINSSModuloContribuicoesContext storeContext, IStringLocalizer<SharedResource> localizer)
        {
            _moduloContribuicoesContext = storeContext;
            _localizer = localizer;
        }

        public IEnumerable<Movimentosbancarios> GetAll()
        {
            return _moduloContribuicoesContext.Movimentosbancarios
                .ToList();
        }

        public Movimentosbancarios Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var campo = _moduloContribuicoesContext.Movimentosbancarios
                .Include(e => e.RelMovimentosporconciliarMovimentos)
                .SingleOrDefault(u => u.Id == id);

            return campo;
        }

        public MovimentosbancariosDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var movimento = _moduloContribuicoesContext.Movimentosbancarios
                .SingleOrDefault(u => u.Id == id);

            MovimentosbancariosDto movimentoDto = Utils.MappClassToDto<Movimentosbancarios, MovimentosbancariosDto>(movimento);
            return movimentoDto;
        }

        public void Add(Movimentosbancarios entity)
        {
            _moduloContribuicoesContext.Movimentosbancarios.Add(entity);
        }

        public void Update(Movimentosbancarios entity)
        {
            Movimentosbancarios entityToUpdate = _moduloContribuicoesContext.Movimentosbancarios
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Movimentosbancarios entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public Movimentosbancarios GetByTarefaAtivoId(long tarefaAtivoId)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var campo = _moduloContribuicoesContext.Movimentosbancarios
                .SingleOrDefault(u => u.TarefaFk == tarefaAtivoId && u.IndActivo);

            return campo;
        }

        public MovimentosListagemResponse GetMovimentosByFilter(MovimentosListagemRequest request)     
        {
            MovimentosListagemResponse response = new MovimentosListagemResponse();

            int index = 0;
            if (request.filter.index.HasValue)
                index = request.filter.index.Value;

            int rows = 5;
            if (request.filter.rows.HasValue)
                rows = request.filter.rows.Value;

            var movimentosQuery = _moduloContribuicoesContext.Movimentosbancarios.Where(x => request.BancoId.HasValue ? x.ContaFk == request.BancoId.Value : x.CaixaFk == request.CaixaId.Value);

            // Filtrar por data de ínicio
            if (request.filter.dateFilterBegin.HasValue && !request.filter.dateFilterEnd.HasValue)
            {
                movimentosQuery = movimentosQuery.Where(u => u.DataValor == request.filter.dateFilterBegin.Value);
            }
            // Filtrar data de ínicio e fim
            else if (request.filter.dateFilterBegin.HasValue && request.filter.dateFilterEnd.HasValue)
            {
                movimentosQuery = movimentosQuery.Where(u => u.DataValor >= request.filter.dateFilterBegin.Value && u.DataValor <= request.filter.dateFilterEnd.Value);
            }
            if (request.filter.filterField == "concilados")
            {
                movimentosQuery = movimentosQuery.Where(u => u.RelMovimentosporconciliarMovimentos.Any());
            }
            else if (request.filter.filterField == "naoConcilados")
                movimentosQuery = movimentosQuery.Where(u => !u.RelMovimentosporconciliarMovimentos.Any());

            var listaMovimentos = movimentosQuery
               // Filtrar por descrição
               .Where(u => (request.filter.filterBy == null || u.Descricao.Contains(request.filter.filterBy)))
               .Select(u => new MovimentosData
               {
                   Id = u.Id,
                   TarefaAtivoId = u.TarefaFk,
                   Descricao = u.Descricao,
                   DataValor = u.DataValor,
                   Credito = u.Credito,
                   Debito = u.Debito,
                   Conciliado = u.RelMovimentosporconciliarMovimentos.Any(),
                   Saldo = !request.GetBalance ? null : _moduloContribuicoesContext.Movimentosbancarios.Where(x => (request.BancoId.HasValue ? x.ContaFk == request.BancoId.Value : x.CaixaFk == request.CaixaId.Value) && x.Id <= u.Id).Sum(a => a.Credito.HasValue ? a.Credito : a.Debito)
               });

            var movimentos = listaMovimentos
                .OrderBy(request.filter.orderBy, request.filter.orderDirection)
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = listaMovimentos.Count();
            response.rows = totalNumber;
            response.movimentos = movimentos;


            return response;
        }

        public MovimentosListagemResponse GetListagemConciliacao(MovimentosBancarioConciliacaoListagemRequest request, int idEstadoPagamentoEmitido)
        {
            MovimentosListagemResponse response = new MovimentosListagemResponse();

            int index = 0;
            if (request.filter.index.HasValue)
                index = request.filter.index.Value;

            int rows = 5;
            if (request.filter.rows.HasValue)
                rows = request.filter.rows.Value;

            var beginDate = request.filter.dateFilterBegin;
            var endDate = request.filter.dateFilterEnd;

            var listaMovimentos = _moduloContribuicoesContext.Movimentosbancarios
               .Where(e => e.IndActivo &&
                    // Filtrar por descrição
                    (request.filter.filterBy == null || e.Descricao.Contains(request.filter.filterBy)) &&
                    // Só podem aparecer movimentos que não estejam com o estado de pagamento emitido
                    !e.RelMovimentosporconciliarMovimentos.Any(a => a.IndActivo.Value && (a.GuiaPagamentoFk.HasValue || a.Estado == idEstadoPagamentoEmitido)) &&
                    // Filtrar por movimentos conciliados a um movimento por conciliar específico
                    (request.ConciliadoCom.HasValue ? e.RelMovimentosporconciliarMovimentos.Any(a => a.IndActivo.Value && ((request.ConciliadoComType == MovimentosPorConciliarListagemType.MovimentoAConciliar && request.ConciliadoCom == a.MovimentoPorConciliarFk) ||
                                                                                                                           (request.ConciliadoComType == MovimentosPorConciliarListagemType.PagamentoExecutado && request.ConciliadoCom == a.PagamentosExecutadosFk))) :
                    // Se o ConciliadoCom for null, só se devolve os movimentos que não têm outros movimentos relacionados com o movimento bancário
                    (!e.RelMovimentosporconciliarMovimentos.Any(a => a.IndActivo.Value) ||
                     e.RelMovimentosporconciliarMovimentos.Count > 1 || e.RelMovimentosporconciliarMovimentos.Any(a => a.IndActivo.Value && ((a.MovimentoPorConciliarFk.HasValue && a.MovimentoPorConciliarFkNavigation.RelMovimentosporconciliarMovimentos.Count == 1) ||
                                                                                                                                            (a.PagamentosExecutadosFk.HasValue && a.PagamentosExecutadosFkNavigation.RelMovimentosporconciliarMovimentos.Count == 1))))) &&
                    // Filtrar por datas
                    (beginDate.HasValue && endDate.HasValue ? e.DataValor >= beginDate && e.DataValor <= endDate :
                     beginDate.HasValue && !endDate.HasValue ? e.DataValor == beginDate :
                     true)
               )
               .Select(u => new MovimentosData
               {
                   Id = u.Id,
                   Descricao = u.Descricao,
                   DataValor = u.DataValor,
                   Credito = u.Credito,
                   Debito = u.Debito
               });

            var movimentos = listaMovimentos
                .OrderBy(request.filter.orderBy, request.filter.orderDirection)
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = listaMovimentos.Count();
            response.rows = totalNumber;
            response.movimentos = movimentos;

            return response;
        }

        public bool TemConciliados(List<int> items)
        {
            if (items == null || items.Count == 0) return false;

            return _moduloContribuicoesContext.RelMovimentosporconciliarMovimentos.Any(e => e.IndActivo.Value && items.Any(a => a == e.MovimentosBancariosFk));
        }

        public List<decimal> GetValores(List<int> ids)
        {
            if (ids == null || ids.Count == 0) return new List<decimal>();

            return _moduloContribuicoesContext.Movimentosbancarios.Where(e => e.IndActivo && ids.Any(a => a == e.Id)).Select(e => e.Credito ?? e.Debito.Value).ToList();
        }

        public List<Movimentosbancarios> GetByIds(List<int> ids)
        {
            if (ids == null || ids.Count == 0) return new List<Movimentosbancarios>();

            return _moduloContribuicoesContext.Movimentosbancarios
                .Include(e => e.ContaFkNavigation)
                .Where(e => ids.Any(a => a == e.Id))
                .ToList();
        }

        public decimal? GetCreditoConciliados(int? contaId, int? caixaId)
        {
            return _moduloContribuicoesContext.Movimentosbancarios
                .Where(x => x.RelMovimentosporconciliarMovimentos.Any() && x.IndActivo && x.Credito != null &&
                            contaId == x.ContaFk && caixaId == x.CaixaFk)
                .Sum(x => x.Credito);
        }

        public decimal? GetDebitoConciliados(int? contaId, int? caixaId)
        {
            return _moduloContribuicoesContext.Movimentosbancarios
                .Where(x => x.RelMovimentosporconciliarMovimentos.Any() && x.IndActivo && x.Debito != null &&
                            contaId == x.ContaFk && caixaId == x.CaixaFk)
                .Sum(x => x.Debito);
        }

        public decimal? GetCreditoPorConciliar(int? contaId, int? caixaId)
        {
            return _moduloContribuicoesContext.Movimentosbancarios
                .Where(x => !x.RelMovimentosporconciliarMovimentos.Any() && x.IndActivo && x.Credito != null &&
                            contaId == x.ContaFk && caixaId == x.CaixaFk)
                .Sum(x => x.Credito);
        }

        public decimal? GetDebitoPorConciliar(int? contaId, int? caixaId)
        {
            return _moduloContribuicoesContext.Movimentosbancarios
                .Where(x => !x.RelMovimentosporconciliarMovimentos.Any() && x.IndActivo && x.Debito != null &&
                            contaId == x.ContaFk && caixaId == x.CaixaFk)
                .Sum(x => x.Debito);
        }

        public string ListMovimentosExcel(MovimentosListagemRequest request, List<MovimentosData> lista)
        {
            // Inicialização do documento excel
            var excelDocument = new ExcelDocument(_localizer["extratosBancarios"].Value, new ExcelDocumentOptions()
            {
                TextStyles = Extensions.ServiceExtensions.ExcelDocumentTextStyles
            });

            var conta = _moduloContribuicoesContext.Contabancaria.First(e => e.Id == request.BancoId);
            var saldo = _moduloContribuicoesContext.Movimentosbancarios.Where(a => a.ContaFk == request.BancoId).Sum(a => a.Credito.HasValue ? a.Credito : a.Debito);

            // Adição dos títulos
            excelDocument.Pages[0].AddText(_localizer["extratosBancarios"].Value, new ExcelDocumentTextPosition(1, 1), "Header", new ExcelDocumentTextPosition(5, 1));
            excelDocument.Pages[0].AddText(_localizer["swift"].Value + ": " + conta.Swift, new ExcelDocumentTextPosition(1, 2), "Header");
            excelDocument.Pages[0].AddText(_localizer["entidadeBancaria"].Value + ": " + conta.EntidadeBancaria, new ExcelDocumentTextPosition(2, 2), "Header");
            excelDocument.Pages[0].AddText(_localizer["descricao"].Value + ": " + conta.Descricao, new ExcelDocumentTextPosition(3, 2), "Header");
            excelDocument.Pages[0].AddText(_localizer["iban"].Value + ": " + conta.Iban, new ExcelDocumentTextPosition(4, 2), "Header");
            excelDocument.Pages[0].AddText(_localizer["numeroConta"].Value + ": " + conta.Numero, new ExcelDocumentTextPosition(5, 2), "Header");
            excelDocument.Pages[0].AddText(_localizer["saldo"].Value + ": " + saldo, new ExcelDocumentTextPosition(1, 3), "Header", new ExcelDocumentTextPosition(5, 3));

            // Adição da tabela
            excelDocument.Pages[0].AddTable(new ExcelDocumentTextPosition(1, 5), lista, new List<ColumnOption<MovimentosData>>()
            {
                new ColumnOption<MovimentosData>()
                {
                    Name = _localizer["descricao"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellWrap",
                    Value = (data) => data.Descricao,
                    Width = 25,
                },
                new ColumnOption<MovimentosData>()
                {
                    Name = _localizer["data"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellWrap",
                    Value = (data) => data.DataValor.ToString("dd/MM/yyyy"),
                    Width = 37,
                },
                new ColumnOption<MovimentosData>()
                {
                    Name = _localizer["credito"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellMoney",
                    Value = (data) => data.Credito,
                    Width = 23,
                },
                new ColumnOption<MovimentosData>()
                {
                    Name = _localizer["debito"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellMoney",
                    Value = (data) => data.Debito,
                    Width = 23,
                },
                new ColumnOption<MovimentosData>()
                {
                    Name = _localizer["saldo"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellMoney",
                    Value = (data) => data.Saldo,
                    Width = 33,
                },
            }, new TableOptions() { AutoFitColumns = false });

            // Adição da linha de Total como footer da tabela
            excelDocument.Pages[0].AddText(_localizer["total"].Value + ":", new ExcelDocumentTextPosition(1, 6 + lista.Count), "HeaderAlignRight", new ExcelDocumentTextPosition(2, 6 + lista.Count));

            var sumRange1 = new ExcelDocumentTextPositionRange(new ExcelDocumentTextPosition(3, 6), new ExcelDocumentTextPosition(3, 6 + lista.Count - 1));
            excelDocument.Pages[0].AddSum(new ExcelDocumentTextPosition(3, 6 + lista.Count), sumRange1, "HeaderAlignLeftMoney");

            var sumRange2 = new ExcelDocumentTextPositionRange(new ExcelDocumentTextPosition(4, 6), new ExcelDocumentTextPosition(4, 6 + lista.Count - 1));
            excelDocument.Pages[0].AddSum(new ExcelDocumentTextPosition(4, 6 + lista.Count), sumRange2, "HeaderAlignLeftMoney", new ExcelDocumentTextPosition(5, 6 + lista.Count));

            // Conversão do documento excel em base64
            return excelDocument.GetFileString();
        }
    }
}