using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class ProcessSummaryDataContract
    {
        [DataMember]
        public int Total { get; set; }

        [DataMember]
        public int EmProcessamento { get; set; }

        [DataMember]
        public int Aprovado { get; set; }
    }

    [DataContract]
    public class DashboardSummaryDataContract
    {
        [DataMember]
        public ProcessSummaryDataContract Ad { get; set; }

        [DataMember]
        public ProcessSummaryDataContract Cabimento { get; set; }

        [DataMember]
        public ProcessSummaryDataContract Compromisso { get; set; }

        [DataMember]
        public ProcessSummaryDataContract Obrigacao { get; set; }

        [DataMember]
        public ProcessSummaryDataContract PagamentoAutorizacao { get; set; }

        [DataMember]
        public int PagamentoExecutado { get; set; }

        [DataMember]
        public int BancoConciliado { get; set; }

        [DataMember]
        public int BancoPendente { get; set; }
    }
}
