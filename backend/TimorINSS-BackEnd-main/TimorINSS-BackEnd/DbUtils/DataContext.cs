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
