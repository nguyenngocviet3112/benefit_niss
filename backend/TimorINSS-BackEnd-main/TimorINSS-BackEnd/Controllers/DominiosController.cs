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
        private readonly ICacheProvider _cache;

        public DominiosController(IDominioDataManager dataManager, ICacheProvider memoryCache)
        {
            _dataManager = dataManager;
            _cache = memoryCache;
        }

        [HttpGet("GetAllTiposDeContracto")]
        public IActionResult GetAllTiposDeContracto()
        {
            DominioDescricaoStringResponse response = _cache.GetFromCache<DominioDescricaoStringResponse>("GetAllTiposDeContracto");
            if (response == null)
            {
                try
                {
                    response = _dataManager.getAllTiposDeContracto();
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
                else
                {
                    _cache.SetCache("GetAllTiposDeContracto", response, response.dominios.Count);
                }
            }
            return Ok(response);
        }

        [HttpGet("GetAllNaturezasDeContracto")]
        public IActionResult GetAllNaturezasDeContracto()
        {
            DominioDescricaoStringResponse response = _cache.GetFromCache<DominioDescricaoStringResponse>("GetAllNaturezasDeContracto");
            if (response == null)
            {
                try
                {
                    response = _dataManager.getAllNaturezasDeContracto();
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
                else
                {
                    _cache.SetCache("GetAllNaturezasDeContracto", response, response.dominios.Count);
                }
            }
            return Ok(response);
        }

        [HttpGet("GetAllLeisLaboraisAplicaveis")]
        public IActionResult GetAllLeisLaboraisAplicaveis()
        {
            DominioDescricaoStringResponse response = _cache.GetFromCache<DominioDescricaoStringResponse>("GetAllLeisLaboraisAplicaveis");
            if (response == null)
            {
                try
                {
                    response = _dataManager.getAllLeisLaboraisAplicaveis();
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
                else
                {
                    _cache.SetCache("GetAllLeisLaboraisAplicaveis", response, response.dominios.Count);
                }
            }
            return Ok(response);
        }

        [HttpGet("GetAllTiposDeDocumento")]
        public IActionResult GetAllTiposDeDocumento()
        {
            DominioDescricaoStringResponse response = _cache.GetFromCache<DominioDescricaoStringResponse>("GetAllTiposDeDocumento");
            if (response == null)
            {
                try
                {
                    response = _dataManager.GetAllTiposDeDocumento();
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
                else
                {
                    _cache.SetCache("GetAllTiposDeDocumento", response, response.dominios.Count);
                }
            }
            return Ok(response);
        }

        [HttpGet("GetAllSexos")]
        public IActionResult GetAllSexos()
        {
            DominioDescricaoStringResponse response = _cache.GetFromCache<DominioDescricaoStringResponse>("GetAllSexos");
            if (response == null)
            {
                try
                {
                    response = _dataManager.GetAllSexos();
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
                else
                {
                    _cache.SetCache("GetAllSexos", response, response.dominios.Count);
                }
            }
            return Ok(response);
        }

        [HttpGet("GetAllEstadosCivis")]
        public IActionResult getAllEstadosCivis()
        {
            DominioDescricaoStringResponse response = _cache.GetFromCache<DominioDescricaoStringResponse>("GetAllEstadosCivis");
            if (response == null)
            {
                try
                {
                    response = _dataManager.getAllEstadosCivis();
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
                else
                {
                    _cache.SetCache("GetAllEstadosCivis", response, response.dominios.Count);
                }
            }
            return Ok(response);
        }

        [HttpGet("GetAllNacionalidades")]
        public IActionResult GetAllNacionalidades()
        {
            DominioDescricaoStringResponse response = _cache.GetFromCache<DominioDescricaoStringResponse>("GetAllNacionalidades");
            if (response == null)
            {
                try
                {
                    response = _dataManager.GetAllNacionalidades();
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
                else
                {
                    _cache.SetCache("GetAllNacionalidades", response, response.dominios.Count);
                }
            }
            return Ok(response);
        }

        [HttpGet("GetAllTipoDivida")]
        public IActionResult GetAllTipoDivida()
        {
            DominioDescricaoStringResponse response = _cache.GetFromCache<DominioDescricaoStringResponse>("GetAllTipoDivida");
            if (response == null)
            {
                try
                {
                    response = _dataManager.GetAllTipoDivida();
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
                else
                {
                    _cache.SetCache("GetAllTipoDivida", response, response.dominios.Count);
                }
            }
            return Ok(response);
        }

        [HttpGet("GetAllSituacaoPagamento")]
        public IActionResult GetAllSituacaoPagamento()
        {
            DominioDescricaoStringResponse response = _cache.GetFromCache<DominioDescricaoStringResponse>("GetAllSituacaoPagamento");
            if (response == null)
            {
                try
                {
                    response = _dataManager.GetAllSituacaoPagamento();
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
                else
                {
                    _cache.SetCache("GetAllSituacaoPagamento", response, response.dominios.Count);
                }
            }
            return Ok(response);
        }

        [HttpGet("GetAllRegimes")]
        public IActionResult GetAllRegimes()
        {
            ListagemRegimesResponse response = _cache.GetFromCache<ListagemRegimesResponse>("GetAllRegimes");
            if (response == null)
            {
                try
                {
                    response = _dataManager.GetAllRegimes();
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
                else
                {
                    _cache.SetCache("GetAllRegimes", response, response.regimes.Count);
                }
            }
            return Ok(response);
        }

        [HttpGet("GetSalarioMinimo")]
        public IActionResult GetSalarioMinimo()
        {
            SingleDominioDescricaoStringResponse response = _cache.GetFromCache<SingleDominioDescricaoStringResponse>("GetSalarioMinimo");
            if (response == null)
            {
                try
                {
                    response = _dataManager.getSalarioMinimo();
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
                else
                {
                    _cache.SetCache("GetSalarioMinimo", response, 1);
                }
            }
            return Ok(response);
        }

        [HttpGet("GetDeclarationDay")]
        public IActionResult GetDeclarationDay()
        {
            SingleDominioDescricaoStringResponse response = _cache.GetFromCache<SingleDominioDescricaoStringResponse>("GetDeclarationDay");
            if (response == null)
            {
                try
                {
                    response = _dataManager.GetDeclarationDay();
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
                else
                {
                    _cache.SetCache("GetDeclarationDay", response, 1);
                }
            }
            return Ok(response);
        }

        [HttpGet("GetAllTiposPagamento")]
        public IActionResult GetAllTiposPagamento()
        {
            DominioDescricaoStringResponse response = _cache.GetFromCache<DominioDescricaoStringResponse>("GetAllTiposPagamento");
            if (response == null)
            {
                try
                {
                    response = _dataManager.getTipoPago();
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
                else
                {
                    _cache.SetCache("GetAllTiposPagamento", response, 1);
                }
            }
            return Ok(response);
        }

        [HttpGet("GetAllTiposGuia")]
        public IActionResult GetAllTiposGuia()
        {
            DominioDescricaoStringResponse response = _cache.GetFromCache<DominioDescricaoStringResponse>("GetAllTiposGuia");
            if (response == null)
            {
                try
                {
                    response = _dataManager.getTipoGuia();
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
                else
                {
                    _cache.SetCache("GetAllTiposGuia", response, 1);
                }
            }
            return Ok(response);
        }

        [HttpGet("GetAllProfissoes")]
        public IActionResult GetAllProfissoes()
        {
            DominioDescricaoStringResponse response = _cache.GetFromCache<DominioDescricaoStringResponse>("GetAllProfissoes");
            if (response == null)
            {
                try
                {
                    response = _dataManager.GetAllProfissoes();
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
                else
                {
                    _cache.SetCache("GetAllProfissoes", response, response.dominios.Count);
                }
            }
            return Ok(response);
        }

        [HttpGet("GetAllFuncoes")]
        public IActionResult GetAllFuncoes()
        {
            DominioDescricaoStringResponse response = _cache.GetFromCache<DominioDescricaoStringResponse>("GetAllFuncoes");
            if (response == null)
            {
                try
                {
                    response = _dataManager.GetAllFuncoes();
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
                else
                {
                    _cache.SetCache("GetAllFuncoes", response, response.dominios.Count);
                }
            }
            return Ok(response);
        }

        [HttpGet("GetAllGruposCamposEditaveis")]
        public IActionResult GetAllGruposCamposEditaveis()
        {
            DominioDescricaoStringResponse response = _cache.GetFromCache<DominioDescricaoStringResponse>("GetAllGruposCamposEditaveis");
            if (response == null)
            {
                try
                {
                    response = _dataManager.GetAllGruposCamposEditaveis();
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
                else
                {
                    _cache.SetCache("GetAllGruposCamposEditaveis", response, 1);
                }
            }
            return Ok(response);
        }

        [HttpGet("GetAllTiposDeRegime")]
        public IActionResult GetAllTiposDeRegime()
        {
            DominioDescricaoStringResponse response = _cache.GetFromCache<DominioDescricaoStringResponse>("GetAllTiposDeRegime");
            if (response == null)
            {
                try
                {
                    response = _dataManager.GetAllTiposDeRegime();
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
                else
                {
                    _cache.SetCache("GetAllTiposDeRegime", response, response.dominios.Count);
                }
            }
            return Ok(response);
        }

        [HttpGet("GetAllTiposDeDocumentoTarefa")]
        public IActionResult GetAllTiposDeDocumentoTarefa()
        {
            DominioDescricaoStringResponse response = _cache.GetFromCache<DominioDescricaoStringResponse>("GetAllTiposDeDocumentoTarefa");
            if (response == null)
            {
                try
                {
                    response = _dataManager.GetAllTiposDeDocumentoTarefa();
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
                else
                {
                    _cache.SetCache("GetAllTiposDeDocumentoTarefa", response, response.dominios.Count);
                }
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

        [HttpGet("getAllCaixas")]
        public IActionResult getAllCaixas()
        {
            DominioDescricaoStringResponse response = new DominioDescricaoStringResponse();

            try
            {
                response = _dataManager.getAllCaixas();
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

        [HttpGet("getAllMovimentosTypes")]
        public IActionResult getAllMovimentosTypes()
        {
            DominioDescricaoStringResponse response = new DominioDescricaoStringResponse();

            try
            {
                response = _dataManager.getAllMovimentosTypes();
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

        [HttpGet("GetAllTiposConta")]
        public IActionResult GetAllTiposConta()
        {
            DominioDescricaoStringResponse response = new DominioDescricaoStringResponse();

            try
            {
                response = _dataManager.GetAllTiposConta();
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

        [HttpGet("GetAllEstadosPagamento")]
        public IActionResult GetAllEstadosPagamento()
        {
            DominioDescricaoStringResponse response = new DominioDescricaoStringResponse();

            try
            {
                response = _dataManager.GetAllEstadosPagamento();
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