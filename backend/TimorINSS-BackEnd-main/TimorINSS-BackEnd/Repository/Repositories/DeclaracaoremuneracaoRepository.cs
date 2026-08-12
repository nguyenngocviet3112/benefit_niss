using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class DeclaracaoremuneracaoRepository : IDeclaracaoremuneracaoRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public DeclaracaoremuneracaoRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Declaracaoremuneracao> GetAll()
        {
            return _moduloContribuicoesContext.Declaracaoremuneracao
                .Include(d => d.DeclaracaoRelEntidadeTrabalhadorFkNavigation)
                .ToList();
        }

        public Declaracaoremuneracao Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var declaracaoremuneracao = _moduloContribuicoesContext.Declaracaoremuneracao
                .Include(d => d.DeclaracaoRelEntidadeTrabalhadorFkNavigation)
                .SingleOrDefault(d => d.IdDeclaracao == id);

            return declaracaoremuneracao;
        }

        public DeclaracaoremuneracaoDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var declaracaoremuneracao = _moduloContribuicoesContext.Declaracaoremuneracao
                .SingleOrDefault(d => d.IdDeclaracao == id);

            DeclaracaoremuneracaoDto declaracaoremuneracaoDto = Utils.MappClassToDto<Declaracaoremuneracao, DeclaracaoremuneracaoDto>(declaracaoremuneracao);
            return declaracaoremuneracaoDto;
        }

        public void Add(Declaracaoremuneracao entity)
        {
            _moduloContribuicoesContext.Declaracaoremuneracao.Add(entity);
        }

        public void Update(Declaracaoremuneracao entity)
        {
            Declaracaoremuneracao entityToUpdate = _moduloContribuicoesContext.Declaracaoremuneracao
                .Single(d => d.IdDeclaracao == entity.IdDeclaracao);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Declaracaoremuneracao entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public List<Declaracaoremuneracao> GetByEntidadeAndDate(int entidadeId, DateTime date)
        {
            return _moduloContribuicoesContext.Declaracaoremuneracao
                .Include(d => d.DeclaracaoRelEntidadeTrabalhadorFkNavigation)
                .Where(d => d.DeclaracaoRelEntidadeTrabalhadorFkNavigation.EntidadeFk == entidadeId && date == d.MesAno && d.IndActivo).ToList();
        }

        public List<Declaracaoremuneracao> GetLastDeclaracaoOficiosa(int entidadeId, DateTime date)
        {
            var mesAno = _moduloContribuicoesContext.Declaracaoremuneracao
                .Where(d => d.DeclaracaoRelEntidadeTrabalhadorFkNavigation.EntidadeFk == entidadeId && date >= d.MesAno && d.IndActivo && d.Oficioso)
                .OrderByDescending(d => d.MesAno).Select(d => d.MesAno).FirstOrDefault();

            if (mesAno >= System.Data.SqlTypes.SqlDateTime.MinValue.Value)
                return _moduloContribuicoesContext.Declaracaoremuneracao
                    .Include(d => d.DeclaracaoRelEntidadeTrabalhadorFkNavigation)
                    .Where(d => d.DeclaracaoRelEntidadeTrabalhadorFkNavigation.EntidadeFk == entidadeId && mesAno == d.MesAno && d.IndActivo).ToList();
            else
                return new List<Declaracaoremuneracao>();
        }

        public DeclaracaoListagem GetDeclaracaoTrabalhadorInfo(DeclaracaoRemuneracaoDataContract declaracao)
        {
            DeclaracaoListagem res = new DeclaracaoListagem();

            DeclaracaoTrabalhadorInfo infoDeclaracao;

            Relentidadetrabalhador rel;

            if (declaracao.regimeFk == 0)
            {
                rel = _moduloContribuicoesContext.Relentidadetrabalhador
                .Include(r => r.TrabalhadorFkNavigation)
                    .ThenInclude(t => t.NacionalidadeTrabalhadorNavigation)
                .Include(r => r.TrabalhadorFkNavigation)
                    .ThenInclude(t => t.SexoTrabalhadorNavigation)
                .Include(r => r.RegimeFkNavigation)
                    .ThenInclude(rg => rg.RegimeRegimePaiNavigation)
                        .ThenInclude(rgp => rgp.TipoRegimeNavigation)
                .Where(r => r.IdRel == declaracao.declaracaoRelEntidadeTrabalhadorFk &&
                            r.RegimeFkNavigation.RegimeRegimePaiNavigation.Any(rgp => rgp.DataInicio <= declaracao.mesAno &&
                                                                                      (rgp.DataFim ?? System.Data.SqlTypes.SqlDateTime.MaxValue.Value) >= declaracao.mesAno)
                )
                .FirstOrDefault();

                string tipoRegime = rel.RegimeFkNavigation.RegimeRegimePaiNavigation.FirstOrDefault().TipoRegimeNavigation.Descricao;

                declaracao.regimeFk = rel.RegimeFkNavigation.RegimeRegimePaiNavigation.FirstOrDefault().IdRegime;
                declaracao.sexoFk = rel.TrabalhadorFkNavigation.SexoTrabalhador;
                declaracao.nacionalidadeFk = rel.TrabalhadorFkNavigation.NacionalidadeTrabalhador;

                infoDeclaracao = new DeclaracaoTrabalhadorInfo
                {
                    nacionalidade = rel.TrabalhadorFkNavigation.NacionalidadeTrabalhadorNavigation.Descricao,
                    niss = rel.TrabalhadorFkNavigation.Niss,
                    nome = rel.TrabalhadorFkNavigation.Nome,
                    regime = rel.RegimeFkNavigation.Descricao,
                    tipoRegime = tipoRegime,
                    sexo = rel.TrabalhadorFkNavigation.SexoTrabalhadorNavigation.Descricao
                };

                if (infoDeclaracao.tipoRegime == "E")
                {
                    Escalao escalao = _moduloContribuicoesContext.Escalao.Where(e => e.IdEscalao == rel.EscalaoFk).FirstOrDefault();
                    if (escalao == null)
                        return res;

                    decimal valorEscalao = escalao.Valor;
                    declaracao.remunDeclarada = valorEscalao;
                }
                else
                {
                    decimal decimoTerceiroMes = getTemDecimoTerceiroMes(declaracao.mesAno, declaracao.declaracaoRelEntidadeTrabalhadorFk);
                    declaracao.decimoTerceiroMes = decimoTerceiroMes > 0;
                    declaracao.decimoTerceiro = decimoTerceiroMes;
                }
            }
            else
            {
                Trabalhador trabalhador = _moduloContribuicoesContext.Relentidadetrabalhador
                    .Include(r => r.TrabalhadorFkNavigation)
                    .Where(t => t.IdRel == declaracao.declaracaoRelEntidadeTrabalhadorFk)
                    .Select(r => r.TrabalhadorFkNavigation).FirstOrDefault();

                if (trabalhador == null)
                    return res;

                Regime regime = _moduloContribuicoesContext.Regime
                    .Include(r => r.TipoRegimeNavigation)
                    .Where(r => r.IdRegime == declaracao.regimeFk).FirstOrDefault();

                if (regime == null)
                    return res;

                Dominio tipoRegime = regime.TipoRegimeNavigation;

                if (tipoRegime == null)
                    return res;

                Dominio nacionalidade = _moduloContribuicoesContext.Dominio
                    .Where(d => d.IdDominio == declaracao.nacionalidadeFk).FirstOrDefault();

                if (nacionalidade == null)
                    return res;

                Dominio sexo = _moduloContribuicoesContext.Dominio
                    .Where(d => d.IdDominio == declaracao.sexoFk).FirstOrDefault();

                if (sexo == null)
                    return res;

                infoDeclaracao = new DeclaracaoTrabalhadorInfo
                {
                    nacionalidade = nacionalidade.Descricao,
                    niss = trabalhador.Niss,
                    nome = trabalhador.Nome,
                    regime = regime.NomeRegime,
                    tipoRegime = tipoRegime.Descricao,
                    sexo = sexo.Descricao
                };

                declaracao.decimoTerceiroMes = declaracao.decimoTerceiro > 0;
            }

            res.declaracao = declaracao;
            res.trabalhadorInfo = infoDeclaracao;

            return res;
        }

        // Versão em lote de GetDeclaracaoTrabalhadorInfo -- mesmas regras/ramos de negócio linha a linha,
        // mas faz um punhado de queries para toda a lista de declaracoes em vez de várias queries (multi-join)
        // por trabalhador. Motivo: GetDeclaracaoByEntidadeAndFilter chama isto dentro de um foreach por
        // trabalhador, o que para uma entidade com muitos trabalhadores gerava um N+1 severo.
        public Dictionary<int, DeclaracaoListagem> GetDeclaracaoTrabalhadorInfoBatch(List<DeclaracaoRemuneracaoDataContract> declaracoes, DateTime mesAno)
        {
            var resultado = new Dictionary<int, DeclaracaoListagem>();

            if (declaracoes == null || !declaracoes.Any())
                return resultado;

            var grupoNovo = declaracoes.Where(d => d.regimeFk == 0).ToList();
            var grupoExistente = declaracoes.Where(d => d.regimeFk != 0).ToList();

            // ---- Grupo "regimeFk == 0" (mesma condição/Include da branch original) ----
            if (grupoNovo.Any())
            {
                var relIds = grupoNovo.Select(d => d.declaracaoRelEntidadeTrabalhadorFk).ToList();

                var rels = _moduloContribuicoesContext.Relentidadetrabalhador
                    .Include(r => r.TrabalhadorFkNavigation)
                        .ThenInclude(t => t.NacionalidadeTrabalhadorNavigation)
                    .Include(r => r.TrabalhadorFkNavigation)
                        .ThenInclude(t => t.SexoTrabalhadorNavigation)
                    .Include(r => r.RegimeFkNavigation)
                        .ThenInclude(rg => rg.RegimeRegimePaiNavigation)
                            .ThenInclude(rgp => rgp.TipoRegimeNavigation)
                    .Where(r => relIds.Contains(r.IdRel) &&
                                r.RegimeFkNavigation.RegimeRegimePaiNavigation.Any(rgp => rgp.DataInicio <= mesAno &&
                                                                                          (rgp.DataFim ?? System.Data.SqlTypes.SqlDateTime.MaxValue.Value) >= mesAno))
                    .ToList()
                    .ToDictionary(r => r.IdRel);

                // 1ª passagem: descobre o tipoRegime de cada linha (precisa disto para saber se vai
                // precisar de Escalao ou de decimoTerceiro no lote seguinte)
                var tipoRegimePorRel = new Dictionary<int, string>();
                var escalaoIdsNecessarios = new List<int>();
                var decimoTerceiroIdsNecessarios = new List<int>();

                foreach (var d in grupoNovo)
                {
                    if (!rels.ContainsKey(d.declaracaoRelEntidadeTrabalhadorFk))
                        continue; // idêntico ao original: rel==null vai rebentar mais abaixo (dado corrompido/impossível dentro do mesmo pedido)

                    var rel = rels[d.declaracaoRelEntidadeTrabalhadorFk];
                    var regimePai = rel.RegimeFkNavigation.RegimeRegimePaiNavigation.FirstOrDefault();
                    var tipoRegime = regimePai.TipoRegimeNavigation.Descricao;
                    tipoRegimePorRel[d.declaracaoRelEntidadeTrabalhadorFk] = tipoRegime;

                    if (tipoRegime == "E")
                    {
                        if (rel.EscalaoFk.HasValue)
                            escalaoIdsNecessarios.Add(rel.EscalaoFk.Value);
                    }
                    else
                    {
                        decimoTerceiroIdsNecessarios.Add(d.declaracaoRelEntidadeTrabalhadorFk);
                    }
                }

                var escalaoDict = _moduloContribuicoesContext.Escalao
                    .Where(e => escalaoIdsNecessarios.Contains(e.IdEscalao))
                    .ToList()
                    .ToDictionary(e => e.IdEscalao);

                var decimoTerceiroDict = _moduloContribuicoesContext.Declaracaoremuneracao
                    .Where(dr => dr.MesAno.Year == mesAno.Year &&
                                 decimoTerceiroIdsNecessarios.Contains(dr.DeclaracaoRelEntidadeTrabalhadorFk) &&
                                 dr.IndActivo && dr.DecimoTerceiro > 0)
                    .Select(dr => new { dr.DeclaracaoRelEntidadeTrabalhadorFk, dr.DecimoTerceiro })
                    .ToList()
                    .GroupBy(x => x.DeclaracaoRelEntidadeTrabalhadorFk)
                    .ToDictionary(g => g.Key, g => g.First().DecimoTerceiro);

                // 2ª passagem: constrói o resultado final de cada linha, replicando exactamente a lógica original
                foreach (var d in grupoNovo)
                {
                    var res = new DeclaracaoListagem();
                    var rel = rels[d.declaracaoRelEntidadeTrabalhadorFk];

                    string tipoRegime = tipoRegimePorRel[d.declaracaoRelEntidadeTrabalhadorFk];

                    d.regimeFk = rel.RegimeFkNavigation.RegimeRegimePaiNavigation.FirstOrDefault().IdRegime;
                    d.sexoFk = rel.TrabalhadorFkNavigation.SexoTrabalhador;
                    d.nacionalidadeFk = rel.TrabalhadorFkNavigation.NacionalidadeTrabalhador;

                    var infoDeclaracao = new DeclaracaoTrabalhadorInfo
                    {
                        nacionalidade = rel.TrabalhadorFkNavigation.NacionalidadeTrabalhadorNavigation.Descricao,
                        niss = rel.TrabalhadorFkNavigation.Niss,
                        nome = rel.TrabalhadorFkNavigation.Nome,
                        regime = rel.RegimeFkNavigation.Descricao,
                        tipoRegime = tipoRegime,
                        sexo = rel.TrabalhadorFkNavigation.SexoTrabalhadorNavigation.Descricao
                    };

                    if (tipoRegime == "E")
                    {
                        if (!rel.EscalaoFk.HasValue || !escalaoDict.ContainsKey(rel.EscalaoFk.Value))
                        {
                            resultado[d.declaracaoRelEntidadeTrabalhadorFk] = res; // igual ao "if (escalao == null) return res;" original
                            continue;
                        }

                        d.remunDeclarada = escalaoDict[rel.EscalaoFk.Value].Valor;
                    }
                    else
                    {
                        decimal decimoTerceiroMes = decimoTerceiroDict.ContainsKey(d.declaracaoRelEntidadeTrabalhadorFk)
                            ? decimoTerceiroDict[d.declaracaoRelEntidadeTrabalhadorFk] : 0;
                        d.decimoTerceiroMes = decimoTerceiroMes > 0;
                        d.decimoTerceiro = decimoTerceiroMes;
                    }

                    res.declaracao = d;
                    res.trabalhadorInfo = infoDeclaracao;
                    resultado[d.declaracaoRelEntidadeTrabalhadorFk] = res;
                }
            }

            // ---- Grupo "regimeFk != 0" (mesma condição/Include da branch original "else") ----
            if (grupoExistente.Any())
            {
                var relIds = grupoExistente.Select(d => d.declaracaoRelEntidadeTrabalhadorFk).ToList();
                var regimeIds = grupoExistente.Select(d => d.regimeFk).Distinct().ToList();
                var nacionalidadeIds = grupoExistente.Select(d => d.nacionalidadeFk).Distinct().ToList();
                var sexoIds = grupoExistente.Select(d => d.sexoFk).Distinct().ToList();

                var trabalhadoresDict = _moduloContribuicoesContext.Relentidadetrabalhador
                    .Include(r => r.TrabalhadorFkNavigation)
                    .Where(r => relIds.Contains(r.IdRel))
                    .ToList()
                    .ToDictionary(r => r.IdRel, r => r.TrabalhadorFkNavigation);

                var regimesDict = _moduloContribuicoesContext.Regime
                    .Include(r => r.TipoRegimeNavigation)
                    .Where(r => regimeIds.Contains(r.IdRegime))
                    .ToList()
                    .ToDictionary(r => r.IdRegime);

                var dominiosNecessarios = nacionalidadeIds.Concat(sexoIds).Distinct().ToList();
                var dominiosDict = _moduloContribuicoesContext.Dominio
                    .Where(dm => dominiosNecessarios.Contains(dm.IdDominio))
                    .ToList()
                    .ToDictionary(dm => dm.IdDominio);

                foreach (var d in grupoExistente)
                {
                    var res = new DeclaracaoListagem();

                    if (!trabalhadoresDict.ContainsKey(d.declaracaoRelEntidadeTrabalhadorFk) || trabalhadoresDict[d.declaracaoRelEntidadeTrabalhadorFk] == null)
                    {
                        resultado[d.declaracaoRelEntidadeTrabalhadorFk] = res; // "if (trabalhador == null) return res;"
                        continue;
                    }
                    var trabalhador = trabalhadoresDict[d.declaracaoRelEntidadeTrabalhadorFk];

                    if (!regimesDict.ContainsKey(d.regimeFk))
                    {
                        resultado[d.declaracaoRelEntidadeTrabalhadorFk] = res; // "if (regime == null) return res;"
                        continue;
                    }
                    var regime = regimesDict[d.regimeFk];

                    var tipoRegime = regime.TipoRegimeNavigation;
                    if (tipoRegime == null)
                    {
                        resultado[d.declaracaoRelEntidadeTrabalhadorFk] = res; // "if (tipoRegime == null) return res;"
                        continue;
                    }

                    if (!dominiosDict.ContainsKey(d.nacionalidadeFk))
                    {
                        resultado[d.declaracaoRelEntidadeTrabalhadorFk] = res; // "if (nacionalidade == null) return res;"
                        continue;
                    }
                    var nacionalidade = dominiosDict[d.nacionalidadeFk];

                    if (!dominiosDict.ContainsKey(d.sexoFk))
                    {
                        resultado[d.declaracaoRelEntidadeTrabalhadorFk] = res; // "if (sexo == null) return res;"
                        continue;
                    }
                    var sexo = dominiosDict[d.sexoFk];

                    var infoDeclaracao = new DeclaracaoTrabalhadorInfo
                    {
                        nacionalidade = nacionalidade.Descricao,
                        niss = trabalhador.Niss,
                        nome = trabalhador.Nome,
                        regime = regime.NomeRegime,
                        tipoRegime = tipoRegime.Descricao,
                        sexo = sexo.Descricao
                    };

                    d.decimoTerceiroMes = d.decimoTerceiro > 0;

                    res.declaracao = d;
                    res.trabalhadorInfo = infoDeclaracao;
                    resultado[d.declaracaoRelEntidadeTrabalhadorFk] = res;
                }
            }

            return resultado;
        }

        public decimal getTemDecimoTerceiroMes(DateTime mesAno, int idRel)
        {
            return _moduloContribuicoesContext.Declaracaoremuneracao
                .Where(d => d.MesAno.Year == mesAno.Year && idRel == d.DeclaracaoRelEntidadeTrabalhadorFk && d.IndActivo && d.DecimoTerceiro > 0)
                .Select(d => d.DecimoTerceiro).FirstOrDefault();
        }

        public List<Tuple<int, DateTime>> GetEntidadesAndFirstDateComDeclaracaoNaoRegistadasPastDay(DateTime date)
        {
            List<Tuple<int, DateTime>> empresas = new List<Tuple<int, DateTime>>();
            var queryResult = _moduloContribuicoesContext.Declaracaoremuneracao
                .Where(d => d.IndActivo)
                .Join(_moduloContribuicoesContext.Relentidadetrabalhador,
                    d => d.DeclaracaoRelEntidadeTrabalhadorFk,
                    r => r.IdRel,
                    (d, r) => new
                    {
                        empresa = r.EntidadeFk,
                        date = d.MesAno
                    })
                .GroupBy(d => d.empresa)
                .Select(gd => new
                {
                    empresa = gd.Key,
                    date = gd.Max(d => d.date)
                })
                .Where(gd => gd.date < date).ToList();

            foreach (var res in queryResult)
            {
                empresas.Add(new Tuple<int, DateTime>(res.empresa, res.date));
            }

            return empresas;
        }

        public bool ExistsDeclaracaoWithRegime(int idRegime)
        {
            return _moduloContribuicoesContext.Declaracaoremuneracao
                .Where(d => d.RegimeFk == idRegime).Count() > 0;
        }

        public RelatorioDeclaracaoRenumeracaoListagemResponse GetDeclaracoesRelatorios(RelatorioDeclaracaoRenumeracaoListagemRequest request)
        {
            RelatorioDeclaracaoRenumeracaoListagemResponse response = new RelatorioDeclaracaoRenumeracaoListagemResponse();

            int index = 0;
            if (request.filter.index.HasValue)
                index = request.filter.index.Value;

            int rows = 5;
            if (request.filter.rows.HasValue)
                rows = request.filter.rows.Value;

            var beginDate = request.filter.dateFilterBegin;
            var endDate = request.filter.dateFilterEnd;


            var listaDeclaracoes = _moduloContribuicoesContext.Declaracaoremuneracao
               .Include(e => e.DeclaracaoRelEntidadeTrabalhadorFkNavigation)
               .ThenInclude(e => e.EntidadeFkNavigation)
               .Include(e => e.ContaCorrenteFkNavigation.Guiapagamento)
               //.ThenInclude(e => e.GuiaPagamentoFkNavigation)
               .Include(e => e.DeclaracaoRelEntidadeTrabalhadorFkNavigation)
               .ThenInclude(e => e.TrabalhadorFkNavigation)
               .Where(e => e.IndActivo &&
                           // Filtrar por NISS/TIN do trabalhador ou entidade empregadora
                           (request.isTrabalhador ? (e.DeclaracaoRelEntidadeTrabalhadorFkNavigation.TrabalhadorFkNavigation.Niss == request.search ||
                                                    e.DeclaracaoRelEntidadeTrabalhadorFkNavigation.TrabalhadorFkNavigation.Tin == request.search) :
                                                 (e.DeclaracaoRelEntidadeTrabalhadorFkNavigation.EntidadeFkNavigation.Niss == request.search ||
                                                  e.DeclaracaoRelEntidadeTrabalhadorFkNavigation.EntidadeFkNavigation.Tin == request.search)
                            ) &&
                            // Filtrar por data
                            (beginDate.HasValue && endDate.HasValue ? (e.MesAno >= beginDate && e.MesAno <= endDate) :
                            beginDate.HasValue && !endDate.HasValue ? (e.MesAno == beginDate) :
                            true)
               )
               .Select(e => new RelatorioDeclaracaoRenumeracaoDataContract
               {
                   id = e.IdDeclaracao,
                   mesAno = e.MesAno,
                   nomeEmpregador = e.DeclaracaoRelEntidadeTrabalhadorFkNavigation.EntidadeFkNavigation.Nome,
                   nomeTrabalhador = e.DeclaracaoRelEntidadeTrabalhadorFkNavigation.TrabalhadorFkNavigation.Nome,
                   valorRenumeracoes = e.RemunDeclarada,
                   valorContribuicoes = e.ContaCorrenteFkNavigation.ValorEntidade,
                   // ✅ Tổng số tiền đã thanh toán (ép kiểu nullable để dùng ?? 0m)
                   valorPago = e.ContaCorrenteFkNavigation.Guiapagamento
                      //.Where(gp => gp.IndActivo) // nếu có cờ active, bật lại
                      .Sum(gp => (decimal?)gp.ValorComprovPag) ?? 0m,

                   // ✅ Nợ = Tổng phải thu - Đã trả
                   valorDivida = (e.ContaCorrenteFkNavigation.ValorTotal)
                     - (e.ContaCorrenteFkNavigation.Guiapagamento
                            //.Where(gp => gp.IndActivo)
                            .Sum(gp => (decimal?)gp.ValorComprovPag) ?? 0m),

                   //valorPago = e.ContaCorrenteFkNavigation.Guiapagamento.Select(e => e.ValorComprovPag).DefaultIfEmpty(0).Sum(e => e ?? 0),
                   //valorDivida = e.ContaCorrenteFkNavigation.ValorTotal - e.ContaCorrenteFkNavigation.Guiapagamento.Select(e => e.ValorComprovPag).DefaultIfEmpty(0).Sum(e => e ?? 0),
               });

            var processos = listaDeclaracoes
                .OrderBy(request.filter.orderBy, request.filter.orderDirection)
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = listaDeclaracoes.Count();
            response.rows = totalNumber;
            response.declaracoes = processos;

            return response;
        }
    }
}