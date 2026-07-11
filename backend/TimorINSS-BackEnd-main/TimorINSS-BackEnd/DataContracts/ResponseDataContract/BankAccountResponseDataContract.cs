using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class BankAccountListResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<BankAccountDataContract> Items { get; set; } = new List<BankAccountDataContract>();
    }

    [DataContract]
    public class SaveBankAccountResponse : ResponseBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }
    }
}
