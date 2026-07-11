using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class OrcamentoBatch
    {
        public OrcamentoBatch()
        {
            OrcamentoLinha = new HashSet<OrcamentoLinha>();
        }

        public int Id { get; set; }
        public int OrcamentoConfigFk { get; set; }
        public string Estado { get; set; }
        public int? SubmittedBy { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public int? ReviewedBy { get; set; }
        public DateTime? ReviewedAt { get; set; }
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

        public virtual Orcamentoconfig OrcamentoConfigFkNavigation { get; set; }
        public virtual ICollection<OrcamentoLinha> OrcamentoLinha { get; set; }
    }
}
