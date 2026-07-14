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

        // Nome do banco isolado (Contabancaria.EntidadeBancaria) — separado de
        // ContaBancariaNome (que já vem formatado como "BANCO (número)") para
        // permitir filtro por banco no frontend sem parsear string (2026-07-14).
        [DataMember]
        public string EntidadeBancaria { get; set; }

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

        [DataMember]
        public decimal ValorCobradoBanco { get; set; }
    }

    // 1 dòng đọc được từ Excel ở bước preview (CLAUDE.md §6) — chưa ghi DB.
    // IsDuplicate = đã có 1 BankStatementLine active khớp Conta+Data+Credito/
    // Debito+Descricao — không phải lỗi chặn, chỉ gợi ý để user tự chọn
    // Insert/Skip (khác NEW/EXISTS/ERROR của Orçamento vì sao kê ngân hàng
    // không có "khóa nghiệp vụ" thật để coi là ghi đè — chỉ có thể trùng lặp
    // do import lại cùng 1 file).
    [DataContract]
    public class BankStatementLineImportRowDataContract
    {
        [DataMember]
        public int RowNum { get; set; }

        [DataMember]
        public DateTime DataValor { get; set; }

        [DataMember]
        public string Descricao { get; set; }

        [DataMember]
        public decimal Credito { get; set; }

        [DataMember]
        public decimal Debito { get; set; }

        [DataMember]
        public bool IsDuplicate { get; set; }
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
