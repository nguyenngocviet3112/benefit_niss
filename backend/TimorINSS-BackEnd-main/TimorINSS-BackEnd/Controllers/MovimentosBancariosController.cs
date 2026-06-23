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
    [Route("api/movimentosBancarios")]
    [ApiController]
    public class MovimentosBancariosController : ControllerBase
    {
        private readonly IMovimentosBancariosDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public MovimentosBancariosController(IMovimentosBancariosDataManager dataManager)
        {
            _dataManager = dataManager;
        }

        [HttpPost("InsertMovimento")]
        public IActionResult InsertMovimento(MovimentosUpsertDataRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.InsertMovimento(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("InsertMovimento", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("UpdateMovimento")]
        public IActionResult UpdateMovimento(MovimentosUpsertDataRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.UpdateMovimento(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("UpdateMovimento", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("DeleteMovimento")]
        public IActionResult DeleteMovimento(MovimentosDeleteRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.DeleteMovimento(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("DeleteMovimento", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("ListMovimentos")]
        public IActionResult ListMovimentos(MovimentosListagemRequest request)
        {
            MovimentosListagemResponse response = new MovimentosListagemResponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.ListMovimentos(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("ListMovimentos", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("ListContasBancarias")]
        public IActionResult ListContasBancarias(bool incluirSaldo = false)
        {
            ContasBancariasListagemResponse response = new ContasBancariasListagemResponse();

            try
            {
                response = _dataManager.ListContasBancarias(incluirSaldo);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("ListContasBancarias", Log, null))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("getMovimentoBancarioDropList")]
        public IActionResult getMovimentoBancarioDropList(int domainFilterId)
        {
            DominioDescricaoStringResponse response = new DominioDescricaoStringResponse();

            try
            {
                response = _dataManager.getMovimentoBancarioDropList(domainFilterId);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("getAllMovimentosTypes", Log, null))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("ListMovimentosConciliacao")]
        public IActionResult ListMovimentosConciliacao(MovimentosBancarioConciliacaoListagemRequest request)
        {
            MovimentosListagemResponse response = new MovimentosListagemResponse();

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
            if (response.ManageErrors("ListMovimentosConciliacao", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("ListSaldoMovimentos")]
        public IActionResult ListSaldoMovimentos(ListSaldoMovimentosRequest request)
        {
            SaldoMovimentosResponse response = new SaldoMovimentosResponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.ListSaldoMovimentos(request.ContaId, request.CaixaId);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("ListSaldoMovimentos", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("ListPermissions")]
        public IActionResult ListPermissions(ConciliarMovimentosPermissionsListRequest request)
        {
            ConciliarMovimentosPermissionsListResponse response = new ConciliarMovimentosPermissionsListResponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.ListPermissions(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("ListPermissions", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("ListMovimentosExcel")]
        public IActionResult ListMovimentosExcel(MovimentosListagemRequest request)
        {
            StringFileReponse response = new StringFileReponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.ListMovimentosExcel(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("ListMovimentosExcel", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("ImportMovimentos")]
        [RequestSizeLimit(100000000)]
        public IActionResult ImportMovimentos([FromForm] ExcelImporterRequest request, int tarefaAtivoId, int? caixaId, int? bancoId)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.ImportMovimentos(request, tarefaAtivoId, caixaId, bancoId);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("ImportMovimentos", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}