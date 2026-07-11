using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class UserPermissionListItemDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public bool IndActivo { get; set; }

        [DataMember]
        public bool Locked { get; set; }

        [DataMember]
        public List<string> Tokens { get; set; } = new List<string>();

        [DataMember]
        public List<string> PresetCodigos { get; set; } = new List<string>();
    }
}
