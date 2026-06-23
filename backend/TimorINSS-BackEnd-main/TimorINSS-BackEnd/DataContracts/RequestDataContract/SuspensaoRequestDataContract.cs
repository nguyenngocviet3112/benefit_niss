using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class SuspensaoRequest : RequestBaseDataContract
    {
        [DataMember]
        public SuspensaoDataContract Suspensao { get; set; }
    }

    [DataContract]
    public class SuspensaoListagemRequest : SearchFilterRequest
    {
        [DataMember]
        public int IdEntidade { get; set; }

        [DataMember]
        public int? IdTrabalhador { get; set; }
    }

    [DataContract]
    public class SuspensaoDeleteRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = true)]
        public int Id { get; set; }
    }
}