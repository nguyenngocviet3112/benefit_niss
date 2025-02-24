using System;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class SuspensaoDataContract
    {
        [DataMember]
        public int IdEntidade { get; set; }

        [DataMember]
        public int? IdTrabalhador { get; set; }

        [DataMember]
        public DateTime DataInicioSuspensao { get; set; }

        [DataMember]
        public DateTime? DataFimSuspensao { get; set; }
    }
}