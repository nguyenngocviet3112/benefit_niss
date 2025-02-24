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
    public class NaturezaJuridicaRepository : INaturezaJuridicaRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public NaturezaJuridicaRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Naturezajuridica> GetAll()
        {
            return _moduloContribuicoesContext.Naturezajuridica.ToList();
        }

        public Naturezajuridica Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var naturezaJuridica = _moduloContribuicoesContext.Naturezajuridica
                .SingleOrDefault(u => u.IdNatJuridica == id);

            return naturezaJuridica;
        }

        public NaturezajuridicaDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var naturezaJuridica = _moduloContribuicoesContext.Naturezajuridica
                .SingleOrDefault(u => u.IdNatJuridica == id);

            NaturezajuridicaDto naturezaJuridicaDto = Utils.MappClassToDto<Naturezajuridica, NaturezajuridicaDto>(naturezaJuridica);
            return naturezaJuridicaDto;
        }

        public void Add(Naturezajuridica entity)
        {
            _moduloContribuicoesContext.Naturezajuridica.Add(entity);
        }

        public void Update(Naturezajuridica entity)
        {
            Naturezajuridica entityToUpdate = _moduloContribuicoesContext.Naturezajuridica
                .Single(d => d.IdNatJuridica == entity.IdNatJuridica);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Naturezajuridica entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public List<SelectDescription> GetAllNaturezaJuridica()
        {
            return _moduloContribuicoesContext.Naturezajuridica
               .Select(u => new SelectDescription
               {
                   id = u.IdNatJuridica,
                   nome = u.Descricao,
                   indActivo = u.IndActivo
               })
                .ToList();
        }

        public ValueCampoEditavelListagemResponse GetAllActiveNaturezaJuridica(SearchFilter filter)
        {
            ValueCampoEditavelListagemResponse response = new ValueCampoEditavelListagemResponse();

            int index = 0;
            if (filter.index.HasValue)
                index = filter.index.Value;

            int rows = 5;
            if (filter.rows.HasValue)
                rows = filter.rows.Value;

            var queryNaturezaJuridica = _moduloContribuicoesContext.Naturezajuridica
               .Where(n => n.IndActivo &&
                           n.Descricao.Contains(filter.filterBy)
                )
               .Select(u => new ValorCamposEditaveis
               {
                   Id = u.IdNatJuridica,
                   Nome = u.Descricao,
                   Parametros = new List<ParametrosAdicionais>()
                   {
                       new ParametrosAdicionais
                        {
                            Nome = "Codigo",
                            Size = "2",
                            Type = "number",
                            Valor = u.Codigo
                        }
                   }
               });

            var naturezas = queryNaturezaJuridica
                .OrderBy("Id")
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = queryNaturezaJuridica.Count();

            response.ValuesCampo = naturezas;
            response.CountValuesCampo = totalNumber;

            return response;
        }

        public bool DoesCodeExists(int id, string code)
        {
            return _moduloContribuicoesContext.Naturezajuridica
                .Where(n => n.Codigo == code && n.IdNatJuridica != id && n.IndActivo)
                .Count() > 0;
        }
    }
}