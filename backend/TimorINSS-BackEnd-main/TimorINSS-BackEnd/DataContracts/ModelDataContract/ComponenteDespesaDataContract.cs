using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class ComponenteDespesaDataContract
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public int tarefaFk { get; set; }

        [DataMember]
        public int registarDespesa { get; set; }

        [DataMember]
        public int visualizarDespesaRParaA { get; set; }

        [DataMember]
        public int visualizarDespesaAParaC { get; set; }

        [DataMember]
        public int visualizarDespesaR { get; set; }

        [DataMember]
        public int visualizarDespesaA { get; set; }

        [DataMember]
        public int visualizarDespesaComCompromisso { get; set; }

        [DataMember]
        public int visualiazarDespesaC { get; set; }

        [DataMember]
        public int executarPagamentos { get; set; }

        [DataMember]
        public int visualizarExecucaoDespesaCabimentada { get; set; }

        [DataMember]
        public int emitirOrdemPagamento { get; set; }
    }
}