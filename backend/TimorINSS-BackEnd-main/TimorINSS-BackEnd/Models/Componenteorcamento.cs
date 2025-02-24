using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Componenteorcamento
    {
        public int Id { get; set; }
        public int TarefaFk { get; set; }
        public int PermissaoDatas { get; set; }
        public int PermissaoInsercoes { get; set; }
        public int PermissaoDetalhes { get; set; }
        public int PermissaoAprovacao { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }

        public virtual Tarefa TarefaFkNavigation { get; set; }
    }
}
