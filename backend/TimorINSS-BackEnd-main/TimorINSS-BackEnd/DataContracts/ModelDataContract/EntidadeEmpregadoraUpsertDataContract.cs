using System;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class EntidadeEmpregadoraUpsertDataContract
    {
        [DataMember]
        public int? Id { get; set; }

        [DataMember]
        public string Nome { get; set; }

        [DataMember]
        public string? Niss { get; set; }

        [DataMember]
        public string? Tin { get; set; }

        [DataMember]
        public string SituacInscricao { get; set; }

        [DataMember]
        public string? Email { get; set; }

        [DataMember]
        public string? Telemovel { get; set; }

        [DataMember]
        public int? IdNaturezaJuridica { get; set; }

        [DataMember]
        public int? IdActividadeEconomica { get; set; }

        [DataMember]
        public int? IdSectorActividade { get; set; }

        [DataMember]
        public int? NumTrabalhador { get; set; }

        [DataMember]
        public DateTime? DataInicioActiv { get; set; }

        [DataMember]
        public DateTime? DataInicioTrabServico { get; set; }

        [DataMember]
        public DateTime? DataInscricao { get; set; }
    }
}