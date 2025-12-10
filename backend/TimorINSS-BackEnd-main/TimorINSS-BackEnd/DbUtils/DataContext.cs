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
