using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class RelProcessoConfigTarefaDataContract
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public bool tarefaInicial { get; set; }
    }
}