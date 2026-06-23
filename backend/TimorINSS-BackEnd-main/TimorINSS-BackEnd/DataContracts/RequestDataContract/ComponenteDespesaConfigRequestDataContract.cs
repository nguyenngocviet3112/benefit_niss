using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class GetComponenteDespesaConfigRequest : RequestBaseDataContract
    {
        [DataMember]
        public int TarefaAtivoId { get; set; }
    }
}