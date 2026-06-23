using System;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class RelatorioPagamentoDataContract
    {
        [DataMember]
        public string numeroPagamento { get; set; }

        [DataMember]
        public string destinatario { get; set; }

        [DataMember]
        public string conta { get; set; }

        [DataMember]
        public string iban { get; set; }

        [DataMember]
        public DateTime dataEmissao { get; set; }

        [DataMember]
        public string estado { get; set; }

        [DataMember]
        public decimal valor { get; set; }

        [DataMember]
        public int countDestinatarios { get; set; }

        [DataMember]
        public int countContas { get; set; }
    }
}