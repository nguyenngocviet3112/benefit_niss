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

        // Filtrar por Banco (Guia de Pagamento/Nota de Crédito) — movimento manual não tem Banco próprio
        [DataMember]
        public string? BankCode { get; set; }
    }

    [DataContract]
    public class ReceitasRelatoriosRequest : SearchFilterRequest
    {
        // Filtrar por Banco. Uma Receita pode reunir movimentos de mais de um banco —
        // o filtro devolve a Receita se PELO MENOS UM dos seus movimentos for do banco
        // pedido (ver ComponenteReceitaRegistoRepository.ReceitasRelatorios).
        [DataMember]
        public string? BankCode { get; set; }
    }
}