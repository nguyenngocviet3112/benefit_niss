using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    // New-mode-only: matches one or more declared Guia de Pagamento (self-reported by the
    // entidade, IndPago in "Comprovativo em Validação"/"Comprovativo Parcial em Validação")
    // against real Movimentosbancarios line(s) before promoting to Paid/Partial Paid.
    // Deliberately does NOT reuse ConciliarMovimentosRequest/TarefaAtivoId — that legacy
    // flow is gated by the old Tarefa permission system, which new-mode users don't have.
    [DataContract]
    public class ConciliarGuiaPagamentoRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = true)]
        public List<int> GuiaIds { get; set; }

        [DataMember(IsRequired = true)]
        public List<int> MovimentosBancarios { get; set; }
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
}
