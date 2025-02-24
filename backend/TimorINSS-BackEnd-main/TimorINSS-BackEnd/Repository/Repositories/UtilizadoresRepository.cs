using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class UtilizadoresRepository : IUtilizadoresRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public UtilizadoresRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Utilizador> GetAll()
        {
            return _moduloContribuicoesContext.Utilizador
                .Include(u => u.UtilizadorEntidadeFkNavigation)
                .Where(u => u.IndActivo)
                .ToList();
        }

        public Utilizador Get(long id)
        {
            var utilizador = _moduloContribuicoesContext.Utilizador
                .Include(u => u.UtilizadorEntidadeFkNavigation)
                .Include(u => u.TrabalhadorFkNavigation)
                .SingleOrDefault(u => u.IdUtilizador == id);

            return utilizador;
        }

        public Utilizador GetByUsername(string username)
        {
            var utilizador = _moduloContribuicoesContext.Utilizador
                .Include(u => u.UtilizadorEntidadeFkNavigation)
                .SingleOrDefault(u => u.Username == username && (u.Interno == null || u.Interno == false));

            return utilizador;
        }

        public Utilizador GetInternalByUsername(string username)
        {
            var utilizador = _moduloContribuicoesContext.Utilizador
                .Include(u => u.UtilizadorEntidadeFkNavigation)
                .SingleOrDefault(u => u.Username == username && u.Interno == true);

            return utilizador;
        }

        public Utilizador GetByEntidadeEmpregadora(long entidadeId)
        {
            var utilizador = _moduloContribuicoesContext.Utilizador
                .SingleOrDefault(u => u.UtilizadorEntidadeFk == entidadeId);

            return utilizador;
        }

        public UtilizadorDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;
            var utilizador = _moduloContribuicoesContext.Utilizador
                .SingleOrDefault(u => u.IdUtilizador == id && u.IndActivo);

            UtilizadorDto utilizadorDto = Utils.MappClassToDto<Utilizador, UtilizadorDto>(utilizador);
            return utilizadorDto;
        }

        public void Add(Utilizador entity)
        {
            _moduloContribuicoesContext.Utilizador.Add(entity);
        }

        public void Update(Utilizador entity)
        {
            Utilizador entityToUpdate = _moduloContribuicoesContext.Utilizador
                .Single(d => d.IdUtilizador == entity.IdUtilizador);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Utilizador entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public UtilizadorListagemResponse GetAllUtilizadoresInterno(SearchFilterRequest request)
        {
            UtilizadorListagemResponse response = new UtilizadorListagemResponse();

            int index = 0;
            if (request.filter.index.HasValue)
                index = request.filter.index.Value;

            int rows = 5;
            if (request.filter.rows.HasValue)
                rows = request.filter.rows.Value;

            var listaUtilizador = _moduloContribuicoesContext.Utilizador
               .Where(u => u.Interno == true &&
                           u.TrabalhadorFk != null &&
                           u.TrabalhadorFkNavigation.Nome.Contains(request.filter.filterBy) &&
                           u.TrabalhadorFkNavigation.Interno
               )
               .Select(u => new UtilizadorListagem
               {
                   id = u.IdUtilizador,
                   utilizador = u.TrabalhadorFkNavigation.Nome,
                   idTrabalhador = u.TrabalhadorFkNavigation.IdTrabalhador
               });

            var utilizadores = listaUtilizador
                .OrderBy(request.filter.orderBy, request.filter.orderDirection)
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = listaUtilizador.Count();
            response.rows = totalNumber;
            response.utilizador = utilizadores;

            return response;
        }

        public Utilizador GetByTrabalhadorFk(long trabalhadorId)
        {
            var utilizador = _moduloContribuicoesContext.Utilizador
                .FirstOrDefault(u => u.TrabalhadorFk == trabalhadorId);

            return utilizador;
        }

        public UtilizadoresAcessoListagemResponse GetAllAcessoUtilizadores(SearchFilterRequest request)
        {
            UtilizadoresAcessoListagemResponse response = new UtilizadoresAcessoListagemResponse();

            int index = 0;
            if (request.filter.index.HasValue)
                index = request.filter.index.Value;

            int rows = 5;
            if (request.filter.rows.HasValue)
                rows = request.filter.rows.Value;

            var userList = _moduloContribuicoesContext.Utilizador.Select(x => x);

            //Existem duas camadas de filtragem, se "filtrar por interno" se encontrar em alguma das camadas, ele filtra por interno
            if (request.filter.filter?.filterField == "Interno" || request.filter.filter?.filter?.filterField == "Interno")
            {
                bool isInterno = request.filter.filter?.filterField == "Interno" ? (bool)(request.filter.filter?.filterBy.Contains('1')) : (bool)(request.filter.filter?.filter?.filterBy.Contains('1'));
                if (isInterno)
                    userList = userList.Where(x => x.Interno.Value == isInterno);
                else
                    userList = userList.Where(x => x.Interno == null);
            }

            //Existem duas camadas de filtragem, se "filtrar por bloqueado" se encontrar em alguma das camadas, ele filtra por bloqueado
            if (request.filter.filter?.filterField == "Lock" || request.filter.filter?.filter?.filterField == "Lock")
            {
                bool isLocked = request.filter.filter?.filterField == "Lock" ? (bool)(request.filter.filter?.filterBy.Contains('1')) : (bool)(request.filter.filter?.filter?.filterBy.Contains('1'));
                userList = userList.Where(x => x.Locked == isLocked);
            }

            var listaUtilizador = userList
               .Where(u => u.IndActivo &&
               (u.TrabalhadorFkNavigation.Nome.Contains(request.filter.filterBy) || u.Username.Contains(request.filter.filterBy)))
               .Select(u => new UtilizadoresAcessoListagem
               {
                   id = u.IdUtilizador,
                   nome = u.TrabalhadorFkNavigation.Nome,
                   utilizador = u.Username,
                   interno = u.Interno.GetValueOrDefault(),
                   locked = u.Locked
               });

            var utilizadores = listaUtilizador
                .OrderBy(request.filter.orderBy, request.filter.orderDirection)
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = listaUtilizador.Count();
            response.rows = totalNumber;
            response.utilizador = utilizadores;

            return response;
        }

        public Utilizador GetAdminUser()
        {
            var utilizador = _moduloContribuicoesContext.Utilizador
                .Include(u => u.UtilizadorEntidadeFkNavigation)
                .SingleOrDefault(u => u.Username == "admin");

            return utilizador;
        }
    }
}