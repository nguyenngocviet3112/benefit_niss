using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class ContatoListagemRequest : SearchFilterRequest
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string IdStr { get; set; }

    }

    public class ContatoRequest : RequestBaseDataContract
    {
        [DataMember]
        public ContactoDataContract Contato { get; set; }
    }

    [DataContract]
    public class ContatoDeleteRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = true)]
        public int Id { get; set; }
        [DataMember(IsRequired = true)]
        public string IdStr { get; set; }
    }
}