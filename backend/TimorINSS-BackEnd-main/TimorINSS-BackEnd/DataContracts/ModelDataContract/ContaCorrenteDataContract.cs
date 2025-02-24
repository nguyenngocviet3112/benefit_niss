using System;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class ContaCorrenteDataContract
    {
        [DataMember]
        public int IdContaCorrente { get; set; }

        [DataMember]
        public int IdEntidade { get; set; }

        [DataMember]
        public int? IdTrabalhador { get; set; }

        [DataMember]
        public string TipoDivida { get; set; }

        [DataMember]
        public DateTime MesAno { get; set; }

        [DataMember]
        public DateTime DataVencimento { get; set; }

        [DataMember]
        public decimal ValorEntidade { get; set; }

        [DataMember]
        public decimal ValorTrabalhador { get; set; }

        [DataMember]
        public decimal ValorTotal { get; set; }

        [DataMember]
        public DateTime? PagoEm { get; set; }

        [DataMember]
        public int SituacaoPagamento { get; set; }
    }
}