using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class ProcessoAtivoRepository : IProcessoAtivoRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public ProcessoAtivoRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Processoativo> GetAll()
        {
            return _moduloContribuicoesContext.Processoativo.ToList();
        }

        public Processoativo Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var regime = _moduloContribuicoesContext.Processoativo
                .Include(u => u.ProcessoConfigFkNavigation)
                .Include(u => u.Tarefaativo)
                .SingleOrDefault(u => u.Id == id);

            return regime;
        }

        public ProcessoativoDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var processoativo = _moduloContribuicoesContext.Processoativo
                .SingleOrDefault(u => u.Id == id);

            ProcessoativoDto processoativoDto = Utils.MappClassToDto<Processoativo, ProcessoativoDto>(processoativo);
            return processoativoDto;
        }

        public void Add(Processoativo entity)
        {
            _moduloContribuicoesContext.Processoativo.Add(entity);
        }

        public void Update(Processoativo entity)
        {
            Processoativo entityToUpdate = _moduloContribuicoesContext.Processoativo
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Processoativo entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public ProcessoativoDto GetLastDto()
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var processoativo = _moduloContribuicoesContext.Processoativo
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();

            ProcessoativoDto processoativoDto = Utils.MappClassToDto<Processoativo, ProcessoativoDto>(processoativo);
            return processoativoDto;
        }

        public ProcessosArquivadosListagemResponse GetAllProcessosArquivados(SearchFilterRequest request)
        {
            ProcessosArquivadosListagemResponse response = new ProcessosArquivadosListagemResponse();

            int index = 0;
            if (request.filter.index.HasValue)
                index = request.filter.index.Value;

            int rows = 5;
            if (request.filter.rows.HasValue)
                rows = request.filter.rows.Value;

            var listaProcessos = _moduloContribuicoesContext.Processoativo
               .Include("ProcessoConfigFkNavigation")
               .Where(u => u.ProcessoConfigFkNavigation.Nome.Contains(request.filter.filterBy) && u.Arquivado)
               .Select(u => new ProcessoArquivadoDataContract
               {
                   id = u.Id,
                   data = u.DataCriacao,
                   numero = u.NumeroProcesso,
                   nome = u.ProcessoConfigFkNavigation.Nome,
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

        public RelatorioProcessosListagemResponse GetProcessosRelatorios(RelatorioProcessosListagemRequest request)
        {
            RelatorioProcessosListagemResponse response = new RelatorioProcessosListagemResponse();

            int index = 0;
            if (request.filter.index.HasValue)
                index = request.filter.index.Value;

            int rows = 5;
            if (request.filter.rows.HasValue)
                rows = request.filter.rows.Value;

            var beginDate = request.filter.dateFilterBegin;
            var endDate = request.filter.dateFilterEnd;

            var listaProcessos = _moduloContribuicoesContext.Processoativo
               .Include(e => e.ProcessoConfigFkNavigation)
               .ThenInclude(e => e.Relprocessoconfigperfil)
               .ThenInclude(e => e.PerfilFkNavigation)
               .Include(e => e.Tarefaativo)
               .ThenInclude(e => e.TarefaconfigFkNavigation)
               .Where(e => (!request.processoConfigId.HasValue || e.ProcessoConfigFk == request.processoConfigId) && // Filtrar por ProcessoConfig
                            // Filtrar por Arquivado/Não arquivado/Todos
                            (request.viewType == RelatorioProcessosListagemRequestViewType.Archived ? e.Arquivado :
                            request.viewType == RelatorioProcessosListagemRequestViewType.NotArchived ? !e.Arquivado :
                            request.viewType == RelatorioProcessosListagemRequestViewType.All) &&
                           // Filtrar por datas
                           // Se o filtro for de Arquivados, é para filtrar pela data de arquivamento
                           // Se o filtro for Todos, é para filtrar pela data de criação
                           // Se o filtro for Não arquivados, é para filtrar pela data da tarefa ativa
                           (
                            beginDate.HasValue && endDate.HasValue ? (
                                                                            request.viewType == RelatorioProcessosListagemRequestViewType.Archived ? e.DataAlteracao >= beginDate && e.DataAlteracao <= endDate :
                                                                            request.viewType == RelatorioProcessosListagemRequestViewType.All ? e.DataCriacao >= beginDate && e.DataCriacao <= endDate :
                                                                            e.Tarefaativo.First(a => a.IndActivo).DataCriacao >= beginDate && e.Tarefaativo.First(a => a.IndActivo).DataCriacao <= endDate
                                                                        ) :
                            beginDate.HasValue && !endDate.HasValue ? (
                                                                            request.viewType == RelatorioProcessosListagemRequestViewType.Archived ? beginDate == e.DataAlteracao :
                                                                            request.viewType == RelatorioProcessosListagemRequestViewType.All ? beginDate == e.DataCriacao :
                                                                            beginDate == e.Tarefaativo.First(a => a.IndActivo).DataCriacao
                                                                        ) :
                                                                        true
                            )
                )
               .Select(e => new RelatorioProcessoDataContract
               {
                   id = e.Id,
                   tipo = e.ProcessoConfigFkNavigation.Nome,
                   perfis = e.ProcessoConfigFkNavigation.Relprocessoconfigperfil.Select(a => a.PerfilFkNavigation.Descricao),
                   ultimaTarefa = new RelatorioProcessoDataContractUltimaTarefa()
                   {
                       id = e.Tarefaativo.OrderBy(e => e.Id).Last().Id,
                       nome = e.Tarefaativo.OrderBy(e => e.Id).Last().TarefaconfigFkNavigation.Nome
                   },
                   arquivado = e.Arquivado
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

        public GetTipoProcessosRelatoriosResponse GetTipoProcessosRelatorios()
        {
            GetTipoProcessosRelatoriosResponse response = new GetTipoProcessosRelatoriosResponse();

            var listaTipoProcessos = _moduloContribuicoesContext.Processoconfig
               .Where(e => e.IndActivo)
               .Select(e => new RelatorioTipoProcessoDataContract
               {
                   id = e.Id,
                   nome = e.Nome
               })
               .ToList();

            response.tipoProcessos = listaTipoProcessos;

            return response;
        }
    }
}