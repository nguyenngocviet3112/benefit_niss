using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class ComponentesReceitaRegistoResponseDataContract : ResponseBaseDataContract
    {
        [DataMember]
        public int rows;

        [DataMember]
        public List<ComponenteReceita> ComponenteReceita;
    }

    [DataContract]
    public class ComponenteReceita
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int IdOrcamentoRegistoAprovado { get; set; }

        [DataMember]
        public int TarefaAtivoFK { get; set; }

        [DataMember]
        public int DepartamentoFk { get; set; }

        [DataMember]
        public int CentroCustoFk { get; set; }

        [DataMember]
        public int TipoContaFk { get; set; }

        [DataMember]
        public int CodigoContaFk { get; set; }

        [DataMember]
        public int AgrupamentoConfigFk { get; set; }

        [DataMember]
        public int InstitutionId { get; set; }

        [DataMember]
        public string Descricao { get; set; }

        [DataMember]
        public decimal Valor { get; set; }

        [DataMember]
        public IEnumerable<int> MovimentosIds { get; set; }

        [DataMember]
        public List<MovimentosPorConciliarListagem> Movimentos { get; set; }
    }

    [DataContract]
    public class GetReceitasNaoConciliadasRelatoriosReponse : ResponseBaseDataContract
    {
        [DataMember]
        public int rows;

        [DataMember]
        public List<ReceitasNaoConciliadasRelatorios> Receitas;
    }

    [DataContract]
    public class ReceitasNaoConciliadasRelatorios
    {
        [DataMember]
        public string Descricao { get; set;}

        [DataMember]
        public string NumeroDocumento { get; set; }
        
        [DataMember]
        public decimal Valor { get; set; }

        [DataMember]
        public DateTime Data { get; set; }

        [DataMember]
        public string Contribuinte { get; set; }

    }
}