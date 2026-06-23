using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class LoginResponse : ResponseBaseDataContract
    {
        [DataMember]
        public User? user { get; set; }

        [DataMember]
        public string? Token { get; set; }
    }

    [DataContract]
    public class User
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public string NISS { get; set; }

        [DataMember]
        public bool isInternal { get; set; }

        [DataMember]
        public int IdEntidade { get; set; }

        [DataMember]
        public List<Permission>? Permissions { get; set; }

        [DataMember]
        public string? Perfil { get; set; }
    }

    [DataContract]
    public class Permission
    {
        [DataMember]
        public long? Module { get; set; }

        [DataMember]
        public long IdFuncionalidade { get; set; }

        [DataMember]
        public bool Create { get; set; }

        [DataMember]
        public bool Read { get; set; }

        [DataMember]
        public bool Update { get; set; }

        [DataMember]
        public bool Delete { get; set; }
    }

    [DataContract]
    public class LogResponse : ResponseBaseDataContract
    {
        [DataMember]
        public string logString { get; set; }
    }
}