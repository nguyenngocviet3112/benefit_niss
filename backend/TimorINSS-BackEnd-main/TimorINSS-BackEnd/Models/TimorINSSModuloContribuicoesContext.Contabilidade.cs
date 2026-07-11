using Microsoft.EntityFrameworkCore;

namespace TimorINSSBackEnd.Models
{
    public partial class TimorINSSModuloContribuicoesContext
    {
        public virtual DbSet<ProgramActivity> ProgramActivity { get; set; }
        public virtual DbSet<FunctionalClassification> FunctionalClassification { get; set; }
        public virtual DbSet<UserModeAccess> UserModeAccess { get; set; }
        public virtual DbSet<EconomicClassification> EconomicClassification { get; set; }
    }
}
