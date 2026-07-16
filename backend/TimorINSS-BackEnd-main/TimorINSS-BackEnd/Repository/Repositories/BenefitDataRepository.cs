using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    // Repository độc lập cho bộ API "api/benefit-data" (BRD Benefit module §18).
    public class BenefitDataRepository : IBenefitDataRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _context;

        public BenefitDataRepository(TimorINSSModuloContribuicoesContext context)
        {
            _context = context;
        }

        public Entidadeempregadora GetCompanyMasterByNiss(string nissCompany)
        {
            return _context.Entidadeempregadora
                .Include(e => e.EntidadeNatJuridicaFkNavigation)
                .Include(e => e.EntidadeSectorActFkNavigation)
                .Include(e => e.EntidadeActEconomicaFkNavigation)
                .Include(e => e.Morada).ThenInclude(m => m.MoradaAldeiaFkNavigation).ThenInclude(a => a.AldeiaSucoFkNavigation).ThenInclude(s => s.SucoPostoAdminFkNavigation).ThenInclude(p => p.PostoAdminMunicipioFkNavigation)
                .Include(e => e.Morada).ThenInclude(m => m.MoradaPaisFkNavigation)
                .Include(e => e.Contacto)
                .AsSplitQuery()
                .FirstOrDefault(e => e.Niss == nissCompany);
        }

        public Trabalhador GetWorkerFullByNiss(string niss)
        {
            return _context.Trabalhador
                .Include(t => t.SexoTrabalhadorNavigation)
                .Include(t => t.EstadoCivilNavigation)
                .Include(t => t.NacionalidadeTrabalhadorNavigation)
                .Include(t => t.Morada).ThenInclude(m => m.MoradaAldeiaFkNavigation).ThenInclude(a => a.AldeiaSucoFkNavigation).ThenInclude(s => s.SucoPostoAdminFkNavigation).ThenInclude(p => p.PostoAdminMunicipioFkNavigation)
                .Include(t => t.Morada).ThenInclude(m => m.MoradaPaisFkNavigation)
                .Include(t => t.Contacto)
                .Include(t => t.Documentoidentificacao).ThenInclude(d => d.TpDocIdentificacaoNavigation)
                .Include(t => t.Inssestrangeiro)
                .AsSplitQuery()
                .FirstOrDefault(t => t.Niss == niss);
        }

        public Documentoidentificacao GetDocumentoById(int idDoc)
        {
            return _context.Documentoidentificacao
                .Include(d => d.TpDocIdentificacaoNavigation)
                .FirstOrDefault(d => d.IdDocIdentificacao == idDoc);
        }

        public (Trabalhador worker, List<Relentidadetrabalhador> contratos, List<Suspensoes> suspensoes) GetContributionHistoryByNiss(string niss)
        {
            Trabalhador worker = _context.Trabalhador.FirstOrDefault(t => t.Niss == niss);
            if (worker == null)
            {
                return (null, new List<Relentidadetrabalhador>(), new List<Suspensoes>());
            }

            List<Relentidadetrabalhador> contratos = _context.Relentidadetrabalhador
                .Include(r => r.EntidadeFkNavigation)
                .Include(r => r.RegimeFkNavigation)
                .Include(r => r.Declaracaoremuneracao.Where(d => d.IndActivo))
                    .ThenInclude(d => d.RegimeFkNavigation)
                .Where(r => r.TrabalhadorFk == worker.IdTrabalhador)
                .AsSplitQuery()
                .ToList();

            List<Suspensoes> suspensoes = _context.Suspensoes
                .Where(s => s.TrabalhadorSuspensaoFk == worker.IdTrabalhador && s.IndActivo)
                .ToList();

            return (worker, contratos, suspensoes);
        }
    }
}
