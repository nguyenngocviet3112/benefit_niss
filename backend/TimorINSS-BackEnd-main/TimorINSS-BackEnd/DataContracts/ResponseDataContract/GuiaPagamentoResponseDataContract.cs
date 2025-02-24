using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class GuiaListagemResponse : ResponseBaseDataContract
    {
        [DataMember]
        public int rows { get; set; }

        [DataMember]
        public List<GuiaListagem> guias { get; set; }
    }

    [DataContract]
    public class GuiaListagem
    {
        [DataMember]
        public int idGuia { get; set; }

        [DataMember]
        public string numDocumento { get; set; }

        [DataMember]
        public DateTime mesAno { get; set; }

        [DataMember]
        public string descricao { get; set; }

        [DataMember]
        public decimal valor { get; set; }

        [DataMember]
        public decimal juros { get; set; }

        [DataMember]
        public decimal total { get; set; }

        [DataMember]
        public DateTime dtValidade { get; set; }

        [DataMember]
        public int tipo { get; set; }

        [DataMember]
        public decimal? valorPago { get; set; }

        [DataMember]
        public DateTime? dtValorPago { get; set; }

        [DataMember]
        public byte[]? comprovativoPagamento { get; set; }

        [DataMember]
        public int estadoPagamento { get; set; }
        [DataMember]
        public string niss { get; set; }
    }

    [DataContract]
    public class RelatorioGuiaPagamentoListagemResponse : ResponseBaseDataContract
    {
        [DataMember]
        public int rows;

        [DataMember]
        public List<RelatorioGuiaPagamentoDataContract> guias;
    }
}