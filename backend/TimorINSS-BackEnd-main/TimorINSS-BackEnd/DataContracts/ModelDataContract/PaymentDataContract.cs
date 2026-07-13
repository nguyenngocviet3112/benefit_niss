using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class PaymentExecutionDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public DateTime DataPagamento { get; set; }

        // Tài khoản NGÂN HÀNG NỘI BỘ của INSS (nguồn chi) — KHÁC với tài khoản
        // người thụ hưởng (xem BeneficiarioNomeConta.../BeneficiaryList trên
        // PaymentAuthorizationDataContract, lấy từ Obligation).
        [DataMember]
        public int? ContaBancariaFk { get; set; }

        [DataMember]
        public string ContaBancariaNome { get; set; }

        [DataMember]
        public string NumeroDocumento { get; set; }

        [DataMember]
        public string Observacao { get; set; }

        [DataMember]
        public DateTime ExecutedAt { get; set; }
    }

    [DataContract]
    public class PaymentAuthorizationDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int Numero { get; set; }

        [DataMember]
        public int Mes { get; set; }

        [DataMember]
        public int Ano { get; set; }

        [DataMember]
        public int ObligationFk { get; set; }

        [DataMember]
        public int ObligationNumero { get; set; }

        [DataMember]
        public int ObligationMes { get; set; }

        [DataMember]
        public string ObligationDescritivo { get; set; }

        [DataMember]
        public decimal ValorObrigacao { get; set; }

        // Thông tin người thụ hưởng — ECHOED read-only từ Obligation (nguồn dữ
        // liệu duy nhất là Obrigação, không nhập lại ở đây). Xác nhận qua sheet
        // Pagamento gốc: khối "Nome conta/N.º conta/IBAN/Swift/Banco/Montante a
        // pagar" ở đây trùng với Obrigação — là tài khoản người thụ hưởng, KHÔNG
        // phải tài khoản nội bộ INSS (đó là PaymentExecution.ContaBancariaFk).
        [DataMember]
        public string BeneficiarioNome { get; set; }

        [DataMember]
        public string BeneficiarioCategoria { get; set; }

        [DataMember]
        public string BeneficiarioNomeConta { get; set; }

        [DataMember]
        public string BeneficiarioNumeroConta { get; set; }

        [DataMember]
        public string BeneficiarioIban { get; set; }

        [DataMember]
        public string BeneficiarioSwift { get; set; }

        [DataMember]
        public string BeneficiarioBanco { get; set; }

        [DataMember]
        public decimal? BeneficiarioMontanteAPagar { get; set; }

        [DataMember]
        public List<ObligationBeneficiaryDataContract> BeneficiaryList { get; set; } = new List<ObligationBeneficiaryDataContract>();

        [DataMember]
        public string Descritivo { get; set; }

        [DataMember]
        public decimal ValorAutorizado { get; set; }

        [DataMember]
        public int? CodigoContaDebitoFk { get; set; }

        [DataMember]
        public string CodigoContaDebitoDesignacao { get; set; }

        [DataMember]
        public int? CodigoContaCreditoFk { get; set; }

        [DataMember]
        public string CodigoContaCreditoDesignacao { get; set; }

        [DataMember]
        public string Estado { get; set; }

        [DataMember]
        public DateTime? SubmittedAt { get; set; }

        [DataMember]
        public DateTime? ApprovedAt { get; set; }

        [DataMember]
        public string LastRejectComment { get; set; }

        [DataMember]
        public DateTime? LastRejectAt { get; set; }

        [DataMember]
        public PaymentExecutionDataContract Execution { get; set; }

        // true khi bút toán Liquidação (sinh lúc Approve) đã bị bỏ qua vì thiếu
        // Tài khoản Phải trả/Ngân hàng — cho phép người dùng bổ sung tài khoản
        // ngay tại màn này để ghi bù, thay vì phải "tìm ở đâu đó" (2026-07-13).
        [DataMember]
        public bool LiquidacaoFaltaConfiguracao { get; set; }

        // true khi bút toán tất toán (sinh lúc Execute) đã bị bỏ qua vì thiếu
        // Tài khoản Phải trả/Ngân hàng.
        [DataMember]
        public bool ExecucaoFaltaConfiguracao { get; set; }
    }

    [DataContract]
    public class ObligacaoDisponivelParaPagamentoDataContract
    {
        [DataMember]
        public int ObligationId { get; set; }

        [DataMember]
        public int Numero { get; set; }

        [DataMember]
        public int Mes { get; set; }

        [DataMember]
        public string DescritivoObrigacao { get; set; }

        [DataMember]
        public decimal ValorObrigacao { get; set; }
    }

    [DataContract]
    public class CodigoContaOptionDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Designacao { get; set; }
    }

    [DataContract]
    public class ContaBancariaOptionDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string EntidadeBancaria { get; set; }

        [DataMember]
        public string Numero { get; set; }
    }
}
