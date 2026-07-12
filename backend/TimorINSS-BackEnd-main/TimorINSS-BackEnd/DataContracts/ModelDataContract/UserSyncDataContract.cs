using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class UserSyncListItemDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public bool IndActivo { get; set; }

        [DataMember]
        public bool Locked { get; set; }

        // Chỉ có ý nghĩa cho tab Internal — tab External luôn false, không
        // hiển thị/sync gì thêm.
        [DataMember]
        public bool HasNewModeAccess { get; set; }

        [DataMember]
        public string Nome { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string DepartamentoNome { get; set; }
    }
}
