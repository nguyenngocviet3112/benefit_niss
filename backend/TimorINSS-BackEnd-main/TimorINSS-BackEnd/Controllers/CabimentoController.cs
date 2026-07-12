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
    [Route("api/cabimento")]
    [ApiController]
    public class CabimentoController : ControllerBase
    {
        private readonly ICabimentoDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICacheProvider _cache;

        public CabimentoController(ICabimentoDataManager dataManager, ICacheProvider memoryCache)
        {
            _dataManager = dataManager;
            _cache = memoryCache;
        }

        [HttpGet("GetByAno/{ano}")]
        public IActionResult GetByAno(int ano)
        {
            CabimentoListResponse response;
            try
            {
                GetCabimentoListRequest request = new GetCabimentoListRequest();
                request.GetHeaderInfo(Request.Headers);
                request.Ano = ano;
                response = _dataManager.GetByAno(request);
            }
            catch (Exception e)
            {
                response = new CabimentoListResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("GetByAno", Log, null))
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("GetAdsDisponiveis/{ano}")]
        public IActionResult GetAdsDisponiveis(int ano)
        {
            AdsDisponiveisParaCabimentoResponse response;
            try
            {
                GetAdsDisponiveisParaCabimentoRequest request = new GetAdsDisponiveisParaCabimentoRequest();
                request.GetHeaderInfo(Request.Headers);
                request.Ano = ano;
                response = _dataManager.GetAdsDisponiveis(request);
            }
            catch (Exception e)
            {
                response = new AdsDisponiveisParaCabimentoResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("GetAdsDisponiveis", Log, null))
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("Create")]
        [RequirePerm("CABIMENTO_SUBMIT")]
        public IActionResult Create(CreateCabimentoRequest request)
        {
            CabimentoResponse response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.Create(request);
            }
            catch (Exception e)
            {
                response = new CabimentoResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("Create", Log, request))
                return BadRequest(response);

            _cache.Reset();
            return Ok(response);
        }

        [HttpPost("Save")]
        [RequirePerm("CABIMENTO_SUBMIT")]
        public IActionResult Save(SaveCabimentoRequest request)
        {
            ResponseBaseDataContract response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.Save(request);
            }
            catch (Exception e)
            {
                response = new ResponseBaseDataContract();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("Save", Log, request))
                return BadRequest(response);

            _cache.Reset();
            return Ok(response);
        }

        [HttpPost("Submit")]
        [RequirePerm("CABIMENTO_SUBMIT")]
        public IActionResult Submit(SubmitCabimentoRequest request)
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
        [RequirePerm("CABIMENTO_APPROVE")]
        public IActionResult Approve(ApproveCabimentoRequest request)
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
