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
    [Route("api/subClassificacao")]
    [ApiController]
    public class SubClassificacoesController : ControllerBase
    {
        private readonly ISubClassificacaoDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICacheProvider _cache;

        public SubClassificacoesController(ISubClassificacaoDataManager dataManager, ICacheProvider memoryCache)
        {
            _dataManager = dataManager;
            _cache = memoryCache;
        }

        [HttpGet("GetAllSubClassificacao")]
        public IActionResult GetAllSubClassificacao()
        {
            SelectDescriptionResponse response = _cache.GetFromCache<SelectDescriptionResponse>("GetAllSubClassificacao");

            if (response == null)
            {
                try
                {
                    response = _dataManager.GetAllSubClassificacao();
                }
                catch (Exception e)
                {
                    response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                }

                // Guardar log do erro no ficheiro de logs
                if (response.ManageErrors("GetAllSubClassificacao", Log, null))
                {
                    return BadRequest(response);
                }
                else
                {
                    _cache.SetCache("GetAllSubClassificacao", response, response.selects.Count);
                }
            }
            return Ok(response);
        }

        [HttpPost("GetAllSubClassificacaoByTarefaAtivaId")]
        public IActionResult GetAllSubClassificacaoByTarefaAtivaId(GetAllSubClassificacaoByTarefaAtivaIdRequest request)
        {
            SelectDescriptionResponse response = new SelectDescriptionResponse();

            try
            {
                response = _dataManager.GetAllSubClassificacaoByTarefaAtivaId(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetAllSubClassificacaoByTarefaAtivaId", Log, request))
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}