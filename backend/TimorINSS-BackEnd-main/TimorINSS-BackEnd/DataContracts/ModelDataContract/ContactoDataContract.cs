using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class ContactoDataContract
    {
        [DataMember]
        public int IdContacto { get; set; }

        [DataMember]
        public int? IdTrabalhador { get; set; }

        [DataMember]
        public int? IdEntidade { get; set; }

        [DataMember]
        public string Telemovel { get; set; }

        [DataMember]
        public string Email { get; set; }
    }
}