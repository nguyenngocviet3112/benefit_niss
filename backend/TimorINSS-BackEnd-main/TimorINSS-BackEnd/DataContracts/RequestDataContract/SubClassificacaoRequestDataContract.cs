using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class GetAllSubClassificacaoByTarefaAtivaIdRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = true)]
        public int tarefaAtivoId { get; set; }
    }
}