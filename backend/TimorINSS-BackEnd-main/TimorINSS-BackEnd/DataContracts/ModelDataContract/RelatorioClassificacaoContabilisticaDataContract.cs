using System;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class RelatorioClassificacaoContabilisticaDataContract
    {
        [DataMember]
        public DateTime? data { get; set; }

        [DataMember]
        public string credito { get; set; }

        [DataMember]
        public string debito { get; set; }

        [DataMember]
        public decimal valor { get; set; }

        [DataMember]
        public string numeroPagamento { get; set; }

        [DataMember]
        public string nomeDestinatario { get; set; }

        [DataMember]
        public string niss { get; set; }

        [DataMember]
        public string tin { get; set; }

        [DataMember]
        public bool isExecucao { get; set; }
    }
}