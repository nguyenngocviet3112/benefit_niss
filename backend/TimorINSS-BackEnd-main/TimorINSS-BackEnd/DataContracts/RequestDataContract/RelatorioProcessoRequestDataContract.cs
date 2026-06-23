using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class RelatorioProcessosListagemRequest : SearchFilterRequest
    {
        [DataMember(IsRequired = false)]
        public int? processoConfigId { get; set; }

        [DataMember(IsRequired = true)]
        public RelatorioProcessosListagemRequestViewType viewType { get; set; }
    }

    public enum RelatorioProcessosListagemRequestViewType
    {
        All = 0,
        NotArchived = 1,
        Archived = 2
    }
}