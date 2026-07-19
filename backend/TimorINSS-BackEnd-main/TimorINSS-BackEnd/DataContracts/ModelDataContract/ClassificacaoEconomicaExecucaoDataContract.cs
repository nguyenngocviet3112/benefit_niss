using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class ClassificacaoEconomicaExecucaoDataContract
    {
        [DataMember]
        public string codigoCE { get; set; }

        [DataMember]
        public string designacaoCE { get; set; }

        [DataMember]
        public int nivel { get; set; }

        [DataMember]
        public decimal valorOrcamentoInicial { get; set; }

        [DataMember]
        public decimal valorOrcamentado { get; set; }

        [DataMember]
        public decimal janeiro { get; set; }

        [DataMember]
        public decimal fevereiro { get; set; }

        [DataMember]
        public decimal marco { get; set; }

        [DataMember]
        public decimal abril { get; set; }

        [DataMember]
        public decimal maio { get; set; }

        [DataMember]
        public decimal junho { get; set; }

        [DataMember]
        public decimal julho { get; set; }

        [DataMember]
        public decimal agosto { get; set; }

        [DataMember]
        public decimal setembro { get; set; }

        [DataMember]
        public decimal outubro { get; set; }

        [DataMember]
        public decimal novembro { get; set; }

        [DataMember]
        public decimal dezembro { get; set; }

        [DataMember]
        public decimal totalExecucao { get; set; }

        [DataMember]
        public decimal taxaExecucao { get; set; }
    }
}
