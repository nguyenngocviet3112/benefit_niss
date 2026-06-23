using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    public class MovimentosUpsertDataRequest : RequestBaseDataContract
    {
        [DataMember]
        public MovimentosUpsertData data { get; set; }
    }

    public class MovimentosDeleteRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int TarefaAtivoId { get; set; }
    }

    [DataContract]
    public class MovimentosListagemRequest : SearchFilterRequest
    {
        [DataMember]
        public int? CaixaId { get; set; }

        [DataMember]
        public int? BancoId { get; set; }

        [DataMember]
        public bool GetBalance { get; set; } = false;
    }

    [DataContract]
    public class MovimentosBancarioConciliacaoListagemRequest : SearchFilterRequest
    {
        [DataMember(IsRequired = true)]
        public int TarefaAtivoId { get; set; }

        [DataMember(IsRequired = false)]
        public int? ConciliadoCom { get; set; }

        [DataMember(IsRequired = false)]
        public MovimentosPorConciliarListagemType ConciliadoComType { get; set; }
    }

    [DataContract]
    public class ConciliarMovimentosPermissionsListRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = true)]
        public int TarefaAtivoId { get; set; }
    }

    public class ListSaldoMovimentosRequest : RequestBaseDataContract
    {
        [DataMember]
        public int? ContaId { get; set; }

        [DataMember]
        public int? CaixaId { get; set; }
    }
}