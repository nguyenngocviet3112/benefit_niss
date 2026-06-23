using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class ComponenteOrcamentoRegistoRepository : IComponenteOrcamentoRegistoRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public ComponenteOrcamentoRegistoRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<ComponenteorcamentoRegisto> GetAll()
        {
            return _moduloContribuicoesContext.ComponenteorcamentoRegisto.Where(u => u.IndActivo).ToList();
        }

        public ComponenteorcamentoRegisto Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var componenteOrcamentoRegisto = _moduloContribuicoesContext.ComponenteorcamentoRegisto
                .SingleOrDefault(u => u.Id == id);

            return componenteOrcamentoRegisto;
        }

        public ComponenteOrcamentoRegistoDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var componenteOrcamentoRegisto = _moduloContribuicoesContext.ComponenteorcamentoRegisto
                .SingleOrDefault(u => u.Id == id);

            ComponenteOrcamentoRegistoDto componenteOrcamentoRegistoDto = Utils.MappClassToDto<ComponenteorcamentoRegisto, ComponenteOrcamentoRegistoDto>(componenteOrcamentoRegisto);
            return componenteOrcamentoRegistoDto;
        }

        public void Add(ComponenteorcamentoRegisto entity)
        {
            _moduloContribuicoesContext.ComponenteorcamentoRegisto.Add(entity);
        }

        public void Update(ComponenteorcamentoRegisto entity)
        {
            ComponenteorcamentoRegisto entityToUpdate = _moduloContribuicoesContext.ComponenteorcamentoRegisto
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(ComponenteorcamentoRegisto entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public ComponenteorcamentoRegisto GetByIdTarefaActivo(int id, bool checkInactive = true)
        {
            return _moduloContribuicoesContext.ComponenteorcamentoRegisto
                .Where(c => (!checkInactive || c.IndActivo) && c.TarefaActivoFk == id)
                .FirstOrDefault();
        }

        public List<ComponenteorcamentoRegisto> GetComponentesOrcamentoRegistoAprovado(DateTime inicio, DateTime fim)
        {
            return _moduloContribuicoesContext.ComponenteorcamentoRegisto
                .Include(c => c.TarefaActivoFkNavigation)
                .Include(c => c.InverseOrcamentoRetificadoFkNavigation)
                .Where(o => o.IndActivo && o.Aprovado &&
                (o.DataInicio > inicio ? o.DataInicio : inicio) <=
                (o.DataFim > fim ? fim : o.DataFim)).ToList();
        }

        public ComponenteorcamentoRegisto GetOrcamentoAprovadoByDataPInicioProcesso(DateTime dataInicioProcesso)
        {
            return _moduloContribuicoesContext.ComponenteorcamentoRegisto
                .Include(u => u.OrcamentoConfigFkNavigation)
                .Where(c => c.IndActivo && c.Aprovado &&
                dataInicioProcesso >= c.DataInicio && dataInicioProcesso <= c.DataFim)
                 .OrderBy("Id", OrderDirectionEnum.descending)
                .FirstOrDefault();
        }

        public ComponenteorcamentoRegisto GetByIdProcessoActivo(int idProcessoAtivo)
        {
            return _moduloContribuicoesContext.ComponenteorcamentoRegisto
                .Include(c => c.TarefaActivoFkNavigation)
                .ThenInclude(c => c.ProcessoAtivoFkNavigation)
                .Where(c => c.IndActivo && c.TarefaActivoFkNavigation.ProcessoAtivoFkNavigation.Id == idProcessoAtivo)
                .OrderBy("Id", OrderDirectionEnum.descending)
                .FirstOrDefault();
        }
    }
}