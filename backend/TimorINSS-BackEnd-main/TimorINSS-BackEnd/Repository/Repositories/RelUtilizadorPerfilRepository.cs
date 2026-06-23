using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class RelUtilizadorPerfilRepository : IRelUtilizadorPerfilRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public RelUtilizadorPerfilRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Relutilizadorperfil> GetAll()
        {
            return _moduloContribuicoesContext.Relutilizadorperfil.ToList();
        }

        public Relutilizadorperfil Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var relUtilizadorPerfil = _moduloContribuicoesContext.Relutilizadorperfil
                .SingleOrDefault(u => u.Id == id);

            return relUtilizadorPerfil;
        }

        public RelUtilizadorPerfilDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var relUtilizadorPerfil = _moduloContribuicoesContext.Relutilizadorperfil
                .SingleOrDefault(u => u.Id == id);

            RelUtilizadorPerfilDto relUtilizadorPerfilDto = Utils.MappClassToDto<Relutilizadorperfil, RelUtilizadorPerfilDto>(relUtilizadorPerfil);
            return relUtilizadorPerfilDto;
        }

        public void Add(Relutilizadorperfil entity)
        {
            _moduloContribuicoesContext.Relutilizadorperfil.Add(entity);
        }

        public void Update(Relutilizadorperfil entity)
        {
            Relutilizadorperfil entityToUpdate = _moduloContribuicoesContext.Relutilizadorperfil
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Relutilizadorperfil entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public List<int> GetPerfisIdsByUtilizador(int userId)
        {
            var list = new List<int>();

            list = _moduloContribuicoesContext.Relutilizadorperfil
                .Where(u => u.UtilizadorFk == userId && u.IndActivo).Select(x => x.PerfilFk).ToList();

            return list;
        }

        public List<Relutilizadorperfil> GetRelUtilizadorPerfilByUserId(int userId)
        {
            List<Relutilizadorperfil> list = new List<Relutilizadorperfil>();

            list = _moduloContribuicoesContext.Relutilizadorperfil
                   .Include(u => u.PerfilFkNavigation)
                   .Where(u => u.UtilizadorFk == userId && u.IndActivo)
                   .ToList();

            return list;
        }

        public PerfilListagemResponse GetPerfisByUserId(int id)
        {
            PerfilListagemResponse response = new PerfilListagemResponse();

            List<PerfilDataContract> listaPerfil = _moduloContribuicoesContext.Relutilizadorperfil
               .Where(u => u.UtilizadorFk == id && u.IndActivo)
               .Select(u => new PerfilDataContract
               {
                   id = u.PerfilFk,
                   descricao = u.PerfilFkNavigation.Descricao,
               })
                .ToList();

            response.perfil = listaPerfil;

            return response;
        }

        public Relutilizadorperfil GetRelByPerfilId(int idPerfil)
        {
            Relutilizadorperfil response = new Relutilizadorperfil();

            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var relUtilizadorPerfil = _moduloContribuicoesContext.Relutilizadorperfil
                .SingleOrDefault(u => u.PerfilFk == idPerfil && u.IndActivo);

            return relUtilizadorPerfil;
        }
    }
}