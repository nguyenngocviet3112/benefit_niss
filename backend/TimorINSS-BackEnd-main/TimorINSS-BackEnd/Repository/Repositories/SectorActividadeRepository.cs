using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class SectorActividadeRepository : ISectorActividadeRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public SectorActividadeRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Sectoractividade> GetAll()
        {
            return _moduloContribuicoesContext.Sectoractividade.ToList();
        }

        public Sectoractividade Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var sectorActividade = _moduloContribuicoesContext.Sectoractividade
                .SingleOrDefault(u => u.IdSectorActividade == id);

            return sectorActividade;
        }

        public SectoractividadeDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var sectorActividade = _moduloContribuicoesContext.Sectoractividade
                .SingleOrDefault(u => u.IdSectorActividade == id);

            SectoractividadeDto sectorActividadeDto = Utils.MappClassToDto<Sectoractividade, SectoractividadeDto>(sectorActividade);
            return sectorActividadeDto;
        }

        public void Add(Sectoractividade entity)
        {
            _moduloContribuicoesContext.Sectoractividade.Add(entity);
        }

        public void Update(Sectoractividade entity)
        {
            Sectoractividade entityToUpdate = _moduloContribuicoesContext.Sectoractividade
                .Single(d => d.IdSectorActividade == entity.IdSectorActividade);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Sectoractividade entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public List<SelectDescription> GetAllSectorActividade()
        {
            return _moduloContribuicoesContext.Sectoractividade
               .Select(u => new SelectDescription
               {
                   id = u.IdSectorActividade,
                   nome = u.Descricao,
                   indActivo = u.IndActivo
               })
                .ToList();
        }

        public ValueCampoEditavelListagemResponse GetAllActiveSectorActividade(SearchFilter filter)
        {
            ValueCampoEditavelListagemResponse response = new ValueCampoEditavelListagemResponse();

            int index = 0;
            if (filter.index.HasValue)
                index = filter.index.Value;

            int rows = 5;
            if (filter.rows.HasValue)
                rows = filter.rows.Value;

            var querySector = _moduloContribuicoesContext.Sectoractividade
                .Where(s => s.IndActivo && s.Descricao.Contains(filter.filterBy))
                .Select(u => new ValorCamposEditaveis
                {
                    Id = u.IdSectorActividade,
                    Nome = u.Descricao,
                    Parametros = new List<ParametrosAdicionais>()
                });

            var sectores = querySector
                .OrderBy("Id")
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = querySector.Count();

            response.ValuesCampo = sectores;
            response.CountValuesCampo = totalNumber;

            return response;
        }
    }
}