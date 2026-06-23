using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class ComponenteDocumentosRegistoRepository : IComponenteDocumentosRegistoRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public ComponenteDocumentosRegistoRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<ComponentedocumentoRegisto> GetAll()
        {
            return _moduloContribuicoesContext.ComponentedocumentoRegisto.Where(u => u.IndActivo).ToList();
        }

        public ComponentedocumentoRegisto Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var componenteOrcamento = _moduloContribuicoesContext.ComponentedocumentoRegisto
                .SingleOrDefault(u => u.Id == id);

            return componenteOrcamento;
        }

        public ComponentedocumentoRegistoDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var componenteOrcamento = _moduloContribuicoesContext.ComponentedocumentoRegisto
                .SingleOrDefault(u => u.Id == id);

            ComponentedocumentoRegistoDto componenteOrcamentoDto = Utils.MappClassToDto<ComponentedocumentoRegisto, ComponentedocumentoRegistoDto>(componenteOrcamento);
            return componenteOrcamentoDto;
        }

        public void Add(ComponentedocumentoRegisto entity)
        {
            _moduloContribuicoesContext.ComponentedocumentoRegisto.Add(entity);
        }

        public void Update(ComponentedocumentoRegisto entity)
        {
            ComponentedocumentoRegisto entityToUpdate = _moduloContribuicoesContext.ComponentedocumentoRegisto
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(ComponentedocumentoRegisto entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public DocumentosTarefaListagemResponse GetListagemByIdProcesso(DocumentosListagemRequest request)
        {
            DocumentosTarefaListagemResponse result = new DocumentosTarefaListagemResponse();
            int index = 0;
            if (request.filter.index.HasValue)
                index = request.filter.index.Value;

            int rows = 5;
            if (request.filter.rows.HasValue)
                rows = request.filter.rows.Value;

            IQueryable<ComponentedocumentoRegisto> queryDocumentosConditional = _moduloContribuicoesContext.ComponentedocumentoRegisto;

            var queryDocumentos = queryDocumentosConditional
                .Where(doc => doc.TarefaAtivoFkNavigation.ProcessoAtivoFk == request.Id && doc.IndActivo)
                .Select(doc => new DocumentosTarefaListagem
                {
                    IdDocumento = doc.Id,
                    TpDocIdentificacao = doc.DocumentoFkNavigation.Descricao,
                    Numero = doc.Numero,
                    DataCriacao = doc.DataCriacao,
                    Tarefa = doc.TarefaAtivoFkNavigation.TarefaconfigFkNavigation.Nome,
                    Utilizador = _moduloContribuicoesContext.Utilizador.FirstOrDefault(u => u.IdUtilizador == doc.UtilizadorCriacao).Username,
                    Documento = doc.Documento
                });

            var documentos = queryDocumentos
                .OrderBy(request.filter.orderBy, request.filter.orderDirection)
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = queryDocumentos.Count();

            result.documentos = documentos;
            result.rows = totalNumber;
            return result;
        }

        public List<ComponentedocumentoRegisto> GetByTarefaIdAndType(long tarefaId, long docType)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var documento = _moduloContribuicoesContext.ComponentedocumentoRegisto
                .Where(u => u.TarefaAtivoFk == tarefaId && u.DocumentoFk == docType).ToList();

            return documento;
        }
    }
}