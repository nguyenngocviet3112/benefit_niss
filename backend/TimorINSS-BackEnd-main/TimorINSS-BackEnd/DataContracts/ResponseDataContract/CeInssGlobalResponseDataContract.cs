using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class CeInssGlobalResponse : ResponseBaseDataContract
    {
        // "INSS Global" when institution filter is null (Both), otherwise the specific Institution.Nome
        [DataMember]
        public string organizationLabel { get; set; }

        [DataMember]
        public List<CeInssGlobalDataContract> receitas { get; set; } = new List<CeInssGlobalDataContract>();

        [DataMember]
        public List<CeInssGlobalDataContract> despesas { get; set; } = new List<CeInssGlobalDataContract>();

        [DataMember]
        public decimal totalReceitaInicial { get; set; }

        [DataMember]
        public decimal totalReceitaCorrigido { get; set; }

        [DataMember]
        public decimal totalDespesaInicial { get; set; }

        [DataMember]
        public decimal totalDespesaCorrigido { get; set; }

        // totalReceitaCorrigido - totalDespesaCorrigido
        [DataMember]
        public decimal saldoOrcamental { get; set; }
    }
}
