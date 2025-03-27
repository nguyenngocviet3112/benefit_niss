using System;
using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class GuiapagamentoDto : BaseDto
    {
        [Mapper]
        public int IdGuia { get; set; }

        [Mapper]
        public int GuiaEntidadeFk { get; set; }

        [Mapper]
        public string NumDocumento { get; set; }

        [Mapper]
        public DateTime DtEmissao { get; set; }

        [Mapper]
        public string Descricao { get; set; }

        [Mapper]
        public string QrInvoice { get; set; }

        [Mapper]
        public decimal Valor { get; set; }

        [Mapper]
        public int IndPago { get; set; }

        [Mapper]
        public byte[]? ComprovativoPag { get; set; }

        [Mapper]
        public DateTime? DataComprovPag { get; set; }

        [Mapper]
        public decimal? ValorComprovPag { get; set; }

        [Mapper]
        public DateTime DtValidade { get; set; }

        [Mapper]
        public int TipoGuia { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        [Mapper]
        public DateTime MesAno { get; set; }

        [Mapper]
        public int ContaCorrenteId { get; set; }

        public virtual EntidadeempregadoraDto GuiaEntidadeFkNavigation { get; set; }
        public virtual DominioDto IndPagoNavigation { get; set; }
    }
}