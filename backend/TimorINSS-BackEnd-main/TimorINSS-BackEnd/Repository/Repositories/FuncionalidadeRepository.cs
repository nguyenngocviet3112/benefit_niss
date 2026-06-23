using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class FuncionalidadeRepository : IFuncionalidadeRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public FuncionalidadeRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Funcionalidade> GetAll()
        {
            return _moduloContribuicoesContext.Funcionalidade.ToList();
        }

        public Funcionalidade Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var funcionalidade = _moduloContribuicoesContext.Funcionalidade
                .SingleOrDefault(u => u.Id == id);

            return funcionalidade;
        }

        public FuncionalidadeDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var funcionalidade = _moduloContribuicoesContext.Funcionalidade
                .SingleOrDefault(u => u.Id == id);

            FuncionalidadeDto funcionalidadeDto = Utils.MappClassToDto<Funcionalidade, FuncionalidadeDto>(funcionalidade);
            return funcionalidadeDto;
        }

        public void Add(Funcionalidade entity)
        {
            _moduloContribuicoesContext.Funcionalidade.Add(entity);
        }

        public void Update(Funcionalidade entity)
        {
            Funcionalidade entityToUpdate = _moduloContribuicoesContext.Funcionalidade
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Funcionalidade entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public List<FuncionalidadeDataContract> GetAllFuncionalidades()
        {
            List<FuncionalidadeDataContract> listaFuncionalidade = _moduloContribuicoesContext.Funcionalidade
               .Where(u => u.IndActivo)
               .Select(u => new FuncionalidadeDataContract
               {
                   id = u.Id,
                   descricao = u.Descricao,
                   create = false,
                   read = false,
                   update = false,
                   delete = false
               })
                .ToList();

            return listaFuncionalidade;
        }
    }
}