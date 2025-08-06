using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Reflection;
using TimorINSSBackEnd.Cache;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;

namespace TimorINSSBackEnd.Controllers
{
    [Authorize]
    [Route("api/dominios")]
    [ApiController]
    public class DominiosController : ControllerBase
    {
        private readonly IDominioDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        //private readonly ICacheProvider _cache;

        public DominiosController(IDominioDataManager dataManager, ICacheProvider memoryCache)
        {
            _dataManager = dataManager;
            //_cache = memoryCache;
        }

        [HttpGet("GetAllTiposDeContracto/{language}")]
        public IActionResult GetAllTiposDeContracto(string language)
        {
            //DominioDescricaoStringResponse response = _cache.GetFromCache<DominioDescricaoStringResponse>("GetAllTiposDeContracto");
            DominioDescricaoStringResponse response = null;
            if (response == null)
            {
                try
                {
                    response = _dataManager.getAllTiposDeContracto(language);
                }
                catch (Exception e)
                {
                    response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                }
                // Guardar log do erro no ficheiro de logs
                if (response.ManageErrors("GetAllTiposDeContracto", Log, null))
                {
                    return BadRequest(response);
                }
                //else
                //{
                //    _cache.SetCache("GetAllTiposDeContracto", response, response.dominios.Count);
                //}
            }
            return Ok(response);
        }

        [HttpGet("GetAllNaturezasDeContracto/{language}")]
        public IActionResult GetAllNaturezasDeContracto(string language)
        {
            //DominioDescricaoStringResponse response = _cache.GetFromCache<DominioDescricaoStringResponse>("GetAllNaturezasDeContracto");
            DominioDescricaoStringResponse response = null;
            if (response == null)
            {
                try
                {
                    response = _dataManager.getAllNaturezasDeContracto(language);
                }
                catch (Exception e)
                {
                    response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                }
                // Guardar log do erro no ficheiro de logs
                if (response.ManageErrors("GetAllNaturezasDeContracto", Log, null))
                {
                    return BadRequest(response);
                }
                //else
                //{
                //    _cache.SetCache("GetAllNaturezasDeContracto", response, response.dominios.Count);
                //}
            }
            return Ok(response);
        }

        [HttpGet("GetAllLeisLaboraisAplicaveis/{language}")]
        public IActionResult GetAllLeisLaboraisAplicaveis(string language)
        {
            DominioDescricaoStringResponse response = null;
            //DominioDescricaoStringResponse response = _cache.GetFromCache<DominioDescricaoStringResponse>("GetAllLeisLaboraisAplicaveis");
            if (response == null)
            {
                try
                {
                    response = _dataManager.getAllLeisLaboraisAplicaveis(language);
                }
                catch (Exception e)
                {
                    response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                }
                // Guardar log do erro no ficheiro de logs
                if (response.ManageErrors("GetAllLeisLaboraisAplicaveis", Log, null))
                {
                    return BadRequest(response);
                }
                //else
                //{
                //    _cache.SetCache("GetAllLeisLaboraisAplicaveis", response, response.dominios.Count);
                //}
            }
            return Ok(response);
        }

        [HttpGet("GetAllTiposDeDocumento/{language}")]
        public IActionResult GetAllTiposDeDocumento(string language)
        {
            DominioDescricaoStringResponse response = null;
            //DominioDescricaoStringResponse response = _cache.GetFromCache<DominioDescricaoStringResponse>("GetAllTiposDeDocumento");
            if (response == null)
            {
                try
                {
                    response = _dataManager.GetAllTiposDeDocumento(language);
                }
                catch (Exception e)
                {
                    response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                }
                // Guardar log do erro no ficheiro de logs
                if (response.ManageErrors("GetAllTiposDeDocumento", Log, null))
                {
                    return BadRequest(response);
                }
                //else
                //{
                //    _cache.SetCache("GetAllTiposDeDocumento", response, response.dominios.Count);
                //}
            }
            return Ok(response);
        }

        [HttpGet("GetAllSexos/{language}")]
        public IActionResult GetAllSexos(string language)
        {
            //DominioDescricaoStringResponse response = _cache.GetFromCache<DominioDescricaoStringResponse>("GetAllSexos");
            DominioDescricaoStringResponse response = null;
            if (response == null)
            {
                try
                {
                    response = _dataManager.GetAllSexos(language);
                }
                catch (Exception e)
                {
                    response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                }
                // Guardar log do erro no ficheiro de logs
                if (response.ManageErrors("GetAllSexos", Log, null))
                {
                    return BadRequest(response);
                }
                //else
                //{
                //    _cache.SetCache("GetAllSexos", response, response.dominios.Count);
                //}
            }
            return Ok(response);
        }

