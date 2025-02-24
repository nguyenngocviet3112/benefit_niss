using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class RelPerfilFuncionalidadeRepository : IRelPerfilFuncionalidadeRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public RelPerfilFuncionalidadeRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Relperfilfuncionalidade> GetAll()
        {
            return _moduloContribuicoesContext.Relperfilfuncionalidade.ToList();
        }

        public Relperfilfuncionalidade Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var relPerfilFuncionalidade = _moduloContribuicoesContext.Relperfilfuncionalidade
                .SingleOrDefault(u => u.Id == id);

            return relPerfilFuncionalidade;
        }

        public RelPerfilFuncionalidadeDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var relPerfilFuncionalidade = _moduloContribuicoesContext.Relperfilfuncionalidade
                .SingleOrDefault(u => u.Id == id);

            RelPerfilFuncionalidadeDto relPerfilFuncionalidadeDto = Utils.MappClassToDto<Relperfilfuncionalidade, RelPerfilFuncionalidadeDto>(relPerfilFuncionalidade);
            return relPerfilFuncionalidadeDto;
        }

        public void Add(Relperfilfuncionalidade entity)
        {
            _moduloContribuicoesContext.Relperfilfuncionalidade.Add(entity);
        }

        public void Update(Relperfilfuncionalidade entity)
        {
            Relperfilfuncionalidade entityToUpdate = _moduloContribuicoesContext.Relperfilfuncionalidade
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Relperfilfuncionalidade entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public List<Relperfilfuncionalidade> GetFuncionalidadeByPerfil(List<int> perfilIds)
        {
            var result = new List<Relperfilfuncionalidade>();

            result = _moduloContribuicoesContext.Relperfilfuncionalidade
                .Include(u => u.PerfilFkNavigation)
                .Where(x => perfilIds.Contains(x.PerfilFk)).ToList();

            return result;
        }

        public List<FuncionalidadeDataContract> GetRelPerfilFuncionalidadeByIdPerfil(int idPerfil)
        {
            List<FuncionalidadeDataContract> listaPerfilFuncionalidade = _moduloContribuicoesContext.Relperfilfuncionalidade
               .Where(u => u.PerfilFk == idPerfil)
               .Select(u => new FuncionalidadeDataContract
               {
                   id = u.FuncionalidadeFkNavigation.Id,
                   descricao = u.FuncionalidadeFkNavigation.Descricao,
                   create = u.Create,
                   read = u.Read,
                   update = u.Update,
                   delete = u.Delete
               })
                .ToList();
            return listaPerfilFuncionalidade;
        }
    }
}