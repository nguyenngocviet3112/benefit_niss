using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class GetUserPermissionRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }
    }

    // Combined create-or-edit user + assign permissions/presets, saved in one
    // action (user explicitly rejected splitting this into 2 screens/steps).
    // Id = 0 -> create a brand new internal Utilizador (Username/Password
    // required); Id > 0 -> edit (Password optional = only set if resetting it).
    [DataContract]
    public class SaveUserPermissionRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public bool IndActivo { get; set; }

        [DataMember]
        public string Nome { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public int? DepartamentoFk { get; set; }

        [DataMember]
        public List<int> PresetIds { get; set; } = new List<int>();

        [DataMember]
        public List<string> Tokens { get; set; } = new List<string>();
    }
}
