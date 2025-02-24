using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class UpdateRelTarefaComponenteRequest : RequestBaseDataContract
    {
        [DataMember]
        public int idTarefa { get; set; }

        [DataMember]
        public List<Componentes> componenteListagem { get; set; }
    }

    [DataContract]
    public class GetAllRelTarefaComponenteByIdTarefaActivoRequest : RequestBaseDataContract
    {
        [DataMember]
        public int IdTarefaActivo { get; set; }
    }
}