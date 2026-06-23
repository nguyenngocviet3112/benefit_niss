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
    public class ActividadeEconomicaRepository : IActividadeEconomicaRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public ActividadeEconomicaRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Actividadeeconomica> GetAll()
        {
            return _moduloContribuicoesContext.Actividadeeconomica.ToList();
        }

        public Actividadeeconomica Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var actividadeeconomica = _moduloContribuicoesContext.Actividadeeconomica
                .SingleOrDefault(u => u.IdActivEconomica == id);

            return actividadeeconomica;
        }

        public ActividadeeconomicaDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var actividadeeconomica = _moduloContribuicoesContext.Actividadeeconomica
                .SingleOrDefault(u => u.IdActivEconomica == id);

            ActividadeeconomicaDto actividadeeconomicaDto = Utils.MappClassToDto<Actividadeeconomica, ActividadeeconomicaDto>(actividadeeconomica);
            return actividadeeconomicaDto;
        }

        public void Add(Actividadeeconomica entity)
        {
            _moduloContribuicoesContext.Actividadeeconomica.Add(entity);
        }

        public void Update(Actividadeeconomica entity)
        {
            Actividadeeconomica entityToUpdate = _moduloContribuicoesContext.Actividadeeconomica
                .Single(d => d.IdActivEconomica == entity.IdActivEconomica);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Actividadeeconomica entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }


        public List<SelectDescription> GetAllActividadeEconomica()
        {
            return _moduloContribuicoesContext.Actividadeeconomica
               .Select(u => new SelectDescription
               {
                   id = u.IdActivEconomica,
                   nome = u.Descricao,
                   indActivo = u.IndActivo
               })
                .ToList();
        }

        public ValueCampoEditavelListagemResponse GetAllActiveActividadeEconomica(SearchFilter filter)
        {
            ValueCampoEditavelListagemResponse response = new ValueCampoEditavelListagemResponse();

            int index = 0;
            if (filter.index.HasValue)
                index = filter.index.Value;

            int rows = 5;
            if (filter.rows.HasValue)
                rows = filter.rows.Value;

            var queryActividadeEconomica = _moduloContribuicoesContext.Actividadeeconomica
               .Where(a => a.IndActivo && a.Descricao.Contains(filter.filterBy))
               .Select(u => new ValorCamposEditaveis
               {
                   Id = u.IdActivEconomica,
                   Nome = u.Descricao,
                   Parametros = new List<ParametrosAdicionais>()
                   {
                        new ParametrosAdicionais
                        {
                            Nome = "Codigo",
                            Size = "3",
                            Type = "number",
                            Valor = u.Codigo
                        }
                   }
               });

            var actividades = queryActividadeEconomica
                .OrderBy("Id")
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = queryActividadeEconomica.Count();

            response.ValuesCampo = actividades;
            response.CountValuesCampo = totalNumber;

            return response;
        }

        public bool DoesCodeExists(int id, string code)
        {
            return _moduloContribuicoesContext.Actividadeeconomica
                .Where(n => n.Codigo == code && n.IdActivEconomica != id && n.IndActivo)
                .Count() > 0;
        }
    }
}