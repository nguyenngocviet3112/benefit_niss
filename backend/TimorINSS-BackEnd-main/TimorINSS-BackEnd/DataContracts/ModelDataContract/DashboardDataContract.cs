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

        // Thực chi/Ngân sách (năm hiện tại) — cùng công thức đã dùng ở CE_OSS_Global
        // (OrcamentoLinha batch APPROVED, loại trừ ngoài phạm vi OSS; Execução tính theo
        // ObligationItem, chỉ PaymentAuthorization đã có PaymentExecution). Xem
        // CeInssGlobalRepository.GetTotaisDespesa.
        [DataMember]
        public decimal OrcamentoTotal { get; set; }

        [DataMember]
        public decimal DespesaExecutada { get; set; }

        // Guia Pagamento đã validate (IndPago = "Guia Paga", Valor domain = 1) / tổng số Guia
        // trong năm hiện tại. Xem GuiaPagamentoRepository.GetValidacaoSummary.
        [DataMember]
        public int GuiaPagamentoValidado { get; set; }

        [DataMember]
        public int GuiaPagamentoTotal { get; set; }
    }
}
