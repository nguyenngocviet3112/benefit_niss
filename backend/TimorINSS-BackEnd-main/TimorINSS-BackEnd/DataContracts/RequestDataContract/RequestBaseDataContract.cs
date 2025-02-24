using Microsoft.AspNetCore.Http;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    public class RequestBaseDataContract
    {
        public string? RequestId { get; set; }

        public int UserId { get; set; }

        public void GetHeaderInfo(IHeaderDictionary header)
        {
            // Parse dos valores do header para o request
            header.TryGetValue("Request-Id", out var requestId);
            header.TryGetValue("User-Id", out var userId);
            this.RequestId = requestId;
            int.TryParse(userId, out var userIdNum);
            this.UserId = userIdNum;
        }
    }
}