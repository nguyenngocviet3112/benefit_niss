using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class SuspensaoResponse : ResponseBaseDataContract
    {
        [DataMember]
        public SuspensaoDataContract suspensao { get; set; }
    }

    [DataContract]
    public class SuspensaoListagemResponse : ResponseBaseDataContract
    {
        [DataMember]
        public int rows { get; set; }

        [DataMember]
        public List<SuspensaoListagem> suspensao { get; set; }
    }

    [DataContract]
    public class SuspensaoListagem
    {
        [DataMember]
        public int idSuspensao { get; set; }

        [DataMember]
        public int? idEntidade { get; set; }

        [DataMember]
        public int? idTrabalhador { get; set; }

        [DataMember]
        public DateTime dataInicioSuspensao { get; set; }

        [DataMember]
        public DateTime? dataFimSuspensao { get; set; }
    }
}