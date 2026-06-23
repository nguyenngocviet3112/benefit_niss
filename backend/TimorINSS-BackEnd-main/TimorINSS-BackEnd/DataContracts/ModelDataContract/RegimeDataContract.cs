using System;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class RegimeDataContract
    {
        [DataMember]
        public int IdRegime { get; set; }

        [DataMember]
        public int TipoRegime { get; set; }

        [DataMember]
        public DateTime DataInicio { get; set; }

        [DataMember]
        public DateTime? DataFim { get; set; }

        [DataMember]
        public decimal PercentEntidadeEmpreg { get; set; }

        [DataMember]
        public decimal PercentTrabalhador { get; set; }

        [DataMember]
        public string NomeRegime { get; set; }

        [DataMember]
        public DateTime DataVenciomento { get; set; }
    }
}