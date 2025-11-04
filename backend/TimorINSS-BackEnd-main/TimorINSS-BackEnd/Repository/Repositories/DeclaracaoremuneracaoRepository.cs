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