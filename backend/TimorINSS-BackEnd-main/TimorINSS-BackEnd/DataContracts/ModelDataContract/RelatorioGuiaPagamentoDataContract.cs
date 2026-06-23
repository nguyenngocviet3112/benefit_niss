using System;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class RelatorioGuiaPagamentoDataContract
    {
        [DataMember]
        public string numeroGuia { get; set; }

        [DataMember]
        public string empregador { get; set; }

        [DataMember]
        public DateTime periodo { get; set; }

        [DataMember]
        public string estado { get; set; }

        [DataMember]
        public decimal valorGuia { get; set; }

        [DataMember]
        public decimal valorComprovativo { get; set; }

        [DataMember]
        public byte[] pdf { get; set; }

        [DataMember]
        public decimal valorDivida { get; set; }
    }
}