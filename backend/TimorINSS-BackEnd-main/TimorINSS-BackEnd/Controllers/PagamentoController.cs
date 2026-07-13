using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using TimorINSSBackEnd.Authorization;
using TimorINSSBackEnd.Cache;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;

namespace TimorINSSBackEnd.Controllers
{
    [Authorize]
    [Route("api/pagamento")]
    [ApiController]
    public class PagamentoController : ControllerBase
    {
        private readonly IPaymentDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICacheProvider _cache;

        public PagamentoController(IPaymentDataManager dataManager, ICacheProvider memoryCache)
        {
            _dataManager = dataManager;
            _cache = memoryCache;
        }

        [HttpGet("GetByAno/{ano}")]
        public IActionResult GetByAno(int ano)
        {
            PaymentAuthorizationListResponse response;
            try
            {
                GetPaymentListRequest request = new GetPaymentListRequest();
                request.GetHeaderInfo(Request.Headers);
                request.Ano = ano;
                response = _dataManager.GetByAno(request);
            }
            catch (Exception e)
            {
                response = new PaymentAuthorizationListResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("GetByAno", Log, null))
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("GetObligacoesDisponiveis/{ano}")]
        public IActionResult GetObligacoesDisponiveis(int ano)
        {
            ObligacoesDisponiveisParaPagamentoResponse response;
            try
            {
                GetObligacoesDisponiveisParaPagamentoRequest request = new GetObligacoesDisponiveisParaPagamentoRequest();
                request.GetHeaderInfo(Request.Headers);
                request.Ano = ano;
                response = _dataManager.GetObligacoesDisponiveis(request);
            }
            catch (Exception e)
            {
                response = new ObligacoesDisponiveisParaPagamentoResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("GetObligacoesDisponiveis", Log, null))
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("GetCodigoContaOptions")]
        public IActionResult GetCodigoContaOptions()
        {
            CodigoContaOptionsResponse response;
            try
            {
                response = _dataManager.GetCodigoContaOptions();
            }
            catch (Exception e)
            {
                response = new CodigoContaOptionsResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("GetCodigoContaOptions", Log, null))
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("GetContaBancariaOptions")]
        public IActionResult GetContaBancariaOptions()
        {
            ContaBancariaOptionsResponse response;
            try
            {
                response = _dataManager.GetContaBancariaOptions();
            }
            catch (Exception e)
            {
                response = new ContaBancariaOptionsResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("GetContaBancariaOptions", Log, null))
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("Create")]
        [RequirePerm("PAG_SUBMIT")]
        public IActionResult Create(CreatePaymentAuthorizationRequest request)
        {
            PaymentAuthorizationResponse response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.Create(request);
            }
            catch (Exception e)
            {
                response = new PaymentAuthorizationResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("Create", Log, request))
                return BadRequest(response);

            _cache.Reset();
            return Ok(response);
        }

        [HttpPost("Submit")]
        [RequirePerm("PAG_SUBMIT")]
        public IActionResult Submit(SubmitPaymentAuthorizationRequest request)
        {
            ResponseBaseDataContract response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.Submit(request);
            }
            catch (Exception e)
            {
                response = new ResponseBaseDataContract();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("Submit", Log, request))
                return BadRequest(response);

            _cache.Reset();
            return Ok(response);
        }

        [HttpPost("Approve")]
        [RequirePerm("PAG_APPROVE")]
        public IActionResult Approve(ApprovePaymentAuthorizationRequest request)
        {
            ResponseBaseDataContract response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.Approve(request);
            }
            catch (Exception e)
            {
                response = new ResponseBaseDataContract();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("Approve", Log, request))
                return BadRequest(response);

            _cache.Reset();
            return Ok(response);
        }

        [HttpPost("Execute")]
        [RequirePerm("PAG_EXECUTE")]
        public IActionResult Execute(ExecutePaymentRequest request)
        {
            ResponseBaseDataContract response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.Execute(request);
            }
            catch (Exception e)
            {
                response = new ResponseBaseDataContract();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("Execute", Log, request))
                return BadRequest(response);

            _cache.Reset();
            return Ok(response);
        }

        [HttpPost("CompletarLancamento")]
        [RequirePerm("PAG_APPROVE", "PAG_EXECUTE")]
        public IActionResult CompletarLancamento(CompletarLancamentoPagamentoRequest request)
        {
            ResponseBaseDataContract response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.CompletarLancamento(request);
            }
            catch (Exception e)
            {
                response = new ResponseBaseDataContract();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("CompletarLancamento", Log, request))
                return BadRequest(response);

            _cache.Reset();
            return Ok(response);
        }
    }
}
