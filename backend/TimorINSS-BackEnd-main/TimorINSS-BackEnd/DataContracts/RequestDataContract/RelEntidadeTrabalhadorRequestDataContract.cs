using System;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class RelEntidadeTrabalhadorRequest : RequestBaseDataContract
    {
        [DataMember]
        public RelEntidadeTrabalhadorDataContract relEntidadeTrabalhador { get; set; }
    }

    [DataContract]
    public class RelEntidadeTrabalhadorRegimeRequest : RequestBaseDataContract
    {
        [DataMember]
        public RelEntidadeTrabalhadorRegimeDataContract relEntidadeTrabalhadorRegime { get; set; }
    }

    [DataContract]
    public class DesvincularTrabalhadorRequest : RequestBaseDataContract
    {
        [DataMember]
        public DateTime DataFimdeVinculo { get; set; }

        [DataMember]
        public int IdRelEntidadeTrabalhador { get; set; }
    }

    [DataContract]
    public class RelEntidadeTrabalhadorRegimeDataContract
    {
        [DataMember]
        public int IdRel { get; set; }

        [DataMember]
        public int RegimeFk { get; set; }

        [DataMember]
        public int? EscalaoFk { get; set; }
    }
}