using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class RelMovimentosporconciliarMovimentosDto : BaseDto
    {
        [Mapper]
        public int Id { get; set; }

        [Mapper]
        public int? MovimentoPorConciliarFk { get; set; }

        [Mapper]
        public int MovimentosBancariosFk { get; set; }

        [Mapper]
        public int? GuiaPagamentoFk { get; set; }

        [Mapper]
        public int? PagamentosExecutadosFk { get; set; }

        [Mapper]
        public bool? IndActivo { get; set; }

        public virtual Guiapagamento GuiaPagamentoFkNavigation { get; set; }
        public virtual Movimentosporconciliar MovimentoPorConciliarFkNavigation { get; set; }
        public virtual Movimentosbancarios MovimentosBancariosFkNavigation { get; set; }
        public virtual Pagamentosexecutados PagamentosExecutadosFkNavigation { get; set; }
    }
}