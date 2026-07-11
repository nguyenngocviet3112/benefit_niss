using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Obligation
    {
        public Obligation()
        {
            ObligationItem = new HashSet<ObligationItem>();
        }

        public int Id { get; set; }
        public int Numero { get; set; }
        public int Mes { get; set; }
        public int Ano { get; set; }
        public string DescritivoObrigacao { get; set; }
        public string Estado { get; set; }
        public int? SubmittedBy { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string LastRejectComment { get; set; }
        public int? LastRejectBy { get; set; }
        public DateTime? LastRejectAt { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }

        public virtual ICollection<ObligationItem> ObligationItem { get; set; }
    }
}
