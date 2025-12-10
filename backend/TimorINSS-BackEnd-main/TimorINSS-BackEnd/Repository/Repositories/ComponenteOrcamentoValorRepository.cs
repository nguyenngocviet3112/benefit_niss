using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class ComponenteOrcamentoValorRepository : IComponenteOrcamentoValorRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public ComponenteOrcamentoValorRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Componenteorcamentovalor> GetAll()
        {
            return _moduloContribuicoesContext.Componenteorcamentovalor.Where(u => u.IndActivo).ToList();
        }

        public Componenteorcamentovalor Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var componenteOrcamentoValor = _moduloContribuicoesContext.Componenteorcamentovalor
                .SingleOrDefault(u => u.Id == id);

            return componenteOrcamentoValor;
        }

        public ComponenteOrcamentoValorDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var componenteorcamentovalor = _moduloContribuicoesContext.Componenteorcamentovalor
                .SingleOrDefault(u => u.Id == id);

            ComponenteOrcamentoValorDto componenteOrcamentoValorDto = Utils.MappClassToDto<Componenteorcamentovalor, ComponenteOrcamentoValorDto>(componenteorcamentovalor);
            return componenteOrcamentoValorDto;
        }

        public void Add(Componenteorcamentovalor entity)
        {
            _moduloContribuicoesContext.Componenteorcamentovalor.Add(entity);
        }

        public void Update(Componenteorcamentovalor entity)
        {
            Componenteorcamentovalor entityToUpdate = _moduloContribuicoesContext.Componenteorcamentovalor
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Componenteorcamentovalor entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public List<Componenteorcamentovalor> getAllByRegistoId(int id)
        {
            return _moduloContribuicoesContext.Componenteorcamentovalor
                .Include(c => c.AgrupamentoFkNavigation)
                    .ThenInclude(a => a.ReltipoDeContaOrcamentoConfigFkNavigation)
                        .ThenInclude(r => r.TipoContaFkNavigation)
                .Where(c => c.ComponenteOrcamentoRegistoFk == id && c.IndActivo)
                .ToList();
        }

        public List<Componenteorcamentovalor> SearchComponenteOrcamentoValor(ComponenteOrcamentoValorSearch filter)
        {
            var queryComponenteOrcamentoValor = _moduloContribuicoesContext.Componenteorcamentovalor
                .Include(c => c.AgrupamentoFkNavigation)
                    .ThenInclude(a => a.ReltipoDeContaOrcamentoConfigFkNavigation)
                        .ThenInclude(r => r.TipoContaFkNavigation)
                .Include(c => c.AgrupamentoFkNavigation)
                    .ThenInclude(a => a.InverseParentFkNavigation)
                .Include(c => c.TipoContaFkNavigation)
                .Include(c => c.DepartamentoFkNavigation)
                .Include(c => c.InstitutionFkNavigation)
                .Include(c => c.CentroCustoFkNavigation)
                .Where(c => c.ComponenteOrcamentoRegistoFk == filter.Id && c.IndActivo);

            if (filter.TiposDeConta != null && filter.TiposDeConta.Count > 0)
            {
                queryComponenteOrcamentoValor = queryComponenteOrcamentoValor.Where(c => c.DepartamentoFk.HasValue && c.CentroCustoFk.HasValue && filter.TiposDeConta.Contains(c.TipoContaFk.Value));
            }

            if (filter.CentrosDeCusto != null && filter.CentrosDeCusto.Count > 0)
            {
                queryComponenteOrcamentoValor = queryComponenteOrcamentoValor.Where(c => c.CentroCustoFk.HasValue && filter.CentrosDeCusto.Contains(c.CentroCustoFk.Value));
            }

            if (filter.Departamentos != null && filter.Departamentos.Count > 0)
            {
                queryComponenteOrcamentoValor = queryComponenteOrcamentoValor.Where(c => c.DepartamentoFk.HasValue && filter.Departamentos.Contains(c.DepartamentoFk.Value));
            }

            return queryComponenteOrcamentoValor.ToList();
        }

        public Componenteorcamentovalor getOrcamentoValorByAgrupamentoAndOrcamentoRegisto(int agrupamentoId, int orcamentoRegistoID)
        {
            return _moduloContribuicoesContext.Componenteorcamentovalor
                .Where(c => c.IndActivo && c.AgrupamentoFk == agrupamentoId)
                .FirstOrDefault();
        }

        public Componenteorcamentovalor getOrcamentoValorByAgrupamentoAndOrcamentoRegistoInactivo(int agrupamentoId, int orcamentoRegistoID)
        {
            return _moduloContribuicoesContext.Componenteorcamentovalor
                .Where(c => c.AgrupamentoFk == agrupamentoId)
                .FirstOrDefault();
        }

        public List<Componenteorcamentovalor> getAllComponentesOrcamentoValorByRegisto(int orcamentoRegistoID)
        {
            return _moduloContribuicoesContext.Componenteorcamentovalor
                .Where(c => c.IndActivo && c.ComponenteOrcamentoRegistoFk == orcamentoRegistoID)
                .ToList();
        }

        public List<Componenteorcamentovalor> getOrcamentoValorByAgrupamentoFkOrcamentoRegistoFk(int agrupamentoId, int orcamentoRegistoID)
        {
            return _moduloContribuicoesContext.Componenteorcamentovalor
                .Where(c => c.IndActivo && c.AgrupamentoFk == agrupamentoId && c.ComponenteOrcamentoRegistoFk == orcamentoRegistoID
                )
                .ToList();
        }

        public List<Componenteorcamentovalor> getOrcamentoValorByAgrupamentoFkOrcamentoRegistoFk(int agrupamentoId, int orcamentoRegistoID,
            int institutionId, int actidadeId, int economicId, int funcionalId)
        {
            return _moduloContribuicoesContext.Componenteorcamentovalor
                .Where(c => c.IndActivo && c.AgrupamentoFk == agrupamentoId && c.ComponenteOrcamentoRegistoFk == orcamentoRegistoID
                && c.InstitutionId == institutionId && c.ActidadeFk == actidadeId
                && c.EconomicFk == economicId && c.FuncionalFk == funcionalId)
                .ToList();
        }

        public Componenteorcamentovalor GetComponenteOrcamentoValorSameForeignKeys(Componenteorcamentovalor componente)
        {
            return _moduloContribuicoesContext.Componenteorcamentovalor
                .Where(c => c.IndActivo && c.ComponenteOrcamentoRegistoFk == componente.ComponenteOrcamentoRegistoFk
                    && c.CentroCustoFk == componente.CentroCustoFk && c.DepartamentoFk == componente.DepartamentoFk
                    && c.AgrupamentoFk == componente.AgrupamentoFk
                    && c.ActidadeFk == componente.ActidadeFk
                    && c.EconomicFk == componente.EconomicFk
                    && c.FuncionalFk == componente.FuncionalFk
                    ).FirstOrDefault();
        }

        public List<ExcTractOrcamentoValor> SearchComponenteOrcamentoValorExtraction(ComponenteOrcamentoValorSearch filter)
        {
            var queryComponenteOrcamentoValor = _moduloContribuicoesContext.Componenteorcamentovalor
                .Include(c => c.AgrupamentoFkNavigation)
                    .ThenInclude(a => a.ReltipoDeContaOrcamentoConfigFkNavigation)
                        .ThenInclude(r => r.TipoContaFkNavigation)
                .Include(c => c.AgrupamentoFkNavigation)
                    .ThenInclude(a => a.InverseParentFkNavigation)
                .Include(c => c.DepartamentoFkNavigation)
                .Include(c => c.CentroCustoFkNavigation)
                .Where(c => c.ComponenteOrcamentoRegistoFk == filter.Id && c.IndActivo && c.CentroCustoFk.HasValue && c.DepartamentoFk.HasValue);

            if (filter.TiposDeConta != null && filter.TiposDeConta.Count > 0)
            {
                queryComponenteOrcamentoValor = queryComponenteOrcamentoValor.Where(c => filter.TiposDeConta.Contains(c.AgrupamentoFkNavigation.ReltipoDeContaOrcamentoConfigFkNavigation.TipoContaFk));
            }

            if (filter.CentrosDeCusto != null && filter.CentrosDeCusto.Count > 0)
            {
                queryComponenteOrcamentoValor = queryComponenteOrcamentoValor.Where(c => filter.CentrosDeCusto.Contains(c.CentroCustoFk.Value));
            }

            if (filter.Departamentos != null && filter.Departamentos.Count > 0)
            {
                queryComponenteOrcamentoValor = queryComponenteOrcamentoValor.Where(c => filter.Departamentos.Contains(c.DepartamentoFk.Value));
            }

            List<Componenteorcamentovalor> queryResult = queryComponenteOrcamentoValor.ToList();
            List<ExcTractOrcamentoValor> result = new List<ExcTractOrcamentoValor>();
            List<Agrupamentoconfig> listaIte;
            List<ExcTractOrcamentoValor> hierarquiaIte;
            Agrupamentoconfig agrupamento;
            ExcTractOrcamentoValor selected;
            ExcTractOrcamentoValor linhaExcel;
            foreach (Componenteorcamentovalor valor in queryResult)
            {
                listaIte = new List<Agrupamentoconfig>();
                int? parentID = valor.AgrupamentoFkNavigation.ParentFk;
                listaIte.Add(valor.AgrupamentoFkNavigation);
                // While para ir buscar o pais
                while (parentID.HasValue)
                {
                    agrupamento = _moduloContribuicoesContext.Agrupamentoconfig.Where(a => a.Id == parentID.Value).FirstOrDefault();
                    listaIte.Add(agrupamento);
                    parentID = agrupamento.ParentFk;
                }

                listaIte.Reverse();
                List<string> savedCodes = new List<string>();
                int x = valor.Id;
                for (int i = 0; i < listaIte.Count(); i++)
                {
                    hierarquiaIte = new List<ExcTractOrcamentoValor>();
                    agrupamento = listaIte[i];
                    selected = result.FirstOrDefault(p => p.AgrupamentoId == agrupamento.Id && p.CentroCustoFk == valor.CentroCustoFk.Value && p.DepartamentoFk == valor.DepartamentoFk.Value);
                    if (selected == null)
                    {
                        linhaExcel = new ExcTractOrcamentoValor
                        {
                            AgrupamentoId = agrupamento.Id,
                            CentroCustoFk = valor.CentroCustoFk.Value,
                            DepartamentoFk = valor.DepartamentoFk.Value,
                            TipoContaFk = valor.AgrupamentoFkNavigation.ReltipoDeContaOrcamentoConfigFkNavigation.TipoContaFk,
                            Designacao = agrupamento.Designacao,
                            CentrosCusto = valor.CentroCustoFkNavigation.Descricao,
                            TipoConta = valor.AgrupamentoFkNavigation.ReltipoDeContaOrcamentoConfigFkNavigation.TipoContaFkNavigation.Descricao,
                            Departamento = valor.DepartamentoFkNavigation.Nome,
                            Valor = valor.Valor
                        };

                        switch (i)
                        {
                            case 4:
                                linhaExcel.Agrupamento = savedCodes[0];
                                linhaExcel.SubAgrupamento = savedCodes[1];
                                linhaExcel.Rubrica = savedCodes[2];
                                linhaExcel.Alinea = savedCodes[3];
                                linhaExcel.SubAlinea = agrupamento.Codigo;
                                break;

                            case 3:
                                linhaExcel.Agrupamento = savedCodes[0];
                                linhaExcel.SubAgrupamento = savedCodes[1];
                                linhaExcel.Rubrica = savedCodes[2];
                                linhaExcel.Alinea = agrupamento.Codigo;
                                break;

                            case 2:
                                linhaExcel.Agrupamento = savedCodes[0];
                                linhaExcel.SubAgrupamento = savedCodes[1];
                                linhaExcel.Rubrica = agrupamento.Codigo;
                                break;

                            case 1:
                                linhaExcel.Agrupamento = savedCodes[0];
                                linhaExcel.SubAgrupamento = agrupamento.Codigo;
                                break;

                            case 0:
                                linhaExcel.Agrupamento = agrupamento.Codigo;
                                break;
                        }
                        result.Add(linhaExcel);
                    }
                    else
                    {
                        selected.Valor += valor.Valor;
                    }
                    savedCodes.Add(agrupamento.Codigo);
                }
            }

            result = result.OrderBy(v => v.TipoContaFk).ThenBy(v => v.DepartamentoFk).ThenBy(v => v.CentroCustoFk).ThenBy(v => v.Agrupamento).ThenBy(v => v.SubAgrupamento).ThenBy(v => v.Rubrica).ThenBy(v => v.Alinea).ThenBy(v => v.SubAlinea).ToList();

            return result;
        }
    }
}