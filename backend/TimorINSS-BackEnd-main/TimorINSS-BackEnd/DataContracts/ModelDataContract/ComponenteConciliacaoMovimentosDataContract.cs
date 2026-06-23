using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    public class ComponenteConciliacaoMovimentosDataContract
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public int tarefaFk { get; set; }

        [DataMember]
        public int permissaoSelecionarMovimentos { get; set; }

        [DataMember]
        public int permissaoMovimentosConciliar { get; set; }

        [DataMember]
        public int permissaoMovimentosBancarios { get; set; }

        [DataMember]
        public int permissaoVerMovimentosAconciliar { get; set; }

        [DataMember]
        public int permissaoConciliar { get; set; }

        [DataMember]
        public int permissaoDesfazerConciliar { get; set; }
    }
}