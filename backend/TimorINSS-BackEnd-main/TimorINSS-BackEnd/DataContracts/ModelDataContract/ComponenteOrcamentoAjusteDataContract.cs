using System;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class ComponenteOrcamentoAjusteDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int ComponenteOrcamentoRegistoFk { get; set; }

        [DataMember]
        public int? RubricaOrigemFk { get; set; }

        [DataMember]
        public string RubricaOrigemDescricao { get; set; }

        [DataMember]
        public int RubricaDestinoFk { get; set; }

        [DataMember]
        public string RubricaDestinoDescricao { get; set; }

        [DataMember]
        public decimal Valor { get; set; }

        [DataMember]
        public string Estado { get; set; }

        [DataMember]
        public string Motivo { get; set; }

        [DataMember]
        public string MotivoRejeicao { get; set; }

        [DataMember]
        public int UtilizadorSolicitacao { get; set; }

        [DataMember]
        public string UtilizadorSolicitacaoNome { get; set; }

        [DataMember]
        public DateTime DataSolicitacao { get; set; }

        [DataMember]
        public int? UtilizadorAprovacao { get; set; }

        [DataMember]
        public string UtilizadorAprovacaoNome { get; set; }

        [DataMember]
        public DateTime? DataAprovacao { get; set; }
    }

    [DataContract]
    public class RubricaDisponivelDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Descricao { get; set; }

        [DataMember]
        public decimal Valor { get; set; }

        [DataMember]
        public decimal SaldoDisponivel { get; set; }
    }
}
