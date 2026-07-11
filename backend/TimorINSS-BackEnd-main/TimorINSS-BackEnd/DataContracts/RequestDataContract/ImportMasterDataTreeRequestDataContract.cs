using Microsoft.AspNetCore.Http;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class ImportMasterDataTreeRequest : RequestBaseDataContract
    {
        public IFormFile File { get; set; }
        public int OrcamentoConfigFk { get; set; }
    }
}
