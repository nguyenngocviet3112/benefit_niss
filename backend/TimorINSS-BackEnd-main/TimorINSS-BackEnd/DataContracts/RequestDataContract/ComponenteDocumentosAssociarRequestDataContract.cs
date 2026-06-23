using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class GetTiposDocumentoPorTarefaAtivaRequest : RequestBaseDataContract
    {
        [DataMember]
        public int TarefaAtivoId { get; set; }
    }
}