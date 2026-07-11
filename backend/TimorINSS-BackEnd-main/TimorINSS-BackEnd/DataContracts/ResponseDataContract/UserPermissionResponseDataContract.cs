using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class PermissionCatalogResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<PermissionGroupDataContract> Groups { get; set; } = new List<PermissionGroupDataContract>();

        [DataMember]
        public List<PermissionPresetDataContract> Presets { get; set; } = new List<PermissionPresetDataContract>();
    }

    [DataContract]
    public class UserPermissionListResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<UserPermissionListItemDataContract> Items { get; set; } = new List<UserPermissionListItemDataContract>();
    }

    [DataContract]
    public class UserPermissionDetailResponse : ResponseBaseDataContract
    {
        [DataMember]
        public UserPermissionListItemDataContract Item { get; set; }
    }

    [DataContract]
    public class SaveUserPermissionResponse : ResponseBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }
    }
}
