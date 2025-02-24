using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class DocumentoIdentificacaoRepository : IDocumentoIdentificacaoRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public DocumentoIdentificacaoRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Documentoidentificacao> GetAll()
        {
            return _moduloContribuicoesContext.Documentoidentificacao.Where(u => u.IndActivo).ToList();
        }

        public Documentoidentificacao Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var documento = _moduloContribuicoesContext.Documentoidentificacao
                .SingleOrDefault(u => u.IdDocIdentificacao == id);

            return documento;
        }

        public List<Documentoidentificacao> GetByTrabalhadorEntidade(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var documento = _moduloContribuicoesContext.Documentoidentificacao
                .Where(u => u.TrabalhadorDocumetoFk == id && u.IndActivo).ToList();

            return documento;
        }

        public List<Documentoidentificacao> GetByResponsavelLegalEntidade(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var documento = _moduloContribuicoesContext.Documentoidentificacao
                .Where(u => u.RespLegalDocumentoFk == id && u.IndActivo).ToList();

            return documento;
        }

        public DocumentoidentificacaoDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var documento = _moduloContribuicoesContext.Documentoidentificacao
                .SingleOrDefault(u => u.IdDocIdentificacao == id);

            DocumentoidentificacaoDto documentoDto = Utils.MappClassToDto<Documentoidentificacao, DocumentoidentificacaoDto>(documento);
            return documentoDto;
        }

        public void Add(Documentoidentificacao entity)
        {
            _moduloContribuicoesContext.Documentoidentificacao.Add(entity);
        }

        public void Update(Documentoidentificacao entity)
        {
            Documentoidentificacao entityToUpdate = _moduloContribuicoesContext.Documentoidentificacao
                .Single(d => d.IdDocIdentificacao == entity.IdDocIdentificacao);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Documentoidentificacao entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public DocumentosListagemResponse GetListagemByFilter(DocumentosListagemRequest request)
        {
            DocumentosListagemResponse result = new DocumentosListagemResponse();
            int index = 0;
            if (request.filter.index.HasValue)
                index = request.filter.index.Value;

            int rows = 5;
            if (request.filter.rows.HasValue)
                rows = request.filter.rows.Value;

            IQueryable<Documentoidentificacao> queryDocumentosConditional = _moduloContribuicoesContext.Documentoidentificacao;
            switch (request.filter.filterField)
            {
                case "TRABALHADOR":
                    queryDocumentosConditional = queryDocumentosConditional.Where(u => u.TrabalhadorDocumetoFk == request.Id && u.IndActivo);
                    break;

                default:
                    result.Errors.Add(new Error
                    {
                        ErrorCode = ((int)ErrorsDataContract.InvalidFilter).ToString(),
                        ErrorMessage = ErrorsDataContract.InvalidFilter.ToString()
                    });
                    return result;
            }

            var queryDocumentos = queryDocumentosConditional
                .Include(doc => doc.TpDocIdentificacaoNavigation)
                .Select(doc => new DocumentosListagem
                {
                    IdDocumento = doc.IdDocIdentificacao,
                    TpDocIdentificacao = doc.TpDocIdentificacaoNavigation.Descricao,
                    Numero = doc.Numero,
                    DataValidade = doc.DataValidade,
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

        public List<Documentoidentificacao> GetDocumentoidentificacaoByTipoENumero(int tipoDocumento, string numero)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var documento = _moduloContribuicoesContext.Documentoidentificacao
                .Where(u => u.TpDocIdentificacao == tipoDocumento && u.Numero == numero && u.IndActivo).ToList();

            return documento;
        }

        public List<Documentoidentificacao> GetDocumentoidentificacaoByTrabalhadorFkETipo(int trabalhadorFk, int tipo)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var documento = _moduloContribuicoesContext.Documentoidentificacao
                .Where(u => u.TpDocIdentificacao == tipo && u.TrabalhadorDocumetoFk == trabalhadorFk && u.IndActivo).ToList();

            return documento;
        }
    }
}