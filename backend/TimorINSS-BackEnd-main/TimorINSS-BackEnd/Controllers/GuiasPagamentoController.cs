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
    [Route("api/guiaPagamento")]
    [ApiController]
    public class GuiaPagamentoController : ControllerBase
    {
        private readonly IGuiaPagamentoDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICacheProvider _cache;

        public GuiaPagamentoController(IGuiaPagamentoDataManager dataManager, ICacheProvider memoryCache)
        {
            _dataManager = dataManager;
            _cache = memoryCache;
        }

        // POST: api/guiaPagamento
        [HttpPost("SaveGuiaPagamento")]
        public IActionResult SaveGuiaPagamento(GuiaPagamentoRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.SaveGuiaPagamento(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("SaveGuiaPagamento", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        // POST: api/guiaPagamento
        [HttpPost("listGuiasByEntidade")]
        public IActionResult listGuiasByEntidade(GetAllGuiasStatesFromYearByFilterRequest request)
        {
            GuiaListagemResponse response = new GuiaListagemResponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.listGuiasByEntidade(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("listGuiasByEntidade", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        // POST: api/guiaPagamento
        [HttpPost("guiaPagamentoDetail")]
        public IActionResult guiaPagamentoDetail(GetGuiaPagamentoRequest request)
        {
            GuiaListagemResponse response = new GuiaListagemResponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.guiaPagamentoDetail(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("guiaPagamentoDetail", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        // POST: api/guiaPagamento
        [HttpPost("listGuiasByEntidadeApprove")]
        public IActionResult listGuiasByEntidadeApprove(GetAllGuiasStatesFromDateByFilterRequest request)
        {
            GuiaListagemResponse response = new GuiaListagemResponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.listGuiasByEntidadeApprove(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("listGuiasByEntidade", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        // POST: api/guiaPagamento
        [HttpPost("useCreditInGuiaPagamento")]
        public IActionResult useCreditInGuiaPagamento(UseCreditInGuiaPagamentoRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.useCreditInGuiaPagamento(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("useCreditInGuiaPagamento", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        // POST: api/guiaPagamento
        [HttpPost("insertComprovativoPagamento")]
        public IActionResult insertComprovativoPagamento(insertComprovativoPagamentoRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.insertComprovativoPagamento(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("insertComprovativoPagamento", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }


        // POST: api/guiaPagamento
        [HttpPost("approveComprovativoPagamento")]
        public IActionResult approveComprovativoPagamento(approveComprovativoPagamentoRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.approveComprovativoPagamento(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("approveComprovativoPagamento", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetGuiasPagamentoRelatorios")]
        public IActionResult GetGuiasPagamentoRelatorios(RelatorioGuiaPagamentoListagemRequest request)
        {
            RelatorioGuiaPagamentoListagemResponse response = new RelatorioGuiaPagamentoListagemResponse();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetGuiasPagamentoRelatorios(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetGuiasPagamentoRelatorios", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}