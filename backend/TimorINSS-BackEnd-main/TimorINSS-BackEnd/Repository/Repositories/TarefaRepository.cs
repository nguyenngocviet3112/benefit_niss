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
    public class TarefaRepository : ITarefaRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public TarefaRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Tarefa> GetAll()
        {
            return _moduloContribuicoesContext.Tarefa.Where(u => u.IndActivo).ToList();
        }

        public Tarefa Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var tarefa = _moduloContribuicoesContext.Tarefa
                .SingleOrDefault(u => u.Id == id);

            return tarefa;
        }

        public TarefaDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var tarefa = _moduloContribuicoesContext.Tarefa
                .SingleOrDefault(u => u.Id == id);

            TarefaDto tarefaDto = Utils.MappClassToDto<Tarefa, TarefaDto>(tarefa);
            return tarefaDto;
        }

        public void Add(Tarefa entity)
        {
            _moduloContribuicoesContext.Tarefa.Add(entity);
        }

        public void Update(Tarefa entity)
        {
            Tarefa entityToUpdate = _moduloContribuicoesContext.Tarefa
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Tarefa entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public TarefaListagemResponse GetAllTarefas(SearchFilterRequest request)
        {
            TarefaListagemResponse response = new TarefaListagemResponse();

            int index = 0;
            if (request.filter.index.HasValue)
                index = request.filter.index.Value;

            int rows = 5;
            if (request.filter.rows.HasValue)
                rows = request.filter.rows.Value;

            var listaTarefa = _moduloContribuicoesContext.Tarefa
               .Where(u => u.Nome.Contains(request.filter.filterBy))
               .OrderBy(u => u.Nome)
               .Select(u => new TarefaDataContract
               {
                   id = u.Id,
                   numero = u.NumeroTarefa,
                   nome = u.Nome,
                   indActivo = u.IndActivo
               });

            var tarefas = listaTarefa
                .OrderBy(request.filter.orderBy, request.filter.orderDirection)
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = listaTarefa.Count();
            response.rows = totalNumber;
            response.tarefa = tarefas;

            return response;
        }

        public List<SelectDescription> GetAllTarefaAtivo()
        {
            return _moduloContribuicoesContext.Tarefa
                .Where(u => u.IndActivo)
               .Select(u => new SelectDescription
               {
                   id = u.Id,
                   nome = u.Nome,
                   indActivo = u.IndActivo
               })
                .ToList();
        }

        public string GetNextNumeroTarefa()
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            int numTarefa = 0;

            Tarefa tarefa = _moduloContribuicoesContext.Tarefa.OrderByDescending(p => p.Id).FirstOrDefault();

            if (tarefa != null)
            {
                numTarefa = int.Parse(tarefa.NumeroTarefa) + 1;
            }

            string numTarefaFinal = numTarefa.ToString();

            if (numTarefa < 10)
            {
                numTarefaFinal = "0" + numTarefaFinal;
            }

            return numTarefaFinal;
        }
    }
}