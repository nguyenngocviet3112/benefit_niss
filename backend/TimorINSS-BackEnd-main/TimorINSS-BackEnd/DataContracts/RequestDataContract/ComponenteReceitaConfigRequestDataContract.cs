using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class GetComponenteReceitaConfigRequest : RequestBaseDataContract
    {
        [DataMember]
        public int TarefaAtivoId { get; set; }
    }
}