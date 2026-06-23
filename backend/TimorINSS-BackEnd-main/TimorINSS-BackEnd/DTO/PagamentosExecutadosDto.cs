using System;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class PagamentosExecutadosDto : BaseDto
    {
        [Mapper]
        public int Id { get; set; }

        [Mapper]
        public int ComponenteDespesaRegistoFk { get; set; }

        [Mapper]
        public int DestinatarioFk { get; set; }

        [Mapper]
        public string NumeroPagamento { get; set; }

        [Mapper]
        public int Estado { get; set; }

        [Mapper]
        public decimal ValorExecutado { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        [Mapper]
        public string Iban { get; set; }

        [Mapper]
        public int ProcessoAtivoFk { get; set; }

        [Mapper]
        public string NumeroConta { get; set; }
        [Mapper]
        public int? CodigoContaDebitoFk { get; set; }
        [Mapper]
        public int? CodigoContaCreditoFk { get; set; }
        [Mapper]
        public DateTime? DataObrigacao { get; set; }

        public virtual ComponenteDespesaRegistoDto ComponenteDespesaRegistoFkNavigation { get; set; }
        public virtual DominioDto EstadoNavigation { get; set; }
        public virtual ProcessoativoDto ProcessoAtivoFkNavigation { get; set; }
    }
}