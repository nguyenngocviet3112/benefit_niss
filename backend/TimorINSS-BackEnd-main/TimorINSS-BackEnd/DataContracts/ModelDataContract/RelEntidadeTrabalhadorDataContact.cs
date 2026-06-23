using System;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class RelEntidadeTrabalhadorDataContract
    {
        [DataMember]
        public int IdRelEntidadeTrabalhador { get; set; }

        [DataMember]
        public int EntidadeFk { get; set; }

        [DataMember]
        public int TrabalhadorFk { get; set; }

        [DataMember]
        public int TipoContrato { get; set; }

        [DataMember]
        public int NaturezaContrato { get; set; }

        [DataMember]
        public int LeiLabAplicavel { get; set; }

        [DataMember]
        public int Profissao { get; set; }

        [DataMember]
        public int? HorasSemana { get; set; }

        [DataMember]
        public int? DiasSemana { get; set; }

        [DataMember]
        public DateTime DtIniVincTrabalhador { get; set; }

        [DataMember]
        public DateTime? DtIniFimTrabalhador { get; set; }

        [DataMember]
        public bool FuncPublico { get; set; }

        [DataMember]
        public string NumFuncPublico { get; set; }

        [DataMember]
        public int RegimeFk { get; set; }

        [DataMember]
        public int? EscalaoFk { get; set; }

        [DataMember]
        public string? ProfissaoOutro { get; set; }
    }
}