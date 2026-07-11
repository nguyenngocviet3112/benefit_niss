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
    [Route("api/obligation")]
    [ApiController]
    public class ObligationController : ControllerBase
    {
        private readonly IObligationDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICacheProvider _cache;

        public ObligationController(IObligationDataManager dataManager, ICacheProvider memoryCache)
        {
            _dataManager = dataManager;
            _cache = memoryCache;
        }

        [HttpGet("GetByAno/{ano}")]
        public IActionResult GetByAno(int ano)
        {
            ObligationListResponse response;
            try
            {
                GetObligationListRequest request = new GetObligationListRequest();
                request.GetHeaderInfo(Request.Headers);
                request.Ano = ano;
                response = _dataManager.GetByAno(request);
            }
            catch (Exception e)
            {
                response = new ObligationListResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("GetByAno", Log, null))
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("GetCompromissosComSaldo/{ano}")]
        public IActionResult GetCompromissosComSaldo(int ano)
        {
            CompromissosComSaldoResponse response;
            try
            {
                GetCompromissosComSaldoRequest request = new GetCompromissosComSaldoRequest();
                request.GetHeaderInfo(Request.Headers);
                request.Ano = ano;
                response = _dataManager.GetCompromissosComSaldo(request);
            }
            catch (Exception e)
            {
                response = new CompromissosComSaldoResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("GetCompromissosComSaldo", Log, null))
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("Create")]
        public IActionResult Create(CreateObligationRequest request)
        {
            ObligationResponse response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.Create(request);
            }
            catch (Exception e)
            {
                response = new ObligationResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("Create", Log, request))
                return BadRequest(response);

            _cache.Reset();
            return Ok(response);
        }

        [HttpPost("AddItem")]
        public IActionResult AddItem(AddObligationItemRequest request)
        {
            ResponseBaseDataContract response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.AddItem(request);
            }
            catch (Exception e)
            {
                response = new ResponseBaseDataContract();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("AddItem", Log, request))
                return BadRequest(response);

            _cache.Reset();
            return Ok(response);
        }

        [HttpPost("RemoveItem")]
        public IActionResult RemoveItem(RemoveObligationItemRequest request)
        {
            ResponseBaseDataContract response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.RemoveItem(request);
            }
            catch (Exception e)
            {
                response = new ResponseBaseDataContract();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("RemoveItem", Log, request))
                return BadRequest(response);

            _cache.Reset();
            return Ok(response);
        }

        [HttpPost("AddBeneficiary")]
        public IActionResult AddBeneficiary(AddObligationBeneficiaryRequest request)
        {
            ResponseBaseDataContract response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.AddBeneficiary(request);
            }
            catch (Exception e)
            {
                response = new ResponseBaseDataContract();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("AddBeneficiary", Log, request))
                return BadRequest(response);

            _cache.Reset();
            return Ok(response);
        }

        [HttpPost("RemoveBeneficiary")]
        public IActionResult RemoveBeneficiary(RemoveObligationBeneficiaryRequest request)
        {
            ResponseBaseDataContract response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.RemoveBeneficiary(request);
            }
            catch (Exception e)
            {
                response = new ResponseBaseDataContract();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("RemoveBeneficiary", Log, request))
                return BadRequest(response);

            _cache.Reset();
            return Ok(response);
        }

        [HttpPost("Submit")]
        public IActionResult Submit(SubmitObligationRequest request)
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
        public IActionResult Approve(ApproveObligationRequest request)
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
    }
}
