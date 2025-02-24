using Microsoft.EntityFrameworkCore;
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
    public class ProcessoConfigRepository : IProcessoConfigRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public ProcessoConfigRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Processoconfig> GetAll()
        {
            return _moduloContribuicoesContext.Processoconfig.ToList();
        }

        public Processoconfig Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var regime = _moduloContribuicoesContext.Processoconfig
                .Include(u => u.Relprocessoconfigtarefa)
                .Include(u => u.Relprocessoconfigperfil)
                .SingleOrDefault(u => u.Id == id);

            return regime;
        }

        public ProcessoconfigDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var regime = _moduloContribuicoesContext.Processoconfig
                .SingleOrDefault(u => u.Id == id);

            ProcessoconfigDto regimeDto = Utils.MappClassToDto<Processoconfig, ProcessoconfigDto>(regime);
            return regimeDto;
        }

        public void Add(Processoconfig entity)
        {
            _moduloContribuicoesContext.Processoconfig.Add(entity);
        }

        public void Update(Processoconfig entity)
        {
            Processoconfig entityToUpdate = _moduloContribuicoesContext.Processoconfig
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Processoconfig entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public ProcessosListagemResponse GetAllProcessos(SearchFilterRequest request)
        {
            ProcessosListagemResponse response = new ProcessosListagemResponse();

            int index = 0;
            if (request.filter.index.HasValue)
                index = request.filter.index.Value;

            int rows = 5;
            if (request.filter.rows.HasValue)
                rows = request.filter.rows.Value;

            var listaProcessos = _moduloContribuicoesContext.Processoconfig
               .Where(u => u.Nome.Contains(request.filter.filterBy))
               .Select(u => new ProcessoDataContract
               {
                   id = u.Id,
                   data = u.DataCriacao,
                   nome = u.Nome,
                   indAtivo = u.IndActivo
               });

            var processos = listaProcessos
                .OrderBy(request.filter.orderBy, request.filter.orderDirection)
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = listaProcessos.Count();
            response.rows = totalNumber;
            response.processos = processos;

            return response;
        }

        public Processoconfig GetWithActiveRelations(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var regime = _moduloContribuicoesContext.Processoconfig
                .Include(u => u.Relprocessoconfigtarefa.Where(x => x.IndActivo == true))
                .Include(u => u.Relprocessoconfigperfil.Where(x => x.IndActivo == true))
                .SingleOrDefault(u => u.Id == id);

            return regime;
        }

        public SelectDescriptionResponse ListIniciarProcessos(List<int> allowedProcessConfigs, bool isAdmin = false)
        {
            SelectDescriptionResponse response = new SelectDescriptionResponse
            {
                selects = _moduloContribuicoesContext.Processoconfig
                            .Where(u => !isAdmin ? allowedProcessConfigs.Contains(u.Id) && u.IndActivo :
                                                   u.IndActivo
               )
               .Select(u => new SelectDescription
               {
                   id = u.Id,
                   nome = u.Nome,
                   indActivo = true,
                   parentId = 0
               })
               .ToList()
            };

            return response;
        }
    }
}