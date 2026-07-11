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
