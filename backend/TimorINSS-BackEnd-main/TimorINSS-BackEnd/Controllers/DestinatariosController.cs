using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    [Route("api/destinatario")]
    [ApiController]
    public class DestinatariosController : ControllerBase
    {
        private readonly IDestinatarioDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICacheProvider _cache;

        public DestinatariosController(IDestinatarioDataManager dataManager, ICacheProvider memoryCache)
        {
            _dataManager = dataManager;
            _cache = memoryCache;
        }

        [HttpPost("GetDestinatarioByNissTin")]
        public IActionResult GetDestinatarioByNissTin(GetDestinatarioRequest request)
        {
            GetDestinatarioResponse response = new GetDestinatarioResponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetDestinatarioByNissTin(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetDestinatarioByNissTin", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("SaveDestinatario")]
        public IActionResult SaveDestinatario(SaveDestinatarioRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.SaveDestinatario(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("SaveDestinatario", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("ImportDestinatarios")]
        [RequestSizeLimit(100000000)]
        public IActionResult ImportDestinatarios([FromForm] ExcelImporterRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.ImportDestinatarios(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("ImportDestinatarios", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }


        //[HttpPost("teste")]
        //[AllowAnonymous]
        //public IActionResult teste([FromForm] IFormFile file)
        //{

        //    var data = ExcelReaderService.ExcelReader.Read<ExcelReaderService.Models.Teste>(file.OpenReadStream());

        //    return Ok(data);
        //}
    }
}