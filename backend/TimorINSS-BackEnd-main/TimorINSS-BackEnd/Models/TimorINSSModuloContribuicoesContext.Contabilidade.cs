using Microsoft.EntityFrameworkCore;

namespace TimorINSSBackEnd.Models
{
    public partial class TimorINSSModuloContribuicoesContext
    {
        public virtual DbSet<ProgramActivity> ProgramActivity { get; set; }
        public virtual DbSet<FunctionalClassification> FunctionalClassification { get; set; }
        public virtual DbSet<UserModeAccess> UserModeAccess { get; set; }
        public virtual DbSet<EconomicClassification> EconomicClassification { get; set; }
        public virtual DbSet<OrcamentoBatch> OrcamentoBatch { get; set; }
        public virtual DbSet<OrcamentoLinha> OrcamentoLinha { get; set; }
        public virtual DbSet<ExpenditureAuthorization> ExpenditureAuthorization { get; set; }
        public virtual DbSet<ExpenditureAuthorizationPlurianualidade> ExpenditureAuthorizationPlurianualidade { get; set; }
        public virtual DbSet<Cabimento> Cabimento { get; set; }
        public virtual DbSet<CompromissoDespesa> CompromissoDespesa { get; set; }
        public virtual DbSet<CompromissoDespesaPlurianualidade> CompromissoDespesaPlurianualidade { get; set; }
        public virtual DbSet<Obligation> Obligation { get; set; }
        public virtual DbSet<ObligationItem> ObligationItem { get; set; }
        public virtual DbSet<ObligationBeneficiary> ObligationBeneficiary { get; set; }
        public virtual DbSet<PermissionPreset> PermissionPreset { get; set; }
        public virtual DbSet<PermissionPresetItem> PermissionPresetItem { get; set; }
        public virtual DbSet<UserPermission> UserPermission { get; set; }
        public virtual DbSet<LanguageConfig> LanguageConfig { get; set; }
        public virtual DbSet<PaymentAuthorization> PaymentAuthorization { get; set; }
        public virtual DbSet<PaymentExecution> PaymentExecution { get; set; }
        public virtual DbSet<UserProfile> UserProfile { get; set; }
        public virtual DbSet<ReceitaPac> ReceitaPac { get; set; }
        public virtual DbSet<OrcamentoSuplementar> OrcamentoSuplementar { get; set; }
        public virtual DbSet<OrcamentoSuplementarLinha> OrcamentoSuplementarLinha { get; set; }
        public virtual DbSet<BankStatementLine> BankStatementLine { get; set; }
    }
}
