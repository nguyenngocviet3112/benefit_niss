using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class RelatorioClassificacaoEconomicaRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = true)]
        public int year { get; set; }

        [DataMember(IsRequired = true)]
        public int institution { get; set; }
    }
}
