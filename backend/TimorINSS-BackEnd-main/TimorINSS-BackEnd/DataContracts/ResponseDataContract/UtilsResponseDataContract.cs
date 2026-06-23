using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class SelectDescriptionResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<SelectDescription> selects { get; set; }
    }

    [DataContract]
    public class SelectDescription
    {
        [DataMember]
        public long id { get; set; }

        [DataMember]
        public string nome { get; set; }

        [DataMember]
        public long parentId { get; set; }

        [DataMember]
        public bool indActivo { get; set; }

        [DataMember]
        public bool? hasInitialValue { get; set; }
    }
}