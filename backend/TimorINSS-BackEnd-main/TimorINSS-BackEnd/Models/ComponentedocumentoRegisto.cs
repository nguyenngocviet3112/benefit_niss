using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class ComponentedocumentoRegisto
    {
        public int Id { get; set; }
        public int TarefaAtivoFk { get; set; }
        public int DocumentoFk { get; set; }
        public byte[] Documento { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }
        public string Numero { get; set; }
        public string NomeDocumento { get; set; }

        public virtual Dominio DocumentoFkNavigation { get; set; }
        public virtual Tarefaativo TarefaAtivoFkNavigation { get; set; }
    }
}
