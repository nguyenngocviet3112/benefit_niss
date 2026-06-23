using System;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Relguiacontacorrente
    {
        public int IdRel { get; set; }
        public int GuiaFk { get; set; }
        public int ContaCorrenteFk { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }

        public virtual Contacorrente ContaCorrenteFkNavigation { get; set; }
        public virtual Guiapagamento GuiaFkNavigation { get; set; }
    }
}