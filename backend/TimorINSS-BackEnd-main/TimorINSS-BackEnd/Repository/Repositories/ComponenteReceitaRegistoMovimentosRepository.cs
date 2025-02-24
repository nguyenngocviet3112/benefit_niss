using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
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
    public class ComponenteReceitaRegistoMovimentosRepository : IComponenteReceitaRegistoMovimentosRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public ComponenteReceitaRegistoMovimentosRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<ComponentereceitaRegistoMovimentos> GetAll()
        {
            return _moduloContribuicoesContext.ComponentereceitaRegistoMovimentos
                .ToList();
        }

        public ComponentereceitaRegistoMovimentos Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var trabalhador = _moduloContribuicoesContext.ComponentereceitaRegistoMovimentos
                .SingleOrDefault(u => u.Id == id);

            return trabalhador;
        }

        public ComponenteReceitaRegistoMovimentosDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var mov = _moduloContribuicoesContext.ComponentereceitaRegistoMovimentos
                .SingleOrDefault(u => u.Id == id);

            ComponenteReceitaRegistoMovimentosDto trabalhadorDto = Utils.MappClassToDto<ComponentereceitaRegistoMovimentos, ComponenteReceitaRegistoMovimentosDto>(mov);
            return trabalhadorDto;
        }

        public void Add(ComponentereceitaRegistoMovimentos entity)
        {
            _moduloContribuicoesContext.ComponentereceitaRegistoMovimentos.Add(entity);
        }

        public void Update(ComponentereceitaRegistoMovimentos entity)
        {
            ComponentereceitaRegistoMovimentos entityToUpdate = _moduloContribuicoesContext.ComponentereceitaRegistoMovimentos
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(ComponentereceitaRegistoMovimentos entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public List<ComponentereceitaRegistoMovimentos> GetMovimentosByIdReceita(int idReceita)
        {
            return _moduloContribuicoesContext.ComponentereceitaRegistoMovimentos
                .Where(u => u.ComponenteReceitaRegistoId == idReceita && u.IndActivo.Value)
                .ToList();
        }
    }
}