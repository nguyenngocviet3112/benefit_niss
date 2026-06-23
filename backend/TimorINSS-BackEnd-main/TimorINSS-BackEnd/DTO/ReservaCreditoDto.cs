using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class ReservaCreditoDto : BaseDto
    {
        [Mapper]
        public int IdReserva { get; set; }

        [Mapper]
        public int ReservaEntidadeFk { get; set; }

        [Mapper]
        public int? ReservaGuiaPagamentoFk { get; set; }

        [Mapper]
        public decimal? Valor { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        public virtual GuiapagamentoDto ReservaGuiaPagamentoFkNavigation { get; set; }
        public virtual EntidadeempregadoraDto ReservaEntidadeFkNavigation { get; set; }
    }
}