using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class GuiaListagemResponse : ResponseBaseDataContract
    {
        [DataMember]
        public int rows { get; set; }

        [DataMember]
        public List<GuiaListagem> guias { get; set; }

        //[DataMember]
        //public Utilizador utilizador { get; set; }
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
        public byte[]? approveFile { get; set; }

        [DataMember]
        public int estadoPagamento { get; set; }
        [DataMember]
        public string niss { get; set; }
        [DataMember]
        public string userName { get; set; }

        [DataMember]
        public string? tin { get; set; }

        [DataMember]
        public string paymentRef { get; set; }

        [DataMember]
        public string qrInvoice { get; set; }

        [DataMember]
        public string bankCode { get; set; }

        [DataMember]
        public DateTime? dataCriacao { get; set; }
        [DataMember]
        public decimal? valorEntidade { get; set; }
        [DataMember]
        public decimal? valorTrabalhador { get; set; }


        //[DataMember]
        //public decimal quotizacoes { get; set; }

        //[DataMember]
        //public decimal contribuicoes { get; set; }
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