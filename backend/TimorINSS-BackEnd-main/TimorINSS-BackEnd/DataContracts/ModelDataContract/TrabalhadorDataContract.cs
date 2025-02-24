using System;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class TrabalhadorDataContract
    {
        [DataMember]
        public int IdTrabalhador { get; set; }

        [DataMember]
        public string Nome { get; set; }

        [DataMember]
        public string Niss { get; set; }

        [DataMember]
        public string Tin { get; set; }

        [DataMember]
        public string NumInscProvisoria { get; set; }

        [DataMember]
        public DateTime DataNasc { get; set; }

        [DataMember]
        public string NomeMae { get; set; }

        [DataMember(IsRequired = true)]
        public bool IndDescNomeMae { get; set; }

        [DataMember]
        public string NomePai { get; set; }

        [DataMember]
        public bool IndDescNomePai { get; set; }

        [DataMember]
        public int Sexo { get; set; }

        [DataMember]
        public int? EstadoCivil { get; set; }

        [DataMember]
        public int Nacionalidade { get; set; }

        [DataMember]
        public string Naturalidade { get; set; }
    }
}