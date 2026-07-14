using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    // New-mode-only: matches one or more declared Guia de Pagamento (self-reported by the
    // entidade, IndPago in "Comprovativo em Validação"/"Comprovativo Parcial em Validação")
    // against real BankStatementLine row(s) before promoting to Paid/Partial Paid.
    // Deliberately does NOT reuse ConciliarMovimentosRequest/TarefaAtivoId — that legacy
    // flow is gated by the old Tarefa permission system, which new-mode users don't have.
    // 2026-07-13: migrated off Movimentosbancarios (old table) onto BankStatementLine (new
    // table already used by Conciliação de Movimentos) — user asked new-mode to stop
    // depending on old-mode's bank-statement infra; see memory
    // bank-statement-line-guia-pagamento-unification.
    [DataContract]
    public class ConciliarGuiaPagamentoRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = true)]
        public List<int> GuiaIds { get; set; }

        [DataMember(IsRequired = true)]
        public List<int> BankStatementLineIds { get; set; }
    }

    // Lấy chứng từ (comprovativo) + các số tự khai của 1 Guia — officer dùng để đối
    // chiếu mắt thường với dòng sao kê ngân hàng thật trước khi chọn khớp. Tách khỏi
    // GuiaListagem/listGuiasByEntidadeApprove (nơi comprovativoPagamento cố tình bị
    // comment out để không kéo blob PDF nặng vào mỗi lần load cả danh sách).
    [DataContract]
    public class GetGuiaComprovativoRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = true)]
        public int IdGuia { get; set; }
    }

    // Báo cáo "Receitas GP" (2026-07-14) — sổ đăng ký toàn bộ Guia Pagamento trong 1 năm,
    // mọi entidade, mọi trạng thái (kể cả đã đối chiếu/đã Paga), chỉ xem. Khác
    // getGuiasAporoveByFilter (dùng cho màn Duyệt, loại trừ "Guia Gerada" theo mặc định) —
    // báo cáo này cố tình lấy đủ mọi trạng thái để không bỏ sót gì.
    [DataContract]
    public class GetReceitasGpReportRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = true)]
        public int Ano { get; set; }
    }

    // Hủy 1 lần đối chiếu ngân hàng đã xác nhận sai (Guia Paga/Parcialmente Paga quay
    // lại Comprovativo em Validação/Parcial) — dùng khi officer chọn nhầm dòng sao kê,
    // để họ tự sửa mà không cần can thiệp DB. Chỉ áp dụng cho Guia đã đối chiếu qua
    // GuiaConciliacaoDataManager (không áp dụng Guia đang ở Guia Gerada/Rejeita).
    [DataContract]
    public class UndoConciliacaoGuiaRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = true)]
        public int GuiaId { get; set; }
    }
}
