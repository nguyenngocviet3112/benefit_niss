using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class SolicitarAjusteOrcamentoRequest : RequestBaseDataContract
    {
        // Deixar a null = Bonificação/reforço (não subtrai de nenhuma rubrica).
        // Preencher = Transferência (subtrai de RubricaOrigemFk, soma a RubricaDestinoFk).
        [DataMember]
        public int? RubricaOrigemFk { get; set; }

        [DataMember]
        public int RubricaDestinoFk { get; set; }

        [DataMember]
        public decimal Valor { get; set; }

        [DataMember]
        public string Motivo { get; set; }
    }

    [DataContract]
    public class AprovarAjusteOrcamentoRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }
    }

    [DataContract]
    public class RejeitarAjusteOrcamentoRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string MotivoRejeicao { get; set; }
    }

    [DataContract]
    public class GetAjustesOrcamentoRequest : RequestBaseDataContract
    {
        [DataMember]
        public int ComponenteOrcamentoRegistoFk { get; set; }
    }
}
