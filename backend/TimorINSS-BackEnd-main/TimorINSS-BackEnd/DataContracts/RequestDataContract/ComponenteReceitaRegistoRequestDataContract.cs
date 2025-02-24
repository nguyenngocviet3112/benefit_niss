using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class RegistoReceitaRequest : RequestBaseDataContract
    {
        [DataMember]
        public ComponenteReceitaRegistoDataContract componenteReceitaRegisto { get; set; }
    }

    [DataContract]
    public class GetComponenteReceitaRegistoByIdContaOSSRequest : SearchFilterRequest
    {
        [DataMember]
        public int ContaOSSId { get; set; }
    }

    [DataContract]
    public class DeleteReceitaRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int MovimentoId { get; set; }

    }

    [DataContract]
    public class ReceitasNaoConciliadasRelatoriosRequest : SearchFilterRequest
    {
        [DataMember]
        public string? Contribuinte { get; set; }
    }
}