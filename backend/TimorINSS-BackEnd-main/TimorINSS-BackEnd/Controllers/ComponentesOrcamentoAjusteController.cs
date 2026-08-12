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
    [Route("api/componenteOrcamentoAjuste")]
    [ApiController]
    public class ComponentesOrcamentoAjusteController : ControllerBase
    {
        private readonly IComponenteOrcamentoAjusteDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ComponentesOrcamentoAjusteController(IComponenteOrcamentoAjusteDataManager dataManager)
        {
            _dataManager = dataManager;
        }

        [HttpPost("SolicitarAjuste")]
        public IActionResult SolicitarAjuste([FromBody] SolicitarAjusteOrcamentoRequest request)
        {
            SolicitarAjusteOrcamentoResponse response = new SolicitarAjusteOrcamentoResponse();
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.SolicitarAjuste(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("SolicitarAjuste", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("AprovarAjuste")]
        public IActionResult AprovarAjuste([FromBody] AprovarAjusteOrcamentoRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.AprovarAjuste(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("AprovarAjuste", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("RejeitarAjuste")]
        public IActionResult RejeitarAjuste([FromBody] RejeitarAjusteOrcamentoRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.RejeitarAjuste(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("RejeitarAjuste", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetPendentes")]
        public IActionResult GetPendentes([FromBody] GetAjustesOrcamentoRequest request)
        {
            GetAjustesOrcamentoResponse response = new GetAjustesOrcamentoResponse();
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetPendentes(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("GetPendentes", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetRubricasDisponiveis")]
        public IActionResult GetRubricasDisponiveis([FromBody] RequestBaseDataContract request)
        {
            GetRubricasDisponiveisResponse response = new GetRubricasDisponiveisResponse();
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetRubricasDisponiveis(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("GetRubricasDisponiveis", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetHistorico")]
        public IActionResult GetHistorico([FromBody] GetAjustesOrcamentoRequest request)
        {
            GetAjustesOrcamentoResponse response = new GetAjustesOrcamentoResponse();
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetHistorico(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("GetHistorico", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
