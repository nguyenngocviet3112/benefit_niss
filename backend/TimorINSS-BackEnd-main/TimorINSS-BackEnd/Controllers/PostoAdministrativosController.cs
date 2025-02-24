using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using TimorINSSBackEnd.Cache;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;

namespace TimorINSSBackEnd.Controllers
{
    [Authorize]
    [Route("api/postoAdministrativo")]
    [ApiController]
    public class PostoAdministrativosController : ControllerBase
    {
        private readonly IPostoAdministrativoDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICacheProvider _cache;

        public PostoAdministrativosController(IPostoAdministrativoDataManager dataManager, ICacheProvider memoryCache)
        {
            _dataManager = dataManager;
            _cache = memoryCache;
        }

        [HttpGet("getPostoAdministrativo")]
        public IActionResult getPostoAdministrativo()
        {
            SelectDescriptionResponse response = _cache.GetFromCache<SelectDescriptionResponse>("getPostoAdministrativo");
            if (response == null)
            {
                try
                {
                    response = _dataManager.getAllPostoAdministrativo();
                }
                catch (Exception e)
                {
                    response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                }

                // Guardar log do erro no ficheiro de logs
                if (response.ManageErrors("getPostoAdministrativo", Log, null))
                {
                    return BadRequest(response);
                }
                else
                {
                    _cache.SetCache("getPostoAdministrativo", response, response.selects.Count);
                }
            }
            return Ok(response);
        }

        [HttpGet("getPostoByIdMunicipio/{id}")]
        public IActionResult getPostoByIdMunicipio(int id)
        {
            SelectDescriptionResponse response = new SelectDescriptionResponse();
            try
            {
                response = _dataManager.getPostoByIdMunicipio(id);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("getPostoByIdMunicipio", Log, null))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}