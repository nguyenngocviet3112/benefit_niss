using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class PagamentoExecutadoDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int CompromissoFk { get; set; }

        [DataMember]
        public int DestinatarioFk { get; set; }

        [DataMember]
        public string NumeroPagamento { get; set; }

        [DataMember]
        public decimal ValorExecutado { get; set; }

        [DataMember]
        public string Estado { get; set; }

        [DataMember]
        public string? Iban { get; set; }

        [DataMember]
        public string? Swift { get; set; }

        [DataMember]
        public string NumeroConta { get; set; }

        [DataMember]
        public int ProcessoId { get; set; }

        [DataMember]
        public int? CodigoContaDebito { get; set; }

        [DataMember]
        public int? CodigoContaCredito { get; set; }

        [DataMember]
        public DateTime? DataObrigacao { get; set; }

        [DataMember]
        public string? BankCode { get; set; }

    }

    [DataContract]
    public class PagamentoExecutadoDestinatrioDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int ComponenteDespesaRegistoFk { get; set; }

        [DataMember]
        public int? CompromissoFk { get; set; }

        [DataMember]
        public string DescricaoDespesa { get; set; }

        [DataMember]
        public Destinatario Destinatario { get; set; }

        [DataMember]
        public string NumeroPagamento { get; set; }

        [DataMember]
        public decimal ValorExecutado { get; set; }

        [DataMember]
        public string Estado { get; set; }

        [DataMember]
        public string Iban { get; set; }

        [DataMember]
        public string Swift { get; set; }

        [DataMember]
        public string? NumeroConta { get; set; }

        [DataMember]
        public int ProcessoId { get; set; }

        [DataMember]
        public int? CodigoContaDebito { get; set; }

        [DataMember]
        public int? CodigoContaCredito { get; set; }

        [DataMember]
        public DateTime? DataObrigacao { get; set; }

        [DataMember]
        public string? BankCode { get; set; }
    }

    [DataContract]
    public class ListaPagamentosDoProcessoDataContract
    {
        [DataMember]
        public int Id { get; set; }
        
        [DataMember]
        public decimal Valor { get; set; }

        [DataMember]
        public string NumeroPagamento { get; set; }
       
        [DataMember]
        public Destinatario Destinatario { get; set; }

        [DataMember]
        public int idContaOGE { get; set; }

        [DataMember]
        public string ContaOGE { get; set; }

        [DataMember]
        public string? Iban { get; set; }

        [DataMember]
        public string? NumeroConta { get; set; }

        [DataMember]
        public int ProcessoId { get; set; }

        [DataMember]
        public int? CodigoContaDebito { get; set; }

        [DataMember]
        public int? CodigoContaCredito { get; set; }

        [DataMember]
        public DateTime? DataObrigacao { get; set; }

        [DataMember]
        public string? BankCode { get; set; }
    }

    public class ClassificacaoContabilisticaExecucao
    {
        [DataMember]
        public int CodigoContaDebito { get; set; }

        [DataMember]
        public int CodigoContaCredito { get; set; }

        [DataMember]
        public DateTime DataExecucao { get; set; }
    }

}