        [HttpGet("GetAllEstadosCivis/{language}")]
        public IActionResult getAllEstadosCivis(string language)
        {
            //DominioDescricaoStringResponse response = _cache.GetFromCache<DominioDescricaoStringResponse>("GetAllEstadosCivis");
            DominioDescricaoStringResponse response = null;
            if (response == null)
            {
                try
                {
                    response = _dataManager.getAllEstadosCivis(language);
                }
                catch (Exception e)
                {
                    response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                }
                // Guardar log do erro no ficheiro de logs
                if (response.ManageErrors("GetAllEstadosCivis", Log, null))
                {
                    return BadRequest(response);
                }
                //else
                //{
                //    _cache.SetCache("GetAllEstadosCivis", response, response.dominios.Count);
                //}
            }
            return Ok(response);
        }

        [HttpGet("GetAllNacionalidades/{language}")]
        public IActionResult GetAllNacionalidades(string language)
        {
            DominioDescricaoStringResponse response = null;
            //DominioDescricaoStringResponse response = _cache.GetFromCache<DominioDescricaoStringResponse>("GetAllNacionalidades");
            if (response == null)
            {
                try
                {
                    response = _dataManager.GetAllNacionalidades(language);
                }
                catch (Exception e)
                {
                    response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                }
                // Guardar log do erro no ficheiro de logs
                if (response.ManageErrors("GetAllNacionalidades", Log, null))
                {
                    return BadRequest(response);
                }
                //else
                //{
                //    _cache.SetCache("GetAllNacionalidades", response, response.dominios.Count);
                //}
            }
            return Ok(response);
        }

        [HttpGet("GetAllTipoDivida/{language}")]
        public IActionResult GetAllTipoDivida(string language)
        {
            DominioDescricaoStringResponse response = null;
            //DominioDescricaoStringResponse response = _cache.GetFromCache<DominioDescricaoStringResponse>("GetAllTipoDivida");
            if (response == null)
            {
                try
                {
                    response = _dataManager.GetAllTipoDivida(language);
                }
                catch (Exception e)
                {
                    response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                }
                // Guardar log do erro no ficheiro de logs
                if (response.ManageErrors("GetAllTipoDivida", Log, null))
                {
                    return BadRequest(response);
                }
                //else
                //{
                //    _cache.SetCache("GetAllTipoDivida", response, response.dominios.Count);
                //}
            }
            return Ok(response);
        }

        [HttpGet("GetAllSituacaoPagamento/{language}")]
        public IActionResult GetAllSituacaoPagamento(string language)
        {
            DominioDescricaoStringResponse response = null;
            //DominioDescricaoStringResponse response = _cache.GetFromCache<DominioDescricaoStringResponse>("GetAllSituacaoPagamento");
            if (response == null)
            {
                try
                {
                    response = _dataManager.GetAllSituacaoPagamento(language);
                }
                catch (Exception e)
                {
                    response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                }
                // Guardar log do erro no ficheiro de logs
                if (response.ManageErrors("GetAllSituacaoPagamento", Log, null))
                {
                    return BadRequest(response);
                }
                //else
                //{
                //    _cache.SetCache("GetAllSituacaoPagamento", response, response.dominios.Count);
                //}
            }
            return Ok(response);
        }

        [HttpGet("GetAllRegimes/{language}")]
        public IActionResult GetAllRegimes(string language)
        {
            //ListagemRegimesResponse response = _cache.GetFromCache<ListagemRegimesResponse>("GetAllRegimes");
            ListagemRegimesResponse response = null;
            if (response == null)
            {
                try
                {
                    response = _dataManager.GetAllRegimes(language);
                }
                catch (Exception e)
                {
                    response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                }
                // Guardar log do erro no ficheiro de logs
                if (response.ManageErrors("GetAllRegimes", Log, null))
                {
                    return BadRequest(response);
                }
                //else
                //{
                //    _cache.SetCache("GetAllRegimes", response, response.regimes.Count);
                //}
            }
            return Ok(response);
        }

