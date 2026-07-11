using Microsoft.EntityFrameworkCore;

namespace TimorINSSBackEnd.Models
{
    public partial class TimorINSSModuloContribuicoesContext : DbContext
    {
        public object RelEntidadeTrabalhador { get; internal set; }
        public object EntidadeEmpregadora { get; internal set; }
        public object DeclaracaoRemuneracao { get; internal set; }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contacorrente>(entity =>
                entity.Property(e => e.ValorJuros).ValueGeneratedOnAddOrUpdate()
            );

            modelBuilder.Entity<Guiapagamento>(entity =>
                entity.Property(e => e.ValorJuros).ValueGeneratedOnAddOrUpdate()
            );

            modelBuilder.Entity<ProgramActivity>(entity =>
            {
                entity.HasOne(d => d.OrcamentoConfigFkNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.OrcamentoConfigFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProgramActivity_OrcamentoConfig");

                entity.HasOne(d => d.ParentFkNavigation)
                    .WithMany(p => p.InverseParentFkNavigation)
                    .HasForeignKey(d => d.ParentFk)
                    .HasConstraintName("FK_ProgramActivity_Parent");
            });

            modelBuilder.Entity<FunctionalClassification>(entity =>
            {
                entity.HasOne(d => d.ParentFkNavigation)
                    .WithMany(p => p.InverseParentFkNavigation)
                    .HasForeignKey(d => d.ParentFk)
                    .HasConstraintName("FK_FunctionalClassification_Parent");
            });

            modelBuilder.Entity<EconomicClassification>(entity =>
            {
                entity.HasOne(d => d.OrcamentoConfigFkNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.OrcamentoConfigFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EconomicClassification_OrcamentoConfig");

                entity.HasOne(d => d.ParentFkNavigation)
                    .WithMany(p => p.InverseParentFkNavigation)
                    .HasForeignKey(d => d.ParentFk)
                    .HasConstraintName("FK_EconomicClassification_Parent");
            });

            modelBuilder.Entity<UserModeAccess>(entity =>
            {
                entity.HasOne(d => d.UtilizadorFkNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.UtilizadorFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserModeAccess_Utilizador");
            });

            modelBuilder.Entity<OrcamentoBatch>(entity =>
            {
                entity.HasOne(d => d.OrcamentoConfigFkNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.OrcamentoConfigFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrcamentoBatch_OrcamentoConfig");
            });

            modelBuilder.Entity<OrcamentoLinha>(entity =>
            {
                entity.Property(e => e.Valor).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.OrcamentoBatchFkNavigation)
                    .WithMany(p => p.OrcamentoLinha)
                    .HasForeignKey(d => d.OrcamentoBatchFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrcamentoLinha_Batch");

                entity.HasOne(d => d.AtividadeFkNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.AtividadeFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrcamentoLinha_Atividade");

                entity.HasOne(d => d.EconomicClassificationFkNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.EconomicClassificationFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrcamentoLinha_EconomicClassification");

                entity.HasOne(d => d.FunctionalClassificationFkNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.FunctionalClassificationFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrcamentoLinha_FunctionalClassification");

                entity.HasOne(d => d.OrganizationFkNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.OrganizationFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrcamentoLinha_Institution");
            });

            modelBuilder.Entity<ExpenditureAuthorization>(entity =>
            {
                entity.Property(e => e.ValorAutorizado).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.Regularizacao).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.OrcamentoLinhaFkNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.OrcamentoLinhaFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ExpenditureAuthorization_OrcamentoLinha");
            });

            modelBuilder.Entity<ExpenditureAuthorizationPlurianualidade>(entity =>
            {
                entity.Property(e => e.Valor).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.ExpenditureAuthorizationFkNavigation)
                    .WithMany(p => p.ExpenditureAuthorizationPlurianualidade)
                    .HasForeignKey(d => d.ExpenditureAuthorizationFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ExpAuthPluri_ExpAuth");
            });

            modelBuilder.Entity<Cabimento>(entity =>
            {
                entity.Property(e => e.ValorCabimentado).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.ExpenditureAuthorizationFkNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.ExpenditureAuthorizationFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Cabimento_ExpenditureAuthorization");
            });

            modelBuilder.Entity<CompromissoDespesa>(entity =>
            {
                entity.Property(e => e.ValorCompromissoGlobal).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.ValorCompromissoAno).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.Regularizacao).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.CabimentoFkNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.CabimentoFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CompromissoDespesa_Cabimento");
            });

            modelBuilder.Entity<CompromissoDespesaPlurianualidade>(entity =>
            {
                entity.Property(e => e.Valor).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.CompromissoDespesaFkNavigation)
                    .WithMany(p => p.CompromissoDespesaPlurianualidade)
                    .HasForeignKey(d => d.CompromissoDespesaFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CompDespPluri_CompDesp");
            });

            modelBuilder.Entity<ObligationItem>(entity =>
            {
                entity.Property(e => e.Value).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.ObligationFkNavigation)
                    .WithMany(p => p.ObligationItem)
                    .HasForeignKey(d => d.ObligationFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ObligationItem_Obligation");

                entity.HasOne(d => d.CompromissoDespesaFkNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.CompromissoDespesaFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ObligationItem_CompromissoDespesa");
            });

            modelBuilder.Entity<Obligation>(entity =>
            {
                entity.Property(e => e.BeneficiarioMontanteAPagar).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<ObligationBeneficiary>(entity =>
            {
                entity.Property(e => e.MontanteAPagar).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.SalarioIliquido).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.Cotizacao4).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.Imposto10).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.SalarioLiquido).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.OutrosSuplementos).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.ObligationFkNavigation)
                    .WithMany(p => p.ObligationBeneficiary)
                    .HasForeignKey(d => d.ObligationFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ObligationBeneficiary_Obligation");
            });

            modelBuilder.Entity<PermissionPresetItem>(entity =>
            {
                entity.HasOne(d => d.PermissionPresetFkNavigation)
                    .WithMany(p => p.PermissionPresetItem)
                    .HasForeignKey(d => d.PermissionPresetFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PermissionPresetItem_Preset");
            });

            modelBuilder.Entity<UserPermission>(entity =>
            {
                entity.HasOne(d => d.UtilizadorFkNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.UtilizadorFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserPermission_Utilizador");

                entity.HasOne(d => d.SourcePresetFkNavigation)
                    .WithMany(p => p.UserPermission)
                    .HasForeignKey(d => d.SourcePresetFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserPermission_Preset");
            });

            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.HasOne(d => d.UtilizadorFkNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.UtilizadorFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserProfile_Utilizador");

                entity.HasOne(d => d.DepartamentoFkNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.DepartamentoFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserProfile_Departamento");
            });

            // Mechanical registration only (mirrors the ExpenditureAuthorization/
            // Cabimento pattern above) — Models/PaymentAuthorization.cs and
            // PaymentExecution.cs already existed from a concurrent change but
            // were missing their DbSet/Fluent config, which broke the whole
            // solution's build (CS1061). No new logic/relationships invented
            // here beyond what the model classes already declare.
            modelBuilder.Entity<PaymentAuthorization>(entity =>
            {
                entity.Property(e => e.ValorAutorizado).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.ObligationFkNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.ObligationFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PaymentAuthorization_Obligation");

                entity.HasOne(d => d.CodigoContaDebitoFkNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.CodigoContaDebitoFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PaymentAuthorization_CodigoContaDebito");

                entity.HasOne(d => d.CodigoContaCreditoFkNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.CodigoContaCreditoFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PaymentAuthorization_CodigoContaCredito");
            });

            modelBuilder.Entity<PaymentExecution>(entity =>
            {
                entity.HasOne(d => d.PaymentAuthorizationFkNavigation)
                    .WithOne(p => p.PaymentExecution)
                    .HasForeignKey<PaymentExecution>(d => d.PaymentAuthorizationFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PaymentExecution_PaymentAuthorization");

                entity.HasOne(d => d.ContaBancariaFkNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.ContaBancariaFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PaymentExecution_ContaBancaria");
            });

            // ORCAMENTOCONFIG pre-existed (old app) with explicit Fluent config in the
            // generated context using lowercase-first column names — Ano/Tipo are new
            // columns added on top (see db_migrations/2026-07-11i_system_settings.sql),
            // so they need the same explicit HasColumnName here (EF convention alone
            // would look for "Ano"/"Tipo", not "ano"/"tipo"). Relocated here from
            // TimorINSSModuloContribuicoesContext.Contabilidade.cs — a partial method
            // can only have one implementing body, and this file already owns it.
            modelBuilder.Entity<Orcamentoconfig>(entity =>
            {
                entity.Property(e => e.Ano).HasColumnName("ano");

                entity.Property(e => e.Tipo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("tipo");
            });

            modelBuilder.Entity<ReceitaPac>(entity =>
            {
                entity.Property(e => e.ValorPac).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.ValorCobradoBanco).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.ValorCobradoCaixa).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.RegimeFkNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.RegimeFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ReceitaPac_Regime");

                entity.HasOne(d => d.AtividadeFkNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.AtividadeFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ReceitaPac_Atividade");

                entity.HasOne(d => d.EconomicClassificationFkNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.EconomicClassificationFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ReceitaPac_EconomicClassification");

                entity.HasOne(d => d.OrganizationFkNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.OrganizationFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ReceitaPac_Institution");
            });
        }

        public TimorINSSModuloContribuicoesContext(string connectionString) : base(GetOptions(connectionString))
        {
        }

        private static DbContextOptions GetOptions(string connectionString)
        {
            return SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder(), connectionString).Options;
        }
    }
}
