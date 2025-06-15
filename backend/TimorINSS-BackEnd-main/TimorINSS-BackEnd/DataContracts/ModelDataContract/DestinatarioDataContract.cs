using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class DestinatarioDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int? EntidadeFk { get; set; }

        [DataMember]
        public int? TrabalhadorFk { get; set; }

        [DataMember]
        public string Niss { get; set; }

        [DataMember]
        public string? Tin { get; set; }

        [DataMember]
        public string Nome { get; set; }

        [DataMember]
        public string Morada { get; set; }
    }
}