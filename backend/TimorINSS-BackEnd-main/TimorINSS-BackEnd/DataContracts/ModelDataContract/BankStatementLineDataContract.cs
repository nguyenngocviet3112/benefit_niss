using System;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class BankStatementLineDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int ContaBancariaFk { get; set; }

        [DataMember]
        public string ContaBancariaNome { get; set; }

        [DataMember]
        public DateTime DataValor { get; set; }

        [DataMember]
        public string Descricao { get; set; }

        [DataMember]
        public decimal Credito { get; set; }

        [DataMember]
        public decimal Debito { get; set; }

        [DataMember]
        public bool IsConciliado { get; set; }

        [DataMember]
        public int? ReceitaPacFk { get; set; }

        [DataMember]
        public string ReceitaPacDescricao { get; set; }

        [DataMember]
        public int? PaymentExecutionFk { get; set; }

        [DataMember]
        public string PaymentExecutionDescricao { get; set; }

        [DataMember]
        public DateTime? ConciliadoAt { get; set; }
    }

    [DataContract]
    public class ReceitaDisponivelParaConciliacaoDataContract
    {
        [DataMember]
        public int ReceitaPacId { get; set; }

        [DataMember]
        public int Numero { get; set; }

        [DataMember]
        public int Mes { get; set; }

        [DataMember]
        public int Ano { get; set; }

        [DataMember]
        public string Descritivo { get; set; }

        [DataMember]
        public decimal ValorPac { get; set; }
    }

    [DataContract]
    public class PagamentoDisponivelParaConciliacaoDataContract
    {
        [DataMember]
        public int PaymentExecutionId { get; set; }

        [DataMember]
        public int ObligationNumero { get; set; }

        [DataMember]
        public string ObligationDescritivo { get; set; }

        [DataMember]
        public DateTime DataPagamento { get; set; }

        [DataMember]
        public string NumeroDocumento { get; set; }

        [DataMember]
        public decimal ValorAutorizado { get; set; }
    }
}
