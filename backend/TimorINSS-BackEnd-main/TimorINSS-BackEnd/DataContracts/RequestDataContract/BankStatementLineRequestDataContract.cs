using System;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class GetBankStatementLinesRequest : RequestBaseDataContract
    {
        [DataMember]
        public int ContaBancariaFk { get; set; }
    }

    [DataContract]
    public class GetReceitasDisponiveisParaConciliacaoRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Ano { get; set; }
    }

    [DataContract]
    public class AddBankStatementLineRequest : RequestBaseDataContract
    {
        [DataMember]
        public int ContaBancariaFk { get; set; }

        [DataMember]
        public DateTime DataValor { get; set; }

        [DataMember]
        public DateTime? DataTransacao { get; set; }

        [DataMember]
        public string CodigoTransacaoBancaria { get; set; }

        [DataMember]
        public string Descricao { get; set; }

        [DataMember]
        public decimal Credito { get; set; }

        [DataMember]
        public decimal Debito { get; set; }
    }

    [DataContract]
    public class DeleteBankStatementLineRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }
    }

    [DataContract]
    public class MatchReceitaRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int ReceitaPacFk { get; set; }
    }

    [DataContract]
    public class MatchPagamentoRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int PaymentExecutionFk { get; set; }
    }

    [DataContract]
    public class UnmatchRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }
    }
}