        [HttpGet("GetSalarioMinimo/{language}")]
        public IActionResult GetSalarioMinimo(string language)
        {
            SingleDominioDescricaoStringResponse response = null;
            //SingleDominioDescricaoStringResponse response = _cache.GetFromCache<SingleDominioDescricaoStringResponse>("GetSalarioMinimo");
            if (response == null)
            {
                try
                {
                    response = _dataManager.getSalarioMinimo(language);
                }
                catch (Exception e)
                {
                    response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                }
                // Guardar log do erro no ficheiro de logs
                if (response.ManageErrors("GetSalarioMinimo", Log, null))
                {
                    return BadRequest(response);
                }
                //else
                //{
                //    _cache.SetCache("GetSalarioMinimo", response, 1);
                //}
            }
            return Ok(response);
        }

        [HttpGet("GetDeclarationDay/{language}")]
        public IActionResult GetDeclarationDay(string language)
        {
            //SingleDominioDescricaoStringResponse response = _cache.GetFromCache<SingleDominioDescricaoStringResponse>("GetDeclarationDay");
            SingleDominioDescricaoStringResponse response = null;
            if (response == null)
            {
                try
                {
                    response = _dataManager.GetDeclarationDay(language);
                }
                catch (Exception e)
                {
                    response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                }
                // Guardar log do erro no ficheiro de logs
                if (response.ManageErrors("GetDeclarationDay", Log, null))
                {
                    return BadRequest(response);
                }
                //else
                //{
                //    _cache.SetCache("GetDeclarationDay", response, 1);
                //}
            }
            return Ok(response);
        }

        [HttpGet("GetAllTiposPagamento/{language}")]
        public IActionResult GetAllTiposPagamento(string language)
        {
            //DominioDescricaoStringResponse response = _cache.GetFromCache<DominioDescricaoStringResponse>("GetAllTiposPagamento");
            DominioDescricaoStringResponse response = null;
            if (response == null)
            {
                try
                {
                    response = _dataManager.getTipoPago(language);
                }
                catch (Exception e)
                {
                    response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                }
                // Guardar log do erro no ficheiro de logs
                if (response.ManageErrors("GetAllTiposPagamento", Log, null))
                {
                    return BadRequest(response);
                }
                //else
                //{
                //    _cache.SetCache("GetAllTiposPagamento", response, 1);
                //}
            }
            return Ok(response);
        }

        [HttpGet("GetAllTiposGuia/{language}")]
        public IActionResult GetAllTiposGuia(string language)
        {
            //DominioDescricaoStringResponse response = _cache.GetFromCache<DominioDescricaoStringResponse>("GetAllTiposGuia");
            DominioDescricaoStringResponse response = null;
            if (response == null)
            {
                try
                {
                    response = _dataManager.getTipoGuia(language);
                }
                catch (Exception e)
                {
                    response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                }
                // Guardar log do erro no ficheiro de logs
                if (response.ManageErrors("GetAllTiposGuia", Log, null))
                {
                    return BadRequest(response);
                }
                //else
                //{
                //    _cache.SetCache("GetAllTiposGuia", response, 1);
                //}
            }
            return Ok(response);
        }

        [HttpGet("GetAllProfissoes/{language}")]
        public IActionResult GetAllProfissoes(string language)
        {
            DominioDescricaoStringResponse response = null;
            //DominioDescricaoStringResponse response = _cache.GetFromCache<DominioDescricaoStringResponse>("GetAllProfissoes");
            if (response == null)
            {
                try
                {
                    response = _dataManager.GetAllProfissoes(language);
                }
                catch (Exception e)
                {
                    response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                }
                // Guardar log do erro no ficheiro de logs
                if (response.ManageErrors("GetAllProfissoes", Log, null))
                {
                    return BadRequest(response);
                }
                //else
                //{
                //    _cache.SetCache("GetAllProfissoes", response, response.dominios.Count);
                //}
            }
            return Ok(response);
        }

        [HttpGet("GetAllFuncoes/{language}")]
        public IActionResult GetAllFuncoes(string language)
        {
            DominioDescricaoStringResponse response = null;
            //DominioDescricaoStringResponse response = _cache.GetFromCache<DominioDescricaoStringResponse>("GetAllFuncoes");
            if (response == null)
            {
                try
                {
                    response = _dataManager.GetAllFuncoes(language);
                }
                catch (Exception e)
                {
                    response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                }
                // Guardar log do erro no ficheiro de logs
                if (response.ManageErrors("GetAllFuncoes", Log, null))
                {
                    return BadRequest(response);
                }
                //else
                //{
                //    _cache.SetCache("GetAllFuncoes", response, response.dominios.Count);
                //}
            }
            return Ok(response);
        }

