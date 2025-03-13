using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using TimorINSSBackEnd.Cache;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;

namespace TimorINSSBackEnd.Controllers
{
    [Authorize]
    [Route("api/khachHang")]
    [ApiController]
    public class KhachHangController : ControllerBase
    {

        
        private readonly IKhachHangDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICacheProvider _cache;

        public KhachHangController(IKhachHangDataManager dataManager, ICacheProvider memoryCache)
        {
            _dataManager = dataManager;
            _cache = memoryCache;
        }

        


        [HttpGet("getKhachHang")]
        public IActionResult getKhachHang()
        {
            SelectKhachHangResponse response = _cache.GetFromCache<SelectKhachHangResponse>("getKhachHang");
            if (response == null)
            {
                try
                {
                    response = _dataManager.GetAllKhachHang();
                }
                catch (Exception e)
                {
                    response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                }
                // Guardar log do erro no ficheiro de logs
                if (response.ManageErrors("getKhachHang", Log, null))
                {
                    return BadRequest(response);
                }
                else
                {
                    _cache.SetCache("getKhachHang", response, response.selects.Count);
                }
            }
            return Ok(response);
        }
    }




}