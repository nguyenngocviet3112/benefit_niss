using System;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class CodigoContaOpeningBalanceDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Codigo { get; set; }

        [DataMember]
        public string Designacao { get; set; }

        [DataMember]
        public decimal? InitialValue { get; set; }

        [DataMember]
        public bool? IsCredit { get; set; }

        [DataMember]
        public DateTime? InitialValueDate { get; set; }
    }
}
