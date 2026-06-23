using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Movimentosbancarios
    {
        public Movimentosbancarios()
        {
            RelMovimentosporconciliarMovimentos = new HashSet<RelMovimentosporconciliarMovimentos>();
        }

        public int Id { get; set; }
        public int? TarefaFk { get; set; }
        public int? CaixaFk { get; set; }
        public int? ContaFk { get; set; }
        public string Descricao { get; set; }
        public DateTime DataValor { get; set; }
        public decimal? Credito { get; set; }
        public decimal? Debito { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }

        public virtual Dominio CaixaFkNavigation { get; set; }
        public virtual Contabancaria ContaFkNavigation { get; set; }
        public virtual Tarefaativo TarefaFkNavigation { get; set; }
        public virtual ICollection<RelMovimentosporconciliarMovimentos> RelMovimentosporconciliarMovimentos { get; set; }
    }
}
