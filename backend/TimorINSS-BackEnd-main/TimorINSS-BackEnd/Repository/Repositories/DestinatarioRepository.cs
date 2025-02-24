using System;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class DestinatarioRepository : IDestinatarioRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public DestinatarioRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Destinatario> GetAll()
        {
            return _moduloContribuicoesContext.Destinatario.ToList();
        }

        public Destinatario Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var destinatario = _moduloContribuicoesContext.Destinatario
                .SingleOrDefault(u => u.Id == id);

            return destinatario;
        }

        public DestinatarioDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var destinatario = _moduloContribuicoesContext.Destinatario
                .SingleOrDefault(u => u.Id == id);

            DestinatarioDto destinatarioDto = Utils.MappClassToDto<Destinatario, DestinatarioDto>(destinatario);
            return destinatarioDto;
        }

        public void Add(Destinatario entity)
        {
            _moduloContribuicoesContext.Destinatario.Add(entity);
        }

        public void Update(Destinatario entity)
        {
            Destinatario entityToUpdate = _moduloContribuicoesContext.Destinatario
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Destinatario entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public DestinatarioDataContract GetDestinatarioByNiss(string niss)
        {
            return _moduloContribuicoesContext.Destinatario
                .Where(u => u.IndActivo && u.Niss == niss)
               .Select(u => new DestinatarioDataContract
               {
                   Id = u.Id,
                   Nome = u.Nome,
                   Niss = u.Niss,
                   Tin = u.Tin,
                   Morada = u.Morada,
               })
                .FirstOrDefault();
        }

        public DestinatarioDataContract GetDestinatarioByTin(string tin)
        {
            return _moduloContribuicoesContext.Destinatario
                .Where(u => u.IndActivo && u.Tin == tin)
               .Select(u => new DestinatarioDataContract
               {
                   Id = u.Id,
                   Nome = u.Nome,
                   Niss = u.Niss,
                   Tin = u.Tin,
                   Morada = u.Morada,
               })
                .FirstOrDefault();
        }

        public Destinatario GetDestinatarioByIdEntidade(int idEntidade)
        {
            return _moduloContribuicoesContext.Destinatario
                .Where(u => u.IndActivo && u.EntidadeFk == idEntidade)
                .FirstOrDefault();
        }

        public List<Destinatario> GetDestinatarioByNissOrTin(List<string> niss, List<string> tin)
        {
            return _moduloContribuicoesContext.Destinatario
                .Where(u => u.IndActivo && (niss.Contains(u.Niss) || tin.Contains(u.Tin)))
                .ToList();
        }

        public Destinatario GetDestinatarioByIdTrabalhador(int idTrabalhador)
        {
            return _moduloContribuicoesContext.Destinatario
                .Where(u => u.IndActivo && u.TrabalhadorFk == idTrabalhador)
                .FirstOrDefault();
        }

        public List<ExcelImporterRel> GetImportRows(Guid importId)
        {
            return _moduloContribuicoesContext.ExcelImporterRel
                .Where(u => u.ImportId == importId)
                .ToList();
        }
    }
}