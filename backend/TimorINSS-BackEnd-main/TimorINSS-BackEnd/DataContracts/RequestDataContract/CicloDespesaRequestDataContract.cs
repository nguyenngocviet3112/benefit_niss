using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class GetCicloDespesaListRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Ano { get; set; }

        // null = todas as Organizations (Both)
        [DataMember]
        public int? Institution { get; set; }
    }
}
