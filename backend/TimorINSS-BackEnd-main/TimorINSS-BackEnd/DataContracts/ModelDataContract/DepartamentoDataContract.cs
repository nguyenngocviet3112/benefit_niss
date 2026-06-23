using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class DepartamentoDataContract
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string nome { get; set; }
    }
}