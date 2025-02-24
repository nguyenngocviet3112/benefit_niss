using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using TimorINSSBackEnd.Cache;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;

namespace TimorINSSBackEnd.Controllers
{
    [Authorize]
    [Route("api/componenteOrcamentoRegisto")]
    [ApiController]
    public class ComponentesOrcamentoRegistoController : ControllerBase
    {
        private readonly IComponenteOrcamentoRegistoDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICacheProvider _cache;

        public ComponentesOrcamentoRegistoController(IComponenteOrcamentoRegistoDataManager dataManager, ICacheProvider memoryCache)
        {
            _dataManager = dataManager;
            _cache = memoryCache;
        }

        [HttpPost("GetComponenteOrcamentoRegisto")]
        public IActionResult GetComponenteOrcamentoRegisto([FromBody] GetComponenteOrcamentoRegistoRequest request)
        {
            GetComponenteOrcamentoRegistoReponse response = new GetComponenteOrcamentoRegistoReponse();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetComponenteOrcamentoRegisto(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetComponenteOrcamentoRegisto", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("UpdateComponenteOrcamentoRegistoDates")]
        public IActionResult UpdateComponenteOrcamentoRegistoDates(UpdateComponenteOrcamentoRegistoDatesRequest request)
        {
            UpdateComponenteOrcamentoRegistoDatesResponse response = new UpdateComponenteOrcamentoRegistoDatesResponse();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.UpdateComponenteOrcamentoRegistoDates(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("UpdateComponenteOrcamentoRegistoDates", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetOrcamentoAprovadoDespesaByIdTarefaActivo")]
        public IActionResult GetOrcamentoAprovadoDespesaByIdTarefaActivo([FromBody] GetComponenteOrcamentoRegistoAprovadoRequest request)
        {
            GetComponenteOrcamentoAprovadoRegistoReponse response = new GetComponenteOrcamentoAprovadoRegistoReponse();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetOrcamentoAprovadoDespesaByIdTarefaActivo(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetOrcamentoAprovadoDespesaByIdTarefaActivo", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("RetificarOrcamentoAprovado")]
        public IActionResult RetificarOrcamentoAprovado([FromBody] UpdateComponenteOrcamentoRegistoDatesRequest request)
        {
            GetComponenteOrcamentoRegistoReponse response = new GetComponenteOrcamentoRegistoReponse();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.RetificarOrcamentoAprovado(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("RetificarOrcamentoAprovado", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("AprovarOrcamento")]
        public IActionResult AprovarOrcamento([FromBody] GetComponenteOrcamentoRegistoAprovadoRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.AprovarOrcamento(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("AprovarOrcamento", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("ExtractToExcel")]
        public IActionResult ExtractToExcel([FromBody] OrcamentoExtractRequest request)
        {
            OrcamentoExtractToExcelReponse response = new OrcamentoExtractToExcelReponse();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.ExtractToExcel(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("ExtractToExcel", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("ExtractToPDF")]
        public IActionResult ExtractToPDF([FromBody] OrcamentoExtractRequest request)
        {
            OrcamentoExtractToPDFReponse response = new OrcamentoExtractToPDFReponse();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.ExtractToPDF(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("ExtractToExcel", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetOrcamentoAprovadoReceitaByIdTarefaActivo")]
        public IActionResult GetOrcamentoAprovadoReceitaByIdTarefaActivo([FromBody] GetComponenteOrcamentoRegistoAprovadoRequest request)
        {
            GetComponenteOrcamentoAprovadoRegistoReponse response = new GetComponenteOrcamentoAprovadoRegistoReponse();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetOrcamentoAprovadoReceitaByIdTarefaActivo(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetOrcamentoAprovadoReceitaByIdTarefaActivo", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}