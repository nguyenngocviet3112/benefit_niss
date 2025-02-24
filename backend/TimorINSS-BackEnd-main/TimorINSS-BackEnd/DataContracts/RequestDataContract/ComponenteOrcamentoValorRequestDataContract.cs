using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class AddComponenteOrcamentoValorRequest : RequestBaseDataContract
    {
        [DataMember]
        public ComponenteOrcamentoValorDataContract ComponenteOrcamentoValor { get; set; }
    }

    [DataContract]
    public class SearchComponenteOrcamentoValorRequest : RequestBaseDataContract
    {
        [DataMember]
        public ComponenteOrcamentoValorSearch Filter { get; set; }
    }

    [DataContract]
    public class EliminarComponenteOrcamentoValorRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }
    }
}