using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class UserSyncListResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<UserSyncListItemDataContract> Items { get; set; } = new List<UserSyncListItemDataContract>();
    }

    [DataContract]
    public class SyncInternalUserResponse : ResponseBaseDataContract
    {
    }
}
