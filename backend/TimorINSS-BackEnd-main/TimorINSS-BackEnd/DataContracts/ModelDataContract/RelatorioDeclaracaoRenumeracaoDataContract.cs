using System;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class RelatorioDeclaracaoRenumeracaoDataContract
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public DateTime mesAno { get; set; }

        [DataMember]
        public string nomeTrabalhador { get; set; }

        [DataMember]
        public string nomeEmpregador { get; set; }

        [DataMember]
        public decimal valorRenumeracoes { get; set; }

        [DataMember]
        public decimal valorContribuicoes { get; set; }

        [DataMember]
        public decimal valorPago { get; set; }

        [DataMember]
        public decimal valorDivida { get; set; }
    }
}