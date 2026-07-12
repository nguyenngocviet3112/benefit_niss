using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class GetReceitaPacListRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Ano { get; set; }
    }

    [DataContract]
    public class SaveReceitaPacRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int Mes { get; set; }

        [DataMember]
        public int Ano { get; set; }

        [DataMember]
        public string Niss { get; set; }

        [DataMember]
        public int RegimeFk { get; set; }

        [DataMember]
        public int? AtividadeFk { get; set; }

        [DataMember]
        public int EconomicClassificationFk { get; set; }

        [DataMember]
        public int OrganizationFk { get; set; }

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
    }

    [DataContract]
    public class DeactivateReceitaPacRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }
    }
}
