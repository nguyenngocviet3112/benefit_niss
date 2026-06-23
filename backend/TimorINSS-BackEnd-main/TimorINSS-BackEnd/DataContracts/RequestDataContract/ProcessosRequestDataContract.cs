using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class SwitchProcessoStateRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = true)]
        public int Id { get; set; }
    }

    [DataContract]
    public class ProcessoConfigRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = false)]
        public int? Id { get; set; }

        [DataMember(IsRequired = true)]
        public string Nome { get; set; }

        [DataMember(IsRequired = true)]
        public List<RelProcessoConfigTarefaDataContract> Tarefas { get; set; }

        [DataMember(IsRequired = true)]
        public List<int> Perfis { get; set; }
    }

    [DataContract]
    public class ListProcessoConfiRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = true)]
        public int Id { get; set; }
    }

    [DataContract]
    public class StartProcessRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = true)]
        public int Id { get; set; }
    }

    [DataContract]
    public class GetProcessoDataRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = true)]
        public int processoId { get; set; }
    }

    [DataContract]
    public class GetHistoricoTextoProcessoRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = true)]
        public int processoId { get; set; }
    }
}