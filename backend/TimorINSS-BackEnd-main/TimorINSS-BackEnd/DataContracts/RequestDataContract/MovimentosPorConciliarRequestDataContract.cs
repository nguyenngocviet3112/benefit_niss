using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class MovimentosPorConciliarListagemRequest : SearchFilterRequest
    {
        [DataMember(IsRequired = false)]
        public int? TarefaAtivoId { get; set; }

        [DataMember(IsRequired = true)]
        public FiltroConciliadoEnum FiltroConciliado { get; set; }

        [DataMember(IsRequired = true)]
        public bool IsReceita { get; set; }

        [DataMember(IsRequired = false)]
        public string BankCode { get; set; }
    }

    [DataContract]
    public class GetMovimentosConciliadosRequest : SearchFilterRequest
    {
        // Filtrar por Bank (Guia de Pagamento/Invoice) — movimento manual não tem Bank próprio
        [DataMember(IsRequired = false)]
        public string BankCode { get; set; }
    }

    [DataContract]
    public class MovimentosPorConciliarConciliacaoListagemRequest : SearchFilterRequest
    {
        [DataMember(IsRequired = true)]
        public int TarefaAtivoId { get; set; }

        [DataMember(IsRequired = false)]
        public int? ConciliadoCom { get; set; }
    }

    [DataContract]
    public class CreateMovimentosPorConciliarRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = true)]
        public int TarefaAtivoId { get; set; }

        [DataMember(IsRequired = false)]
        public int MovimentoBancarioId { get; set; }

        [DataMember(IsRequired = true)]
        public decimal Valor { get; set; }

        [DataMember(IsRequired = true)]
        public bool IsReceita { get; set; }

        [DataMember(IsRequired = true)]
        public string TipoDocumento { get; set; }

        [DataMember(IsRequired = true)]
        public string NumeroDocumento { get; set; }

        [DataMember(IsRequired = false)]
        [Document]
        public string Comprovativo { get; set; }

        [DataMember]
        public string NomeComprovativo { get; set; }

        [DataMember]
        public int? ContabilidadeCredito { get; set; }

        [DataMember]
        public int? ContabilidadeDebito { get; set; }

        [DataMember]
        public int? DepartamentoINSS { get; set; }

        [DataMember]
        public int? CentroCusto { get; set; }

        [DataMember]
        public int? TipoConta { get; set; }

        [DataMember]
        public int? ContaOSS { get; set; }

        [DataMember]
        public bool? IsGuia { get; set; }

        [DataMember]
        public bool? IsReserva { get; set; }
    }

    [DataContract]
    public class UpdateMovimentosPorConciliarRequest : CreateMovimentosPorConciliarRequest
    {
        [DataMember(IsRequired = true)]
        public int Id { get; set; }
    }

    public enum FiltroConciliadoEnum
    {
        Todos = 0,
        Conciliados = 1,
        NaoConciliados = 2
    }

    [DataContract]
    public class ConciliarMovimentosRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = true)]
        public int TarefaAtivoId { get; set; }

        [DataMember(IsRequired = true)]
        public List<int> MovimentosBancarios { get; set; }

        [DataMember(IsRequired = true)]
        public List<MovimentosAConciliar> MovimentosAConciliar { get; set; }
    }

    public class MovimentosAConciliar
    {
        [DataMember(IsRequired = true)]
        public int Id { get; set; }

        [DataMember(IsRequired = true)]
        public MovimentosPorConciliarListagemType Type { get; set; }
    }

    [DataContract]
    public class DesfazerConciliacaoMovimentosRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = true)]
        public int TarefaAtivoId { get; set; }

        [DataMember(IsRequired = true)]
        public int Id { get; set; }

        [DataMember(IsRequired = false)]
        public MovimentosPorConciliarListagemType? Type { get; set; }
    }

    [DataContract]
    public class GetGuiaMovimentoRequest : RequestBaseDataContract
    {

        [DataMember(IsRequired = true)]
        public int Id { get; set; }

        [DataMember(IsRequired = true)]
        public MovimentosPorConciliarListagemType Type { get; set; }
    }
}