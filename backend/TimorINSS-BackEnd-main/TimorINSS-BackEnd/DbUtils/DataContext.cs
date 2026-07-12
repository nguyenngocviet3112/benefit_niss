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
                entity.HasOne(d => d.BudgetPeriodFkNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.BudgetPeriodFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProgramActivity_BudgetPeriod");

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
                entity.HasOne(d => d.BudgetPeriodFkNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.BudgetPeriodFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EconomicClassification_BudgetPeriod");

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
                entity.HasOne(d => d.BudgetPeriodFkNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.BudgetPeriodFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrcamentoBatch_BudgetPeriod");
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

            modelBuilder.Entity<OrcamentoSuplementar>(entity =>
            {
                entity.HasOne(d => d.BudgetPeriodFkNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.BudgetPeriodFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrcamentoSuplementar_BudgetPeriod");
            });

            modelBuilder.Entity<OrcamentoSuplementarLinha>(entity =>
            {
                entity.Property(e => e.OldValue).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.AdjustmentValue).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.FinalValue).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.OrcamentoSuplementarFkNavigation)
                    .WithMany(p => p.OrcamentoSuplementarLinha)
                    .HasForeignKey(d => d.OrcamentoSuplementarFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OSL_OrcamentoSuplementar");

                entity.HasOne(d => d.OrcamentoLinhaFkNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.OrcamentoLinhaFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OSL_OrcamentoLinha");
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

                entity.HasOne(d => d.ContaBancariaFkNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.ContaBancariaFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ReceitaPac_ContaBancaria");

                entity.HasOne(d => d.CodigoContaDebitoFkNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.CodigoContaDebitoFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ReceitaPac_CodigoContaDebito");

                entity.HasOne(d => d.CodigoContaCreditoFkNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.CodigoContaCreditoFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ReceitaPac_CodigoContaCredito");
            });

            modelBuilder.Entity<BankStatementLine>(entity =>
            {
                entity.Property(e => e.Credito).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.Debito).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.ContaBancariaFkNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.ContaBancariaFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BankStatementLine_ContaBancaria");

                entity.HasOne(d => d.ReceitaPacFkNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.ReceitaPacFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BankStatementLine_ReceitaPac");

                entity.HasOne(d => d.PaymentExecutionFkNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.PaymentExecutionFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BankStatementLine_PaymentExecution");
            });

            modelBuilder.Entity<Lancamento>(entity =>
            {
                entity.Property(e => e.Valor).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.CodigoContaDebitoFkNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.CodigoContaDebitoFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Lancamento_CodigoContaDebito");

                entity.HasOne(d => d.CodigoContaCreditoFkNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.CodigoContaCreditoFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Lancamento_CodigoContaCredito");
            });

            modelBuilder.Entity<GuiaPagamentoContaConfig>(entity =>
            {
                entity.HasOne(d => d.CodigoContaDebitoFkNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.CodigoContaDebitoFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GuiaPagamentoContaConfig_CodigoContaDebito");

                entity.HasOne(d => d.CodigoContaCreditoFkNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.CodigoContaCreditoFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GuiaPagamentoContaConfig_CodigoContaCredito");
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
