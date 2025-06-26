using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    public class RecoverRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = true)]
        public string Niss { get; set; }

        [DataMember(IsRequired = true)]
        public string Email { get; set; }
    }

    public class RecoverSetPasswordRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = true)]
        public string Token { get; set; }

        [DataMember(IsRequired = true)]
        public string Username { get; set; }

        [DataMember(IsRequired = true)]
        public string Password { get; set; }

        [DataMember(IsRequired = true)]
        public string ConfirmPassword { get; set; }

        [DataMember]
        public bool UsernameChange { get; set; }
    }

    public class CreateNissInforRequest : RequestBaseDataContract
    {


        [DataMember]
        public string Niss { get; set; }

        [DataMember(IsRequired = true)]
        public string Name { get; set; }

        [DataMember(IsRequired = true)]
        public string Email { get; set; }

        [DataMember(IsRequired = true)]
        public bool InternalUser { get; set; }
    }

    public class ValidTokenRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = true)]
        public string token { get; set; }

        [DataMember(IsRequired = true)]
        public bool isRecover { get; set; }
    }
}