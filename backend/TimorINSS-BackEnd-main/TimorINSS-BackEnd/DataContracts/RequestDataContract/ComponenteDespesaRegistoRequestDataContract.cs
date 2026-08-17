using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class RegistoDespesaRequest : RequestBaseDataContract
    {
        [DataMember]
        public ComponenteDespesaRegistoDataContract despesa { get; set; }

        // Quando já existem outras despesas em curso para a mesma combinação de 5 parâmetros, o
        // registo não é bloqueado: devolve-se a lista dessas despesas para o utilizador ver o que
        // já lá está e decidir. Se ele confirmar, o pedido volta com este campo a true e grava.
        // [VI] Khi đã có despesa khác đang mở cùng tổ hợp 5 tham số, hệ thống KHÔNG chặn: trả về
        // danh sách để người dùng nhìn thấy rồi tự quyết. Nếu họ xác nhận, request gửi lại với
        // cờ này = true và tiến hành lưu.
        [DataMember]
        public bool ConfirmarDespesasEmCurso { get; set; }
    }

    [DataContract]
    public class GetAllDespesaRegistadaRequest : RequestBaseDataContract
    {
        [DataMember]
        public int TarefaAtivoId { get; set; }
    }

    [DataContract]
    public class DeleteDespesaRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }
    }

    [DataContract]
    public class DeleteRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }
    }

    [DataContract]
    public class GetValoresDespesaByIdCodigoOrcamentoRequest : RequestBaseDataContract
    {
        [DataMember]
        public int AgrupamentoFk { get; set; }

        [DataMember]
        public int OrcamentoRegistoFk { get; set; }
        [DataMember]
        public int InstitutionId { get; set; }
        [DataMember]
        public int ActidadeFk { get; set; }

        [DataMember]
        public int FuncionalFk { get; set; }

        // 5.º parâmetro da chave: sem ele os valores mostrados no ecrã somam todos os centros de custo.
        // [VI] Tham số thứ 5 của khóa: thiếu nó thì số hiển thị trên màn hình gộp mọi centro de custo.
        [DataMember]
        public int CentroCustoFk { get; set; }
    }

    [DataContract]
    public class DeleteListaDespesaRequest : RequestBaseDataContract
    {
        [DataMember]
        public List<int> Ids { get; set; }
    }

    [DataContract]
    public class UpdateDespesaRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Estado { get; set; }
    }

    public enum EstadoDespesaEnum
    {
        Autorizado = 1,
        Cabimentado = 2,
        Compromisso = 3,
        Obricacao = 4,
        Executado = 5,
        OrdemPagamentoEmitida = 6
    }

    [DataContract]
    public class GetDespesasRelatorioRequest : SearchFilterRequest
    {
        [DataMember]
        public EstadoDespesaEnum EstadoDespesa { get; set; }
    }

    [DataContract]
    public class GetDespesasCompromissoRequest : RequestBaseDataContract
    {
        [DataMember]
        public int TarefaAtivoId { get; set; }
    }

    [DataContract]
    public class CompromissoUpsertRequest : RequestBaseDataContract
    {
        [DataMember]
        public CompromissoDataContract Compromisso { get; set; }
        [DataMember]
        public int TarefaAtivoId { get; set; }
    }

    [DataContract]
    public class UpdateDespesaCabimentadaRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public decimal Valor { get; set; }
    }
}