using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class ReservaCreditoListagemResponse : ResponseBaseDataContract
    {
        [DataMember]
        public int rows { get; set; }

        [DataMember]
        public List<ReservaCreditoListagem> ReservaCredito { get; set; }
    }

    [DataContract]
    public class ReservaCreditoListagem
    {
        [DataMember]
        public int IdEntidade { get; set; }

        [DataMember]
        public decimal? Valor { get; set; }

        [DataMember]
        public bool IndAtivo { get; set; }
    }
}