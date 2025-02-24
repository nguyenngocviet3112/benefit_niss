using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class DominioRepository : IDominioRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public DominioRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Dominio> GetAll()
        {
            return _moduloContribuicoesContext.Dominio.ToList();
        }

        public Dominio Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var dominio = _moduloContribuicoesContext.Dominio
                .SingleOrDefault(u => u.IdDominio == id);

            return dominio;
        }

        public DominioDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var dominio = _moduloContribuicoesContext.Dominio
                .SingleOrDefault(u => u.IdDominio == id);

            DominioDto dominioDto = Utils.MappClassToDto<Dominio, DominioDto>(dominio);
            return dominioDto;
        }

        public void Add(Dominio entity)
        {
            _moduloContribuicoesContext.Dominio.Add(entity);
        }

        public void Update(Dominio entity)
        {
            Dominio entityToUpdate = _moduloContribuicoesContext.Dominio
                .Single(d => d.IdDominio == entity.IdDominio);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Dominio entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public List<DominioDescricaoString> getAllTiposDeDominio(TiposDominio tipo)
        {
            return _moduloContribuicoesContext.Dominio
                .Where(u => u.Dominio1 == tipo.ToString())
                .Select(u => new DominioDescricaoString
                {
                    id = u.IdDominio,
                    value = u.Valor,
                    descricao = u.Descricao,
                    indActivo = u.IndActivo
                })
                .ToList();
        }

        public DominioDescricaoString getTipoDeDominio(TiposDominio tipo)
        {
            return _moduloContribuicoesContext.Dominio
                .Where(u => u.Dominio1 == tipo.ToString() && u.IndActivo)
                .Select(u => new DominioDescricaoString
                {
                    id = u.IdDominio,
                    value = u.Valor,
                    descricao = u.Descricao,
                    indActivo = u.IndActivo
                })
                .FirstOrDefault();
        }

        public int getIdDominio(string dominio, int valor)
        {
            var entity = _moduloContribuicoesContext.Dominio
                 .FirstOrDefault(item => item.Dominio1 == dominio && item.Valor == valor);

            int idDominio = 0;
            if (entity != null)
            {
                idDominio = entity.IdDominio;
            }

            return idDominio;
        }

        public List<int> getIdDominios(string dominio, List<int> valores)
        {
            return _moduloContribuicoesContext.Dominio
                 .Where(item => item.Dominio1 == dominio && valores.Contains(item.Valor)).Select(e => e.IdDominio).ToList();
        }

        public Dominio getDominioByDescricao(TiposDominio tipo, string descricao)
        {
            Dominio response = _moduloContribuicoesContext.Dominio
                .Where(d => d.Descricao.Equals(descricao) && d.Dominio1 == tipo.ToString())
                .FirstOrDefault();

            return response;
        }

        public ValueCampoEditavelListagemResponse getAllActiveTiposDeDominio(TiposDominio tipo, SearchFilter filter, bool valor)
        {
            ValueCampoEditavelListagemResponse response = new ValueCampoEditavelListagemResponse();

            int index = 0;
            if (filter.index.HasValue)
                index = filter.index.Value;

            int rows = 5;
            if (filter.rows.HasValue)
                rows = filter.rows.Value;

            var queryDominio = _moduloContribuicoesContext.Dominio
                .Where(u => u.Dominio1 == tipo.ToString() && u.IndActivo && u.Descricao.Contains(filter.filterBy));

            IQueryable<ValorCamposEditaveis> queryDominioS;

            if (valor)
                queryDominioS = queryDominio.Select(u => new ValorCamposEditaveis
                {
                    Id = u.IdDominio,
                    Nome = u.Descricao,
                    Parametros = new List<ParametrosAdicionais>()
                    {
                        new ParametrosAdicionais
                        {
                            Nome = "Valor",
                            Size = "10",
                            Type = "number",
                            Valor = u.Valor.ToString()
                        }
                    }
                });
            else
                queryDominioS = queryDominio.Select(u => new ValorCamposEditaveis
                {
                    Id = u.IdDominio,
                    Nome = u.Descricao,
                    Parametros = new List<ParametrosAdicionais>()
                });

            var dominios = queryDominioS
                .OrderBy("Id")
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = queryDominio.Count();

            response.ValuesCampo = dominios;
            response.CountValuesCampo = totalNumber;

            return response;
        }

        public int getNextValue(string dominio)
        {
            int max = _moduloContribuicoesContext.Dominio
                .Where(d => d.Dominio1 == dominio)
                .GroupBy(d => d.Dominio1)
                .Select(d => d.Max(d => d.Valor)).FirstOrDefault();

            return (max + 1);
        }

        public List<SelectDescription> getAllActiveTiposDeDominio(TiposDominio tipo)
        {
            return _moduloContribuicoesContext.Dominio
                .Where(u => u.Dominio1 == tipo.ToString() && u.IndActivo)
                .Select(u => new SelectDescription
                {
                    id = u.IdDominio,
                    nome = u.Descricao,
                    indActivo = u.IndActivo
                })
                .ToList();
        }
    }
}