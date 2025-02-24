using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class MoradaListagemRequest : SearchFilterRequest
    {
        [DataMember]
        public int Id { get; set; }
    }

    public class MoradaRequest : RequestBaseDataContract
    {
        [DataMember]
        public MoradaDataContract Morada { get; set; }
    }

    [DataContract]
    public class MoradaDeleteRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = true)]
        public int Id { get; set; }
    }
}