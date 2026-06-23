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
    public class InstitutionRepository : IInstitutionRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public InstitutionRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Institution> GetAll()
        {
            return _moduloContribuicoesContext.Institution.Where(u => u.IndActivo).ToList();
        }

        public Institution Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var departamento = _moduloContribuicoesContext.Institution
                .SingleOrDefault(u => u.Id == id);

            return departamento;
        }

        public InstitutionDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var departamento = _moduloContribuicoesContext.Institution
                .SingleOrDefault(u => u.Id == id);

            InstitutionDto departamentoDto = Utils.MappClassToDto<Institution, InstitutionDto>(departamento);
            return departamentoDto;
        }

        public void Add(Institution entity)
        {
            _moduloContribuicoesContext.Institution.Add(entity);
        }

        public void Update(Institution entity)
        {
            Institution entityToUpdate = _moduloContribuicoesContext.Institution
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Institution entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public ValueCampoEditavelListagemResponse getAllActiveDepartamento(SearchFilter filter)
        {
            ValueCampoEditavelListagemResponse response = new ValueCampoEditavelListagemResponse();

            int index = 0;
            if (filter.index.HasValue)
                index = filter.index.Value;

            int rows = 5;
            if (filter.rows.HasValue)
                rows = filter.rows.Value;

            var queryDepartamento = _moduloContribuicoesContext.Departamento
                .Where(a => a.IndActivo && a.Nome.Contains(filter.filterBy))
               .Select(u => new ValorCamposEditaveis
               {
                   Id = u.Id,
                   Nome = u.Nome,
                   Parametros = new List<ParametrosAdicionais>()
               });

            var dominios = queryDepartamento
                .OrderBy("Id")
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = queryDepartamento.Count();

            response.ValuesCampo = dominios;
            response.CountValuesCampo = totalNumber;

            return response;
        }

        public List<SelectDescription> GetAllDepartamentosAtivo()
        {
            return _moduloContribuicoesContext.Departamento
                .Where(u => u.IndActivo)
               .Select(u => new SelectDescription
               {
                   id = u.Id,
                   nome = u.Nome,
                   indActivo = u.IndActivo
               })
                .ToList();
        }

        public List<SelectDescription> GetAllInstitutionAtivo()
        {
            return _moduloContribuicoesContext.Institution
                .Where(u => u.IndActivo)
               .Select(u => new SelectDescription
               {
                   id = u.Id,
                   nome = u.Nome,
                   indActivo = u.IndActivo
               })
                .ToList();
        }
    }
}