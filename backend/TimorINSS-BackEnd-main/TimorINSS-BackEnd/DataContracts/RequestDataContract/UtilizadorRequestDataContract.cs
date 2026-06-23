using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class UtilizadorRequest : RequestBaseDataContract
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public List<DepartamentoDataContract> departamento { get; set; }

        [DataMember]
        public List<PerfilDataContract> perfil { get; set; }
    }

    public class DadosUtilizadorRequest : RequestBaseDataContract
    {
        [DataMember]
        public int idUtilizador { get; set; }

        [DataMember]
        public int idTrabalhador { get; set; }
    }

    public class UserUpdateRequest : RequestBaseDataContract
    {
        [DataMember]
        public int id { get; set; }
    }
}