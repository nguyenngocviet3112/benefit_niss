using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class PerfilRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = true)]
        public int Id { get; set; }
    }

    public class AddPerfilRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = true)]
        public string Descricao { get; set; }

        [DataMember]
        public int? idPerfil { get; set; }

        [DataMember(IsRequired = true)]
        public List<FuncionalidadeDataContract> Funcionalidade;
    }
}