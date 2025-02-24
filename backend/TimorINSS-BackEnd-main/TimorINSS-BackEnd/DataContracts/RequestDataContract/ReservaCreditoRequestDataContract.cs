using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class ReservaCreditoListagemRequest : SearchFilterRequest
    {
        [DataMember]
        public int IdEntidade { get; set; }
    }
}