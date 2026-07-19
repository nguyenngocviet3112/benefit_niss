using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using TimorINSSBackEnd.Cache;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;

namespace TimorINSSBackEnd.Controllers
{
    [Authorize]
    [Route("api/componenteReceitaRegisto")]
    [ApiController]
    public class ComponentesReceitaRegistoController : ControllerBase
    {
        private readonly IComponenteReceitaRegistoDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICacheProvider _cache;

        public ComponentesReceitaRegistoController(IComponenteReceitaRegistoDataManager dataManager, ICacheProvider memoryCache)
        {
            _dataManager = dataManager;
            _cache = memoryCache;
        }

        [HttpPost("AddEditComponenteReceitaRegisto")]
        public IActionResult AddEditComponenteReceitaRegisto(RegistoReceitaRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.AddEditComponenteReceitaRegisto(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("AddEditComponenteReceitaRegisto", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetComponenteReceitaRegistoByContaOSSId")]
        public IActionResult GetComponenteReceitaRegistoByContaOSSId(GetComponenteReceitaRegistoByIdContaOSSRequest request)
        {
            ComponentesReceitaRegistoResponseDataContract response = new ComponentesReceitaRegistoResponseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetComponenteReceitaRegistoByContaOSSId(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetComponenteReceitaRegistoByContaOSSId", Log, null))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("DeleteReceita")]
        public IActionResult DeleteReceita(DeleteReceitaRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.DeleteReceita(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("DeleteReceita", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetExecucaoOrcamental")]
        public IActionResult GetExecucaoOrcamental(RelatorioExecucaoOrcamentalListagemRequest request)
        {
            ExecucaoOrcamentalListagemResponse response = new ExecucaoOrcamentalListagemResponse();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetExecucaoOrcamental(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetExecucaoOrcamental", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetExecucaoOrcamentalPorClassificacaoEconomica")]
        public IActionResult GetExecucaoOrcamentalPorClassificacaoEconomica(RelatorioClassificacaoEconomicaRequest request)
        {
            ClassificacaoEconomicaExecucaoListagemResponse response = new ClassificacaoEconomicaExecucaoListagemResponse();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetExecucaoOrcamentalPorClassificacaoEconomica(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetExecucaoOrcamentalPorClassificacaoEconomica", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetExecucaoOrcamentalExcel")]
        public IActionResult GetExecucaoOrcamentalExcel(RelatorioExecucaoOrcamentalListagemRequest request)
        {
            StringFileReponse response = new StringFileReponse();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetExecucaoOrcamentalExcel(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetExecucaoOrcamentalExcel", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }


        [HttpPost("ReceitasRelatorios")]
        public IActionResult ReceitasRelatorios(SearchFilterRequest request)
        {
            GetDespesasRelatoriosReponse response = new GetDespesasRelatoriosReponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.ReceitasRelatorios(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("ReceitasRelatorios", Log, null))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("ReceitasRelatoriosExcel")]
        public IActionResult ReceitasRelatoriosExcel(SearchFilterRequest request)
        {
            StringFileReponse response = new StringFileReponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.ReceitasRelatoriosExcel(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("ReceitasRelatoriosExcel", Log, null))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("ReceitasNaoConciliadasRelatorios")]
        public IActionResult ReceitasNaoConciliadasRelatorios(ReceitasNaoConciliadasRelatoriosRequest request)
        {
            GetReceitasNaoConciliadasRelatoriosReponse response = new GetReceitasNaoConciliadasRelatoriosReponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.ReceitasNaoConciliadasRelatorios(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("ReceitasNaoConciliadasRelatorios", Log, null))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("ReceitasNaoConciliadasRelatoriosExcel")]
        public IActionResult ReceitasNaoConciliadasRelatoriosExcel(ReceitasNaoConciliadasRelatoriosRequest request)
        {
            StringFileReponse response = new StringFileReponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.ReceitasNaoConciliadasRelatoriosExcel(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("ReceitasNaoConciliadasRelatoriosExcel", Log, null))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}