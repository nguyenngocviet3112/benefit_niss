using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;

namespace TimorINSSBackEnd.Controllers
{
    [Authorize]
    [Route("api/movimentosPorConciliar")]
    [ApiController]
    public class MovimentosPorConciliarController : ControllerBase
    {
        private readonly IMovimentosPorConciliarDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public MovimentosPorConciliarController(IMovimentosPorConciliarDataManager dataManager)
        {
            _dataManager = dataManager;
        }

        [HttpPost("GetListagem")]
        public IActionResult GetListagem(MovimentosPorConciliarListagemRequest request)
        {
            MovimentosPorConciliar response = new MovimentosPorConciliar();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetListagem(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetListagem", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("CreateMovimentoPorConciliar")]
        public IActionResult CreateMovimentoPorConciliar(CreateMovimentosPorConciliarRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.CreateMovimentoPorConciliar(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("CreateMovimentoPorConciliar", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("UpdateMovimentoPorConciliar")]
        public IActionResult UpdateMovimentoPorConciliar(UpdateMovimentosPorConciliarRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.UpdateMovimentoPorConciliar(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("UpdateMovimentoPorConciliar", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("ConciliarMovimentos")]
        public IActionResult ConciliarMovimentos(ConciliarMovimentosRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.ConciliarMovimentos(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("ConciliarMovimentos", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetListagemConciliacao")]
        public IActionResult GetListagemConciliacao(MovimentosPorConciliarConciliacaoListagemRequest request)
        {
            MovimentosPorConciliar response = new MovimentosPorConciliar();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetListagemConciliacao(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetListagemConciliacao", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("DesfazerConciliacao")]
        public IActionResult DesfazerConciliacao(DesfazerConciliacaoMovimentosRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.DesfazerConciliacao(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("DesfazerConciliacao", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetMovimentosConciliados")]
        public IActionResult GetMovimentosConciliados(SearchFilterRequest request)
        {
            MovimentosPorConciliar response = new MovimentosPorConciliar();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetMovimentosConciliados(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetMovimentosConciliados", Log, null))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetMovimentoGuia")]
        public IActionResult GetMovimentoGuia(GetGuiaMovimentoRequest request)
        {
            MovimentoPorConciliarResponse response = new MovimentoPorConciliarResponse();

            try
            {
                if (request.Type != MovimentosPorConciliarListagemType.GuiaPagamento && request.Type != MovimentosPorConciliarListagemType.ReservaCredito)
                    throw new Exception("Type not valid");

                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response.movimento = _dataManager.GetByGuiaOrReserva(request.Id, request.Type == MovimentosPorConciliarListagemType.GuiaPagamento);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetMovimentosConciliados", Log, null))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

    }
}