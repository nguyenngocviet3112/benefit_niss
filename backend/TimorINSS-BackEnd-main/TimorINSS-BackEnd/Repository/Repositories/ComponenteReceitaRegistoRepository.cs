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
    public class ComponenteReceitaRegistoRepository : IComponenteReceitaRegistoRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public ComponenteReceitaRegistoRepository(TimorINSSModuloContribuicoesContext storeContext, IStringLocalizer<SharedResource> localizer)
        {
            _moduloContribuicoesContext = storeContext;
            _localizer = localizer;
        }

        public IEnumerable<ComponentereceitaRegisto> GetAll()
        {
            return _moduloContribuicoesContext.ComponentereceitaRegisto.ToList();
        }

        public ComponentereceitaRegisto Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var componenteReceitaRegisto = _moduloContribuicoesContext.ComponentereceitaRegisto
                .Include(e => e.ComponentereceitaRegistoMovimentos)
                .SingleOrDefault(u => u.Id == id);

            return componenteReceitaRegisto;
        }

        public ComponenteReceitaRegistoDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var componenteReceitaRegisto = _moduloContribuicoesContext.ComponentereceitaRegisto
                .SingleOrDefault(u => u.Id == id);

            ComponenteReceitaRegistoDto componenteReceitaRegistoDto = Utils.MappClassToDto<ComponentereceitaRegisto, ComponenteReceitaRegistoDto>(componenteReceitaRegisto);
            return componenteReceitaRegistoDto;
        }

        public void Add(ComponentereceitaRegisto entity)
        {
            _moduloContribuicoesContext.ComponentereceitaRegisto.Add(entity);
        }

        public void Update(ComponentereceitaRegisto entity)
        {
            ComponentereceitaRegisto entityToUpdate = _moduloContribuicoesContext.ComponentereceitaRegisto
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(ComponentereceitaRegisto entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public ComponentesReceitaRegistoResponseDataContract GetComponenteReceitaRegistoByContaOSSId(GetComponenteReceitaRegistoByIdContaOSSRequest request)
        {
            ComponentesReceitaRegistoResponseDataContract result = new ComponentesReceitaRegistoResponseDataContract();

            int index = 0;
            if (request.filter.index.HasValue)
                index = request.filter.index.Value;

            int rows = 5;
            if (request.filter.rows.HasValue)
                rows = request.filter.rows.Value;

            var query = _moduloContribuicoesContext.ComponentereceitaRegisto
                .Include(e => e.ComponentereceitaRegistoMovimentos.Where(a => a.IndActivo.Value))
               .Where(e => e.IndActivo && e.AgrupamentoConfigFk == request.ContaOSSId)
               .Select(e => new ComponenteReceita
               {
                   Id = e.Id,
                   DepartamentoFk = (int)e.DepartamentoFk,
                   CentroCustoFk = e.CentroCustoFk,
                   TipoContaFk = e.TipoContaFk,
                   CodigoContaFk = e.CodigoContaFk,
                   AgrupamentoConfigFk = e.AgrupamentoConfigFk,
                   InstitutionId = e.InstitutionId ?? 0,
                   Descricao = e.Descricao,
                   Valor = e.Valor,
                   MovimentosIds = e.ComponentereceitaRegistoMovimentos.Select(a => a.RelMovimentosPorConciliarMovimentosId)
               });

            var receita = query
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = query.Count();

            result.ComponenteReceita = receita;
            result.rows = totalNumber;

            return result;
        }

        public List<ComponentereceitaRegisto> GetComponenteReceitaRegistoByIdOrcamento(int idOrcamentoRegisto, DateTime dataInicio, DateTime dataFim)
        {
            return _moduloContribuicoesContext.ComponentereceitaRegisto
                .Where(u => u.IndActivo && u.ComponenteOrcamentoRegistoFk == idOrcamentoRegisto
                && u.DataCriacao >= dataInicio && u.DataCriacao < dataFim)
                .ToList();

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
                            && a.ComponenteOrcamentoRegistoFkNavigation.DataFim.Year >= request.year)
                            && e.Componenteorcamentovalor.Any(a =>
                            a.ComponenteOrcamentoRegistoFkNavigation.Aprovado
                                && (a.InstitutionId == request.institution)
                            )
                //&& e.Componenteorcamentovalor.Instituti
                );

            var contas = listaContas
                .Include(e => e.Componenteorcamentovalor)
                .ThenInclude(e => e.CentroCustoFkNavigation)
                .Include(e => e.Componenteorcamentovalor)
                .ThenInclude(e => e.InstitutionFkNavigation)
                .Include(e => e.Componenteorcamentovalor)
                .ThenInclude(e => e.ComponenteOrcamentoRegistoFkNavigation)
                .Include(e => e.ComponentereceitaRegisto)
                .ThenInclude(e => e.ComponentereceitaRegistoMovimentos)
                .ThenInclude(e => e.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation)
          
                .Include(e => e.InverseParentFkNavigation)
                .OrderBy(request.filter.orderBy, request.filter.orderDirection)
                .Skip(index * rows)
                .Take(rows)
                .ToList()
                .Select(e => new ExecucaoOrcamentalDataContract()
                {
                    contaOGE = e.Codigo + " - " + e.Designacao,
                    instiutiton = e.Componenteorcamentovalor.Where(a => a.InstitutionFkNavigation != null).Select(a => a.InstitutionFkNavigation.Nome).FirstOrDefault(),
                    //instiutiton = e.Componenteorcamentovalor.InstitutionFkNavigation.,
                    // Lista de centros de custo
                    centrosCusto = e.Componenteorcamentovalor.Where(a => a.CentroCustoFk.HasValue).Select(a => a.CentroCustoFkNavigation.Descricao),
                    // Rubricas do agrupamento (códigos do 3º nível do agrupamento - InverseParentFkNavigation são os filhos)
                    rubricas = e.ParentFk != null ? e.InverseParentFkNavigation.SelectMany(a => a.InverseParentFkNavigation.Select(s => s.Codigo)).Distinct() : new List<string>(),
                    valorOrcamentoInicial = e.Componenteorcamentovalor.Where(a => a.ComponenteOrcamentoRegistoFkNavigation.DataInicio.Year <= request.year && a.ComponenteOrcamentoRegistoFkNavigation.DataFim.Year >= request.year).Select(a => a.Valor).First(),
                    valorOrcamentado = e.Componenteorcamentovalor.Where(a => a.ComponenteOrcamentoRegistoFkNavigation.Aprovado && a.ComponenteOrcamentoRegistoFkNavigation.DataInicio.Year <= request.year && a.ComponenteOrcamentoRegistoFkNavigation.DataFim.Year >= request.year).Select(a => a.Valor).Last(),
                    // Soma dos pagamentos executados do ano anterior ao filtro
                    valorAnoAnterior = e.ComponentereceitaRegisto.Where(s => s.IndActivo && s.ComponentereceitaRegistoMovimentos.Any(a => a.IndActivo.Value && a.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor >= lastYearBegin && a.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor <= lastYearEnd)).Sum(s => s.ComponentereceitaRegistoMovimentos.Where(a => a.IndActivo.Value).Sum(a => a.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.Credito.Value)),
                    //valorAnoAnterior = e.ComponentereceitaRegisto.Where(s => s.IndActivo && s.MovimentosConciliadosFkNavigation.MovimentosBancariosFkNavigation.DataValor >= lastYearBegin && s.MovimentosConciliadosFkNavigation.MovimentosBancariosFkNavigation.DataValor <= lastYearEnd).Sum(s => s.MovimentosConciliadosFkNavigation.MovimentosBancariosFkNavigation.Credito.Value),
                    // Soma dos pagamentos executados de janeiro
                    janeiro = e.ComponentereceitaRegisto.Where(s => s.IndActivo && s.ComponentereceitaRegistoMovimentos.Any(a => a.IndActivo.Value && a.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Month == 1 && a.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Year == now.Year)).Sum(s => s.ComponentereceitaRegistoMovimentos.Where(a => a.IndActivo.Value).Sum(a => a.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.Credito.Value)),
                    //janeiro = e.ComponentereceitaRegisto.Where(s => s.IndActivo && s.MovimentosConciliadosFkNavigation.MovimentosBancariosFkNavigation.DataValor.Month == 1 && s.MovimentosConciliadosFkNavigation.MovimentosBancariosFkNavigation.DataValor.Year == now.Year).Sum(s => s.MovimentosConciliadosFkNavigation.MovimentosBancariosFkNavigation.Credito.Value),
                    // Soma dos pagamentos executados de fevereiro
                    fevereiro = e.ComponentereceitaRegisto.Where(s => s.IndActivo && s.ComponentereceitaRegistoMovimentos.Any(a => a.IndActivo.Value && a.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Month == 2 && a.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Year == now.Year)).Sum(s => s.ComponentereceitaRegistoMovimentos.Where(a => a.IndActivo.Value).Sum(a => a.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.Credito.Value)),
                    //fevereiro = e.ComponentereceitaRegisto.Where(s => s.IndActivo && s.MovimentosConciliadosFkNavigation.MovimentosBancariosFkNavigation.DataValor.Month == 2 && s.MovimentosConciliadosFkNavigation.MovimentosBancariosFkNavigation.DataValor.Year == now.Year).Sum(s => s.MovimentosConciliadosFkNavigation.MovimentosBancariosFkNavigation.Credito.Value),
                    // Soma dos pagamentos executados de março
                    marco = e.ComponentereceitaRegisto.Where(s => s.IndActivo && s.ComponentereceitaRegistoMovimentos.Any(a => a.IndActivo.Value && a.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Month == 3 && a.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Year == now.Year)).Sum(s => s.ComponentereceitaRegistoMovimentos.Where(a => a.IndActivo.Value).Sum(a => a.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.Credito.Value)),
                    //marco = e.ComponentereceitaRegisto.Where(s => s.IndActivo && s.MovimentosConciliadosFkNavigation.MovimentosBancariosFkNavigation.DataValor.Month == 3 && s.MovimentosConciliadosFkNavigation.MovimentosBancariosFkNavigation.DataValor.Year == now.Year).Sum(s => s.MovimentosConciliadosFkNavigation.MovimentosBancariosFkNavigation.Credito.Value),
                    // Soma dos pagamentos executados de abril
                    abril = e.ComponentereceitaRegisto.Where(s => s.IndActivo && s.ComponentereceitaRegistoMovimentos.Any(a => a.IndActivo.Value && a.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Month == 4 && a.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Year == now.Year)).Sum(s => s.ComponentereceitaRegistoMovimentos.Where(a => a.IndActivo.Value).Sum(a => a.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.Credito.Value)),
                    //abril = e.ComponentereceitaRegisto.Where(s => s.IndActivo && s.MovimentosConciliadosFkNavigation.MovimentosBancariosFkNavigation.DataValor.Month == 4 && s.MovimentosConciliadosFkNavigation.MovimentosBancariosFkNavigation.DataValor.Year == now.Year).Sum(s => s.MovimentosConciliadosFkNavigation.MovimentosBancariosFkNavigation.Credito.Value),
                    // Soma dos pagamentos executados de maio
                    maio = e.ComponentereceitaRegisto.Where(s => s.IndActivo && s.ComponentereceitaRegistoMovimentos.Any(a => a.IndActivo.Value && a.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Month == 5 && a.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Year == now.Year)).Sum(s => s.ComponentereceitaRegistoMovimentos.Where(a => a.IndActivo.Value).Sum(a => a.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.Credito.Value)),
                    //maio = e.ComponentereceitaRegisto.Where(s => s.IndActivo && s.MovimentosConciliadosFkNavigation.MovimentosBancariosFkNavigation.DataValor.Month == 5 && s.MovimentosConciliadosFkNavigation.MovimentosBancariosFkNavigation.DataValor.Year == now.Year).Sum(s => s.MovimentosConciliadosFkNavigation.MovimentosBancariosFkNavigation.Credito.Value),
                    // Soma dos pagamentos executados de junho
                    junho = e.ComponentereceitaRegisto.Where(s => s.IndActivo && s.ComponentereceitaRegistoMovimentos.Any(a => a.IndActivo.Value && a.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Month == 6 && a.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Year == now.Year)).Sum(s => s.ComponentereceitaRegistoMovimentos.Where(a => a.IndActivo.Value).Sum(a => a.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.Credito.Value)),
                    //junho = e.ComponentereceitaRegisto.Where(s => s.IndActivo && s.MovimentosConciliadosFkNavigation.MovimentosBancariosFkNavigation.DataValor.Month == 6 && s.MovimentosConciliadosFkNavigation.MovimentosBancariosFkNavigation.DataValor.Year == now.Year).Sum(s => s.MovimentosConciliadosFkNavigation.MovimentosBancariosFkNavigation.Credito.Value),
                    // Soma dos pagamentos executados de julho
                    julho = e.ComponentereceitaRegisto.Where(s => s.IndActivo && s.ComponentereceitaRegistoMovimentos.Any(a => a.IndActivo.Value && a.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Month == 7 && a.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Year == now.Year)).Sum(s => s.ComponentereceitaRegistoMovimentos.Where(a => a.IndActivo.Value).Sum(a => a.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.Credito.Value)),
                    //julho = e.ComponentereceitaRegisto.Where(s => s.IndActivo && s.MovimentosConciliadosFkNavigation.MovimentosBancariosFkNavigation.DataValor.Month == 7 && s.MovimentosConciliadosFkNavigation.MovimentosBancariosFkNavigation.DataValor.Year == now.Year).Sum(s => s.MovimentosConciliadosFkNavigation.MovimentosBancariosFkNavigation.Credito.Value),
                    // Soma dos pagamentos executados de agosto
                    agosto = e.ComponentereceitaRegisto.Where(s => s.IndActivo && s.ComponentereceitaRegistoMovimentos.Any(a => a.IndActivo.Value && a.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Month == 8 && a.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Year == now.Year)).Sum(s => s.ComponentereceitaRegistoMovimentos.Where(a => a.IndActivo.Value).Sum(a => a.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.Credito.Value)),
                    //agosto = e.ComponentereceitaRegisto.Where(s => s.IndActivo && s.MovimentosConciliadosFkNavigation.MovimentosBancariosFkNavigation.DataValor.Month == 8 && s.MovimentosConciliadosFkNavigation.MovimentosBancariosFkNavigation.DataValor.Year == now.Year).Sum(s => s.MovimentosConciliadosFkNavigation.MovimentosBancariosFkNavigation.Credito.Value),
                    // Soma dos pagamentos executados de setembro
                    setembro = e.ComponentereceitaRegisto.Where(s => s.IndActivo && s.ComponentereceitaRegistoMovimentos.Any(a => a.IndActivo.Value && a.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Month == 9 && a.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Year == now.Year)).Sum(s => s.ComponentereceitaRegistoMovimentos.Where(a => a.IndActivo.Value).Sum(a => a.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.Credito.Value)),
                    //setembro = e.ComponentereceitaRegisto.Where(s => s.IndActivo && s.MovimentosConciliadosFkNavigation.MovimentosBancariosFkNavigation.DataValor.Month == 9 && s.MovimentosConciliadosFkNavigation.MovimentosBancariosFkNavigation.DataValor.Year == now.Year).Sum(s => s.MovimentosConciliadosFkNavigation.MovimentosBancariosFkNavigation.Credito.Value),
                    // Soma dos pagamentos executados de outubro
                    outubro = e.ComponentereceitaRegisto.Where(s => s.IndActivo && s.ComponentereceitaRegistoMovimentos.Any(a => a.IndActivo.Value && a.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Month == 10 && a.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Year == now.Year)).Sum(s => s.ComponentereceitaRegistoMovimentos.Where(a => a.IndActivo.Value).Sum(a => a.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.Credito.Value)),
                    //outubro = e.ComponentereceitaRegisto.Where(s => s.IndActivo && s.MovimentosConciliadosFkNavigation.MovimentosBancariosFkNavigation.DataValor.Month == 10 && s.MovimentosConciliadosFkNavigation.MovimentosBancariosFkNavigation.DataValor.Year == now.Year).Sum(s => s.MovimentosConciliadosFkNavigation.MovimentosBancariosFkNavigation.Credito.Value),
                    // Soma dos pagamentos executados de novembro
                    novembro = e.ComponentereceitaRegisto.Where(s => s.IndActivo && s.ComponentereceitaRegistoMovimentos.Any(a => a.IndActivo.Value && a.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Month == 11 && a.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Year == now.Year)).Sum(s => s.ComponentereceitaRegistoMovimentos.Where(a => a.IndActivo.Value).Sum(a => a.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.Credito.Value)),
                    //novembro = e.ComponentereceitaRegisto.Where(s => s.IndActivo && s.MovimentosConciliadosFkNavigation.MovimentosBancariosFkNavigation.DataValor.Month == 11 && s.MovimentosConciliadosFkNavigation.MovimentosBancariosFkNavigation.DataValor.Year == now.Year).Sum(s => s.MovimentosConciliadosFkNavigation.MovimentosBancariosFkNavigation.Credito.Value),
                    // Soma dos pagamentos executados de dezembro
                    dezembro = e.ComponentereceitaRegisto.Where(s => s.IndActivo && s.ComponentereceitaRegistoMovimentos.Any(a => a.IndActivo.Value && a.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Month == 12 && a.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Year == now.Year)).Sum(s => s.ComponentereceitaRegistoMovimentos.Where(a => a.IndActivo.Value).Sum(a => a.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.Credito.Value)),
                    //dezembro = e.ComponentereceitaRegisto.Where(s => s.IndActivo && s.MovimentosConciliadosFkNavigation.MovimentosBancariosFkNavigation.DataValor.Month == 12 && s.MovimentosConciliadosFkNavigation.MovimentosBancariosFkNavigation.DataValor.Year == now.Year).Sum(s => s.MovimentosConciliadosFkNavigation.MovimentosBancariosFkNavigation.Credito.Value),
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
        /// Relatório CE_OSS_Global (Receita): mesma lógica de resolução do lado Despesa
        /// (ver PagamentosExecutadosRepository.GetExecucaoOrcamentalPorClassificacaoEconomica), mas usando o bucket
        /// "Receita" de TIPOCONTA e os movimentos bancários conciliados (ComponentereceitaRegistoMovimentos) como
        /// fonte de execução mensal em vez de Pagamentosexecutados.
        /// </summary>
        public ClassificacaoEconomicaExecucaoListagemResponse GetExecucaoOrcamentalPorClassificacaoEconomica(RelatorioClassificacaoEconomicaRequest request)
        {
            ClassificacaoEconomicaExecucaoListagemResponse response = new ClassificacaoEconomicaExecucaoListagemResponse();

            int tipoContaReceitaId = _moduloContribuicoesContext.Dominio
                .Where(d => d.Dominio1 == "TIPOCONTA" && d.Descricao == "Receita")
                .Select(d => d.IdDominio)
                .First();

            var allNodes = _moduloContribuicoesContext.Agrupamentoconfig
                .Where(a => a.IndActivo && a.ReltipoDeContaOrcamentoConfigFkNavigation.TipoContaFk == tipoContaReceitaId)
                .Include(a => a.Componenteorcamentovalor)
                    .ThenInclude(v => v.ComponenteOrcamentoRegistoFkNavigation)
                .Include(a => a.ComponentereceitaRegisto)
                    .ThenInclude(r => r.ComponentereceitaRegistoMovimentos)
                    .ThenInclude(m => m.RelMovimentosPorConciliarMovimentos)
                    .ThenInclude(rm => rm.MovimentosBancariosFkNavigation)
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

                // Esquema CE já é o novo/real (mã 401-409) -- usa o próprio nó, no nível em que já está
                if (int.TryParse(root.Codigo, out int rootCode) && rootCode >= 401 && rootCode <= 409)
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
                    // [EN] BUGFIX 2026-07-22: see identical fix + full explanation in
                    // PagamentosExecutadosRepository.GetExecucaoOrcamentalPorClassificacaoEconomica --
                    // must sum all Centro de Custo rows within a registo before picking Inicial/Corrigido.
                    // [VI] SỬA LỖI 2026-07-22: xem giải thích đầy đủ ở hàm cùng tên bên
                    // PagamentosExecutadosRepository -- phải cộng hết các dòng Centro de Custo trong
                    // cùng 1 đợt trước khi chọn Inicial/Corrigido.
                    ValorInicial = a.Componenteorcamentovalor
                        .Where(v => v.InstitutionId == request.institution
                            && v.ComponenteOrcamentoRegistoFkNavigation.DataInicio.Year <= request.year
                            && v.ComponenteOrcamentoRegistoFkNavigation.DataFim.Year >= request.year)
                        .GroupBy(v => v.ComponenteOrcamentoRegistoFk)
                        .OrderBy(g => g.Key)
                        .Select(g => g.Sum(v => v.Valor))
                        .FirstOrDefault(),
                    ValorCorrigido = a.Componenteorcamentovalor
                        .Where(v => v.InstitutionId == request.institution
                            && v.ComponenteOrcamentoRegistoFkNavigation.Aprovado
                            && v.ComponenteOrcamentoRegistoFkNavigation.DataInicio.Year <= request.year
                            && v.ComponenteOrcamentoRegistoFkNavigation.DataFim.Year >= request.year)
                        .GroupBy(v => v.ComponenteOrcamentoRegistoFk)
                        .OrderByDescending(g => g.Key)
                        .Select(g => g.Sum(v => v.Valor))
                        .FirstOrDefault()
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

            // Receita Liquidada, por Conta OGE original, depois agregado por raiz CE -- valor total registado em
            // ComponentereceitaRegisto (reconhecido/a cobrar), independentemente de já ter sido recebido/conciliado
            // ou não. ComponentereceitaRegisto não tem um campo "Estado" tipo Cabimento/Compromisso (ao contrário
            // de ComponentedespesaRegisto) -- é o equivalente mais próximo de "Obrigação" do lado Despesa.
            var receitaLiquidadaPorConta = allNodes.Values
                .Select(a => new
                {
                    AgrupamentoId = a.Id,
                    ReceitaLiquidada = a.ComponentereceitaRegisto.Where(s => s.IndActivo && s.InstitutionId == request.institution && s.DataCriacao.Year == request.year).Sum(s => s.Valor),
                })
                .Where(x => x.ReceitaLiquidada != 0)
                .ToList();

            foreach (var item in receitaLiquidadaPorConta)
            {
                var target = ResolveToEconomicTarget(item.AgrupamentoId);
                if (target == null) continue;
                CreditAncestors(target, ce =>
                {
                    ce.receitaLiquidada += item.ReceitaLiquidada;
                });
            }

            // Execução mensal (movimentos bancários conciliados), por Conta OGE original, depois agregado por raiz CE
            var receitaPorConta = allNodes.Values
                .Select(a => new
                {
                    AgrupamentoId = a.Id,
                    Janeiro = a.ComponentereceitaRegisto.Where(s => s.IndActivo && s.InstitutionId == request.institution && s.ComponentereceitaRegistoMovimentos.Any(m => m.IndActivo.Value && m.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Month == 1 && m.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Year == request.year)).Sum(s => s.ComponentereceitaRegistoMovimentos.Where(m => m.IndActivo.Value).Sum(m => m.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.Credito.Value)),
                    Fevereiro = a.ComponentereceitaRegisto.Where(s => s.IndActivo && s.InstitutionId == request.institution && s.ComponentereceitaRegistoMovimentos.Any(m => m.IndActivo.Value && m.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Month == 2 && m.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Year == request.year)).Sum(s => s.ComponentereceitaRegistoMovimentos.Where(m => m.IndActivo.Value).Sum(m => m.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.Credito.Value)),
                    Marco = a.ComponentereceitaRegisto.Where(s => s.IndActivo && s.InstitutionId == request.institution && s.ComponentereceitaRegistoMovimentos.Any(m => m.IndActivo.Value && m.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Month == 3 && m.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Year == request.year)).Sum(s => s.ComponentereceitaRegistoMovimentos.Where(m => m.IndActivo.Value).Sum(m => m.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.Credito.Value)),
                    Abril = a.ComponentereceitaRegisto.Where(s => s.IndActivo && s.InstitutionId == request.institution && s.ComponentereceitaRegistoMovimentos.Any(m => m.IndActivo.Value && m.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Month == 4 && m.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Year == request.year)).Sum(s => s.ComponentereceitaRegistoMovimentos.Where(m => m.IndActivo.Value).Sum(m => m.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.Credito.Value)),
                    Maio = a.ComponentereceitaRegisto.Where(s => s.IndActivo && s.InstitutionId == request.institution && s.ComponentereceitaRegistoMovimentos.Any(m => m.IndActivo.Value && m.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Month == 5 && m.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Year == request.year)).Sum(s => s.ComponentereceitaRegistoMovimentos.Where(m => m.IndActivo.Value).Sum(m => m.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.Credito.Value)),
                    Junho = a.ComponentereceitaRegisto.Where(s => s.IndActivo && s.InstitutionId == request.institution && s.ComponentereceitaRegistoMovimentos.Any(m => m.IndActivo.Value && m.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Month == 6 && m.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Year == request.year)).Sum(s => s.ComponentereceitaRegistoMovimentos.Where(m => m.IndActivo.Value).Sum(m => m.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.Credito.Value)),
                    Julho = a.ComponentereceitaRegisto.Where(s => s.IndActivo && s.InstitutionId == request.institution && s.ComponentereceitaRegistoMovimentos.Any(m => m.IndActivo.Value && m.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Month == 7 && m.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Year == request.year)).Sum(s => s.ComponentereceitaRegistoMovimentos.Where(m => m.IndActivo.Value).Sum(m => m.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.Credito.Value)),
                    Agosto = a.ComponentereceitaRegisto.Where(s => s.IndActivo && s.InstitutionId == request.institution && s.ComponentereceitaRegistoMovimentos.Any(m => m.IndActivo.Value && m.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Month == 8 && m.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Year == request.year)).Sum(s => s.ComponentereceitaRegistoMovimentos.Where(m => m.IndActivo.Value).Sum(m => m.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.Credito.Value)),
                    Setembro = a.ComponentereceitaRegisto.Where(s => s.IndActivo && s.InstitutionId == request.institution && s.ComponentereceitaRegistoMovimentos.Any(m => m.IndActivo.Value && m.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Month == 9 && m.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Year == request.year)).Sum(s => s.ComponentereceitaRegistoMovimentos.Where(m => m.IndActivo.Value).Sum(m => m.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.Credito.Value)),
                    Outubro = a.ComponentereceitaRegisto.Where(s => s.IndActivo && s.InstitutionId == request.institution && s.ComponentereceitaRegistoMovimentos.Any(m => m.IndActivo.Value && m.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Month == 10 && m.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Year == request.year)).Sum(s => s.ComponentereceitaRegistoMovimentos.Where(m => m.IndActivo.Value).Sum(m => m.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.Credito.Value)),
                    Novembro = a.ComponentereceitaRegisto.Where(s => s.IndActivo && s.InstitutionId == request.institution && s.ComponentereceitaRegistoMovimentos.Any(m => m.IndActivo.Value && m.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Month == 11 && m.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Year == request.year)).Sum(s => s.ComponentereceitaRegistoMovimentos.Where(m => m.IndActivo.Value).Sum(m => m.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.Credito.Value)),
                    Dezembro = a.ComponentereceitaRegisto.Where(s => s.IndActivo && s.InstitutionId == request.institution && s.ComponentereceitaRegistoMovimentos.Any(m => m.IndActivo.Value && m.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Month == 12 && m.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Year == request.year)).Sum(s => s.ComponentereceitaRegistoMovimentos.Where(m => m.IndActivo.Value).Sum(m => m.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.Credito.Value)),
                })
                .ToList();

            foreach (var item in receitaPorConta)
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

            // Garante que TODOS os nós da árvore CE (esquema novo/real, mã 401-409) aparecem no relatório,
            // mesmo com valor zero -- tal como no ficheiro Excel original do cliente
            foreach (var node in allNodes.Values)
            {
                var root = GetRootAncestor(node);
                if (int.TryParse(root.Codigo, out int rootCode) && rootCode >= 401 && rootCode <= 409)
                {
                    GetOrCreate(node);
                }
            }

            foreach (var ce in resultado.Values)
            {
                ce.totalExecucao = ce.janeiro + ce.fevereiro + ce.marco + ce.abril + ce.maio + ce.junho + ce.julho + ce.agosto + ce.setembro + ce.outubro + ce.novembro + ce.dezembro;
                ce.taxaExecucao = ce.valorOrcamentado == 0 || ce.totalExecucao == 0 ? 0 : ce.totalExecucao / ce.valorOrcamentado;

                // [EN] "Saldo"/gap columns, exact formulas from sheet CE_OSS_Global rows 6-9 (Receita block).
                // [VI] Các cột "Saldo", đúng công thức từ sheet CE_OSS_Global dòng 6-9 (khối Receita).
                ce.saldoExecucao = ce.valorOrcamentado - ce.totalExecucao;                          // (7) = (2) - (5)
                ce.saldoReceitaLiquidadaNaoCobrada = ce.receitaLiquidada - ce.totalExecucao;         // (8) = (3) - (5) -- DÍVIDA
            }

            response.lista = resultado.Values.OrderBy(c => c.codigoCE).ToList();

            return response;
        }

        public GetDespesasRelatoriosReponse ReceitasRelatorios(ReceitasRelatoriosRequest request)
        {
            GetDespesasRelatoriosReponse response = new GetDespesasRelatoriosReponse();

            int index = 0;
            if (request.filter.index.HasValue)
                index = request.filter.index.Value;

            int rows = 5;
            if (request.filter.rows.HasValue)
                rows = request.filter.rows.Value;

            var beginDate = request.filter.dateFilterBegin;
            var endDate = request.filter.dateFilterEnd;

            var listaReceitas = _moduloContribuicoesContext.ComponentereceitaRegisto
                .Include(e => e.DepartamentoFkNavigation)
                .Include(e => e.CentroCustoFkNavigation)
                .Include(e => e.TipoContaFkNavigation)
                .Include(e => e.TarefaActivoFkNavigation)
                .ThenInclude(e => e.ProcessoAtivoFkNavigation)
                .Include(e => e.AgrupamentoConfigFkNavigation)
                .ThenInclude(e => e.ParentFkNavigation)
                .Where(u => u.IndActivo &&
                                (beginDate.HasValue && !endDate.HasValue ? u.DataCriacao.Date == beginDate :
                                 beginDate.HasValue && endDate.HasValue ? u.DataCriacao.Date >= beginDate && u.DataCriacao.Date <= endDate : true) &&
                                // Filtrar por Banco — uma Receita (1) pode reunir vários movimentos conciliados (N),
                                // cada um com o seu próprio Banco (via Guia de Pagamento); devolve a Receita se
                                // PELO MENOS UM dos seus movimentos ativos for do banco pedido.
                                (request.BankCode == null || u.ComponentereceitaRegistoMovimentos.Any(m =>
                                    m.IndActivo == true &&
                                    m.RelMovimentosPorConciliarMovimentos.GuiaPagamentoFkNavigation != null &&
                                    m.RelMovimentosPorConciliarMovimentos.GuiaPagamentoFkNavigation.BankCode == request.BankCode)))
                .Select(u => new DespesasRelatoriosDataContract
                {
                    Id = u.Id,
                    DepartamentoINSS = u.DepartamentoFkNavigation.Nome,
                    CentroCusto = u.CentroCustoFkNavigation.Descricao,
                    TipoConta = u.TipoContaFkNavigation.Descricao,
                    _ContaOSS = u.AgrupamentoConfigFkNavigation,
                    Descricao = u.Descricao,
                    Valor = u.Valor,
                    Data = u.DataAlteracao ?? u.DataCriacao,
                    NumeroProcesso = u.TarefaActivoFkNavigation.ProcessoAtivoFkNavigation.NumeroProcesso,
                    UtilizadorAlteracao = _moduloContribuicoesContext.Utilizador.First(e => e.IdUtilizador == u.UtilizadorCriacao).Username,
                    // Uma Receita pode ter movimentos de bancos diferentes — mostramos apenas o
                    // PRIMEIRO banco encontrado (movimento manual/sem Guia não tem Banco, fica de fora).
                    // Ver nota completa em DespesasRelatoriosDataContract.BankCode.
                    BankCode = u.ComponentereceitaRegistoMovimentos
                        .Where(m => m.IndActivo == true && m.RelMovimentosPorConciliarMovimentos.GuiaPagamentoFkNavigation != null)
                        .Select(m => m.RelMovimentosPorConciliarMovimentos.GuiaPagamentoFkNavigation.BankCode)
                        .FirstOrDefault(),
                });


            var receitas = listaReceitas
                .OrderBy(request.filter.orderBy, request.filter.orderDirection)
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = listaReceitas.Count();
            response.rows = totalNumber;
            response.Despesas = receitas;

            return response;
        }

        public string ReceitasRelatoriosExcel(ReceitasRelatoriosRequest request, List<DespesasRelatoriosDataContract> lista)
        {
            // Inicialização do documento excel
            var excelDocument = new ExcelDocument(_localizer["receitas"].Value, new ExcelDocumentOptions()
            {
                TextStyles = Extensions.ServiceExtensions.ExcelDocumentTextStyles
            });

            // Adição dos títulos
            excelDocument.Pages[0].AddText(_localizer["receitas"].Value, new ExcelDocumentTextPosition(1, 1), "Header", new ExcelDocumentTextPosition(9, 1));

            // Adição da tabela
            excelDocument.Pages[0].AddTable(new ExcelDocumentTextPosition(1, 3), lista, new List<ColumnOption<DespesasRelatoriosDataContract>>()
            {
                new ColumnOption<DespesasRelatoriosDataContract>()
                {
                    Name = _localizer["departamentosINSS"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellWrap",
                    Value = (data) => data.DepartamentoINSS,
                    Width = 25,
                },
                new ColumnOption<DespesasRelatoriosDataContract>()
                {
                    Name = _localizer["centroCusto"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellWrap",
                    Value = (data) => data.CentroCusto,
                    Width = 23,
                },
                new ColumnOption<DespesasRelatoriosDataContract>()
                {
                    Name = _localizer["tipoConta"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellWrap",
                    Value = (data) => data.TipoConta,
                    Width = 20,
                },
                new ColumnOption<DespesasRelatoriosDataContract>()
                {
                    Name = _localizer["contaOSS"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellWrap",
                    Value = (data) => data.ContaOSS,
                    Width = 20,
                },
                new ColumnOption<DespesasRelatoriosDataContract>()
                {
                    Name = _localizer["descricao"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellWrap",
                    Value = (data) => data.Descricao,
                    Width = 22,
                },
                new ColumnOption<DespesasRelatoriosDataContract>()
                {
                    Name = _localizer["valor"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellMoney",
                    Value = (data) => data.Valor,
                    Width = 20
                },
                new ColumnOption<DespesasRelatoriosDataContract>()
                {
                    Name = _localizer["data"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellWrap",
                    Value = (data) => data.Data.ToString("dd/MM/yyyy"),
                    Width = 17,
                },
                new ColumnOption<DespesasRelatoriosDataContract>()
                {
                    Name = _localizer["nProcesso"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellWrap",
                    Value = (data) => data.NumeroProcesso,
                    Width = 17,
                },
                new ColumnOption<DespesasRelatoriosDataContract>()
                {
                    Name = _localizer["utilizador"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellWrap",
                    Value = (data) => data.UtilizadorAlteracao,
                    Width = 20
                },
                new ColumnOption<DespesasRelatoriosDataContract>()
                {
                    // Ver nota em DespesasRelatoriosDataContract.BankCode — mostra apenas o primeiro banco
                    Name = _localizer["banco"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellWrap",
                    Value = (data) => data.BankCode,
                    Width = 15,
                },
            }, new TableOptions() { AutoFitColumns = false });

            // Adição da linha de Total como footer da tabela
            excelDocument.Pages[0].AddText(_localizer["total"].Value + ":", new ExcelDocumentTextPosition(1, 4 + lista.Count), "HeaderAlignRight", new ExcelDocumentTextPosition(5, 4 + lista.Count));

            var sumRange = new ExcelDocumentTextPositionRange(new ExcelDocumentTextPosition(6, 4), new ExcelDocumentTextPosition(6, 4 + lista.Count - 1));
            excelDocument.Pages[0].AddSum(new ExcelDocumentTextPosition(6, 4 + lista.Count), sumRange, "HeaderAlignLeftMoney", new ExcelDocumentTextPosition(9, 4 + lista.Count));

            // Conversão do documento excel em base64
            return excelDocument.GetFileString();
        }

        public GetReceitasNaoConciliadasRelatoriosReponse ReceitasNaoConciliadasRelatorios(ReceitasNaoConciliadasRelatoriosRequest request)
        {
            GetReceitasNaoConciliadasRelatoriosReponse response = new GetReceitasNaoConciliadasRelatoriosReponse();

            int index = 0;
            if (request.filter.index.HasValue)
                index = request.filter.index.Value;

            int rows = 5;
            if (request.filter.rows.HasValue)
                rows = request.filter.rows.Value;

            var beginDate = request.filter.dateFilterBegin;
            var endDate = request.filter.dateFilterEnd;

            var listaReceitas = _moduloContribuicoesContext.Guiapagamento
                .Include(e => e.RelMovimentosporconciliarMovimentos)
                .Where(e => e.IndActivo &&
                    (request.Contribuinte == null || e.GuiaEntidadeFkNavigation.Tin.Contains(request.Contribuinte)) &&
                    e.ValorComprovPag.HasValue &&
                    // Não conciliados
                    !e.RelMovimentosporconciliarMovimentos.Any(a => a.IndActivo.Value) &&
                    // Filtrar por datas
                    (beginDate.HasValue && endDate.HasValue ? e.DataCriacao.Date >= beginDate && e.DataCriacao.Date <= endDate :
                        beginDate.HasValue && !endDate.HasValue ? e.DataCriacao.Date == beginDate :
                        true)
                )
                .Where(e =>
                    // Filtrar por Banco
                    (request.BankCode == null || e.BankCode == request.BankCode)
                )
                .Select(e => new ReceitasNaoConciliadasRelatorios
                {
                    Descricao = e.Descricao,
                    NumeroDocumento = e.NumDocumento,
                    Valor = e.Valor,
                    Contribuinte = Convert.ToString(e.GuiaEntidadeFkNavigation.Tin),
                    Data = e.DataCriacao,
                    BankCode = e.BankCode,
                });

            if (request.Contribuinte == null || request.Contribuinte == "")
            {

                listaReceitas = listaReceitas.Concat(
                    _moduloContribuicoesContext.Movimentosporconciliar
                   .Include(e => e.RelMovimentosporconciliarMovimentos)
                   .Include(e => e.MovimentoBancarioFkNavigation)
                   .Where(e => e.IndActivo.Value && e.IsReceita &&
                        // Não conciliados
                        !e.RelMovimentosporconciliarMovimentos.Any(a => a.IndActivo.Value) &&
                        // Filtrar por datas
                        (beginDate.HasValue && endDate.HasValue ? e.DataCriacao.Date >= beginDate && e.DataCriacao.Date <= endDate :
                         beginDate.HasValue && !endDate.HasValue ? e.DataCriacao.Date == beginDate :
                         true) &&
                        // Movimento manual, không có Banco riêng — chỉ hiện khi không lọc theo Banco
                        request.BankCode == null
                   )
                   .Select(e => new ReceitasNaoConciliadasRelatorios
                   {
                       Descricao = e.MovimentoBancarioFkNavigation.Descricao,
                       NumeroDocumento = e.NumeroDocumento,
                       Valor = e.Valor,
                       Data = e.DataCriacao,
                       Contribuinte = "",
                       BankCode = null,
                   })
                );
                listaReceitas = listaReceitas.Concat(
                    _moduloContribuicoesContext.Reservacredito
                    .Include(e => e.ReservaGuiaPagamentoFkNavigation)
                    .Include(e => e.RelMovimentosporconciliarMovimentos)
                    .Where(e => !e.IndActivo &&
                        // Não conciliados
                        !e.RelMovimentosporconciliarMovimentos.Any(a => a.IndActivo.Value) &&
                        // Filtrar por datas
                        (beginDate.HasValue && endDate.HasValue ? e.DataCriacao.Date >= beginDate && e.DataCriacao.Date <= endDate :
                            beginDate.HasValue && !endDate.HasValue ? e.DataCriacao.Date == beginDate :
                            true) &&
                        // Nota de crédito thừa kế Banco từ Guia gốc
                        (request.BankCode == null || e.ReservaGuiaPagamentoFkNavigation.BankCode == request.BankCode)
                    )
                    .Select(e => new ReceitasNaoConciliadasRelatorios
                    {
                        Descricao = e.ReservaGuiaPagamentoFkNavigation.Descricao + " - " + _localizer["notaCredito"].Value,
                        NumeroDocumento = e.ReservaGuiaPagamentoFkNavigation.NumDocumento,
                        Valor = e.Valor.Value,
                        Data = e.DataCriacao,
                        Contribuinte = "",
                        BankCode = e.ReservaGuiaPagamentoFkNavigation.BankCode,
                    })
                );

            }


            var receitas = listaReceitas
                .OrderBy(request.filter.orderBy, request.filter.orderDirection)
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = listaReceitas.Count();
            response.rows = totalNumber;
            response.Receitas = receitas;

            return response;
        }

        public string ReceitasNaoConciliadasRelatoriosExcel(ReceitasNaoConciliadasRelatoriosRequest request, List<ReceitasNaoConciliadasRelatorios> lista)
        {
            // Inicialização do documento excel
            var excelDocument = new ExcelDocument(_localizer["receitas"].Value, new ExcelDocumentOptions()
            {
                TextStyles = Extensions.ServiceExtensions.ExcelDocumentTextStyles
            });

            // Adição dos títulos
            excelDocument.Pages[0].AddText(_localizer["receitas"].Value, new ExcelDocumentTextPosition(1, 1), "Header", new ExcelDocumentTextPosition(5, 1));
            excelDocument.Pages[0].AddText(_localizer["tin"].Value + ":" + request.Contribuinte, new ExcelDocumentTextPosition(1, 2), "Header", new ExcelDocumentTextPosition(5, 2));

            // Adição da tabela
            excelDocument.Pages[0].AddTable(new ExcelDocumentTextPosition(1, 4), lista, new List<ColumnOption<ReceitasNaoConciliadasRelatorios>>()
            {
                new ColumnOption<ReceitasNaoConciliadasRelatorios>()
                {
                    Name = _localizer["descricao"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellWrap",
                    Value = (data) => data.Descricao,
                    Width = 25,
                },
                new ColumnOption<ReceitasNaoConciliadasRelatorios>()
                {
                    Name = _localizer["nDocumento"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellWrap",
                    Value = (data) => data.NumeroDocumento,
                    Width = 20,
                },
                new ColumnOption<ReceitasNaoConciliadasRelatorios>()
                {
                    Name = _localizer["valor"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellMoney",
                    Value = (data) => data.Valor,
                    Width = 17,
                },
                new ColumnOption<ReceitasNaoConciliadasRelatorios>()
                {
                    Name = _localizer["tin"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellWrap",
                    Value = (data) => data.Contribuinte,
                    Width = 18,
                },
                new ColumnOption<ReceitasNaoConciliadasRelatorios>()
                {
                    Name = _localizer["data"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellWrap",
                    Value = (data) => data.Data.ToString("dd/MM/yyyy"),
                    Width = 18,
                },
                new ColumnOption<ReceitasNaoConciliadasRelatorios>()
                {
                    Name = _localizer["banco"].Value,
                    ColumnTextStyleKey = "HeaderWrap",
                    DataTextStyleKey = "TableCellWrap",
                    Value = (data) => data.BankCode,
                    Width = 15,
                },
            }, new TableOptions() { AutoFitColumns = false });

            // Adição da linha de Total como footer da tabela
            excelDocument.Pages[0].AddText(_localizer["total"].Value + ":", new ExcelDocumentTextPosition(1, 5 + lista.Count), "HeaderAlignRight", new ExcelDocumentTextPosition(2, 5 + lista.Count));

            var sumRange = new ExcelDocumentTextPositionRange(new ExcelDocumentTextPosition(3, 5), new ExcelDocumentTextPosition(3, 5 + lista.Count - 1));
            excelDocument.Pages[0].AddSum(new ExcelDocumentTextPosition(3, 5 + lista.Count), sumRange, "HeaderAlignLeftMoney", new ExcelDocumentTextPosition(5, 5 + lista.Count));

            // Conversão do documento excel em base64
            return excelDocument.GetFileString();
        }
    }
}