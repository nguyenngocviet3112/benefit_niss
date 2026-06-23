using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    public class LoginRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = true)]
        public string Username { get; set; }

        [DataMember(IsRequired = true)]
        public string Password { get; set; }
    }
}