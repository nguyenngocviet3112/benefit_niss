using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class ComponenteTextoRepository : IComponenteTextoRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public ComponenteTextoRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Componentetexto> GetAll()
        {
            return _moduloContribuicoesContext.Componentetexto.Where(u => u.IndActivo).ToList();
        }

        public Componentetexto Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var componenteTexto = _moduloContribuicoesContext.Componentetexto
                .SingleOrDefault(u => u.Id == id);

            return componenteTexto;
        }

        public ComponenteTextoDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var componenteTexto = _moduloContribuicoesContext.Componentetexto
                .SingleOrDefault(u => u.Id == id);

            ComponenteTextoDto componenteTextoDto = Utils.MappClassToDto<Componentetexto, ComponenteTextoDto>(componenteTexto);
            return componenteTextoDto;
        }

        public void Add(Componentetexto entity)
        {
            _moduloContribuicoesContext.Componentetexto.Add(entity);
        }

        public void Update(Componentetexto entity)
        {
            Componentetexto entityToUpdate = _moduloContribuicoesContext.Componentetexto
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Componentetexto entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public ComponenteTexto GetByIdTarefa(int idTarefa)
        {
            var queryComponenteTexto = _moduloContribuicoesContext.Componentetexto
                .Where(u => u.TarefaFk == idTarefa && u.IndActivo)
                .Select(componenteTexto => new ComponenteTexto
                {
                    id = componenteTexto.Id,
                    texto1 = (bool)componenteTexto.Texto1,
                    expandir1 = (bool)componenteTexto.Expandir1,
                    titulo1 = componenteTexto.Titulo1,
                    quantCaracteres1 = (int)componenteTexto.QuantCaracteres1,
                    obrigatorio1 = (bool)componenteTexto.Obrigatorio1,
                    obrigatorioArquivar1 = (bool)componenteTexto.ObrigatorioAoArquivar1,
                    texto2 = (bool)componenteTexto.Texto2,
                    expandir2 = (bool)componenteTexto.Expandir2,
                    titulo2 = componenteTexto.Titulo2,
                    quantCaracteres2 = (int)componenteTexto.QuantCaracteres2,
                    obrigatorio2 = (bool)componenteTexto.Obrigatorio2,
                    obrigatorioArquivar2 = (bool)componenteTexto.ObrigatorioAoArquivar2,
                });

            ComponenteTexto entity = queryComponenteTexto
            .SingleOrDefault();

            return entity;
        }

        public Componentetexto GetComponenteByIdTarefa(int idTarefa)
        {
            var queryComponenteTexto = _moduloContribuicoesContext.Componentetexto
                .Where(u => u.TarefaFk == idTarefa && u.IndActivo)
                .Select(componenteTexto => new Componentetexto
                {
                    Id = componenteTexto.Id,
                    Texto1 = (bool)componenteTexto.Texto1,
                    Expandir1 = (bool)componenteTexto.Expandir1,
                    Titulo1 = componenteTexto.Titulo1,
                    QuantCaracteres1 = (int)componenteTexto.QuantCaracteres1,
                    Obrigatorio1 = (bool)componenteTexto.Obrigatorio1,
                    ObrigatorioAoArquivar1 = (bool)componenteTexto.ObrigatorioAoArquivar1,
                    Texto2 = (bool)componenteTexto.Texto2,
                    Expandir2 = (bool)componenteTexto.Expandir2,
                    Titulo2 = componenteTexto.Titulo2,
                    QuantCaracteres2 = (int)componenteTexto.QuantCaracteres2,
                    Obrigatorio2 = (bool)componenteTexto.Obrigatorio2,
                    ObrigatorioAoArquivar2 = (bool)componenteTexto.ObrigatorioAoArquivar2,
                    UtilizadorCriacao = componenteTexto.UtilizadorCriacao,
                    DataCriacao = componenteTexto.DataCriacao,
                    TarefaFk = componenteTexto.TarefaFk,
                    IndActivo = componenteTexto.IndActivo,
                });

            Componentetexto entity = queryComponenteTexto
            .SingleOrDefault();

            return entity;
        }
    }
}