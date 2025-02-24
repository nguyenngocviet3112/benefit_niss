using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class RelUtilizadorDepartamentoRepository : IRelUtilizadorDepartamentoRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public RelUtilizadorDepartamentoRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Relutilizadordepartamento> GetAll()
        {
            return _moduloContribuicoesContext.Relutilizadordepartamento.ToList();
        }

        public Relutilizadordepartamento Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var relUtilizadorDepartamento = _moduloContribuicoesContext.Relutilizadordepartamento
                .SingleOrDefault(u => u.Id == id);

            return relUtilizadorDepartamento;
        }

        public RelUtilizadorDepartamentoDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var relUtilizadorDepartamento = _moduloContribuicoesContext.Relutilizadordepartamento
                .SingleOrDefault(u => u.Id == id);

            RelUtilizadorDepartamentoDto relUtilizadorDepartamentoDto = Utils.MappClassToDto<Relutilizadordepartamento, RelUtilizadorDepartamentoDto>(relUtilizadorDepartamento);
            return relUtilizadorDepartamentoDto;
        }

        public void Add(Relutilizadordepartamento entity)
        {
            _moduloContribuicoesContext.Relutilizadordepartamento.Add(entity);
        }

        public void Update(Relutilizadordepartamento entity)
        {
            Relutilizadordepartamento entityToUpdate = _moduloContribuicoesContext.Relutilizadordepartamento
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Relutilizadordepartamento entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public List<Relutilizadordepartamento> GetRelUtilizadorDepartamentoByUserId(int userId)
        {
            List<Relutilizadordepartamento> list = new List<Relutilizadordepartamento>();

            list = _moduloContribuicoesContext.Relutilizadordepartamento
                   .Include(u => u.DepartamentoFkNavigation)
                   .Where(u => u.UtilizadorFk == userId && u.IndActivo)
                   .ToList();

            return list;
        }

        public DepartamentoListagemResponse GetDepartamentosByUserId(int id)
        {
            DepartamentoListagemResponse response = new DepartamentoListagemResponse();

            List<DepartamentoListagem> listaDepartamento = _moduloContribuicoesContext.Relutilizadordepartamento
               .Where(u => u.UtilizadorFk == id && u.IndActivo)
               .Select(u => new DepartamentoListagem
               {
                   id = u.DepartamentoFk,
                   nome = u.DepartamentoFkNavigation.Nome,
               })
                .ToList();

            response.departamento = listaDepartamento;

            return response;
        }

        public Relutilizadordepartamento GetRelByDepartamentoId(int idDepartamento)
        {
            Relutilizadordepartamento response = new Relutilizadordepartamento();

            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var relUtilizadorDepartamento = _moduloContribuicoesContext.Relutilizadordepartamento
                .SingleOrDefault(u => u.DepartamentoFk == idDepartamento && u.IndActivo);

            return relUtilizadorDepartamento;
        }
    }
}