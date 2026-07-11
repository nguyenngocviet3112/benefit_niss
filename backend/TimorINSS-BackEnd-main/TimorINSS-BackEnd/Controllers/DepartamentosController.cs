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
    [Route("api/departamento")]
    [ApiController]
    public class DepartamentosController : ControllerBase
    {
        private readonly IDepartamentoDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICacheProvider _cache;

        public DepartamentosController(IDepartamentoDataManager dataManager, ICacheProvider memoryCache)
        {
            _dataManager = dataManager;
            _cache = memoryCache;
        }

        [HttpGet("GetAllDepartamentosAtivo")]
        public IActionResult GetAllDepartamentosAtivo()
        {
            SelectDescriptionResponse response = _cache.GetFromCache<SelectDescriptionResponse>("GetAllDepartamentosAtivo");

            if (response == null)
            {
                try
                {
                    response = _dataManager.GetAllDepartamentosAtivo();
                }
                catch (Exception e)
                {
                    response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                }

                // Guardar log do erro no ficheiro de logs
                if (response.ManageErrors("GetAllDepartamentosAtivo", Log, null))
                {
                    return BadRequest(response);
                }
                else
                {
                    _cache.SetCache("GetAllDepartamentosAtivo", response, response.selects.Count);
                }
            }
            return Ok(response);
        }

        [HttpGet("GetAllConfig")]
        public IActionResult GetAllConfig()
        {
            DepartamentoConfigListResponse response;
            try
            {
                response = _dataManager.GetAllConfig();
            }
            catch (Exception e)
            {
                response = new DepartamentoConfigListResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("GetAllConfig", Log, null))
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("SaveConfig")]
        public IActionResult SaveConfig(SaveDepartamentoConfigRequest request)
        {
            DepartamentoConfigResponse response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.SaveConfig(request);
            }
            catch (Exception e)
            {
                response = new DepartamentoConfigResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("SaveConfig", Log, request))
                return BadRequest(response);

            _cache.Reset();
            return Ok(response);
        }

        [HttpPost("DeactivateConfig")]
        public IActionResult DeactivateConfig(DeactivateDepartamentoConfigRequest request)
        {
            ResponseBaseDataContract response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.DeactivateConfig(request);
            }
            catch (Exception e)
            {
                response = new ResponseBaseDataContract();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("DeactivateConfig", Log, request))
                return BadRequest(response);

            _cache.Reset();
            return Ok(response);
        }
    }
}