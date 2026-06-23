using System;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class GuiaPagamentoDataContract
    {
        [DataMember]
        public int IdGuia { get; set; }

        [DataMember]
        public int GuiaEntidadeFk { get; set; }

        [DataMember]
        public string NumDocumento { get; set; }

        [DataMember]
        public DateTime DtEmissao { get; set; }

        [DataMember]
        public string Descricao { get; set; }

        [DataMember]
        public decimal Valor { get; set; }

        [DataMember]
        public decimal Juros { get; set; }

        [DataMember]
        public int IndPago { get; set; }

        [DataMember]
        public DateTime? DataComprovPag { get; set; }

        [DataMember]
        public decimal? ValorComprovPag { get; set; }

        [DataMember]
        public DateTime DtValidade { get; set; }

        [DataMember]
        public int TipoGuia { get; set; }

        [DataMember]
        public int IdContaCorrente { get; set; }

        [DataMember]
        public DateTime MesAno { get; set; }

        [DataMember]
        public string BankCode { get; set; }
    }
}