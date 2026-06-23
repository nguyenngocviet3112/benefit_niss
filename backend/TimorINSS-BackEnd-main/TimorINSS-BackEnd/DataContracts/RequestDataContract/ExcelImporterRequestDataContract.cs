using Microsoft.AspNetCore.Http;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class ExcelImporterRequest : RequestBaseDataContract
    {
        public IFormFile File { get; set; }
    }
}