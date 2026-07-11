using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class FunctionalClassification
    {
        public FunctionalClassification()
        {
            InverseParentFkNavigation = new HashSet<FunctionalClassification>();
        }

        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Designacao { get; set; }
        public int? ParentFk { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }

        public virtual FunctionalClassification ParentFkNavigation { get; set; }
        public virtual ICollection<FunctionalClassification> InverseParentFkNavigation { get; set; }
    }
}
