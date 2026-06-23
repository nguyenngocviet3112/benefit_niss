using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class ContaCorrenteResponse : ResponseBaseDataContract
    {
        [DataMember]
        public ContaCorrenteDataContract contaCorrente { get; set; }
    }

    [DataContract]
    public class ContaCorrenteListagemResponse : ResponseBaseDataContract
    {
        [DataMember]
        public int rows { get; set; }

        [DataMember]
        public List<ContaCorrenteListagem> contaCorrente { get; set; }
    }

    [DataContract]
    public class ResumoContaCorrenteListagemResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<ResumoContaCorrenteListagem> data { get; set; }
    }

    [DataContract]
    public class ResumoContaCorrenteListagem
    {
        [DataMember]
        public int Ano { get; set; }

        [DataMember]
        public decimal Contribuicoes { get; set; }

        [DataMember]
        public decimal Quotizacoes { get; set; }

        [DataMember]
        public decimal ValorAPagar { get; set; }

        [DataMember]
        public decimal TotalJuros { get; set; }

        [DataMember]
        public decimal TotalPago { get; set; }

    }
    public class ContaCorrenteListagem
    {
        [DataMember]
        public int IdContaCorrente { get; set; }

        [DataMember]
        public int? IdEntidade { get; set; }

        [DataMember]
        public int? IdTrabalhador { get; set; }

        [DataMember]
        public string TipoDivida { get; set; }

        [DataMember]
        public DateTime MesAno { get; set; }

        [DataMember]
        public DateTime DataVencimento { get; set; }

        [DataMember]
        public DateTime DataCriacao { get; set; }

        [DataMember]
        public decimal ValorEntidade { get; set; }

        [DataMember]
        public decimal ValorTrabalhador { get; set; }

        [DataMember]
        public decimal ValorTotal { get; set; }

        [DataMember]
        public DateTime? PagoEm { get; set; }

        [DataMember]
        public int SituacaoPagamento { get; set; }

        [DataMember]
        public decimal? ValorPago { get; set; }

        [DataMember]
        public string NumDocumento { get; set; }

        [DataMember]
        public decimal? JuroApurado { get; set; }

        [DataMember]
        public bool GerarGuia { get; set; }

        [DataMember]
        public int? TipoGuia { get; set; }

        [DataMember]
        public bool? IndActivoGuiaPagamento { get; set; }

        [DataMember]
        public string Niss { get; set; }
    }

    [DataContract]
    public class GetAllContasStatesFromYearByFilterResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<ContasState> contasState { get; set; }
    }

    [DataContract]
    public class ContasState
    {
        [DataMember(IsRequired = true)]
        public int id;

        [DataMember(IsRequired = true)]
        public string state;

        [DataMember(IsRequired = true)]
        public int month;

        [DataMember(IsRequired = true)]
        public int stateId;
    }
}