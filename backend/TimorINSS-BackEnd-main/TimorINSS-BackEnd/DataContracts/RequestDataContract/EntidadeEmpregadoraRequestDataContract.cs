using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class EntidadeEmpregadoraRequest : RequestBaseDataContract
    {
        [DataMember]
        public EntidadeEmpregadoraDataContract EntidadeEmpregadora { get; set; }
    }

    [DataContract]
    public class EntidadeEmpregadoraUpsertRequest : RequestBaseDataContract
    {
        [DataMember]
        public EntidadeEmpregadoraUpsertDataContract EntidadeEmpregadora { get; set; }
    }
    

    [DataContract]
    public class EntidadeEmpregadoraIdRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = true)]
        public int IdEntidade { get; set; }
        [DataMember(IsRequired = true)]
        public string IdEntidadeStr { get; set; }
    }

    [DataContract]
    public class EntidadeEmpregadoraNissRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = true)]
        public string Niss { get; set; }
    }
}