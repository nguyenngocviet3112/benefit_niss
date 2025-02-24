using System;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class DeclaracaoRemuneracaoDataContract
    {
        [DataMember]
        public int idDeclaracao { get; set; }

        [DataMember]
        public int declaracaoRelEntidadeTrabalhadorFk { get; set; }

        [DataMember]
        public decimal diasContrato { get; set; }

        [DataMember]
        public decimal diasEfecTrabalhados { get; set; }

        [DataMember]
        public int faltasInjustific { get; set; }

        [DataMember]
        public int diasParentalidade { get; set; }

        [DataMember]
        public decimal diasTrabcontabSegSocial { get; set; }

        [DataMember]
        public decimal remunDeclarada { get; set; }

        [DataMember]
        public decimal decimoTerceiro { get; set; }

        [DataMember]
        public DateTime mesAno { get; set; }

        [DataMember]
        public bool flagImportado { get; set; }

        [DataMember]
        public int contaCorrenteFk { get; set; }

        [DataMember]
        public bool decimoTerceiroMes { get; set; }

        [DataMember]
        public bool oficioso { get; set; }

        [DataMember]
        public int nacionalidadeFk { get; set; }

        [DataMember]
        public int regimeFk { get; set; }

        [DataMember]
        public int sexoFk { get; set; }
    }
}