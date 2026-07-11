using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class SaveBankAccountRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string EntidadeBancaria { get; set; }

        [DataMember]
        public string Descricao { get; set; }

        [DataMember]
        public string Swift { get; set; }

        [DataMember]
        public string Iban { get; set; }

        [DataMember]
        public string Numero { get; set; }
    }
}
