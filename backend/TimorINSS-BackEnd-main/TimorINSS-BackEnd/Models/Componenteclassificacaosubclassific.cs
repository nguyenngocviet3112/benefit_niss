using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Componenteclassificacaosubclassific
    {
        public int Id { get; set; }
        public int TarefaFk { get; set; }
        public int SubclassificacaoFk { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }

        public virtual Subclassificacao SubclassificacaoFkNavigation { get; set; }
        public virtual Tarefa TarefaFkNavigation { get; set; }
    }
}
