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
    [Route("api/compromissodespesa")]
    [ApiController]
    public class CompromissoDespesaController : ControllerBase
    {
        private readonly ICompromissoDespesaDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICacheProvider _cache;

        public CompromissoDespesaController(ICompromissoDespesaDataManager dataManager, ICacheProvider memoryCache)
        {
            _dataManager = dataManager;
            _cache = memoryCache;
        }

        [HttpGet("GetByAno/{ano}")]
        public IActionResult GetByAno(int ano)
        {
            CompromissoDespesaListResponse response;
            try
            {
                GetCompromissoDespesaListRequest request = new GetCompromissoDespesaListRequest();
                request.GetHeaderInfo(Request.Headers);
                request.Ano = ano;
                response = _dataManager.GetByAno(request);
            }
            catch (Exception e)
            {
                response = new CompromissoDespesaListResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("GetByAno", Log, null))
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("GetByAnoExcel/{ano}")]
        public IActionResult GetByAnoExcel(int ano)
        {
            StringFileReponse response;
            try
            {
                GetCompromissoDespesaListRequest request = new GetCompromissoDespesaListRequest();
                request.GetHeaderInfo(Request.Headers);
                request.Ano = ano;
                response = _dataManager.GetByAnoExcel(request);
            }
            catch (Exception e)
            {
                response = new StringFileReponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("GetByAnoExcel", Log, null))
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("GetCabimentosDisponiveis/{ano}")]
        public IActionResult GetCabimentosDisponiveis(int ano)
        {
            CabimentosDisponiveisResponse response;
            try
            {
                GetCabimentosDisponiveisRequest request = new GetCabimentosDisponiveisRequest();
                request.GetHeaderInfo(Request.Headers);
                request.Ano = ano;
                response = _dataManager.GetCabimentosDisponiveis(request);
            }
            catch (Exception e)
            {
                response = new CabimentosDisponiveisResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("GetCabimentosDisponiveis", Log, null))
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("Create")]
        [RequirePerm("COMPROMISSO_SUBMIT")]
        public IActionResult Create(CreateCompromissoDespesaRequest request)
        {
            CompromissoDespesaResponse response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.Create(request);
            }
            catch (Exception e)
            {
                response = new CompromissoDespesaResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("Create", Log, request))
                return BadRequest(response);

            _cache.Reset();
            return Ok(response);
        }

        [HttpPost("Save")]
        [RequirePerm("COMPROMISSO_SUBMIT")]
        public IActionResult Save(SaveCompromissoDespesaRequest request)
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

        [HttpPost("SavePlurianualidade")]
        [RequirePerm("COMPROMISSO_SUBMIT")]
        public IActionResult SavePlurianualidade(SaveCompromissoDespesaPlurianualidadeRequest request)
        {
            ResponseBaseDataContract response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.SavePlurianualidade(request);
            }
            catch (Exception e)
            {
                response = new ResponseBaseDataContract();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("SavePlurianualidade", Log, request))
                return BadRequest(response);

            _cache.Reset();
            return Ok(response);
        }

        [HttpPost("Submit")]
        [RequirePerm("COMPROMISSO_SUBMIT")]
        public IActionResult Submit(SubmitCompromissoDespesaRequest request)
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

        [HttpPost("Review")]
        [RequirePerm("COMPROMISSO_REVIEW")]
        public IActionResult Review(ReviewCompromissoDespesaRequest request)
        {
            ResponseBaseDataContract response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.Review(request);
            }
            catch (Exception e)
            {
                response = new ResponseBaseDataContract();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("Review", Log, request))
                return BadRequest(response);

            _cache.Reset();
            return Ok(response);
        }

        [HttpPost("Approve")]
        [RequirePerm("COMPROMISSO_APPROVE")]
        public IActionResult Approve(ApproveCompromissoDespesaRequest request)
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