        [HttpGet("GetAllGruposCamposEditaveis/{language}")]
        public IActionResult GetAllGruposCamposEditaveis(string language)
        {
            //DominioDescricaoStringResponse response = _cache.GetFromCache<DominioDescricaoStringResponse>("GetAllGruposCamposEditaveis");
            DominioDescricaoStringResponse response = null;
            if (response == null)
            {
                try
                {
                    response = _dataManager.GetAllGruposCamposEditaveis(language);
                }
                catch (Exception e)
                {
                    response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                }
                // Guardar log do erro no ficheiro de logs
                if (response.ManageErrors("GetAllGruposCamposEditaveis", Log, null))
                {
                    return BadRequest(response);
                }
                //else
                //{
                //    _cache.SetCache("GetAllGruposCamposEditaveis", response, 1);
                //}
            }
            return Ok(response);
        }

        [HttpGet("GetAllTiposDeRegime/{language}")]
        public IActionResult GetAllTiposDeRegime(string language)
        {
            //DominioDescricaoStringResponse response = _cache.GetFromCache<DominioDescricaoStringResponse>("GetAllTiposDeRegime");
            DominioDescricaoStringResponse response = null;
            if (response == null)
            {
                try
                {
                    response = _dataManager.GetAllTiposDeRegime(language);
                }
                catch (Exception e)
                {
                    response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                }
                // Guardar log do erro no ficheiro de logs
                if (response.ManageErrors("GetAllTiposDeRegime", Log, null))
                {
                    return BadRequest(response);
                }
                //else
                //{
                //    _cache.SetCache("GetAllTiposDeRegime", response, response.dominios.Count);
                //}
            }
            return Ok(response);
        }

        [HttpGet("GetAllTiposDeDocumentoTarefa/{language}")]
        public IActionResult GetAllTiposDeDocumentoTarefa(string language)
        {
            DominioDescricaoStringResponse response = null;
            //DominioDescricaoStringResponse response = _cache.GetFromCache<DominioDescricaoStringResponse>("GetAllTiposDeDocumentoTarefa");
            if (response == null)
            {
                try
                {
                    response = _dataManager.GetAllTiposDeDocumentoTarefa(language);
                    response.dominios = response.dominios.OrderBy(x => x.descricao).ToList();
                }
                catch (Exception e)
                {
                    response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                }
                // Guardar log do erro no ficheiro de logs
                if (response.ManageErrors("GetAllTiposDeDocumentoTarefa", Log, null))
                {
                    return BadRequest(response);
                }
                //else
                //{
                //    _cache.SetCache("GetAllTiposDeDocumentoTarefa", response, response.dominios.Count);
                //}
            }
            return Ok(response);
        }

        [HttpPost("GetTiposDocumentoPorTarefaAtiva")]
        public IActionResult GetTiposDocumentoPorTarefaAtiva(GetTiposDocumentoPorTarefaAtivaRequest request)
        {
            DominiosComGruposResponse response = new DominiosComGruposResponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetTiposDocumentoPorTarefaAtiva(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetAllTarefasAtivas", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("getAllCaixas/{language}")]
        public IActionResult getAllCaixas(string language)
        {
            DominioDescricaoStringResponse response = new DominioDescricaoStringResponse();

            try
            {
                response = _dataManager.getAllCaixas(language);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("getAllCaixas", Log, null))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("getAllMovimentosTypes/{language}")]
        public IActionResult getAllMovimentosTypes(string language)
        {
            DominioDescricaoStringResponse response = new DominioDescricaoStringResponse();

            try
            {
                response = _dataManager.getAllMovimentosTypes(language);
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

        [HttpGet("GetAllTiposConta/{language}")]
        public IActionResult GetAllTiposConta(string language)
        {
            DominioDescricaoStringResponse response = new DominioDescricaoStringResponse();

            try
            {
                response = _dataManager.GetAllTiposConta(language);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetAllTiposConta", Log, null))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("GetAllEstadosPagamento/{language}")]
        public IActionResult GetAllEstadosPagamento(string language)
        {
            DominioDescricaoStringResponse response = new DominioDescricaoStringResponse();

            try
            {
                response = _dataManager.GetAllEstadosPagamento(language);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetAllEstadosPagamento", Log, null))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}