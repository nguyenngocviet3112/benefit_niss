using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts;
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
    public class PagamentosExecutadosRepository : IPagamentosExecutadosRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public PagamentosExecutadosRepository(TimorINSSModuloContribuicoesContext storeContext, IStringLocalizer<SharedResource> localizer)
        {
            _moduloContribuicoesContext = storeContext;
            _localizer = localizer;
        }

        public IEnumerable<Pagamentosexecutados> GetAll()
        {
            return _moduloContribuicoesContext.Pagamentosexecutados.ToList();
        }

        public Pagamentosexecutados Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var pagamentosExecutados = _moduloContribuicoesContext.Pagamentosexecutados
                .SingleOrDefault(u => u.Id == id);

            return pagamentosExecutados;
        }

        public PagamentosExecutadosDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var pagamentosExecutados = _moduloContribuicoesContext.Pagamentosexecutados
                .SingleOrDefault(u => u.Id == id);

            PagamentosExecutadosDto pagamentosExecutadosDto = Utils.MappClassToDto<Pagamentosexecutados, PagamentosExecutadosDto>(pagamentosExecutados);
            return pagamentosExecutadosDto;
        }

        public void Add(Pagamentosexecutados entity)
        {
            _moduloContribuicoesContext.Pagamentosexecutados.Add(entity);
        }

        public void Update(Pagamentosexecutados entity)
        {
            Pagamentosexecutados entityToUpdate = _moduloContribuicoesContext.Pagamentosexecutados
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Pagamentosexecutados entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public List<Pagamentosexecutados> GetPagamentosexecutadosByIdDespesa(int idDespesa)
        {
            return _moduloContribuicoesContext.Pagamentosexecutados
                .Where(u => u.IndActivo && u.CompromissoFk == idDespesa)
                .ToList();
        }

        public List<Pagamentosexecutados> GetPagamentosexecutadosById(List<int> ids)
        {
            return _moduloContribuicoesContext.Pagamentosexecutados
                .Where(u => u.IndActivo && ids.Contains(u.Id))
                .ToList();
        }

        public List<PagamentoExecutadoDestinatrioDataContract> GetPagamentosexecutadosByNumPagamento(string numPagamento)
        {
            return _moduloContribuicoesContext.Pagamentosexecutados
                .AsNoTracking()
                .Include(u => u.DestinatarioFkNavigation)
                .Include(u => u.CompromissoFkNavigation)
                .ThenInclude(u => u.ComponenteDespesaRegistoFkNavigation)
                .Where(u => u.IndActivo && u.NumeroPagamento == numPagamento)
                 .Select(u => new PagamentoExecutadoDestinatrioDataContract
                 {
                     Id = u.Id,
                     CompromissoFk = u.CompromissoFk,
                     ComponenteDespesaRegistoFk = u.CompromissoFkNavigation.ComponenteDespesaRegistoFk,
                     DescricaoDespesa = u.CompromissoFkNavigation.ComponenteDespesaRegistoFkNavigation.Descricao,
                     Destinatario = u.DestinatarioFkNavigation,
                     NumeroPagamento = u.NumeroPagamento,
                     Estado = u.EstadoNavigation.Descricao,
                     ValorExecutado = u.ValorExecutado,
                     Iban = u.Iban,
                     Swift = u.Swift,
                     ProcessoId = u.ProcessoAtivoFk,
                     NumeroConta = u.NumeroConta,
                     CodigoContaCredito = u.CodigoContaCreditoFk,
                     CodigoContaDebito = u.CodigoContaDebitoFk,
                     DataObrigacao = u.DataObrigacao,
                     BankCode = u.BankCode
                 })
                .ToList();
        }


        public List<PagamentoExecutadoDestinatrioDataContract> GetPagamentosexecutadosByProcessoAtivoID(int processoAtivoId)
        {
            return _moduloContribuicoesContext.Pagamentosexecutados
                .Include(u => u.DestinatarioFkNavigation)
                .Include(u => u.CompromissoFkNavigation)
                .ThenInclude(u => u.ComponenteDespesaRegistoFkNavigation)
                .Where(u => u.IndActivo && u.ProcessoAtivoFk == processoAtivoId)
                 .Select(u => new PagamentoExecutadoDestinatrioDataContract
                 {
                     Id = u.Id,
                     ComponenteDespesaRegistoFk = u.CompromissoFkNavigation.ComponenteDespesaRegistoFk,
                     DescricaoDespesa = u.CompromissoFkNavigation.ComponenteDespesaRegistoFkNavigation.Descricao,
                     Destinatario = u.DestinatarioFkNavigation,
                     NumeroPagamento = u.NumeroPagamento,
                     Estado = u.EstadoNavigation.Descricao,
                     ValorExecutado = u.ValorExecutado,
                     Iban = u.Iban,
                     ProcessoId = u.ProcessoAtivoFk,
                     NumeroConta = u.NumeroConta,
                     CodigoContaCredito = u.CodigoContaCreditoFk,
                     CodigoContaDebito = u.CodigoContaDebitoFk,
                     DataObrigacao = u.DataObrigacao,
                     BankCode = u.BankCode
                 })
                .ToList();
        }

        public List<Pagamentosexecutados> GetPagamentosexecutadosByIdProcessoAtivo(int idProcessoAtivo, bool includeConciliadosRel = false)
        {
            IQueryable<Pagamentosexecutados> query = _moduloContribuicoesContext.Pagamentosexecutados;

            if (includeConciliadosRel) query = query.Include(e => e.RelMovimentosporconciliarMovimentos);

            return query.Where(u => u.IndActivo && u.ProcessoAtivoFk == idProcessoAtivo).ToList();
        }

        public List<Pagamentosexecutados> GetPagamentosexecutadosByIdProcessoAtivoEstado(int idProcessoAtivo, int estado)
        {
            return _moduloContribuicoesContext.Pagamentosexecutados
                .Where(u => u.IndActivo && u.ProcessoAtivoFk == idProcessoAtivo && u.Estado == estado)
                .ToList();
        }

        public List<PagamentoExecutadoDestinatrioDataContract> GetPagamentosexecutadosByIdDestinatario(int idDestinatario)
        {
            return _moduloContribuicoesContext.Pagamentosexecutados
               .Include(u => u.DestinatarioFkNavigation)
               .Include(u => u.CompromissoFkNavigation)
               .ThenInclude(u => u.ComponenteDespesaRegistoFkNavigation)
               .Where(u => u.IndActivo && u.DestinatarioFk == idDestinatario)
                .Select(u => new PagamentoExecutadoDestinatrioDataContract
                {
                    Id = u.Id,
                    ComponenteDespesaRegistoFk = u.CompromissoFkNavigation.ComponenteDespesaRegistoFk,
                    DescricaoDespesa = u.CompromissoFkNavigation.ComponenteDespesaRegistoFkNavigation.Descricao,
                    Destinatario = u.DestinatarioFkNavigation,
                    NumeroPagamento = u.NumeroPagamento,
                    Estado = u.EstadoNavigation.Descricao,
                    ValorExecutado = u.ValorExecutado,
                    Iban = u.Iban,
                    ProcessoId = u.ProcessoAtivoFk
                })
               .ToList();
        }

        public RelatorioPagamentosListagemResponse GetPagamentosRelatoriosGrouped(RelatorioPagamentosListagemRequest request)
        {
            RelatorioPagamentosListagemResponse response = new RelatorioPagamentosListagemResponse();

            int index = 0;
            if (request.filter.index.HasValue)
                index = request.filter.index.Value;

            int rows = 5;
            if (request.filter.rows.HasValue)
                rows = request.filter.rows.Value;

            var beginDate = request.filter.dateFilterBegin;
            var endDate = request.filter.dateFilterEnd;

            var listaPagamentos = _moduloContribuicoesContext.Pagamentosexecutados
                .Where(e => e.IndActivo &&
                            // Filtrar por NISS/TIN/Nome de destinatário
                            (request.search == null ||
                             e.DestinatarioFkNavigation.Niss.Contains(request.search) ||
                             e.DestinatarioFkNavigation.Tin.Contains(request.search) ||
                             e.DestinatarioFkNavigation.Nome.Contains(request.search)
                            ) &&
                            // Filtrar por centro de custo
                            (!request.centroCusto.HasValue || request.centroCusto == e.CompromissoFkNavigation.ComponenteDespesaRegistoFkNavigation.CentroCustoFk) &&
                            // Filtrar por estado
                            (!request.estado.HasValue || request.estado == e.Estado) &&
                            // Filtrar por número de pagamento
                            (request.numeroPagamento == null || request.numeroPagamento == e.NumeroPagamento) &&
                            // Filtrar por Conta OGE
                            (!request.contaOGE.HasValue || request.contaOGE == e.CompromissoFkNavigation.ComponenteDespesaRegistoFkNavigation.AgrupamentoConfigFk) &&
                            // Filtrar por datas
                            (beginDate.HasValue && endDate.HasValue ? (e.DataCriacao.Date >= beginDate && e.DataCriacao.Date <= endDate) :
                            beginDate.HasValue && !endDate.HasValue ? (e.DataCriacao.Date == beginDate) :
                            true)
                )
                // Agrupar por Número de pagamento
                .Select(e => e.NumeroPagamento)
                .Distinct();

            var pagamentos = listaPagamentos
                .OrderBy(request.filter.orderBy, request.filter.orderDirection)
                .Skip(index * rows)
                .Take(rows)
                .Select(e => new
                {
                    PrimeiroPagamento = _moduloContribuicoesContext.Pagamentosexecutados
                                                                   .Include(a => a.DestinatarioFkNavigation)
                                                                   .Include(a => a.CompromissoFkNavigation.ComponenteDespesaRegistoFkNavigation.AgrupamentoConfigFkNavigation)
                                                                   .FirstOrDefault(a => a.NumeroPagamento == e && a.IndActivo),
                    CountDestinatarios = _moduloContribuicoesContext.Pagamentosexecutados.Where(a => a.NumeroPagamento == e && a.IndActivo).Select(a => a.DestinatarioFk).Distinct().Count(),
                    CountContas = _moduloContribuicoesContext.Pagamentosexecutados.Where(a => a.NumeroPagamento == e && a.IndActivo).Select(a => a.CompromissoFkNavigation.ComponenteDespesaRegistoFkNavigation).Distinct().Count(),
                    Valor = _moduloContribuicoesContext.Pagamentosexecutados.Where(a => a.NumeroPagamento == e && a.IndActivo).Select(a => a.ValorExecutado).Sum(),
                })
                .Select(e => new RelatorioPagamentoDataContract()
                {
                    numeroPagamento = e.PrimeiroPagamento.NumeroPagamento,
                    destinatario = e.PrimeiroPagamento.DestinatarioFkNavigation.Nome,
                    conta = e.PrimeiroPagamento.CompromissoFkNavigation.ComponenteDespesaRegistoFkNavigation.AgrupamentoConfigFkNavigation.Codigo + " - " + e.PrimeiroPagamento.CompromissoFkNavigation.ComponenteDespesaRegistoFkNavigation.AgrupamentoConfigFkNavigation.Designacao,
                    dataEmissao = e.PrimeiroPagamento.DataCriacao,
                    estado = e.PrimeiroPagamento.EstadoNavigation.Descricao,
                    valor = e.Valor,
                    countDestinatarios = e.CountDestinatarios,
                    countContas = e.CountContas
                })
                .ToList();

            var totalNumber = listaPagamentos.Count();
            response.rows = totalNumber;
            response.pagamentos = pagamentos;

            return response;
        }

        public SelectDescriptionResponse GetDropdownContasOGE(RelatorioContaOGEDropdownListagemRequest request)
        {
            SelectDescriptionResponse response = new SelectDescriptionResponse();

            var data = _moduloContribuicoesContext.Agrupamentoconfig
                .Include(e => e.ParentFkNavigation)
                .Where(e => e.IndActivo &&
                            e.ReltipoDeContaOrcamentoConfigFkNavigation.TipoContaFk == 71 && // Despesa
                            e.ComponentedespesaRegisto.Any(a => a.IndActivo && a.CentroCustoFk == request.centroCusto)
                )
                .ToList();

            var selects = new List<SelectDescription>();

            data.ForEach(agrupamento =>
            {
                var select = new SelectDescription()
                {
                    id = agrupamento.Id,
                    nome = ""
                };

                var current = agrupamento;

                while (current != null)
                {
                    select.nome = current.Codigo + select.nome;
                    current = current.ParentFkNavigation;
                }

                select.nome += " - " + agrupamento.Designacao;

                selects.Add(select);
            });

            response.selects = selects;

            return response;
        }

        public RelatorioPagamentosListagemResponse GetPagamentosRelatoriosExcel(RelatorioPagamentosListagemRequest request)
        {
            RelatorioPagamentosListagemResponse response = new RelatorioPagamentosListagemResponse();

            var beginDate = request.filter.dateFilterBegin;
            var endDate = request.filter.dateFilterEnd;

            var pagamentos = _moduloContribuicoesContext.Pagamentosexecutados
                .Where(e => e.IndActivo &&
                            // Filtrar por NISS/TIN/Nome de destinatário
                            (request.search == null ||
                             e.DestinatarioFkNavigation.Niss.Contains(request.search) ||
                             e.DestinatarioFkNavigation.Tin.Contains(request.search) ||
                             e.DestinatarioFkNavigation.Nome.Contains(request.search)
                            ) &&
                            // Filtrar por centro de custo
                            (!request.centroCusto.HasValue || request.centroCusto == e.CompromissoFkNavigation.ComponenteDespesaRegistoFkNavigation.CentroCustoFk) &&
                            // Filtrar por estado
                            (!request.estado.HasValue || request.estado == e.Estado) &&
                            // Filtrar por número de pagamento
                            (request.numeroPagamento == null || request.numeroPagamento == e.NumeroPagamento) &&
                            // Filtrar por Conta OGE
                            (!request.contaOGE.HasValue || request.contaOGE == e.CompromissoFkNavigation.ComponenteDespesaRegistoFkNavigation.AgrupamentoConfigFk) &&
                            // Filtrar por datas
                            (beginDate.HasValue && endDate.HasValue ? (e.DataCriacao.Date >= beginDate && e.DataCriacao.Date <= endDate) :
                            beginDate.HasValue && !endDate.HasValue ? (e.DataCriacao.Date == beginDate) :
                            true)
                )
                .Select(e => new RelatorioPagamentoDataContract()
                {
                    numeroPagamento = e.NumeroPagamento,
                    destinatario = e.DestinatarioFkNavigation.Nome,
                    conta = e.CompromissoFkNavigation.ComponenteDespesaRegistoFkNavigation.AgrupamentoConfigFkNavigation.Codigo + " - " + e.CompromissoFkNavigation.ComponenteDespesaRegistoFkNavigation.AgrupamentoConfigFkNavigation.Designacao,
                    dataEmissao = e.DataCriacao,
                    estado = e.EstadoNavigation.Descricao,
                    valor = e.ValorExecutado
                })
                .ToList();

            response.pagamentos = pagamentos;

            return response;
        }

        public RelatorioPagamentosListagemResponse GetPagamentosRelatorios(RelatorioPagamentosListagemRequest request)
        {
            RelatorioPagamentosListagemResponse response = new RelatorioPagamentosListagemResponse();

            int index = 0;
            if (request.filter.index.HasValue)
                index = request.filter.index.Value;

            int rows = 5;
            if (request.filter.rows.HasValue)
                rows = request.filter.rows.Value;

            var beginDate = request.filter.dateFilterBegin;
            var endDate = request.filter.dateFilterEnd;

            var listaPagamentos = _moduloContribuicoesContext.Pagamentosexecutados
                .Where(e => e.IndActivo &&
                            // Filtrar por NISS/TIN/Nome de destinatário
                            (request.search == null ||
                             e.DestinatarioFkNavigation.Niss.Contains(request.search) ||
                             e.DestinatarioFkNavigation.Tin.Contains(request.search) ||
                             e.DestinatarioFkNavigation.Nome.Contains(request.search)
                            ) &&
                            // Filtrar por centro de custo
                            (!request.centroCusto.HasValue || request.centroCusto == e.CompromissoFkNavigation.ComponenteDespesaRegistoFkNavigation.CentroCustoFk) &&
                            // Filtrar por estado
                            (!request.estado.HasValue || request.estado == e.Estado) &&
                            // Filtrar por número de pagamento
                            (request.numeroPagamento == null || request.numeroPagamento == e.NumeroPagamento) &&
                            // Filtrar por Conta OGE
                            (!request.contaOGE.HasValue || request.contaOGE == e.CompromissoFkNavigation.ComponenteDespesaRegistoFkNavigation.AgrupamentoConfigFk) &&
                            // Filtrar por datas
                            (beginDate.HasValue && endDate.HasValue ? (e.DataCriacao.Date >= beginDate && e.DataCriacao.Date <= endDate) :
                            beginDate.HasValue && !endDate.HasValue ? (e.DataCriacao.Date == beginDate) :
                            true)
                )
                .Select(e => new RelatorioPagamentoDataContract()
                {
                    numeroPagamento = e.NumeroPagamento,
                    destinatario = e.DestinatarioFkNavigation.Nome,
                    iban = e.Iban,
                    conta = e.CompromissoFkNavigation.ComponenteDespesaRegistoFkNavigation.AgrupamentoConfigFkNavigation.Codigo + " - " + e.CompromissoFkNavigation.ComponenteDespesaRegistoFkNavigation.AgrupamentoConfigFkNavigation.Designacao,
                    dataEmissao = e.DataCriacao,
                    estado = e.EstadoNavigation.Descricao,
                    valor = e.ValorExecutado
                });

            var pagamentos = listaPagamentos
                .OrderBy(request.filter.orderBy, request.filter.orderDirection)
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = listaPagamentos.Count();
            response.rows = totalNumber;
            response.pagamentos = pagamentos;

            return response;
        }

        public List<Pagamentosexecutados> GetPagamentosexecutadosByIdDespesaIdContaOSS(int idDespesa, int idContaOSS)
        {
            return _moduloContribuicoesContext.Pagamentosexecutados
                .Include(e => e.CompromissoFkNavigation)
                .ThenInclude(e => e.ComponenteDespesaRegistoFkNavigation)
                .Where(u => u.IndActivo && u.CompromissoFkNavigation.ComponenteDespesaRegistoFk == idDespesa && u.CompromissoFkNavigation.ComponenteDespesaRegistoFkNavigation.AgrupamentoConfigFk == idContaOSS)
                .ToList();
        }

        IEnumerable<Agrupamentoconfig> GetLeafNodes(Agrupamentoconfig node)
        {
            // Không có con → đây là leaf (level 3)
            if (node.InverseParentFkNavigation == null
                || !node.InverseParentFkNavigation.Any())
            {
                // Nếu có cha thì trả cả cha
                if (node.ParentFkNavigation != null)
                    yield return node.ParentFkNavigation;

                // Và luôn trả chính nó
                yield return node;
                yield break;
            }

            // Có con → đi tiếp xuống các con
            foreach (var child in node.InverseParentFkNavigation)
            {
                foreach (var leaf in GetLeafNodes(child))
                    yield return leaf;
            }
        }


        public ExecucaoOrcamentalListagemResponse GetExecucaoOrcamental(RelatorioExecucaoOrcamentalListagemRequest request)
        {
            ExecucaoOrcamentalListagemResponse response = new ExecucaoOrcamentalListagemResponse();

            int index = 0;
            if (request.filter.index.HasValue)
                index = request.filter.index.Value;

            int rows = 5;
            if (request.filter.rows.HasValue)
                rows = request.filter.rows.Value;

            var now = DateTime.Now;
            var lastYearBegin = new DateTime(now.Year - 1, 1, 1);
            var lastYearEnd = new DateTime(now.Year - 1, 12, 31);

            var currentYearBegin = new DateTime(now.Year, 1, 1);
            var currentYearEnd = new DateTime(now.Year, 12, 31);

            var listaContas = _moduloContribuicoesContext.Agrupamentoconfig
                .Where(e => e.IndActivo &&
                            // Filtrar por tipo de conta
                            e.ReltipoDeContaOrcamentoConfigFkNavigation.TipoContaFk == request.tipoConta &&
                            // Filtrar pelo ano do orçamento
                            e.Componenteorcamentovalor.Any(a => a.ComponenteOrcamentoRegistoFkNavigation.Aprovado 
                            && a.ComponenteOrcamentoRegistoFkNavigation.DataInicio.Year <= request.year 
                            && a.ComponenteOrcamentoRegistoFkNavigation.DataFim.Year >= request.year
                            && a.InstitutionId == request.institution)
                );

            var contas = listaContas
                .Include(e => e.Componenteorcamentovalor)
                .ThenInclude(e => e.CentroCustoFkNavigation)
                .Include(e => e.Componenteorcamentovalor)
                .ThenInclude(e => e.InstitutionFkNavigation)
                .Include(e => e.Componenteorcamentovalor)
                .ThenInclude(e => e.ComponenteOrcamentoRegistoFkNavigation)
                .Include(e => e.ComponentedespesaRegisto)
                .ThenInclude(e => e.Compromisso)
                .ThenInclude(e => e.Pagamentosexecutados)
                .Include(e => e.InverseParentFkNavigation)
                .OrderBy(request.filter.orderBy, request.filter.orderDirection)
                .Skip(index * rows)
                .Take(rows)
                .ToList()
                .Select(e => new ExecucaoOrcamentalDataContract()
                {
                    contaOGE = e.Codigo + " - " + e.Designacao,
                    // Nome da instituição (INSS/FRSS) associada ao valor orçamentado filtrado
                    instiutiton = e.Componenteorcamentovalor.Where(a => a.InstitutionId == request.institution && a.InstitutionFkNavigation != null).Select(a => a.InstitutionFkNavigation.Nome).FirstOrDefault(),
                    // Lista de centros de custo
                    centrosCusto = e.Componenteorcamentovalor.Where(a => a.CentroCustoFk.HasValue).Select(a => a.CentroCustoFkNavigation.Descricao),
                    // Rubricas do agrupamento (códigos do 3º nível do agrupamento - InverseParentFkNavigation são os filhos)
                    //rubricas = e.ParentFk != null ? e.InverseParentFkNavigation.SelectMany(a => a.InverseParentFkNavigation.Select(s => s.Codigo)).Distinct() : new List<string>(),
                    rubricas = GetLeafNodes(e).Select(x => x.Codigo).Distinct().ToList(),

                    valorOrcamentoInicial = e.Componenteorcamentovalor.Where(a => a.ComponenteOrcamentoRegistoFkNavigation.DataInicio.Year <= request.year && a.ComponenteOrcamentoRegistoFkNavigation.DataFim.Year >= request.year).Select(a => a.Valor).First(),
                    valorOrcamentado = e.Componenteorcamentovalor.Where(a => a.ComponenteOrcamentoRegistoFkNavigation.Aprovado && a.ComponenteOrcamentoRegistoFkNavigation.DataInicio.Year <= request.year && a.ComponenteOrcamentoRegistoFkNavigation.DataFim.Year >= request.year).Select(a => a.Valor).Last(),
                    // Soma dos pagamentos executados do ano anterior ao filtro
                    valorAnoAnterior = e.ComponentedespesaRegisto.Select(x => x.Compromisso.SelectMany(a => a.Pagamentosexecutados.Where(s => s.IndActivo && s.DataCriacao.Date >= lastYearBegin && s.DataCriacao.Date <= lastYearEnd)).Sum(s => s.ValorExecutado)).Sum(),
                    // Soma das despesas cabimentadas (ESTADODESPESA = 89 = 'C')
                    cabimentos = e.ComponentedespesaRegisto.Where(x => x.IndActivo && x.Estado == 89 && x.DataCriacao.Year == request.year).Sum(x => x.Valor),
                    // Soma dos compromissos assumidos
                    compromissos = e.ComponentedespesaRegisto.SelectMany(x => x.Compromisso.Where(c => c.IndActivo && c.DataCriacao.Year == request.year)).Sum(c => c.Valor),
                    // Soma dos pagamentos com obrigação registada (proxy: não existe entidade Obrigação dedicada)
                    obrigacoes = e.ComponentedespesaRegisto.Select(x => x.Compromisso.SelectMany(a => a.Pagamentosexecutados.Where(s => s.IndActivo && s.DataObrigacao.HasValue && s.DataObrigacao.Value.Year == request.year)).Sum(s => s.ValorExecutado)).Sum(),
                    // Soma dos pagamentos executados de janeiro
                    janeiro = e.ComponentedespesaRegisto.Select(x => x.Compromisso.SelectMany(a => a.Pagamentosexecutados.Where(s => s.IndActivo && s.DataCriacao.Month == 1 && s.DataCriacao.Year == now.Year)).Sum(s => s.ValorExecutado)).Sum(),
                    // Soma dos pagamentos executados de fevereiro
                    fevereiro = e.ComponentedespesaRegisto.Select(x => x.Compromisso.SelectMany(a => a.Pagamentosexecutados.Where(s => s.IndActivo && s.DataCriacao.Month == 2 && s.DataCriacao.Year == now.Year)).Sum(s => s.ValorExecutado)).Sum(),
                    // Soma dos pagamentos executados de março
                    marco = e.ComponentedespesaRegisto.Select(x => x.Compromisso.SelectMany(a => a.Pagamentosexecutados.Where(s => s.IndActivo && s.DataCriacao.Month == 3 && s.DataCriacao.Year == now.Year)).Sum(s => s.ValorExecutado)).Sum(),
                    // Soma dos pagamentos executados de abril
                    abril = e.ComponentedespesaRegisto.Select(x => x.Compromisso.SelectMany(a => a.Pagamentosexecutados.Where(s => s.IndActivo && s.DataCriacao.Month == 4 && s.DataCriacao.Year == now.Year)).Sum(s => s.ValorExecutado)).Sum(),
                    // Soma dos pagamentos executados de maio
                    maio = e.ComponentedespesaRegisto.Select(x => x.Compromisso.SelectMany(a => a.Pagamentosexecutados.Where(s => s.IndActivo && s.DataCriacao.Month == 5 && s.DataCriacao.Year == now.Year)).Sum(s => s.ValorExecutado)).Sum(),
                    // Soma dos pagamentos executados de junho
                    junho = e.ComponentedespesaRegisto.Select(x => x.Compromisso.SelectMany(a => a.Pagamentosexecutados.Where(s => s.IndActivo && s.DataCriacao.Month == 6 && s.DataCriacao.Year == now.Year)).Sum(s => s.ValorExecutado)).Sum(),
                    // Soma dos pagamentos executados de julho
                    julho = e.ComponentedespesaRegisto.Select(x => x.Compromisso.SelectMany(a => a.Pagamentosexecutados.Where(s => s.IndActivo && s.DataCriacao.Month == 7 && s.DataCriacao.Year == now.Year)).Sum(s => s.ValorExecutado)).Sum(),
                    // Soma dos pagamentos executados de agosto
                    agosto = e.ComponentedespesaRegisto.Select(x => x.Compromisso.SelectMany(a => a.Pagamentosexecutados.Where(s => s.IndActivo && s.DataCriacao.Month == 8 && s.DataCriacao.Year == now.Year)).Sum(s => s.ValorExecutado)).Sum(),
                    // Soma dos pagamentos executados de setembro
                    setembro = e.ComponentedespesaRegisto.Select(x => x.Compromisso.SelectMany(a => a.Pagamentosexecutados.Where(s => s.IndActivo && s.DataCriacao.Month == 9 && s.DataCriacao.Year == now.Year)).Sum(s => s.ValorExecutado)).Sum(),
                    // Soma dos pagamentos executados de outubro
                    outubro = e.ComponentedespesaRegisto.Select(x => x.Compromisso.SelectMany(a => a.Pagamentosexecutados.Where(s => s.IndActivo && s.DataCriacao.Month == 10 && s.DataCriacao.Year == now.Year)).Sum(s => s.ValorExecutado)).Sum(),
                    // Soma dos pagamentos executados de novembro
                    novembro = e.ComponentedespesaRegisto.Select(x => x.Compromisso.SelectMany(a => a.Pagamentosexecutados.Where(s => s.IndActivo && s.DataCriacao.Month == 11 && s.DataCriacao.Year == now.Year)).Sum(s => s.ValorExecutado)).Sum(),
                    // Soma dos pagamentos executados de dezembro
                    dezembro = e.ComponentedespesaRegisto.Select(x => x.Compromisso.SelectMany(a => a.Pagamentosexecutados.Where(s => s.IndActivo && s.DataCriacao.Month == 12 && s.DataCriacao.Year == now.Year)).Sum(s => s.ValorExecutado)).Sum()
                })
                .ToList();

            contas.ForEach(conta =>
            {
                conta.centrosCusto = conta.centrosCusto.Distinct();
                conta.totalExecucao = conta.janeiro + conta.fevereiro + conta.marco + conta.abril + conta.maio + conta.junho + conta.julho + conta.agosto + conta.setembro + conta.outubro + conta.novembro + conta.dezembro;
                conta.taxaExecucao = conta.valorOrcamentado == 0 || conta.totalExecucao == 0 ? 0 : conta.totalExecucao / conta.valorOrcamentado;
                conta.variacaoExecucao = conta.valorAnoAnterior == 0 || conta.totalExecucao - conta.valorAnoAnterior == 0 ? 0 : (conta.totalExecucao - conta.valorAnoAnterior) / conta.valorAnoAnterior;
            });

            var totalNumber = listaContas.Count();
            response.rows = totalNumber;
            response.lista = contas;

            return response;
        }

        /// <summary>
        /// Relatório CE_OSS_Global (Despesa): agrupa por mã Classificação Económica (501-506), não por Conta OGE/Programa.
        /// Resolve cada Conta OGE (esquema antigo 01-12 OU esquema novo/CE real 50-56, ambos já existentes em AGRUPAMENTOCONFIG)
        /// para o seu nó-raiz de Classificação Económica -- diretamente se já estiver no esquema novo, ou via a tabela
        /// de correspondência provisória RELAGRUPAMENTOCONFIGCLASSIFICACAOECONOMICA caso contrário. Ver plano/memória
        /// "ce-oss-global-prod-dev-feasibility" para o racional completo.
        /// </summary>
        public ClassificacaoEconomicaExecucaoListagemResponse GetExecucaoOrcamentalPorClassificacaoEconomica(RelatorioClassificacaoEconomicaRequest request)
        {
            ClassificacaoEconomicaExecucaoListagemResponse response = new ClassificacaoEconomicaExecucaoListagemResponse();

            int tipoContaDespesaId = _moduloContribuicoesContext.Dominio
                .Where(d => d.Dominio1 == "TIPOCONTA" && d.Descricao == "Despesa")
                .Select(d => d.IdDominio)
                .First();

            var allNodes = _moduloContribuicoesContext.Agrupamentoconfig
                .Where(a => a.IndActivo && a.ReltipoDeContaOrcamentoConfigFkNavigation.TipoContaFk == tipoContaDespesaId)
                .Include(a => a.Componenteorcamentovalor)
                    .ThenInclude(v => v.ComponenteOrcamentoRegistoFkNavigation)
                .Include(a => a.ComponentedespesaRegisto)
                    .ThenInclude(d => d.Compromisso)
                    .ThenInclude(c => c.Pagamentosexecutados)
                .ToList()
                .ToDictionary(a => a.Id);

            var crosswalk = _moduloContribuicoesContext.RelAgrupamentoConfigClassificacaoEconomica
                .Where(c => c.IndActivo)
                .ToDictionary(c => c.AgrupamentoConfigOrigemFk, c => c.AgrupamentoConfigCeFk);

            Agrupamentoconfig GetRootAncestor(Agrupamentoconfig node)
            {
                var current = node;
                while (current.ParentFk.HasValue && allNodes.ContainsKey(current.ParentFk.Value))
                {
                    current = allNodes[current.ParentFk.Value];
                }
                return current;
            }

            // Código completo (concatena os códigos de todos os ancestrais, mesmo padrão de GetFullCodigoTransactionless)
            string GetFullCode(Agrupamentoconfig node)
            {
                var sb = new System.Text.StringBuilder(node.Codigo);
                var current = node;
                while (current.ParentFk.HasValue && allNodes.ContainsKey(current.ParentFk.Value))
                {
                    current = allNodes[current.ParentFk.Value];
                    sb.Insert(0, current.Codigo);
                }
                return sb.ToString();
            }

            int GetDepth(Agrupamentoconfig node)
            {
                int depth = 0;
                var current = node;
                while (current.ParentFk.HasValue && allNodes.ContainsKey(current.ParentFk.Value))
                {
                    current = allNodes[current.ParentFk.Value];
                    depth++;
                }
                return depth;
            }

            // Resolve para o nó CE exato (não só a raiz) -- preserva o nível real quando o registo já é do
            // esquema novo; usa o alvo exato do crosswalk (pode ser raiz ou mais fundo) quando é esquema antigo
            Agrupamentoconfig ResolveToEconomicTarget(int? id)
            {
                if (!id.HasValue || !allNodes.ContainsKey(id.Value)) return null;

                var node = allNodes[id.Value];
                var root = GetRootAncestor(node);

                // Esquema CE já é o novo/real (mã 50-56) -- usa o próprio nó, no nível em que já está
                if (int.TryParse(root.Codigo, out int rootCode) && rootCode >= 50 && rootCode <= 56)
                {
                    return node;
                }

                // Esquema antigo -- sobe a cadeia de ancestrais (incluindo o próprio nó) à procura do crosswalk mais próximo
                var current = node;
                while (current != null)
                {
                    if (crosswalk.TryGetValue(current.Id, out int targetId) && allNodes.ContainsKey(targetId))
                    {
                        return allNodes[targetId];
                    }
                    current = current.ParentFk.HasValue && allNodes.ContainsKey(current.ParentFk.Value) ? allNodes[current.ParentFk.Value] : null;
                }

                return null; // sem correspondência -- fica fora do relatório até haver crosswalk
            }

            var resultado = new Dictionary<int, ClassificacaoEconomicaExecucaoDataContract>();

            ClassificacaoEconomicaExecucaoDataContract GetOrCreate(Agrupamentoconfig node)
            {
                if (!resultado.TryGetValue(node.Id, out var item))
                {
                    item = new ClassificacaoEconomicaExecucaoDataContract
                    {
                        codigoCE = GetFullCode(node),
                        designacaoCE = node.Designacao,
                        nivel = GetDepth(node)
                    };
                    resultado[node.Id] = item;
                }
                return item;
            }

            // Credita o valor no nó-alvo E em todos os seus ancestrais (rollup), para que cada nível
            // (raiz/sub/sub-sub) mostre a soma de tudo o que está por baixo dele, como no ficheiro Excel original
            void CreditAncestors(Agrupamentoconfig target, Action<ClassificacaoEconomicaExecucaoDataContract> apply)
            {
                var current = target;
                while (current != null)
                {
                    apply(GetOrCreate(current));
                    current = current.ParentFk.HasValue && allNodes.ContainsKey(current.ParentFk.Value) ? allNodes[current.ParentFk.Value] : null;
                }
            }

            // Orçamento (OSS inicial / OSS corrigido), por Conta OGE original, depois agregado por raiz CE
            var orcamentoPorConta = allNodes.Values
                .Where(a => a.Componenteorcamentovalor.Any(v => v.InstitutionId == request.institution
                    && v.ComponenteOrcamentoRegistoFkNavigation.DataInicio.Year <= request.year
                    && v.ComponenteOrcamentoRegistoFkNavigation.DataFim.Year >= request.year))
                .Select(a => new
                {
                    AgrupamentoId = a.Id,
                    ValorInicial = a.Componenteorcamentovalor
                        .Where(v => v.InstitutionId == request.institution
                            && v.ComponenteOrcamentoRegistoFkNavigation.DataInicio.Year <= request.year
                            && v.ComponenteOrcamentoRegistoFkNavigation.DataFim.Year >= request.year)
                        .Select(v => v.Valor).FirstOrDefault(),
                    ValorCorrigido = a.Componenteorcamentovalor
                        .Where(v => v.InstitutionId == request.institution
                            && v.ComponenteOrcamentoRegistoFkNavigation.Aprovado
                            && v.ComponenteOrcamentoRegistoFkNavigation.DataInicio.Year <= request.year
                            && v.ComponenteOrcamentoRegistoFkNavigation.DataFim.Year >= request.year)
                        .Select(v => v.Valor).LastOrDefault()
                })
                .ToList();

            foreach (var item in orcamentoPorConta)
            {
                var target = ResolveToEconomicTarget(item.AgrupamentoId);
                if (target == null) continue;
                CreditAncestors(target, ce =>
                {
                    ce.valorOrcamentoInicial += item.ValorInicial;
                    ce.valorOrcamentado += item.ValorCorrigido;
                });
            }

            // Execução mensal (Pagamentosexecutados), por Conta OGE original, depois agregado por raiz CE
            var despesaPorConta = allNodes.Values
                .Select(a => new
                {
                    AgrupamentoId = a.Id,
                    Janeiro = a.ComponentedespesaRegisto.Where(x => x.InstitutionId == request.institution).Select(x => x.Compromisso.SelectMany(c => c.Pagamentosexecutados.Where(p => p.IndActivo && p.DataCriacao.Month == 1 && p.DataCriacao.Year == request.year)).Sum(p => p.ValorExecutado)).Sum(),
                    Fevereiro = a.ComponentedespesaRegisto.Where(x => x.InstitutionId == request.institution).Select(x => x.Compromisso.SelectMany(c => c.Pagamentosexecutados.Where(p => p.IndActivo && p.DataCriacao.Month == 2 && p.DataCriacao.Year == request.year)).Sum(p => p.ValorExecutado)).Sum(),
                    Marco = a.ComponentedespesaRegisto.Where(x => x.InstitutionId == request.institution).Select(x => x.Compromisso.SelectMany(c => c.Pagamentosexecutados.Where(p => p.IndActivo && p.DataCriacao.Month == 3 && p.DataCriacao.Year == request.year)).Sum(p => p.ValorExecutado)).Sum(),
                    Abril = a.ComponentedespesaRegisto.Where(x => x.InstitutionId == request.institution).Select(x => x.Compromisso.SelectMany(c => c.Pagamentosexecutados.Where(p => p.IndActivo && p.DataCriacao.Month == 4 && p.DataCriacao.Year == request.year)).Sum(p => p.ValorExecutado)).Sum(),
                    Maio = a.ComponentedespesaRegisto.Where(x => x.InstitutionId == request.institution).Select(x => x.Compromisso.SelectMany(c => c.Pagamentosexecutados.Where(p => p.IndActivo && p.DataCriacao.Month == 5 && p.DataCriacao.Year == request.year)).Sum(p => p.ValorExecutado)).Sum(),
                    Junho = a.ComponentedespesaRegisto.Where(x => x.InstitutionId == request.institution).Select(x => x.Compromisso.SelectMany(c => c.Pagamentosexecutados.Where(p => p.IndActivo && p.DataCriacao.Month == 6 && p.DataCriacao.Year == request.year)).Sum(p => p.ValorExecutado)).Sum(),
                    Julho = a.ComponentedespesaRegisto.Where(x => x.InstitutionId == request.institution).Select(x => x.Compromisso.SelectMany(c => c.Pagamentosexecutados.Where(p => p.IndActivo && p.DataCriacao.Month == 7 && p.DataCriacao.Year == request.year)).Sum(p => p.ValorExecutado)).Sum(),
                    Agosto = a.ComponentedespesaRegisto.Where(x => x.InstitutionId == request.institution).Select(x => x.Compromisso.SelectMany(c => c.Pagamentosexecutados.Where(p => p.IndActivo && p.DataCriacao.Month == 8 && p.DataCriacao.Year == request.year)).Sum(p => p.ValorExecutado)).Sum(),
                    Setembro = a.ComponentedespesaRegisto.Where(x => x.InstitutionId == request.institution).Select(x => x.Compromisso.SelectMany(c => c.Pagamentosexecutados.Where(p => p.IndActivo && p.DataCriacao.Month == 9 && p.DataCriacao.Year == request.year)).Sum(p => p.ValorExecutado)).Sum(),
                    Outubro = a.ComponentedespesaRegisto.Where(x => x.InstitutionId == request.institution).Select(x => x.Compromisso.SelectMany(c => c.Pagamentosexecutados.Where(p => p.IndActivo && p.DataCriacao.Month == 10 && p.DataCriacao.Year == request.year)).Sum(p => p.ValorExecutado)).Sum(),
                    Novembro = a.ComponentedespesaRegisto.Where(x => x.InstitutionId == request.institution).Select(x => x.Compromisso.SelectMany(c => c.Pagamentosexecutados.Where(p => p.IndActivo && p.DataCriacao.Month == 11 && p.DataCriacao.Year == request.year)).Sum(p => p.ValorExecutado)).Sum(),
                    Dezembro = a.ComponentedespesaRegisto.Where(x => x.InstitutionId == request.institution).Select(x => x.Compromisso.SelectMany(c => c.Pagamentosexecutados.Where(p => p.IndActivo && p.DataCriacao.Month == 12 && p.DataCriacao.Year == request.year)).Sum(p => p.ValorExecutado)).Sum(),
                })
                .Where(x => x.Janeiro != 0 || x.Fevereiro != 0 || x.Marco != 0 || x.Abril != 0 || x.Maio != 0 || x.Junho != 0
                    || x.Julho != 0 || x.Agosto != 0 || x.Setembro != 0 || x.Outubro != 0 || x.Novembro != 0 || x.Dezembro != 0)
                .ToList();

            foreach (var item in despesaPorConta)
            {
                var target = ResolveToEconomicTarget(item.AgrupamentoId);
                if (target == null) continue;
                CreditAncestors(target, ce =>
                {
                    ce.janeiro += item.Janeiro;
                    ce.fevereiro += item.Fevereiro;
                    ce.marco += item.Marco;
                    ce.abril += item.Abril;
                    ce.maio += item.Maio;
                    ce.junho += item.Junho;
                    ce.julho += item.Julho;
                    ce.agosto += item.Agosto;
                    ce.setembro += item.Setembro;
                    ce.outubro += item.Outubro;
                    ce.novembro += item.Novembro;
                    ce.dezembro += item.Dezembro;
                });
            }

            // Garante que TODOS os nós da árvore CE (esquema novo/real, mã 50-56) aparecem no relatório,
            // mesmo com valor zero -- tal como no ficheiro Excel original do cliente
            foreach (var node in allNodes.Values)
            {
                var root = GetRootAncestor(node);
                if (int.TryParse(root.Codigo, out int rootCode) && rootCode >= 50 && rootCode <= 56)
                {
                    GetOrCreate(node);
                }
            }

            foreach (var ce in resultado.Values)
            {
                ce.totalExecucao = ce.janeiro + ce.fevereiro + ce.marco + ce.abril + ce.maio + ce.junho + ce.julho + ce.agosto + ce.setembro + ce.outubro + ce.novembro + ce.dezembro;
                ce.taxaExecucao = ce.valorOrcamentado == 0 || ce.totalExecucao == 0 ? 0 : ce.totalExecucao / ce.valorOrcamentado;
            }

            response.lista = resultado.Values.OrderBy(c => c.codigoCE).ToList();

            return response;
        }


        public List<ListaPagamentosDoProcessoDataContract> GetListaPagamentosByProcessoAtivoID(int processoAtivoId)
        {
            return _moduloContribuicoesContext.Pagamentosexecutados
                .Include(u => u.DestinatarioFkNavigation)
                .Include(u => u.CompromissoFkNavigation)
                .ThenInclude(u => u.ComponenteDespesaRegistoFkNavigation)
                .Where(u => u.IndActivo && u.ProcessoAtivoFk == processoAtivoId)
                 .Select(e => e.NumeroPagamento)
                 .Distinct()
                 .Select(u => new ListaPagamentosDoProcessoDataContract
                 {
                     NumeroPagamento = u
                 })


                .ToList();
        }

        public List<ListaPagamentosDoProcessoDataContract> GetListaPagamentoByNumPagamento(string numPagamento)
        {
            return _moduloContribuicoesContext.Pagamentosexecutados
                .Include(u => u.DestinatarioFkNavigation)
                .Include(u => u.CompromissoFkNavigation)
                .ThenInclude(u => u.ComponenteDespesaRegistoFkNavigation)
                .Where(u => u.IndActivo && u.NumeroPagamento == numPagamento)
                 .Select(u => new ListaPagamentosDoProcessoDataContract
                 {
                     Id = u.Id,
                     Valor = u.ValorExecutado,
                     Destinatario = u.DestinatarioFkNavigation,
                     Iban = u.Iban,
                     ProcessoId = u.ProcessoAtivoFk,
                     idContaOGE = u.CompromissoFkNavigation.ComponenteDespesaRegistoFkNavigation.AgrupamentoConfigFk,
                     NumeroConta = u.NumeroConta,
                     CodigoContaCredito = u.CodigoContaCreditoFk,
                     CodigoContaDebito = u.CodigoContaDebitoFk,
                     DataObrigacao = u.DataObrigacao,
                     BankCode = u.BankCode
                 })
                .ToList();
        }

        public ClassificacaoContabilisticaRelatorios ClassificacaoContabilisticaRelatorios(SearchFilterRequest request)
        {
            ClassificacaoContabilisticaRelatorios response = new ClassificacaoContabilisticaRelatorios();

            int index = 0;
            if (request.filter.index.HasValue)
                index = request.filter.index.Value;

            int rows = 5;
            if (request.filter.rows.HasValue)
                rows = request.filter.rows.Value;

            bool? filterByObrigacao = null;
            bool? filterByExecucao = null;

            if (request.filter?.filterBy != null)
            {
                filterByObrigacao = request.filter?.filterBy == "0";
                filterByExecucao = request.filter?.filterBy == "1";
            }

            var beginDate = request.filter.dateFilterBegin;
            var endDate = request.filter.dateFilterEnd;

            var listaPagamentos = _moduloContribuicoesContext.Pagamentosexecutados
                .Include(e => e.CodigoContaCreditoFkNavigation)
                .Include(e => e.CodigoContaDebitoFkNavigation)
                .Include(e => e.DestinatarioFkNavigation)
                .Where(e => e.IndActivo &&
                            (filterByObrigacao == true || filterByObrigacao == null) &&
                            // Estado obrigação ou superior
                            e.EstadoNavigation.Valor >= 1 && e.EstadoNavigation.Dominio1 == "ESTADOPAGAMENTO" &&
                            // Filtrar por datas
                            (beginDate.HasValue && endDate.HasValue ? e.DataObrigacao.Value.Date >= beginDate && e.DataObrigacao.Value.Date <= endDate :
                                beginDate.HasValue && !endDate.HasValue ? e.DataObrigacao.Value.Date == beginDate :
                                true)
                )
                .Select(e => new RelatorioClassificacaoContabilisticaDataContract()
                {
                    data = e.DataObrigacao,
                    credito = e.CodigoContaCreditoFkNavigation == null ? "" : e.CodigoContaCreditoFkNavigation.Codigo + " - " + e.CodigoContaCreditoFkNavigation.Designacao,
                    debito = e.CodigoContaDebitoFkNavigation == null ? "" : e.CodigoContaDebitoFkNavigation.Codigo + " - " + e.CodigoContaDebitoFkNavigation.Designacao,
                    valor = e.ValorExecutado,
                    numeroPagamento = e.NumeroPagamento,
                    nomeDestinatario = e.DestinatarioFkNavigation.Nome,
                    niss = e.DestinatarioFkNavigation == null ? "" : e.DestinatarioFkNavigation.Niss,
                    tin = e.DestinatarioFkNavigation == null ? "" : e.DestinatarioFkNavigation.Tin,
                    isExecucao = false
                });

            listaPagamentos = listaPagamentos.Concat(_moduloContribuicoesContext.Pagamentosexecutados
                .Include(e => e.CodigoContaCreditoFkNavigation)
                .Include(e => e.CodigoContaDebitoFkNavigation)
                .Include(e => e.DestinatarioFkNavigation)
                .Where(e => e.IndActivo &&
                            e.DataExecucao.HasValue &&
                            (filterByExecucao == true || filterByExecucao == null) &&
                            // Estado obrigação ou superior
                            e.EstadoNavigation.Valor >= 1 && e.EstadoNavigation.Dominio1 == "ESTADOPAGAMENTO" &&
                            // Filtrar por datas
                            (beginDate.HasValue && endDate.HasValue ? e.DataExecucao.Value.Date >= beginDate && e.DataExecucao.Value.Date <= endDate :
                                beginDate.HasValue && !endDate.HasValue ? e.DataExecucao.Value.Date == beginDate :
                                true)
                )
                .Select(e => new RelatorioClassificacaoContabilisticaDataContract()
                {
                    data = e.DataExecucao,
                    credito = e.CreditoExecucaoFkNavigation == null ? "" : e.CreditoExecucaoFkNavigation.Codigo + " - " + e.CreditoExecucaoFkNavigation.Designacao,
                    debito = e.DebitoExecucaoFkNavigation == null ? "" : e.DebitoExecucaoFkNavigation.Codigo + " - " + e.DebitoExecucaoFkNavigation.Designacao,
                    valor = e.ValorExecutado,
                    numeroPagamento = e.NumeroPagamento,
                    nomeDestinatario = e.DestinatarioFkNavigation.Nome,
                    niss = e.DestinatarioFkNavigation == null ? "" : e.DestinatarioFkNavigation.Niss,
                    tin = e.DestinatarioFkNavigation == null ? "" : e.DestinatarioFkNavigation.Tin,
                    isExecucao = true
                }));
                

            var receitas = listaPagamentos
                .OrderBy(request.filter.orderBy, request.filter.orderDirection)
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = listaPagamentos.Count();
            response.rows = totalNumber;
            response.movimentos = receitas;

            return response;
        }

        public string ClassificacaoContabilisticaRelatoriosExcel(SearchFilterRequest request, List<RelatorioClassificacaoContabilisticaDataContract> lista)
        {
            // Inicialização do documento excel
            var excelDocument = new ExcelDocument(_localizer["classificacaoContabilistica"].Value, new ExcelDocumentOptions()
            {
                TextStyles = Extensions.ServiceExtensions.ExcelDocumentTextStyles
            });

            // Adição dos títulos
            excelDocument.Pages[0].AddText(_localizer["classificacaoContabilistica"].Value, new ExcelDocumentTextPosition(1, 1), "Header", new ExcelDocumentTextPosition(8, 1));

            // Adição da tabela
            excelDocument.Pages[0].AddTable(new ExcelDocumentTextPosition(1, 3), lista, new List<ColumnOption<RelatorioClassificacaoContabilisticaDataContract>>()
            {
                new ColumnOption<RelatorioClassificacaoContabilisticaDataContract>()
                {
                    Name = _localizer["data"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellWrap",
                    Value = (data) => data.data?.ToString("dd/MM/yyyy"),
                    Width = 17,
                },
                new ColumnOption<RelatorioClassificacaoContabilisticaDataContract>()
                {
                    Name = _localizer["fase"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellWrap",
                    Value = (data) => data.isExecucao ? "Execução" : "Obrigação",
                    Width = 17,
                },
                new ColumnOption<RelatorioClassificacaoContabilisticaDataContract>()
                {
                    Name = _localizer["credito"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellWrap",
                    Value = (data) => data.credito,
                    Width = 23,
                },
                new ColumnOption<RelatorioClassificacaoContabilisticaDataContract>()
                {
                    Name = _localizer["debito"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellWrap",
                    Value = (data) => data.debito,
                    Width = 23,
                },
                new ColumnOption<RelatorioClassificacaoContabilisticaDataContract>()
                {
                    Name = _localizer["valor"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellMoney",
                    Value = (data) => data.valor
                },
                new ColumnOption<RelatorioClassificacaoContabilisticaDataContract>()
                {
                    Name = _localizer["numeroPagamento"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellWrap",
                    Value = (data) => data.numeroPagamento,
                    Width = 20,
                },
                new ColumnOption<RelatorioClassificacaoContabilisticaDataContract>()
                {
                    Name = _localizer["nome"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellWrap",
                    Value = (data) => data.nomeDestinatario,
                    Width = 19,
                },
                new ColumnOption<RelatorioClassificacaoContabilisticaDataContract>()
                {
                    Name = _localizer["niss"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellWrap",
                    Value = (data) => data.niss,
                    Width = 15,
                },
                new ColumnOption<RelatorioClassificacaoContabilisticaDataContract>()
                {
                    Name = _localizer["tin"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellWrap",
                    Value = (data) => data.tin
                },
            }, new TableOptions() { AutoFitColumns = false });

            // Adição da linha de Total como footer da tabela
            excelDocument.Pages[0].AddText(_localizer["total"].Value + ":", new ExcelDocumentTextPosition(1, 4 + lista.Count), "HeaderAlignRight", new ExcelDocumentTextPosition(3, 4 + lista.Count));

            var sumRange = new ExcelDocumentTextPositionRange(new ExcelDocumentTextPosition(4, 3), new ExcelDocumentTextPosition(4, 3 + lista.Count - 1));
            excelDocument.Pages[0].AddSum(new ExcelDocumentTextPosition(4, 4 + lista.Count), sumRange, "HeaderAlignLeftMoney", new ExcelDocumentTextPosition(8, 4 + lista.Count));

            // Conversão do documento excel em base64
            return excelDocument.GetFileString();
        }

        public FornecedoresRelatorios FornecedoresRelatorios(FornecedoresRelatoriosRequest request)
        {
            FornecedoresRelatorios response = new FornecedoresRelatorios();

            int index = 0;
            if (request.filter.index.HasValue)
                index = request.filter.index.Value;

            int rows = 5;
            if (request.filter.rows.HasValue)
                rows = request.filter.rows.Value;

            var beginDate = request.filter.dateFilterBegin;
            var endDate = request.filter.dateFilterEnd;

            var listaPagamentos = _moduloContribuicoesContext.Pagamentosexecutados
                .Include(e => e.CodigoContaCreditoFkNavigation)
                .Include(e => e.CodigoContaDebitoFkNavigation)
                .Include(e => e.DestinatarioFkNavigation)
                .Where(e => e.IndActivo &&
                            // Filtrar por TIN
                            (request.Tin == null || e.DestinatarioFkNavigation.Tin.Contains(request.Tin)) &&
                            // Filtrar por NISS
                            (request.Niss == null || e.DestinatarioFkNavigation.Niss.Contains(request.Niss)) &&
                            // Estado obrigação ou superior
                            e.EstadoNavigation.Valor >= 1 && e.EstadoNavigation.Dominio1 == "ESTADOPAGAMENTO" &&
                            // Filtrar por datas
                            (beginDate.HasValue && endDate.HasValue ? e.DataObrigacao.Value.Date >= beginDate && e.DataObrigacao.Value.Date <= endDate :
                                beginDate.HasValue && !endDate.HasValue ? e.DataObrigacao.Value.Date == beginDate :
                                true)
                )
                .Select(e => new RelatorioFornecedoresDataContract()
                {
                    niss = e.DestinatarioFkNavigation == null ? "" : e.DestinatarioFkNavigation.Niss,
                    tin = e.DestinatarioFkNavigation == null ? "" : e.DestinatarioFkNavigation.Tin,
                    nome = e.DestinatarioFkNavigation.Nome,
                    pagamento = e.NumeroPagamento,
                    descricao = e.CompromissoFkNavigation.ComponenteDespesaRegistoFkNavigation.Descricao,
                    valor = e.ValorExecutado,
                    data = e.DataObrigacao,
                    conciliado = e.RelMovimentosporconciliarMovimentos.Any(e => e.IndActivo.Value)

                });

            var receitas = listaPagamentos
                .OrderBy(request.filter.orderBy, request.filter.orderDirection)
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = listaPagamentos.Count();
            response.rows = totalNumber;
            response.movimentos = receitas;

            return response;
        }

        public string FornecedoresRelatoriosExcel(FornecedoresRelatoriosRequest request, List<RelatorioFornecedoresDataContract> lista)
        {
            // Inicialização do documento excel
            var excelDocument = new ExcelDocument(_localizer["consultaFornecedores"].Value, new ExcelDocumentOptions()
            {
                TextStyles = Extensions.ServiceExtensions.ExcelDocumentTextStyles
            });

            // Adição dos títulos
            excelDocument.Pages[0].AddText(_localizer["consultaFornecedores"].Value, new ExcelDocumentTextPosition(1, 1), "Header", new ExcelDocumentTextPosition(8, 1));
            excelDocument.Pages[0].AddText(_localizer["niss"].Value + ": " + request.Niss, new ExcelDocumentTextPosition(1, 2), "Header", new ExcelDocumentTextPosition(8, 2));
            excelDocument.Pages[0].AddText(_localizer["tin"].Value + ": " + request.Tin, new ExcelDocumentTextPosition(1, 3), "Header", new ExcelDocumentTextPosition(8, 3));

            // Adição da tabela
            excelDocument.Pages[0].AddTable(new ExcelDocumentTextPosition(1, 5), lista, new List<ColumnOption<RelatorioFornecedoresDataContract>>()
            {
                new ColumnOption<RelatorioFornecedoresDataContract>()
                {
                    Name = _localizer["niss"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellWrap",
                    Value = (data) => data.niss,
                    Width = 15,
                },
                new ColumnOption<RelatorioFornecedoresDataContract>()
                {
                    Name = _localizer["tin"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellWrap",
                    Value = (data) => data.tin
                },
                new ColumnOption<RelatorioFornecedoresDataContract>()
                {
                    Name = _localizer["nome"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellWrap",
                    Value = (data) => data.nome
                },
                new ColumnOption<RelatorioFornecedoresDataContract>()
                {
                    Name = _localizer["numeroPagamento"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellWrap",
                    Value = (data) => data.pagamento,
                    Width = 20,
                },
                new ColumnOption<RelatorioFornecedoresDataContract>()
                {
                    Name = _localizer["descricao"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellWrap",
                    Value = (data) => data.descricao,
                    Width = 15,
                },
                new ColumnOption<RelatorioFornecedoresDataContract>()
                {
                    Name = _localizer["valor"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellMoney",
                    Value = (data) => data.valor
                },
                new ColumnOption<RelatorioFornecedoresDataContract>()
                {
                    Name = _localizer["dataObrigacao"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellWrap",
                    Value = (data) => data.data?.ToString("dd/MM/yyyy"),
                    Width = 17,
                },
                new ColumnOption<RelatorioFornecedoresDataContract>()
                {
                    Name = _localizer["conciliado"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKeyFunc = (data) => data.conciliado ? "TableCellWrapGreenCenter" : "TableCellWrapRedCenter",
                    Value = (data) => data.conciliado ? "✔" : "✘",
                    Width = 15,
                }
            }, new TableOptions() { AutoFitColumns = false });

            // Adição da linha de Total como footer da tabela
            excelDocument.Pages[0].AddText(_localizer["total"].Value + ":", new ExcelDocumentTextPosition(1, 6 + lista.Count), "HeaderAlignRight", new ExcelDocumentTextPosition(5, 6 + lista.Count));

            var sumRange = new ExcelDocumentTextPositionRange(new ExcelDocumentTextPosition(6, 6), new ExcelDocumentTextPosition(6, 6 + lista.Count - 1));
            excelDocument.Pages[0].AddSum(new ExcelDocumentTextPosition(6, 6 + lista.Count), sumRange, "HeaderAlignLeftMoney", new ExcelDocumentTextPosition(8, 6 + lista.Count));

            // Conversão do documento excel em base64
            return excelDocument.GetFileString();
        }

        private bool BalancoRelatoriosCodigoContaSearch(Codigoconta codigoConta, int nivel)
        {
            var currLevel = 1;
            var current = codigoConta;
            while (true)
            {
                if (current.ParentFk == null)
                {
                    return true;
                }
                currLevel++;
                current = current.ParentFkNavigation;

                if (nivel < currLevel) return false;
            }
        }

        public BalancoRelatorios BalancoRelatorios(BalancoRelatoriosRequest request)
        {
            BalancoRelatorios response = new BalancoRelatorios();

            var beginDate = request.filter.dateFilterBegin;
            var endDate = request.filter.dateFilterEnd;

            var listaContas = _moduloContribuicoesContext.Codigoconta
                    .Include(e => e.PagamentosexecutadosCodigoContaCreditoFkNavigation)
                    .Include(e => e.PagamentosexecutadosCodigoContaDebitoFkNavigation)
                    .Select(e => new RelatorioBalancoDataContract()
                    {
                        id = e.Id,
                        parentId = e.ParentFk,
                        nome = e.Designacao,
                        credito = e.PagamentosexecutadosCodigoContaCreditoFkNavigation.Where(e => e.DataObrigacao >= beginDate.Value.Date && e.DataObrigacao <= endDate.Value.Date).Select(e => e.ValorExecutado).Sum(),
                        debito = e.PagamentosexecutadosCodigoContaDebitoFkNavigation.Where(e => e.DataObrigacao >= beginDate.Value.Date && e.DataObrigacao <= endDate.Value.Date).Select(e => e.ValorExecutado).Sum(),
                        codigo = e.Codigo,
                    })
                    .ToList();

            listaContas = BalancoResolveOrder(listaContas);
            BalancoResolve(listaContas.Where(e => e.parentId == null).ToList(), listaContas, request.Nivel);

            response.lista = listaContas;

            return response;
        }

        public string BalancoRelatoriosExcel(BalancoRelatoriosRequest request, List<RelatorioBalancoDataContract> lista)
        {
            // Inicialização do documento excel
            var excelDocument = new ExcelDocument(_localizer["consultaBalanco"].Value, new ExcelDocumentOptions()
            {
                TextStyles = Extensions.ServiceExtensions.ExcelDocumentTextStyles
            });

            // Adição dos títulos
            excelDocument.Pages[0].AddText(_localizer["consultaBalanco"].Value, new ExcelDocumentTextPosition(1, 1), "Header", new ExcelDocumentTextPosition(7, 1));
            excelDocument.Pages[0].AddText(_localizer["nivel"].Value + ": " + request.Nivel, new ExcelDocumentTextPosition(1, 2), "Header", new ExcelDocumentTextPosition(7, 2));
            excelDocument.Pages[0].AddText(_localizer["compararAnoAnterior"].Value + ": " + (request.CompararAnoAnterior ? _localizer["sim"].Value : _localizer["nao"].Value), new ExcelDocumentTextPosition(1, 3), "Header", new ExcelDocumentTextPosition(7, 3));

            // Colunas extra
            excelDocument.Pages[0].AddText(request.filter.dateFilterBegin.Value.ToString("dd/MM/yyyy") + " - " + request.filter.dateFilterEnd.Value.ToString("dd/MM/yyyy"), new ExcelDocumentTextPosition(2, 5), "Header", new ExcelDocumentTextPosition(3, 6));
            excelDocument.Pages[0].AddText(request.filter.dateFilterBegin.Value.AddYears(-1).ToString("dd/MM/yyyy") + " - " + request.filter.dateFilterEnd.Value.AddYears(-1).ToString("dd/MM/yyyy"), new ExcelDocumentTextPosition(4, 5), "Header", new ExcelDocumentTextPosition(5, 6));
            excelDocument.Pages[0].AddText("%", new ExcelDocumentTextPosition(6, 5), "Header", new ExcelDocumentTextPosition(7, 6));

            // Adição da tabela
            excelDocument.Pages[0].AddTable(new ExcelDocumentTextPosition(1, 5), lista, new List<ColumnOption<RelatorioBalancoDataContract>>()
            {
                new ColumnOption<RelatorioBalancoDataContract>()
                {
                    Name = _localizer["conta"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellWrap",
                    Value = (data) => data.codigo + " - " + data.nome,
                    RowSpan = 2,
                    Width = 28,
                },
                new ColumnOption<RelatorioBalancoDataContract>()
                {
                    Name = _localizer["receita"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellMoney",
                    Value = (data) => data.credito,
                    Width = 19
                },
                new ColumnOption<RelatorioBalancoDataContract>()
                {
                    Name = _localizer["despesa"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellMoney",
                    Value = (data) => data.debito,
                    Width = 19
                },
                new ColumnOption<RelatorioBalancoDataContract>()
                {
                    Name = _localizer["receita"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellMoney",
                    Value = (data) => data.creditoAntes,
                    Width = 19,
                },
                new ColumnOption<RelatorioBalancoDataContract>()
                {
                    Name = _localizer["despesa"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellMoney",
                    Value = (data) => data.debitoAntes,
                    Width = 19,
                },
                new ColumnOption<RelatorioBalancoDataContract>()
                {
                    Name = _localizer["receita"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellWrap",
                    Value = (data) => request.CompararAnoAnterior && data.credito != 0 && data.creditoAntes != 0 ? (data.credito / data.creditoAntes * 100) + "%" : "",
                    Width= 19
                },
                new ColumnOption<RelatorioBalancoDataContract>()
                {
                    Name = _localizer["despesa"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellWrap",
                    Value = (data) => request.CompararAnoAnterior && data.debito != 0 && data.debitoAntes != 0 ? (data.debito / data.debitoAntes * 100) + "%" : "",
                    Width = 19,
                },
            }, new TableOptions() { AutoFitColumns = false });

            var totalCredito = lista.Where(e => e.parentId == null).Sum(e => e.credito);
            var totalDebito = lista.Where(e => e.parentId == null).Sum(e => e.debito);
            var totalCreditoAntes = request.CompararAnoAnterior ? lista.Where(e => e.parentId == null).Sum(e => e.creditoAntes) : (decimal?)null;
            var totalDebitoAntes = request.CompararAnoAnterior ? lista.Where(e => e.parentId == null).Sum(e => e.debitoAntes) : (decimal?)null;

            // Adição da linha de Total como footer da tabela
            excelDocument.Pages[0].AddText(_localizer["total"].Value + ":", new ExcelDocumentTextPosition(1, 8 + lista.Count), "HeaderWrap");
            excelDocument.Pages[0].AddText(totalCredito, new ExcelDocumentTextPosition(2, 8 + lista.Count), "HeaderAlignLeftMoney");
            excelDocument.Pages[0].AddText(totalDebito, new ExcelDocumentTextPosition(3, 8 + lista.Count), "HeaderAlignLeftMoney");
            excelDocument.Pages[0].AddText(totalCreditoAntes, new ExcelDocumentTextPosition(4, 8 + lista.Count), "HeaderAlignLeftMoney");
            excelDocument.Pages[0].AddText(totalDebitoAntes, new ExcelDocumentTextPosition(5, 8 + lista.Count), "HeaderAlignLeftMoney");
            excelDocument.Pages[0].AddText(totalCreditoAntes != 0 && totalCreditoAntes != null ? (totalCredito / totalCreditoAntes * 100) + "%" : "", new ExcelDocumentTextPosition(6, 8 + lista.Count), "HeaderAlignLeftMoney");
            excelDocument.Pages[0].AddText(totalDebitoAntes != 0 && totalDebitoAntes != null ? (totalDebito / totalDebitoAntes * 100) + "%" : "", new ExcelDocumentTextPosition(7, 8 + lista.Count), "HeaderAlignLeftMoney");

            excelDocument.Pages[0].AddText("", new ExcelDocumentTextPosition(1, 9 + lista.Count), "HeaderWrap");
            excelDocument.Pages[0].AddText(totalCredito != 0 && totalDebito != 0 ? (totalCredito / totalDebito * 100 + "%") : "", new ExcelDocumentTextPosition(2, 9 + lista.Count), "HeaderWrap", new ExcelDocumentTextPosition(3, 9 + lista.Count));
            excelDocument.Pages[0].AddText(request.CompararAnoAnterior && totalCreditoAntes != 0 && totalDebitoAntes != 0 ? (totalCreditoAntes / totalDebitoAntes * 100 + "%") : "", new ExcelDocumentTextPosition(4, 9 + lista.Count), "HeaderWrap", new ExcelDocumentTextPosition(5, 9 + lista.Count));
            excelDocument.Pages[0].AddText("", new ExcelDocumentTextPosition(6, 9 + lista.Count), "HeaderWrap", new ExcelDocumentTextPosition(7, 9 + lista.Count));

            // Conversão do documento excel em base64
            return excelDocument.GetFileString();
        }

        #region Balanco Utils
        private List<RelatorioBalancoDataContract> BalancoResolveOrder(List<RelatorioBalancoDataContract> lista)
        {

            List<RelatorioBalancoDataContract> ordered = new List<RelatorioBalancoDataContract>();
            var filhos = lista.Where(e => e.parentId == null).ToList();
            BalancoResolveOrderRecursive(lista, filhos, ordered);
            return ordered;
        }

        private void BalancoResolveOrderRecursive(List<RelatorioBalancoDataContract> lista, List<RelatorioBalancoDataContract> filhos, List<RelatorioBalancoDataContract> ordered, string codigoPai = "")
        {
            foreach (var item in filhos)
            {
                item.codigo = codigoPai + item.codigo;
                ordered.Add(item);

                var novosFilhos = lista.Where(e => e.parentId == item.id).ToList();

                BalancoResolveOrderRecursive(lista, novosFilhos, ordered, item.codigo);
            }
        }

        private void BalancoResolve(List<RelatorioBalancoDataContract> lista, List<RelatorioBalancoDataContract> listaCompleta, int nivel, int nivelAtual = 1)
        {
            var toRemove = new List<int>();
            foreach (var item in lista)
            {
                var filhos = listaCompleta.Where(e => e.parentId == item.id).ToList();

                if (nivelAtual > nivel) toRemove.Add(item.parentId.Value);
                else
                {
                    item.credito = item.credito += BalancoGetChildrenCredito(filhos, listaCompleta);
                    item.debito = item.debito += BalancoGetChildrenDebito(filhos, listaCompleta);
                }
                if (filhos.Any()) BalancoResolve(filhos, listaCompleta, nivel, nivelAtual + 1);
            }

            foreach (var item in toRemove)
            {
                listaCompleta.RemoveAll(e => e.parentId == item);
            }
        }

        private decimal BalancoGetChildrenCredito(List<RelatorioBalancoDataContract> lista, List<RelatorioBalancoDataContract> listaOriginal)
        {
            decimal value = 0;

            if (lista == null || !lista.Any()) return 0;


            foreach (var item in lista)
            {
                var filhos = listaOriginal.Where(e => e.parentId == item.id).ToList();

                value += item.credito;
                if (filhos.Any()) value += BalancoGetChildrenCredito(filhos, listaOriginal);
            }

            return value;
        }

        private decimal BalancoGetChildrenDebito(List<RelatorioBalancoDataContract> lista, List<RelatorioBalancoDataContract> listaOriginal)
        {
            decimal value = 0;

            if (lista == null || !lista.Any()) return 0;

            foreach (var item in lista)
            {
                var filhos = listaOriginal.Where(e => e.parentId == item.id).ToList();

                value += item.debito;
                if (filhos.Any()) value += BalancoGetChildrenDebito(filhos, listaOriginal);
            }

            return value;
        }
        #endregion


    }
}