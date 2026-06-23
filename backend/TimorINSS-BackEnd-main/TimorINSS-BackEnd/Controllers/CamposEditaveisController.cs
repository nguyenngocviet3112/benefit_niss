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
    [Route("api/camposEditaveis")]
    [ApiController]
    public class CamposEditaveisController : ControllerBase
    {
        private readonly ICamposEditaveisDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICacheProvider _cache;

        public CamposEditaveisController(ICamposEditaveisDataManager dataManager, ICacheProvider memoryCache)
        {
            _dataManager = dataManager;
            _cache = memoryCache;
        }

        [HttpGet("GetAllCamposEditaveis")]
        public IActionResult GetAllCamposEditaveis()
        {
            CamposEditaveisListagemResponse response = _cache.GetFromCache<CamposEditaveisListagemResponse>("GetAllCamposEditaveis");
            if (response == null)
            {
                try
                {
                    RequestBaseDataContract request = new RequestBaseDataContract();
                    // Parse dos valores do header para o request
                    request.GetHeaderInfo(Request.Headers);
                    response = _dataManager.GetAllCamposEditaveis(request);
                }
                catch (Exception e)
                {
                    response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                }
                // Guardar log do erro no ficheiro de logs
                if (response.ManageErrors("GetAllCamposEditaveis", Log, null))
                {
                    return BadRequest(response);
                }
                else
                {
                    _cache.SetCache("GetAllCamposEditaveis", response, 1);
                }
            }
            return Ok(response);
        }

        [HttpPost("GetValorCampoEditavel")]
        public IActionResult GetValorCampoEditavel(ValorCamposEditaveisRequest request)
        {
            ValueCampoEditavelListagemResponse response = new ValueCampoEditavelListagemResponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetValorCampoEditavel(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetValorCampoEditavel", Log, request))
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("SaveValorCampoEditavel")]
        public IActionResult SaveValorCampoEditavel(ValorCamposEditaveisSaveRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.SaveValorCampoEditavel(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("SaveValorCampoEditavel", Log, request))
                return BadRequest(response);

            _cache.Reset();
            return Ok(response);
        }

        [HttpPost("DeleteValorCampoEditavel")]
        public IActionResult DeleteValorCampoEditavel(ValorCamposEditaveisDeleteRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.DeleteValorCampoEditavel(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("DeleteValorCampoEditavel", Log, request))
                return BadRequest(response);

            _cache.Reset();
            return Ok(response);
        }

        [HttpPost("SaveRegimeCampoEditavel")]
        public IActionResult SaveRegimeCampoEditavel(RegimeCampoEditaveisDeleteRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.SaveRegimeCampoEditavel(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("SaveRegimeCampoEditavel", Log, request))
                return BadRequest(response);

            _cache.Reset();
            return Ok(response);
        }
    }
}