using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class GetLancamentoListRequest : RequestBaseDataContract
    {
        [DataMember]
        public int? Ano { get; set; }

        [DataMember]
        public int? Mes { get; set; }

        [DataMember]
        public string OrigemTipo { get; set; }
    }
}
