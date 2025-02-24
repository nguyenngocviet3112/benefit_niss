using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class ComponenteReceitaDataContract
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public int tarefaFk { get; set; }

        [DataMember]
        public int classificarMovSelecionados { get; set; }

        [DataMember]
        public int selecionarMovRecebidosParaRegisto { get; set; }

        [DataMember]
        public int verificarExecucaoOrcamentoEditarSelecao { get; set; }
    }
}