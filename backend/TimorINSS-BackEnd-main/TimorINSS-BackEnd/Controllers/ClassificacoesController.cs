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
    [Route("api/classificacao")]
    [ApiController]
    public class ClassificacoesController : ControllerBase
    {
        private readonly IClassificacaoDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICacheProvider _cache;

        public ClassificacoesController(IClassificacaoDataManager dataManager, ICacheProvider memoryCache)
        {
            _dataManager = dataManager;
            _cache = memoryCache;
        }

        [HttpGet("GetAllClassificacao")]
        public IActionResult GetAllClassificacao()
        {
            SelectDescriptionResponse response = _cache.GetFromCache<SelectDescriptionResponse>("GetAllClassificacao");

            if (response == null)
            {
                try
                {
                    response = _dataManager.GetAllClassificacao();
                }
                catch (Exception e)
                {
                    response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                }

                // Guardar log do erro no ficheiro de logs
                if (response.ManageErrors("GetAllClassificacao", Log, null))
                {
                    return BadRequest(response);
                }
                else
                {
                    _cache.SetCache("GetAllClassificacao", response, response.selects.Count);
                }
            }
            return Ok(response);
        }
    }
}