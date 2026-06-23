using System;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class RelatorioFornecedoresDataContract
    {
        [DataMember]
        public string niss { get; set; }

        [DataMember]
        public string tin { get; set; }

        [DataMember]
        public string nome { get; set; }

        [DataMember]
        public string pagamento { get; set; }

        [DataMember]
        public string descricao { get; set; }

        [DataMember]
        public decimal valor { get; set; }

        [DataMember]
        public DateTime? data { get; set; }

        [DataMember]
        public bool conciliado { get; set; }
    }
}