using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class RelatorioDeclaracaoRenumeracaoListagemRequest : SearchFilterRequest
    {
        [DataMember(IsRequired = true)]
        public bool isTrabalhador { get; set; }

        [DataMember(IsRequired = true)]
        public string search { get; set; }
    }
}