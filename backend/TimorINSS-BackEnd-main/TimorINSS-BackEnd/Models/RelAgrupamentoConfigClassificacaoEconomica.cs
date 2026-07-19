using System;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class RelAgrupamentoConfigClassificacaoEconomica
    {
        public int Id { get; set; }
        public int AgrupamentoConfigOrigemFk { get; set; }
        public int AgrupamentoConfigCeFk { get; set; }
        public string Confianca { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }

        public virtual Agrupamentoconfig AgrupamentoConfigOrigemFkNavigation { get; set; }
        public virtual Agrupamentoconfig AgrupamentoConfigCeFkNavigation { get; set; }
    }
}
