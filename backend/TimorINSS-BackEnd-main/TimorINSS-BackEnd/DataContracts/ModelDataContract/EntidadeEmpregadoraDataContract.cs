using System;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class EntidadeEmpregadoraDataContract
    {
        [DataMember]
        public int IdEntidadeEmpreg { get; set; }

        [DataMember]
        public string Nome { get; set; }

        [DataMember]
        public string Niss { get; set; }

        [DataMember]
        public string? Tin { get; set; }

        [DataMember]
        public DateTime DtInscricao { get; set; }

        [DataMember]
        public string SituacInscricao { get; set; }

        [DataMember]
        public DateTime DataInicioActiv { get; set; }

        [DataMember]
        public DateTime? DataFimActiv { get; set; }

        [DataMember]
        public DateTime DataInicioTrabServico { get; set; }

        [DataMember]
        public int NumTrabalhador { get; set; }

        [DataMember]
        public int IdNaturezaJuridica { get; set; }

        [DataMember]
        public int IdActividadeEconomica { get; set; }

        [DataMember]
        public int IdSectorActividade { get; set; }

        [DataMember]
        public DateTime DtHoraUltimoAcesso { get; set; }
    }
}