using System;
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
    public class ImporterRepository : IImporterRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public ImporterRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<ExcelImporterRel> GetAll()
        {
            return _moduloContribuicoesContext.ExcelImporterRel.ToList();
        }

        public ExcelImporterRel Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var import = _moduloContribuicoesContext.ExcelImporterRel
                .SingleOrDefault(u => u.Id == id);

            return import;
        }

        public ExcelImporterRel GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var import = _moduloContribuicoesContext.ExcelImporterRel
                .SingleOrDefault(u => u.Id == id);

            return import;
        }

        public void Add(ExcelImporterRel entity)
        {
            _moduloContribuicoesContext.ExcelImporterRel.Add(entity);
        }

        public void Update(ExcelImporterRel entity)
        {
            ExcelImporterRel entityToUpdate = _moduloContribuicoesContext.ExcelImporterRel
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(ExcelImporterRel entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public void DeleteOld()
        {
            _moduloContribuicoesContext.ExcelImporterRel.RemoveRange(_moduloContribuicoesContext.ExcelImporterRel.Where(x => x.Date <= DateTime.Now.AddDays(-2)));
        }
    }
}