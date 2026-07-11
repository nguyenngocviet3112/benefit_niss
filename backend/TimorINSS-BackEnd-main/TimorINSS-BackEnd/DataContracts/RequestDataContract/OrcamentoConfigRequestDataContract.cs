using System;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class SaveOrcamentoConfigRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int Ano { get; set; }

        [DataMember]
        public DateTime DataInicio { get; set; }

        [DataMember]
        public DateTime? DataFim { get; set; }
    }

    [DataContract]
    public class DeactivateOrcamentoConfigRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }
    }
}
