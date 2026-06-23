using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class ComponenteCarregarDocumentosRepository : IComponenteCarregarDocumentoRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public ComponenteCarregarDocumentosRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Componentecarregardocumento> GetAll()
        {
            return _moduloContribuicoesContext.Componentecarregardocumento.Where(u => u.IndActivo).ToList();
        }

        public Componentecarregardocumento Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var componenteCarregarDocuumento = _moduloContribuicoesContext.Componentecarregardocumento
                .SingleOrDefault(u => u.Id == id);

            return componenteCarregarDocuumento;
        }

        public ComponenteCarregarDocumentoDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var componenteCarregarDocumento = _moduloContribuicoesContext.Componentecarregardocumento
                .SingleOrDefault(u => u.Id == id);

            ComponenteCarregarDocumentoDto componenteCarregarDocumentoDto = Utils.MappClassToDto<Componentecarregardocumento, ComponenteCarregarDocumentoDto>(componenteCarregarDocumento);
            return componenteCarregarDocumentoDto;
        }

        public void Add(Componentecarregardocumento entity)
        {
            _moduloContribuicoesContext.Componentecarregardocumento.Add(entity);
        }

        public void Update(Componentecarregardocumento entity)
        {
            Componentecarregardocumento entityToUpdate = _moduloContribuicoesContext.Componentecarregardocumento
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Componentecarregardocumento entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public List<ComponenteDocumentoTarefa> GetByIdTarefa(int idTarefa)
        {
            var componenteCarregarDocumentos = _moduloContribuicoesContext.Componentecarregardocumento
                   .Where(u => u.TarefaFk == idTarefa && u.IndActivo)
                   .Select(componente => new ComponenteDocumentoTarefa
                   {
                       id = componente.Id,
                       idDocumento = componente.DocumentoFk,
                       nomeDocumento = componente.DocumentoFkNavigation.Descricao,
                       obrigatorio = componente.Obrigatorio
                   });

            List<ComponenteDocumentoTarefa> listaComponenteCarregarDocumentos = componenteCarregarDocumentos
              .ToList();

            return listaComponenteCarregarDocumentos;
        }

        public List<Componentecarregardocumento> GetComponenteByIdTarefa(int idTarefa)
        {
            var componenteCarregarDocumentos = _moduloContribuicoesContext.Componentecarregardocumento
                   .Where(u => u.TarefaFk == idTarefa && u.IndActivo)
                   .Select(componente => new Componentecarregardocumento
                   {
                       Id = componente.Id,
                       DocumentoFk = componente.DocumentoFk,
                       TarefaFk = componente.TarefaFk,
                       Obrigatorio = componente.Obrigatorio,
                       UtilizadorCriacao = componente.UtilizadorCriacao,
                       DataCriacao = componente.DataCriacao,
                       IndActivo = componente.IndActivo
                   });

            List<Componentecarregardocumento> listaComponenteCarregarDocumentos = componenteCarregarDocumentos
              .ToList();

            return listaComponenteCarregarDocumentos;
        }
    }
}