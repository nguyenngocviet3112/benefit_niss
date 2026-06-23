using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class ComponenteReceitaRegistoDataContract
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
        public int CodigoContaDebitoFk { get; set; }

        [DataMember]
        public int AgrupamentoConfigFk { get; set; }

        [DataMember]
        public string Descricao { get; set; }

        [DataMember]
        public decimal Valor { get; set; }

        [DataMember]
        public List<MovimentosPorConciliarListagem> ListaMovimentosConciliados { get; set; }
    }
}