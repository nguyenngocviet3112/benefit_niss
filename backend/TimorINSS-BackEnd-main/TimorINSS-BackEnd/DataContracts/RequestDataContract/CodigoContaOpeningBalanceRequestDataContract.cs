using System;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class UpdateCodigoContaOpeningBalanceRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public decimal? InitialValue { get; set; }

        [DataMember]
        public bool? IsCredit { get; set; }

        [DataMember]
        public DateTime? InitialValueDate { get; set; }
    }
}
