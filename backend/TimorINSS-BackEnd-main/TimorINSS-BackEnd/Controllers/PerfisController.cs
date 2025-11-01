using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using TimorINSSBackEnd.Cache;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;

namespace TimorINSSBackEnd.Controllers
{
    [Authorize]
    [Route("api/perfil")]
    [ApiController]
    public class PefisController : ControllerBase
    {
        private readonly IPerfilDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICacheProvider _cache;

        public PefisController(IPerfilDataManager dataManager, ICacheProvider memoryCache)
        {
            _dataManager = dataManager;
            _cache = memoryCache;
        }

        [HttpPost("GetAllPerfis")]
        public IActionResult GetAllPerfis(SearchFilterRequest request)
        {
            PerfilListagemResponse response = new PerfilListagemResponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetAllPerfis(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetAllPerfis", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("UpdatePerfil")]
        public IActionResult UpdatePerfil(PerfilRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.UpdatePerfil(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("UpdatePerfil", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("AddPerfil")]
        public IActionResult AddPerfil(AddPerfilRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.AddPerfil(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("AddPerfil", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("EditPerfil")]
        public IActionResult EditPerfil(AddPerfilRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.EditPerfil(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("EditPerfil", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("GetAllPerfisAtivo")]
        public IActionResult GetAllPerfisAtivo()
        {
            SelectDescriptionResponse response = new SelectDescriptionResponse();
            //SelectDescriptionResponse response = _cache.GetFromCache<SelectDescriptionResponse>("GetAllPerfisAtivo");
            //if (response == null)
            //{
                try
                {
                    response = _dataManager.GetAllPerfisAtivo();
                }
                catch (Exception e)
                {
                    response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                }

                // Guardar log do erro no ficheiro de logs
                if (response.ManageErrors("GetAllPerfisAtivo", Log, null))
                {
                    return BadRequest(response);
                }
                //else
                //{
                //    _cache.SetCache("GetAllPerfisAtivo", response, response.selects.Count);
                //}
            //}
            return Ok(response);
        }
    }
}