using System.Collections.Generic;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IDataRepository<TEntity, TDto>
    {
        IEnumerable<TEntity> GetAll();

        TEntity Get(long id);

        TDto GetDto(long id);

        void Add(TEntity entity);

        void Update(TEntity entity);

        void Delete(TEntity entity);
    }
}