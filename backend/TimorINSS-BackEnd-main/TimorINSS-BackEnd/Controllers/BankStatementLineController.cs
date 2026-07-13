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
    [Route("api/bankstatementline")]
    [ApiController]
    public class BankStatementLineController : ControllerBase
    {
        private readonly IBankStatementLineDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICacheProvider _cache;

        public BankStatementLineController(IBankStatementLineDataManager dataManager, ICacheProvider memoryCache)
        {
            _dataManager = dataManager;
            _cache = memoryCache;
        }

        [HttpGet("GetByContaBancaria/{contaBancariaFk?}")]
        public IActionResult GetByContaBancaria(int? contaBancariaFk, [FromQuery] DateTime? dataInicio, [FromQuery] DateTime? dataFim)
        {
            BankStatementLineListResponse response;
            try
            {
                GetBankStatementLinesRequest request = new GetBankStatementLinesRequest();
                request.GetHeaderInfo(Request.Headers);
                request.ContaBancariaFk = contaBancariaFk;
                request.DataInicio = dataInicio;
                request.DataFim = dataFim;
                response = _dataManager.GetByContaBancaria(request);
            }
            catch (Exception e)
            {
                response = new BankStatementLineListResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("GetByContaBancaria", Log, null))
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("GetReceitasDisponiveis/{ano}")]
        public IActionResult GetReceitasDisponiveis(int ano)
        {
            ReceitasDisponiveisParaConciliacaoResponse response;
            try
            {
                GetReceitasDisponiveisParaConciliacaoRequest request = new GetReceitasDisponiveisParaConciliacaoRequest();
                request.GetHeaderInfo(Request.Headers);
                request.Ano = ano;
                response = _dataManager.GetReceitasDisponiveis(request);
            }
            catch (Exception e)
            {
                response = new ReceitasDisponiveisParaConciliacaoResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("GetReceitasDisponiveis", Log, null))
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("GetPagamentosDisponiveis")]
        public IActionResult GetPagamentosDisponiveis()
        {
            PagamentosDisponiveisParaConciliacaoResponse response;
            try
            {
                response = _dataManager.GetPagamentosDisponiveis();
            }
            catch (Exception e)
            {
                response = new PagamentosDisponiveisParaConciliacaoResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("GetPagamentosDisponiveis", Log, null))
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("AddLine")]
        [RequirePerm("BANCO_CONCILIAR")]
        public IActionResult AddLine(AddBankStatementLineRequest request)
        {
            ResponseBaseDataContract response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.AddLine(request);
            }
            catch (Exception e)
            {
                response = new ResponseBaseDataContract();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("AddLine", Log, request))
                return BadRequest(response);

            _cache.Reset();
            return Ok(response);
        }

        [HttpPost("DeleteLine")]
        [RequirePerm("BANCO_CONCILIAR")]
        public IActionResult DeleteLine(DeleteBankStatementLineRequest request)
        {
            ResponseBaseDataContract response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.DeleteLine(request);
            }
            catch (Exception e)
            {
                response = new ResponseBaseDataContract();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("DeleteLine", Log, request))
                return BadRequest(response);

            _cache.Reset();
            return Ok(response);
        }

        [HttpPost("MatchReceita")]
        [RequirePerm("BANCO_CONCILIAR")]
        public IActionResult MatchReceita(MatchReceitaRequest request)
        {
            ResponseBaseDataContract response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.MatchReceita(request);
            }
            catch (Exception e)
            {
                response = new ResponseBaseDataContract();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("MatchReceita", Log, request))
                return BadRequest(response);

            _cache.Reset();
            return Ok(response);
        }

        [HttpPost("MatchPagamento")]
        [RequirePerm("BANCO_CONCILIAR")]
        public IActionResult MatchPagamento(MatchPagamentoRequest request)
        {
            ResponseBaseDataContract response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.MatchPagamento(request);
            }
            catch (Exception e)
            {
                response = new ResponseBaseDataContract();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("MatchPagamento", Log, request))
                return BadRequest(response);

            _cache.Reset();
            return Ok(response);
        }

        [HttpPost("Unmatch")]
        [RequirePerm("BANCO_CONCILIAR")]
        public IActionResult Unmatch(UnmatchRequest request)
        {
            ResponseBaseDataContract response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.Unmatch(request);
            }
            catch (Exception e)
            {
                response = new ResponseBaseDataContract();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("Unmatch", Log, request))
                return BadRequest(response);

            _cache.Reset();
            return Ok(response);
        }
    }
}
