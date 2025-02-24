using System;
using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class ContacorrenteDto : BaseDto
    {
        [Mapper]
        public int IdContaCorrente { get; set; }

        [Mapper]
        public int? ContaCorrenteEntidadeFk { get; set; }

        [Mapper]
        public int? ContaCorrenteTrabalhadorFk { get; set; }

        [Mapper]
        public int TipoDivida { get; set; }

        [Mapper]
        public DateTime MesAno { get; set; }

        [Mapper]
        public DateTime DataVencimento { get; set; }

        [Mapper]
        public decimal ValorEntidade { get; set; }

        [Mapper]
        public decimal ValorTrabalhador { get; set; }

        [Mapper]
        public decimal ValorTotal { get; set; }

        [Mapper]
        public DateTime? PagoEm { get; set; }

        [Mapper]
        public int SituacaoPagamento { get; set; }

        [Mapper]
        public int? ContaCorrenteTaxaJuroFk { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        public virtual EntidadeempregadoraDto ContaCorrenteEntidade { get; set; }
        public virtual TaxajuromensalDto ContaCorrenteTaxaJuro { get; set; }
        public virtual TrabalhadorDto ContaCorrenteTrabalhador { get; set; }
        public virtual DominioDto SituacPagamento { get; set; }
        public virtual DominioDto TpDivida { get; set; }
        public virtual GuiapagamentoDto GuiaPagamento { get; set; }
        public virtual ICollection<DeclaracaoremuneracaoDto> Declaracaoremuneracao { get; set; }
    }
}