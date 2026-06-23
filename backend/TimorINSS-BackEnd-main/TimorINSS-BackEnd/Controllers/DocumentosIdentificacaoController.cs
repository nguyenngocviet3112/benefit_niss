using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;

namespace TimorINSSBackEnd.Controllers
{
    [Authorize]
    [Route("api/documentos")]
    [ApiController]
    public class DocumentosIdentificacaoController : ControllerBase
    {
        private readonly IDocumentoIdentificacaoDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public DocumentosIdentificacaoController(IDocumentoIdentificacaoDataManager dataManager)
        {
            _dataManager = dataManager;
        }

        [HttpPost("GetByIdTrabalhador")]
        public IActionResult GetByIdTrabalhador([FromBody] DocumentosListagemRequest request)
        {
            DocumentosListagemResponse response = new DocumentosListagemResponse();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);

                response = _dataManager.GetDocumentosIdentificacaoByIdTrabalhador(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetByIdTrabalhador", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        // POST: api/documentos
        [HttpPost("GetDocumentoById")]
        public IActionResult GetDocumentoById(DocumentoIdRequest request)
        {
            DocumentoResponse response = new DocumentoResponse();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);

                response = _dataManager.GetDocumentosIdentificacaoById(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetDocumentoById", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        // POST: api/documentos
        [HttpPost("SaveDocumento")]
        public IActionResult SaveDocumento(DocumentoRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);

                response = _dataManager.SaveDocumento(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("SaveDocumento", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        // POST: api/documentos
        [HttpPost("UpdateDocumento")]
        public IActionResult UpdateDocumento(DocumentoRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);

                response = _dataManager.UpdateDocumento(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("UpdateDocumento", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        // POST: api/documentos
        [HttpPost("DeleteDocumento")]
        public IActionResult DeleteDocumento(DocumentoIdRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);

                response = _dataManager.DeleteDocumento(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("DeleteDocumento", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("getDocumentosByIdTarefaAtivo")]
        public IActionResult getDocumentosByIdTarefaAtivo([FromBody] DocumentosListagemRequest request)
        {
            DocumentosTarefaListagemResponse response = new DocumentosTarefaListagemResponse();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);

                response = _dataManager.getDocumentosByIdTarefaAtivo(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("getDocumentosByIdTarefaAtivo", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("getDocumentosByIdProcessoAtivo")]
        public IActionResult getDocumentosByIdProcessoAtivo([FromBody] DocumentosListagemRequest request)
        {
            DocumentosTarefaListagemResponse response = new DocumentosTarefaListagemResponse();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);

                response = _dataManager.getDocumentosByIdProcessoAtivo(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("getDocumentosByIdProcessoAtivo", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        // POST: api/documentos
        [HttpPost("SaveTarefaDocumento")]
        public IActionResult SaveTarefaDocumento(TarefaDocumentoRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);

                response = _dataManager.SaveTarefaDocumento(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("SaveDocumento", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        // POST: api/documentos
        [HttpPost("DeleteDocumentoComponente")]
        public IActionResult DeleteDocumentoComponente(DocumentoIdRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);

                response = _dataManager.DeleteDocumentoComponente(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("DeleteDocumentoComponente", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}