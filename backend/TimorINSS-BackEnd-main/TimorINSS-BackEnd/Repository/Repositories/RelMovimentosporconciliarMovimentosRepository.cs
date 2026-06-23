using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class RelMovimentosporconciliarMovimentosRepository : IRelMovimentosporconciliarMovimentosRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public RelMovimentosporconciliarMovimentosRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<RelMovimentosporconciliarMovimentos> GetAll()
        {
            return _moduloContribuicoesContext.RelMovimentosporconciliarMovimentos.Where(u => u.IndActivo.Value).ToList();
        }

        public RelMovimentosporconciliarMovimentos Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var relMovimentosporconciliarMovimentos = _moduloContribuicoesContext.RelMovimentosporconciliarMovimentos
                .SingleOrDefault(u => u.Id == id);

            return relMovimentosporconciliarMovimentos;
        }

        public RelMovimentosporconciliarMovimentosDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var relMovimentosporconciliarMovimentos = _moduloContribuicoesContext.RelMovimentosporconciliarMovimentos
                .SingleOrDefault(u => u.Id == id);

            RelMovimentosporconciliarMovimentosDto relMovimentosporconciliarMovimentosDto = Utils.MappClassToDto<RelMovimentosporconciliarMovimentos, RelMovimentosporconciliarMovimentosDto>(relMovimentosporconciliarMovimentos);
            return relMovimentosporconciliarMovimentosDto;
        }

        public void Add(RelMovimentosporconciliarMovimentos entity)
        {
            _moduloContribuicoesContext.RelMovimentosporconciliarMovimentos.Add(entity);
        }

        public void Update(RelMovimentosporconciliarMovimentos entity)
        {
            RelMovimentosporconciliarMovimentos entityToUpdate = _moduloContribuicoesContext.RelMovimentosporconciliarMovimentos
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(RelMovimentosporconciliarMovimentos entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public IEnumerable<RelMovimentosporconciliarMovimentos> GetAllFromMovimentoBancario(int id)
        {
            return _moduloContribuicoesContext.RelMovimentosporconciliarMovimentos
                                              .Where(e => e.IndActivo.Value && e.MovimentosBancariosFk == id)
                                              .ToList();
        }

        public IEnumerable<RelMovimentosporconciliarMovimentos> GetAllFromMovimentosPorConciliar(int id, MovimentosPorConciliarListagemType type)
        {
            return _moduloContribuicoesContext.RelMovimentosporconciliarMovimentos
                                              .Where(e => e.IndActivo.Value &&
                                                          ((type == MovimentosPorConciliarListagemType.GuiaPagamento && e.GuiaPagamentoFk == id) ||
                                                           (type == MovimentosPorConciliarListagemType.MovimentoAConciliar && e.MovimentoPorConciliarFk == id) ||
                                                           (type == MovimentosPorConciliarListagemType.PagamentoExecutado && e.PagamentosExecutadosFk == id)))
                                              .ToList();
        }
    }
}