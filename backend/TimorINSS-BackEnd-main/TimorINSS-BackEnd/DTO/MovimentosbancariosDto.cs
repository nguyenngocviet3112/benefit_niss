using System;

namespace TimorINSSBackEnd.DTO
{
    public class MovimentosbancariosDto
    {
        public int Id { get; set; }
        public int? TarefaFk { get; set; }
        public int? CaixaFk { get; set; }
        public int? ContaFk { get; set; }
        public string Descricao { get; set; }
        public DateTime DataValor { get; set; }
        public decimal? Credito { get; set; }
        public decimal? Debito { get; set; }
        public bool IndActivo { get; set; }
        public virtual DominioDto CaixaFkNavigation { get; set; }
        public virtual ContaBancariaDto ContaFkNavigation { get; set; }
        public virtual TarefaativoDto TarefaFkNavigation { get; set; }
    }
}