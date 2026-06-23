using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Componentetexto
    {
        public int Id { get; set; }
        public int TarefaFk { get; set; }
        public bool? Texto1 { get; set; }
        public bool? Expandir1 { get; set; }
        public string Titulo1 { get; set; }
        public int? QuantCaracteres1 { get; set; }
        public bool? Obrigatorio1 { get; set; }
        public bool? ObrigatorioAoArquivar1 { get; set; }
        public bool? Texto2 { get; set; }
        public bool? Expandir2 { get; set; }
        public string Titulo2 { get; set; }
        public int? QuantCaracteres2 { get; set; }
        public bool? Obrigatorio2 { get; set; }
        public bool? ObrigatorioAoArquivar2 { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }

        public virtual Tarefa TarefaFkNavigation { get; set; }
    }
}
