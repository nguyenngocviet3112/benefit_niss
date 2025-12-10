using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
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
    public class ComponenteDespesaRegistoRepository : IComponenteDespesaRegistoRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public ComponenteDespesaRegistoRepository(TimorINSSModuloContribuicoesContext storeContext, IStringLocalizer<SharedResource> localizer)
        {
            _moduloContribuicoesContext = storeContext;
            _localizer = localizer;
        }

        public IEnumerable<ComponentedespesaRegisto> GetAll()
        {
            return _moduloContribuicoesContext.ComponentedespesaRegisto.ToList();
        }

        public ComponentedespesaRegisto Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var componenteDespesaRegisto = _moduloContribuicoesContext.ComponentedespesaRegisto
                .Include(u => u.Compromisso.Where(x => x.IndActivo))
                .SingleOrDefault(u => u.Id == id);

            return componenteDespesaRegisto;
        }

        public ComponenteDespesaRegistoDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var componenteDespesaRegisto = _moduloContribuicoesContext.ComponentedespesaRegisto
                .SingleOrDefault(u => u.Id == id);

            ComponenteDespesaRegistoDto componenteDespesaRegistoDto = Utils.MappClassToDto<ComponentedespesaRegisto, ComponenteDespesaRegistoDto>(componenteDespesaRegisto);
            return componenteDespesaRegistoDto;
        }

        public void Add(ComponentedespesaRegisto entity)
        {
            _moduloContribuicoesContext.ComponentedespesaRegisto.Add(entity);
        }

        public void Update(ComponentedespesaRegisto entity)
        {
            ComponentedespesaRegisto entityToUpdate = _moduloContribuicoesContext.ComponentedespesaRegisto
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(ComponentedespesaRegisto entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public List<DespesaRegistadaDataContract> GetAllDespesaRegistadaByTarefaAtivoId(int tarefaAtivo)
        {

            var tarefa = _moduloContribuicoesContext.Tarefaativo.First(e => e.Id == tarefaAtivo);

            return _moduloContribuicoesContext.ComponentedespesaRegisto
               .Where(u => u.IndActivo && u.TarefaActivoFkNavigation.ProcessoAtivoFk == tarefa.ProcessoAtivoFk)
              .Select(u => new DespesaRegistadaDataContract
              {
                  Id = u.Id,
                  //IdContabilidade = u.CodigoContaFk,
                  compromissos = u.Compromisso.Select(e => e.Id),
                  idOrcamento = u.AgrupamentoConfigFk,
                  descricaoDespesa = u.Descricao,
                  valorRegistado = u.Valor,
                  estado = u.EstadoNavigation.Descricao,
                  idDepartamento = (int)u.DepartamentoFk,
                  idCentroCusto = u.CentroCustoFk,
                  idTipoConta = u.TipoContaFk,
                  idInstitution = u.InstitutionId,
                  idActidade = u.ActidadeFk,
                  idEconomic = u.EconomicFk,
                  idFuncional = u.FuncionalFk
              })
               .ToList();
        }

        public List<ComponentedespesaRegisto> GetAllDespesaRegistadaByAgrupamentoConfigFk(int agrupamentoConfigFk,
            int institutionId, int actidadeId, int economicId, int funcionalId)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var componenteDespesaRegisto = _moduloContribuicoesContext.ComponentedespesaRegisto
                .Where(u => u.AgrupamentoConfigFk == agrupamentoConfigFk && u.IndActivo && u.InstitutionId == institutionId && u.ActidadeFk == actidadeId
                && u.EconomicFk == economicId && u.FuncionalFk == funcionalId)
                .ToList();

            return componenteDespesaRegisto;
        }

        public List<ComponentedespesaRegisto> GetAllDespesaRegistadaByAgrupamentoConfigFk(int agrupamentoConfigFk)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var componenteDespesaRegisto = _moduloContribuicoesContext.ComponentedespesaRegisto
                .Where(u => u.AgrupamentoConfigFk == agrupamentoConfigFk)
                .ToList();

            return componenteDespesaRegisto;
        }

        public List<DespesaCabimentadasParaExecucaoDataContract> GetAllDespesaCabimentadasParaExecucaoByTarefaAtivoId(int processoId, int estado)
        {
            return _moduloContribuicoesContext.ComponentedespesaRegisto
                .Include(u => u.TarefaActivoFkNavigation)
                .Where(u => u.TarefaActivoFkNavigation.ProcessoAtivoFk == processoId)
               .Include(u => u.AgrupamentoConfigFkNavigation)
               .Where(u => u.IndActivo && u.Estado == estado)
              .Select(u => new DespesaCabimentadasParaExecucaoDataContract
              {
                  Id = u.Id,
                  DescricaoDespesa = u.Descricao,
                  ValorCabimentado = u.Valor
              })
               .ToList();
        }

        public List<DespesaCabimentadasParaExecucaoDataContract> GetAllDespesaCompromissosParaExecucaoByTarefaAtivoId(int processoId)
        {
            return _moduloContribuicoesContext.Compromisso
                .Include(u => u.TarefaAtivoFkNavigation)
                .Where(u => u.TarefaAtivoFkNavigation.ProcessoAtivoFk == processoId)
               .Where(u => u.IndActivo)
              .Select(u => new DespesaCabimentadasParaExecucaoDataContract
              {
                  Id = u.Id,
                  DescricaoDespesa = u.Nome,
                  ValorCabimentado = u.Valor
              })
               .ToList();
        }

        public List<ComponentedespesaRegisto> GetComponenteDespesaRegistoByIdOrcamento(int idOrcamentoRegisto, DateTime dataInicio, DateTime dataFim)
        {
            return _moduloContribuicoesContext.ComponentedespesaRegisto
                .Where(u => u.IndActivo && u.ComponenteOrcamentoRegistoFk == idOrcamentoRegisto
                && u.DataCriacao >= dataInicio && u.DataCriacao < dataFim)
                .ToList();

        }

        public GetDespesasRelatoriosReponse GetDespesasRelatorio(GetDespesasRelatorioRequest request, List<int> estadosDespesa, int? estadoPagamento, EstadoDespesaEnum tipo)
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

            IQueryable<DespesasRelatoriosDataContract> listaDespesas = Enumerable.Empty<DespesasRelatoriosDataContract>().AsQueryable();

            if (tipo != EstadoDespesaEnum.Compromisso)
            {
                listaDespesas = _moduloContribuicoesContext.ComponentedespesaRegisto
                        .Include(e => e.Compromisso)
                        .ThenInclude(e => e.Pagamentosexecutados)
                        .Include(e => e.DepartamentoFkNavigation)
                        .Include(e => e.CentroCustoFkNavigation)
                        .Include(e => e.TipoContaFkNavigation)
                        .Include(e => e.TarefaActivoFkNavigation)
                        .ThenInclude(e => e.ProcessoAtivoFkNavigation)
                        .Include(e => e.AgrupamentoConfigFkNavigation)
                        .ThenInclude(e => e.ParentFkNavigation)
                        .Where(u => u.IndActivo &&
                                    ((estadosDespesa.Any() && estadosDespesa.Contains(u.Estado)) ||
                                     (estadoPagamento.HasValue && u.Compromisso.Any(x => x.Pagamentosexecutados.Any(e => e.IndActivo && e.Estado == estadoPagamento)) ||
                                     (tipo == EstadoDespesaEnum.Executado && u.Compromisso.Any(x => x.Pagamentosexecutados.Any(e => e.IndActivo && e.RelMovimentosporconciliarMovimentos.Any(a => a.IndActivo == true)))))) &&
                                    (beginDate.HasValue && !endDate.HasValue ? u.DataAlteracao.Value.Date == beginDate :
                                     beginDate.HasValue && endDate.HasValue ? u.DataAlteracao.Value.Date >= beginDate && u.DataAlteracao <= endDate : true))
                        .Select(u => new DespesasRelatoriosDataContract
                        {
                            Id = u.Id,
                            DepartamentoINSS = u.DepartamentoFkNavigation.Nome,
                            CentroCusto = u.CentroCustoFkNavigation.Descricao,
                            TipoConta = u.TipoContaFkNavigation.Descricao,
                            _ContaOSS = u.AgrupamentoConfigFkNavigation,
                            Descricao = u.Descricao,
                            Valor = estadosDespesa.Any() ? u.Valor : u.Compromisso.SelectMany(p => _moduloContribuicoesContext.Pagamentosexecutados.Where(b => b.IndActivo && b.CompromissoFk == p.Id)).Sum(b => b.ValorExecutado),
                            //Valor = estadosDespesa.Any() ? u.Valor - u.Compromisso.SelectMany(p => _moduloContribuicoesContext.Pagamentosexecutados.Where(b => b.IndActivo && b.CompromissoFk == p.Id)).Sum(b => b.ValorExecutado)
                            //: u.Compromisso.SelectMany(p => _moduloContribuicoesContext.Pagamentosexecutados.Where(b => b.IndActivo && b.CompromissoFk == p.Id)).Sum(b => b.ValorExecutado),
                            Data = estadoPagamento.HasValue ?
                                            u.Compromisso.OrderByDescending(x => x.Id)
                                            .Where(x => x.IndActivo && x.Pagamentosexecutados.Any(xx => xx.IndActivo))
                                            .First().
                                            Pagamentosexecutados.OrderByDescending(e => e.Id)
                                                                  .Where(e => e.IndActivo)
                                                                  .Select(e => e.DataAlteracao ?? e.DataCriacao)
                                                                  .First() : u.DataAlteracao ?? u.DataCriacao,
                            NumeroProcesso = u.TarefaActivoFkNavigation.ProcessoAtivoFkNavigation.NumeroProcesso,
                            UtilizadorAlteracao = _moduloContribuicoesContext.Utilizador.First(e => e.IdUtilizador == u.UtilizadorAlteracao).Username,
                        });
            }
            else
            {
                listaDespesas = _moduloContribuicoesContext.Compromisso
                        .Include(e => e.ComponenteDespesaRegistoFkNavigation)
                        .ThenInclude(e => e.DepartamentoFkNavigation)
                        .Include(e => e.ComponenteDespesaRegistoFkNavigation)
                        .ThenInclude(e => e.CentroCustoFkNavigation)
                        .Include(e => e.ComponenteDespesaRegistoFkNavigation)
                        .ThenInclude(e => e.TipoContaFkNavigation)
                        .Include(e => e.ComponenteDespesaRegistoFkNavigation)
                        .ThenInclude(e => e.TarefaActivoFkNavigation)
                        .ThenInclude(e => e.ProcessoAtivoFkNavigation)
                        .Include(e => e.ComponenteDespesaRegistoFkNavigation)
                        .ThenInclude(e => e.AgrupamentoConfigFkNavigation)
                        .ThenInclude(e => e.ParentFkNavigation)
                        .Where(u => u.IndActivo && 
                                    (beginDate.HasValue && !endDate.HasValue ? u.DataAlteracao.HasValue ? u.DataAlteracao.Value.Date == beginDate : u.DataCriacao.Date == beginDate :
                                     beginDate.HasValue && endDate.HasValue ? u.DataAlteracao.HasValue ? u.DataAlteracao.Value.Date >= beginDate && u.DataAlteracao.Value.Date <= endDate : u.DataCriacao.Date >= beginDate && u.DataCriacao.Date <= endDate : true))
                        .Select(u => new DespesasRelatoriosDataContract
                        {
                            Id = u.Id,
                            DepartamentoINSS = u.ComponenteDespesaRegistoFkNavigation.DepartamentoFkNavigation.Nome,
                            CentroCusto = u.ComponenteDespesaRegistoFkNavigation.CentroCustoFkNavigation.Descricao,
                            TipoConta = u.ComponenteDespesaRegistoFkNavigation.TipoContaFkNavigation.Descricao,
                            _ContaOSS = u.ComponenteDespesaRegistoFkNavigation.AgrupamentoConfigFkNavigation,
                            Descricao = u.Nome,
                            Valor = u.Valor,
                            Data = u.DataAlteracao ?? u.DataCriacao,
                            NumeroProcesso = u.ComponenteDespesaRegistoFkNavigation.TarefaActivoFkNavigation.ProcessoAtivoFkNavigation.NumeroProcesso,
                            UtilizadorAlteracao = _moduloContribuicoesContext.Utilizador.First(e => e.IdUtilizador == u.UtilizadorAlteracao).Username,
                        });
            }


            var despesas = listaDespesas
                .OrderBy(request.filter.orderBy, request.filter.orderDirection)
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = listaDespesas.Count();
            response.rows = totalNumber;
            response.Despesas = despesas;

            return response;
        }

        public string GetDespesasRelatorioExcel(GetDespesasRelatorioRequest request, List<DespesasRelatoriosDataContract> lista)
        {
            // Inicialização do documento excel
            var excelDocument = new ExcelDocument(_localizer["despesas"].Value, new ExcelDocumentOptions()
            {
                TextStyles = Extensions.ServiceExtensions.ExcelDocumentTextStyles
            });

            string estado = "";

            switch (request.EstadoDespesa)
            {
                case EstadoDespesaEnum.Autorizado:
                    estado = _localizer["autorizada"].Value;
                    break;
                case EstadoDespesaEnum.Cabimentado:
                    estado = _localizer["cabimentada"].Value;
                    break;
                case EstadoDespesaEnum.Compromisso:
                    estado = _localizer["compromisso"].Value;
                    break;
                case EstadoDespesaEnum.Obricacao:
                    estado = _localizer["obrigacao"].Value;
                    break;
                case EstadoDespesaEnum.Executado:
                    estado = _localizer["executada"].Value;
                    break;
                default:
                    break;
            }

            // Adição dos títulos
            excelDocument.Pages[0].AddText(_localizer["despesas"].Value, new ExcelDocumentTextPosition(1, 1), "Header", new ExcelDocumentTextPosition(9, 1));
            excelDocument.Pages[0].AddText(_localizer["estadoDespesa"].Value + ": " + estado, new ExcelDocumentTextPosition(1, 2), "Header", new ExcelDocumentTextPosition(9, 2));

            // Adição da tabela
            excelDocument.Pages[0].AddTable(new ExcelDocumentTextPosition(1, 4), lista, new List<ColumnOption<DespesasRelatoriosDataContract>>()
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
                    Name = _localizer["contaOSS"].Value,
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
                    Name = _localizer["numeroProcesso"].Value,
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
            }, new TableOptions() { AutoFitColumns = false });

            // Adição da linha de Total como footer da tabela
            excelDocument.Pages[0].AddText(_localizer["total"].Value + ":", new ExcelDocumentTextPosition(1, 5 + lista.Count), "HeaderAlignRight", new ExcelDocumentTextPosition(5, 5 + lista.Count));

            var sumRange = new ExcelDocumentTextPositionRange(new ExcelDocumentTextPosition(6, 5), new ExcelDocumentTextPosition(6, 5 + lista.Count - 1));
            excelDocument.Pages[0].AddSum(new ExcelDocumentTextPosition(6, 5 + lista.Count), sumRange, "HeaderAlignLeftMoney", new ExcelDocumentTextPosition(9, 5 + lista.Count));

            // Conversão do documento excel em base64
            return excelDocument.GetFileString();
        }

        public List<DespesaCompromissoDataContract> GetAllDespesaCompromissoByTarefaAtivoId(int tarefaAtivo)
        {

            var tarefa = _moduloContribuicoesContext.Tarefaativo.First(e => e.Id == tarefaAtivo);

            return _moduloContribuicoesContext.Compromisso
              .Where(u => u.IndActivo && u.TarefaAtivoFkNavigation.ProcessoAtivoFk == tarefa.ProcessoAtivoFk)
              .Select(u => new DespesaCompromissoDataContract
              {
                  Id = u.Id,
                  descricaoDespesa = u.ComponenteDespesaRegistoFkNavigation.Descricao,
                  nomeCompromisso = u.Nome,
                  valorCompromisso = u.Valor,
                  dataCompromisso = u.Data,
                  despesaRegistadaFk = u.ComponenteDespesaRegistoFk
              })
               .ToList();
        }
    }
}