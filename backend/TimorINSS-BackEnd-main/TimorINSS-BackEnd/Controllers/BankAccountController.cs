using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;

namespace TimorINSSBackEnd.Controllers
{
    [Authorize]
    [Route("api/bankaccount")]
    [ApiController]
    public class BankAccountController : ControllerBase
    {
        private readonly IBankAccountDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public BankAccountController(IBankAccountDataManager dataManager)
        {
            _dataManager = dataManager;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            BankAccountListResponse response;
            try
            {
                response = _dataManager.GetAll();
            }
            catch (Exception e)
            {
                response = new BankAccountListResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("GetAll", Log, null))
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("Save")]
        public IActionResult Save(SaveBankAccountRequest request)
        {
            SaveBankAccountResponse response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.Save(request);
            }
            catch (Exception e)
            {
                response = new SaveBankAccountResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("Save", Log, request))
                return BadRequest(response);

            return Ok(response);
        }
    }
}
