using System;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class INSSCompanyStaffData
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public DateTime? StarDate { get; set; }

        [DataMember]
        public DateTime? EndDate { get; set; }

        [DataMember]
        public double? ContributeMonth { get; set; }

        [DataMember]
        public double? ContributeMoney { get; set; }

    }
}