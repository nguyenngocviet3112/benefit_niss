using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class MovimentosPorConciliar : ResponseBaseDataContract
    {
        [DataMember]
        public int rows;

        [DataMember]
        public List<MovimentosPorConciliarListagem> movimentos;
    }

    [DataContract]
    public class MovimentosPorConciliarListagem
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public MovimentosPorConciliarDescricao descricao { get; set; }

        [DataMember]
        public byte[] comprovativo { get; set; }

        [DataMember]
        public string numeroDocumento { get; set; }

        [DataMember]
        public decimal valor { get; set; }

        [DataMember]
        public bool conciliado { get; set; }

        [DataMember]
        public MovimentosPorConciliarListagemType type { get; set; }

        [DataMember]
        public int movimentoBancarioId { get; set; }

        [DataMember]
        public string tipoDocumento { get; set; }

        [DataMember]
        public string nomeComprovativo { get; set; }

        [DataMember]
        public bool? editavel { get; set; }

        [DataMember]
        public int? contabilidadeCredito { get; set; }

        [DataMember]
        public int? contabilidadeDebito { get; set; }

        [DataMember]
        public int? departamentoINSS { get; set; }

        [DataMember]
        public int? centroCusto { get; set; }

        [DataMember]
        public int? tipoConta { get; set; }

        [DataMember]
        public int? contaOSS { get; set; }

        [DataMember]
        public bool? isClassificada { get; set; }
    }

    public enum MovimentosPorConciliarListagemType
    {
        MovimentoAConciliar = 1,
        GuiaPagamento = 2,
        PagamentoExecutado = 3,
        ReservaCredito = 4
    }

    public class MovimentosPorConciliarDescricao
    {
        [DataMember]
        public int? id { get; set; }

        [DataMember]
        public string descricao { get; set; }
    }

    public class MovimentoPorConciliarResponse : ResponseBaseDataContract
    {
        [DataMember]
        public Movimentosporconciliar? movimento;
    }
}