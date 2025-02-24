using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Globalization;
using System.Reflection;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;

namespace TimorINSSBackEnd.Controllers
{
    [Authorize]
    [Route("api/declaracao")]
    [ApiController]
    public class DeclaracaoRemuneracaoController : ControllerBase
    {
        private readonly IDeclaracaoremuneracaoDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public DeclaracaoRemuneracaoController(IDeclaracaoremuneracaoDataManager dataManager)
        {
            _dataManager = dataManager;
        }

        [HttpPost("GetDeclaracaoByEntidadeAndFilter")]
        public IActionResult GetDeclaracaoByEntidadeAndFilter(GetDeclaracaoByEntidadeAndFilterRequest request)
        {
            GetDeclaracaoByEntidadeAndFilterResponse response = new GetDeclaracaoByEntidadeAndFilterResponse();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);

                response = _dataManager.GetDeclaracaoByEntidadeAndFilter(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetDeclaracaoByEntidadeAndFilter", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetResumoDeclaracao")]
        public IActionResult GetResumoDeclaracao(GetResumoDeclaracaoRequest request)
        {
            ResumoDeclaracaoResponse response = new ResumoDeclaracaoResponse();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);

                response = _dataManager.GetResumoDeclaracao(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetResumoDeclaracao", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("SaveDeclaracao")]
        public IActionResult SaveDeclaracao(SaveDeclaracoesRequest request)
        {
            var culture = CultureInfo.CurrentCulture.Name;
            CultureInfo cultureinfo = new CultureInfo(culture);
            request.data = DateTime.Parse(request.data.ToString("dd MM yyyy"), cultureinfo);

            ResponseBaseDataContract response = new ResponseBaseDataContract();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);

                response = _dataManager.SubmitDeclaracao(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("SaveDeclaracao", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetDeclaracoesRelatorios")]
        public IActionResult GetDeclaracoesRelatorios(RelatorioDeclaracaoRenumeracaoListagemRequest request)
        {
            RelatorioDeclaracaoRenumeracaoListagemResponse response = new RelatorioDeclaracaoRenumeracaoListagemResponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetDeclaracoesRelatorios(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetDeclaracoesRelatorios", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("ExtractToExcelRelatorios")]
        public IActionResult ExtractToExcelRelatorios(RelatorioDeclaracaoRenumeracaoListagemRequest request)
        {
            StringFileReponse response = new StringFileReponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.ExtractToExcelRelatorios(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("ExtractToExcelRelatorios", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}