using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class ReceitaPacDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int Numero { get; set; }

        [DataMember]
        public int Mes { get; set; }

        [DataMember]
        public int Ano { get; set; }

        [DataMember]
        public string Niss { get; set; }

        [DataMember]
        public int RegimeFk { get; set; }

        [DataMember]
        public string RegimeDesignacao { get; set; }

        [DataMember]
        public int? AtividadeFk { get; set; }

        [DataMember]
        public string AtividadeCodigo { get; set; }

        [DataMember]
        public string AtividadeDesignacao { get; set; }

        [DataMember]
        public int EconomicClassificationFk { get; set; }

        [DataMember]
        public string EconomicClassificationCodigo { get; set; }

        [DataMember]
        public string EconomicClassificationDesignacao { get; set; }

        [DataMember]
        public int OrganizationFk { get; set; }

        [DataMember]
        public string OrganizationNome { get; set; }

        [DataMember]
        public string Descritivo { get; set; }

        [DataMember]
        public decimal ValorPac { get; set; }

        [DataMember]
        public decimal ValorCobradoBanco { get; set; }

        [DataMember]
        public decimal ValorCobradoCaixa { get; set; }

        [DataMember]
        public int? ContaBancariaFk { get; set; }

        [DataMember]
        public string ContaBancariaNome { get; set; }

        [DataMember]
        public int? CodigoContaDebitoFk { get; set; }

        [DataMember]
        public string CodigoContaDebitoDesignacao { get; set; }

        [DataMember]
        public int? CodigoContaCreditoFk { get; set; }

        [DataMember]
        public string CodigoContaCreditoDesignacao { get; set; }

        [DataMember]
        public decimal ValorCobradoTotal { get; set; }

        [DataMember]
        public decimal SaldoPorCobrar { get; set; }
    }
}
