using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Componenteconciliacaomovimentos
    {
        public int Id { get; set; }
        public int TarefaFk { get; set; }
        public int PermissaoSelecionarMovimentos { get; set; }
        public int PermissaoMovimentosConciliar { get; set; }
        public int PermissaoMovimentosBancarios { get; set; }
        public int PermissaoVerMovimentosAconciliar { get; set; }
        public int PermissaoConciliar { get; set; }
        public int PermissaoDesfazerConciliar { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }

        public virtual Tarefa TarefaFkNavigation { get; set; }
    }
}
