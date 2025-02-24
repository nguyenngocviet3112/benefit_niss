using System;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class PerfilDataContract
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public DateTime dataCriacao { get; set; }

        [DataMember]
        public string descricao { get; set; }

        [DataMember]
        public bool indActivo { get; set; }
    }
}