using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class RegistoDespesaRequest : RequestBaseDataContract
    {
        [DataMember]
        public ComponenteDespesaRegistoDataContract despesa { get; set; }
    }

    [DataContract]
    public class GetAllDespesaRegistadaRequest : RequestBaseDataContract
    {
        [DataMember]
        public int TarefaAtivoId { get; set; }
    }

    [DataContract]
    public class DeleteDespesaRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }
    }

    [DataContract]
    public class DeleteRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }
    }

    [DataContract]
    public class GetValoresDespesaByIdCodigoOrcamentoRequest : RequestBaseDataContract
    {
        [DataMember]
        public int AgrupamentoFk { get; set; }

        [DataMember]
        public int OrcamentoRegistoFk { get; set; }
        [DataMember]
        public int InstitutionId { get; set; }
        [DataMember]
        public int ActidadeFk { get; set; }
        [DataMember]
        public int EconomicFk { get; set; }
        [DataMember]
        public int FuncionalFk { get; set; }
    }

    [DataContract]
    public class DeleteListaDespesaRequest : RequestBaseDataContract
    {
        [DataMember]
        public List<int> Ids { get; set; }
    }

    [DataContract]
    public class UpdateDespesaRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Estado { get; set; }
    }

    public enum EstadoDespesaEnum
    {
        Autorizado = 1,
        Cabimentado = 2,
        Compromisso = 3,
        Obricacao = 4,
        Executado = 5
    }

    [DataContract]
    public class GetDespesasRelatorioRequest : SearchFilterRequest
    {
        [DataMember]
        public EstadoDespesaEnum EstadoDespesa { get; set; }
    }

    [DataContract]
    public class GetDespesasCompromissoRequest : RequestBaseDataContract
    {
        [DataMember]
        public int TarefaAtivoId { get; set; }
    }

    [DataContract]
    public class CompromissoUpsertRequest : RequestBaseDataContract
    {
        [DataMember]
        public CompromissoDataContract Compromisso { get; set; }
        [DataMember]
        public int TarefaAtivoId { get; set; }
    }

    [DataContract]
    public class UpdateDespesaCabimentadaRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public decimal Valor { get; set; }
    }
}