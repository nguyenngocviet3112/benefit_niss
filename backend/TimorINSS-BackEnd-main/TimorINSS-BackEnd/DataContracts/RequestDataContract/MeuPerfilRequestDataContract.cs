using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class GetMeuPerfilRequest : RequestBaseDataContract
    {
    }

    [DataContract]
    public class AlterarEmailRequest : RequestBaseDataContract
    {
        [DataMember]
        public string Email { get; set; }
    }

    [DataContract]
    public class AlterarSenhaRequest : RequestBaseDataContract
    {
        [DataMember]
        public string SenhaAtual { get; set; }

        [DataMember]
        public string SenhaNova { get; set; }

        [DataMember]
        public string ConfirmarSenhaNova { get; set; }
    }
}
