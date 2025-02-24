using Microsoft.EntityFrameworkCore;
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
    public class TarefaAtivoRepository : ITarefaAtivoRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public TarefaAtivoRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Tarefaativo> GetAll()
        {
            return _moduloContribuicoesContext.Tarefaativo.Where(u => u.IndActivo).ToList();
        }

        public Tarefaativo Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var tarefa = _moduloContribuicoesContext.Tarefaativo
                .SingleOrDefault(u => u.Id == id);

            return tarefa;
        }

        public TarefaativoDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var tarefa = _moduloContribuicoesContext.Tarefaativo
                .SingleOrDefault(u => u.Id == id);

            TarefaativoDto tarefaDto = Utils.MappClassToDto<Tarefaativo, TarefaativoDto>(tarefa);
            return tarefaDto;
        }

        public void Add(Tarefaativo entity)
        {
            _moduloContribuicoesContext.Tarefaativo.Add(entity);
        }

        public void Update(Tarefaativo entity)
        {
            Tarefaativo entityToUpdate = _moduloContribuicoesContext.Tarefaativo
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Tarefaativo entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public TarefasAtivasListagemResponse GetAllTarefasAtivas(SearchFilterRequest request, List<int> allowedTarefaIds)
        {
            TarefasAtivasListagemResponse response = new TarefasAtivasListagemResponse();

            int index = 0;
            if (request.filter.index.HasValue)
                index = request.filter.index.Value;

            int rows = 5;
            if (request.filter.rows.HasValue)
                rows = request.filter.rows.Value;

            var listaTarefa = _moduloContribuicoesContext.Tarefaativo
               .Where(u =>
                   // Filtrar por Nome ou Numero de processo
                   (u.TarefaconfigFkNavigation.Nome.Contains(request.filter.filterBy) || u.ProcessoAtivoFkNavigation.NumeroProcesso.Contains(request.filter.filterBy))
                   // Filtrar por tarefas específicas
                   && allowedTarefaIds.Contains(u.TarefaconfigFk)
                   // Filtrar pelo utilizador responsável
                   && (u.UtilizadorResponsavel == null || u.UtilizadorResponsavel == request.UserId)
                   && u.IndActivo
                )
               .Select(u => new TarefaAtivoDataContract
               {
                   id = u.Id,
                   dataInicioProcesso = u.ProcessoAtivoFkNavigation.DataCriacao.Date.ToString(),
                   nomeProcesso = u.ProcessoAtivoFkNavigation.ProcessoConfigFkNavigation.Nome,
                   nome = u.TarefaconfigFkNavigation.Nome,
                   // Se o PrazoTarefa for 0, a tarefa não tem prazo (SemPrazo)
                   //estado = !u.TarefaconfigFkNavigation.PrazoTarefa.HasValue || u.TarefaconfigFkNavigation.PrazoTarefa.Value == 0 ? (int)TarefaDateState.SemPrazo :
                   //         // Se a o intervalo de dias entre a data de criação e o dia de hoje for maior que o PrazoTarefa, a tarefa está atrasada (EmAtraso)
                   //         u.DataCriacao.AddDays(u.TarefaconfigFkNavigation.PrazoTarefa.Value) < DateTime.Now ? (int)TarefaDateState.EmAtraso :
                   //         // Regra de negócio: se chegar a metade dos dias estabelicdos como prazo, ele entra em estado pendente
                   //         u.DataCriacao.AddDays(u.TarefaconfigFkNavigation.PrazoTarefa.Value) > DateTime.Now && u.DataCriacao.AddDays(u.TarefaconfigFkNavigation.PrazoTarefa.Value / 2) > DateTime.Now ? (int)TarefaDateState.Pendente :
                   //         // Ainda não acabou o tempo para acabar a tarefa
                   //         (int)TarefaDateState.EmPrazo,
                   estado = !u.TarefaconfigFkNavigation.PrazoTarefa.HasValue || u.TarefaconfigFkNavigation.PrazoTarefa.Value == 0 ? (int)TarefaDateState.SemPrazo :
                            // Se a o intervalo de dias entre a data de criação e o dia de hoje for maior que o PrazoTarefa, a tarefa está atrasada (EmAtraso)
                            (u.DataCriacao.AddDays(u.TarefaconfigFkNavigation.PrazoTarefa.Value) - DateTime.Now).Days < 0 ? (int)TarefaDateState.EmAtraso :
                            //Regra de negócio: se chegar a metade dos dias estabelicdos como prazo, ele entra em estado pendente
                            (u.DataCriacao.AddDays(u.TarefaconfigFkNavigation.PrazoTarefa.Value) - DateTime.Now).Days > 0 && (u.DataCriacao.AddDays(u.TarefaconfigFkNavigation.PrazoTarefa.Value) - DateTime.Now).Days < (u.TarefaconfigFkNavigation.PrazoTarefa.Value / 2) ? (int)TarefaDateState.Pendente :
                            // Ainda não acabou o tempo para acabar a tarefa
                            (int)TarefaDateState.EmPrazo,
                   //estadoDays = (!u.TarefaconfigFkNavigation.PrazoTarefa.HasValue || u.TarefaconfigFkNavigation.PrazoTarefa.Value == 0) ? 99999999 : (int)((DateTime.Now.Ticks - u.DataCriacao.Ticks + u.TarefaconfigFkNavigation.PrazoTarefa.Value * 24 * 60 * 60 * 1000) / 24 / 60 / 60 / 1000),
                   estadoDays = (u.DataCriacao.AddDays(u.TarefaconfigFkNavigation.PrazoTarefa.Value) - DateTime.Now).Days,
                   //estadoDays = !u.TarefaconfigFkNavigation.PrazoTarefa.HasValue || u.TarefaconfigFkNavigation.PrazoTarefa.Value == 0 ? 99999999 : (u.DataCriacao.AddDays(u.TarefaconfigFkNavigation.PrazoTarefa.Value) - DateTime.Now).Days,
                   ultimaAtualizacao = u.DataAlteracao ?? u.DataCriacao,
                   numeroProcesso = u.ProcessoAtivoFkNavigation.NumeroProcesso
               });

            var tarefas = listaTarefa
                .OrderBy(request.filter.orderBy, request.filter.orderDirection)
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = listaTarefa.Count();
            response.rows = totalNumber;
            response.tarefas = tarefas;

            return response;
        }

        public List<int> GetAllTarefasAtivasSemControlo()
        {
            List<int> response = new List<int>();

            response = _moduloContribuicoesContext.Tarefaativo
                .Include(x => x.TarefaconfigFkNavigation)
                .Where(x => x.TarefaconfigFkNavigation.Componentecontroleacesso.Count == 0)
                .Select(x => x.TarefaconfigFk)
                .ToList();

            return response;
        }

        public Tarefaativo GetTarefaAtivoById(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var tarefa = _moduloContribuicoesContext.Tarefaativo
                .Include(u => u.ProcessoAtivoFkNavigation)
                .ThenInclude(z => z.ProcessoConfigFkNavigation)
                .Include(u => u.TarefaconfigFkNavigation)
                .ThenInclude(z => z.Componentetexto)
                .Include(u => u.TarefaconfigFkNavigation)
                .ThenInclude(z => z.Componentecarregardocumento)
                .Include(u => u.TarefaconfigFkNavigation)
                .ThenInclude(z => z.Componenteconciliacaomovimentos)
                .Include(u => u.ComponentetextoRegisto)
                .Include(u => u.ComponenteclassificacaosubRegisto)
                .Include(u => u.ComponentedocumentoRegisto)
                .Include(u => u.ProcessoAtivoFkNavigation)
                .ThenInclude(z => z.Tarefaativo)
                .ThenInclude(y => y.ComponenteorcamentoRegisto)
                .ThenInclude(c => c.Componenteorcamentovalor)
                .SingleOrDefault(u => u.Id == id);

            return tarefa;
        }

        public List<int> GetAllTarefasIdsByProcessoAtivoIdExcludingCurrent(long processoAtivoId, long currentTarefaId)
        {
            List<int> response = new List<int>();

            response = _moduloContribuicoesContext.Tarefaativo
                .Where(x => x.Id != currentTarefaId && x.ProcessoAtivoFk == processoAtivoId)
                .Select(x => x.Id)
                .ToList();

            return response;
        }

        public List<int> GetAllTarefasIdsByProcessoAtivoId(long processoAtivoId)
        {
            List<int> response = new List<int>();

            response = _moduloContribuicoesContext.Tarefaativo
                .Where(x => x.ProcessoAtivoFk == processoAtivoId)
                .Select(x => x.Id)
                .ToList();

            return response;
        }
    }
}