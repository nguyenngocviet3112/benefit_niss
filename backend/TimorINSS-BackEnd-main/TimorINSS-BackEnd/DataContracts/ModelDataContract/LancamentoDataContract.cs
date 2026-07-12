using System;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class LancamentoDataContract
    {
        [DataMember] public int Id { get; set; }
        [DataMember] public DateTime Data { get; set; }
        [DataMember] public string Descricao { get; set; }
        [DataMember] public int CodigoContaDebitoFk { get; set; }
        [DataMember] public string CodigoContaDebitoCodigo { get; set; }
        [DataMember] public string CodigoContaDebitoDesignacao { get; set; }
        [DataMember] public int CodigoContaCreditoFk { get; set; }
        [DataMember] public string CodigoContaCreditoCodigo { get; set; }
        [DataMember] public string CodigoContaCreditoDesignacao { get; set; }
        [DataMember] public decimal Valor { get; set; }
        [DataMember] public string OrigemTipo { get; set; }
        [DataMember] public int? OrigemId { get; set; }
    }
}
