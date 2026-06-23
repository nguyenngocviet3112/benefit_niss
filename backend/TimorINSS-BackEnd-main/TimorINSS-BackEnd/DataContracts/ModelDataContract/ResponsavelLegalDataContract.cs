using System;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class ResponsavelLegalDataContract
    {
        [DataMember]
        public int IdResponsavelLegal { get; set; }

        [DataMember(IsRequired = true)]
        public string Nome { get; set; }

        [DataMember(IsRequired = true)]
        public string? Tin { get; set; }

        [DataMember(IsRequired = true)]
        public DateTime DataNascimento { get; set; }

        [DataMember(IsRequired = true)]
        public int Sexo { get; set; }

        [DataMember(IsRequired = true)]
        public int Nacionalidade { get; set; }

        [DataMember(IsRequired = true)]
        public string Naturalidade { get; set; }

        [DataMember(IsRequired = true)]
        public int Funcao { get; set; }

        [DataMember(IsRequired = true)]
        public string? FuncaoOutro { get; set; }

        [DataMember(IsRequired = true)]
        public bool IndFuncaoRem { get; set; }

        [DataMember]
        public int? RespLegalTabalhadorFk { get; set; }
    }
}