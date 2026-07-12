using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class EconomicClassification
    {
        public EconomicClassification()
        {
            InverseParentFkNavigation = new HashSet<EconomicClassification>();
        }

        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Designacao { get; set; }
        public int Nivel { get; set; }
        public int? ParentFk { get; set; }
        public int BudgetPeriodFk { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }
        public string Tipo { get; set; }

        public virtual EconomicClassification ParentFkNavigation { get; set; }
        public virtual BudgetPeriod BudgetPeriodFkNavigation { get; set; }
        public virtual ICollection<EconomicClassification> InverseParentFkNavigation { get; set; }
    }
}
