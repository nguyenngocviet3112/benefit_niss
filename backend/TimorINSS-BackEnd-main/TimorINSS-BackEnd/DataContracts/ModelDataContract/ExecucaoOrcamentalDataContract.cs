using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class ExecucaoOrcamentalDataContract
    {
        [DataMember]
        public string contaOGE { get; set; }

        [DataMember]
        public IEnumerable<string> centrosCusto { get; set; }

        [DataMember]
        public string instiutiton { get; set; }

        [DataMember]
        public IEnumerable<string> rubricas { get; set; }

        [DataMember]
        public decimal valorOrcamentoInicial { get; set; }

        [DataMember]
        public decimal valorOrcamentado { get; set; }

        [DataMember]
        public decimal valorAnoAnterior { get; set; }

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

        [DataMember]
        public decimal variacaoExecucao { get; set; }
    }
}