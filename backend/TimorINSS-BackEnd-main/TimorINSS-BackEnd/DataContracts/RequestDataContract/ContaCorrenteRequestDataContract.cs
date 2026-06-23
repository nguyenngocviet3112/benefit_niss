using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class ContaCorrenteRequest : RequestBaseDataContract
    {
        [DataMember]
        public ContaCorrenteDataContract ContaCorrente { get; set; }
    }

    [DataContract]
    public class ContaCorrenteListagemRequest : SearchFilterRequest
    {
        [DataMember]
        public int IdEntidade { get; set; }

        [DataMember]
        public string IdEntidadeStr { get; set; }

        [DataMember]
        public int? IdTrabalhador { get; set; }
    }

    [DataContract]
    public class ResumoContaCorrenteListagemRequest : SearchFilterRequest
    {
        [DataMember]
        public int IdEntidade { get; set; }
        [DataMember]
        public int IdEntidadeStr { get; set; }
    }

    public class GetAllContasStatesFromYearByFilterRequest : SearchFilterRequest
    {
        [DataMember(IsRequired = true)]
        public int idEntidade { get; set; }
        [DataMember(IsRequired = true)]
        public string idEntidadeStr { get; set; }
    }
}