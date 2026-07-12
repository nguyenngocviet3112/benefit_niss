using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class ProgramActivity
    {
        public ProgramActivity()
        {
            InverseParentFkNavigation = new HashSet<ProgramActivity>();
        }

        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Designacao { get; set; }
        public int Nivel { get; set; }
        public int? ParentFk { get; set; }
        public int BudgetPeriodFk { get; set; }
        // False = outside the OSS budget perimeter (e.g. A08 Regime Contributivo de
        // Capitalização, per OSS_Global_2026_FINAL_livro.xlsx) — the record still exists
        // fully in the system, it's just excluded from CE_OSS_Global's totals.
        public bool IsOssPerimeter { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }

        public virtual ProgramActivity ParentFkNavigation { get; set; }
        public virtual BudgetPeriod BudgetPeriodFkNavigation { get; set; }
        public virtual ICollection<ProgramActivity> InverseParentFkNavigation { get; set; }
    }
}
