using System;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class TimorINSSModuloContribuicoesContext : DbContext
    {
        public TimorINSSModuloContribuicoesContext()
        {
        }

        public TimorINSSModuloContribuicoesContext(DbContextOptions<TimorINSSModuloContribuicoesContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Actividadeeconomica> Actividadeeconomica { get; set; }

        public virtual DbSet<KhachHang> KhachHang { get; set; }

        public virtual DbSet<Agrupamentoconfig> Agrupamentoconfig { get; set; }
        public virtual DbSet<Aldeia> Aldeia { get; set; }
        public virtual DbSet<Camposeditaveis> Camposeditaveis { get; set; }
        public virtual DbSet<Centrocusto> Centrocusto { get; set; }
        public virtual DbSet<Classificacao> Classificacao { get; set; }
        public virtual DbSet<Codigoconta> Codigoconta { get; set; }
        public virtual DbSet<Componente> Componente { get; set; }
        public virtual DbSet<Componenteaccoestarefa> Componenteaccoestarefa { get; set; }
        public virtual DbSet<ComponenteaccoestarefaRegisto> ComponenteaccoestarefaRegisto { get; set; }
        public virtual DbSet<Componentecarregardocumento> Componentecarregardocumento { get; set; }
        public virtual DbSet<ComponenteclassificacaosubRegisto> ComponenteclassificacaosubRegisto { get; set; }
        public virtual DbSet<Componenteclassificacaosubclassific> Componenteclassificacaosubclassific { get; set; }
        public virtual DbSet<Componenteconciliacaomovimentos> Componenteconciliacaomovimentos { get; set; }
        public virtual DbSet<Componentecontroleacesso> Componentecontroleacesso { get; set; }
        public virtual DbSet<Componentedespesa> Componentedespesa { get; set; }
        public virtual DbSet<ComponentedespesaRegisto> ComponentedespesaRegisto { get; set; }
        public virtual DbSet<ComponentedocumentoRegisto> ComponentedocumentoRegisto { get; set; }
        public virtual DbSet<Componenteorcamento> Componenteorcamento { get; set; }
        public virtual DbSet<ComponenteorcamentoRegisto> ComponenteorcamentoRegisto { get; set; }
        public virtual DbSet<Componenteorcamentovalor> Componenteorcamentovalor { get; set; }
        public virtual DbSet<Componentereceita> Componentereceita { get; set; }
        public virtual DbSet<ComponentereceitaRegisto> ComponentereceitaRegisto { get; set; }
        public virtual DbSet<ComponentereceitaRegistoMovimentos> ComponentereceitaRegistoMovimentos { get; set; }
        public virtual DbSet<Componentetexto> Componentetexto { get; set; }
        public virtual DbSet<ComponentetextoRegisto> ComponentetextoRegisto { get; set; }
        public virtual DbSet<Compromisso> Compromisso { get; set; }
        public virtual DbSet<Contabancaria> Contabancaria { get; set; }
        public virtual DbSet<Contacorrente> Contacorrente { get; set; }
        public virtual DbSet<Contacto> Contacto { get; set; }
        public virtual DbSet<Declaracaoremuneracao> Declaracaoremuneracao { get; set; }
        
        
        public virtual DbSet<Destinatario> Destinatario { get; set; }
        public virtual DbSet<Dispensacontributiva> Dispensacontributiva { get; set; }
        public virtual DbSet<Documentoidentificacao> Documentoidentificacao { get; set; }
        public virtual DbSet<Dominio> Dominio { get; set; }
        public virtual DbSet<Entidadeempregadora> Entidadeempregadora { get; set; }
        public virtual DbSet<Escalao> Escalao { get; set; }
        public virtual DbSet<ExcelImporterRel> ExcelImporterRel { get; set; }
        public virtual DbSet<Funcionalidade> Funcionalidade { get; set; }
        public virtual DbSet<Guiapagamento> Guiapagamento { get; set; }
        public virtual DbSet<Inssestrangeiro> Inssestrangeiro { get; set; }
        public virtual DbSet<Morada> Morada { get; set; }
        public virtual DbSet<Movimentobancario> Movimentobancario { get; set; }
        public virtual DbSet<Movimentosbancarios> Movimentosbancarios { get; set; }
        public virtual DbSet<Movimentosporconciliar> Movimentosporconciliar { get; set; }
        public virtual DbSet<Municipio> Municipio { get; set; }
        public virtual DbSet<Naturezajuridica> Naturezajuridica { get; set; }
        public virtual DbSet<Orcamentoconfig> Orcamentoconfig { get; set; }
        public virtual DbSet<Pagamentosexecutados> Pagamentosexecutados { get; set; }
        public virtual DbSet<Pais> Pais { get; set; }
        public virtual DbSet<Perfil> Perfil { get; set; }
        public virtual DbSet<Postoadministrativo> Postoadministrativo { get; set; }
        public virtual DbSet<Processoativo> Processoativo { get; set; }
        public virtual DbSet<Processoconfig> Processoconfig { get; set; }
        public virtual DbSet<Regime> Regime { get; set; }
        public virtual DbSet<RelMovimentosporconciliarMovimentos> RelMovimentosporconciliarMovimentos { get; set; }
        public virtual DbSet<Relcodigocontaagrupamentoconfig> Relcodigocontaagrupamentoconfig { get; set; }
        public virtual DbSet<Relentidaderesplegal> Relentidaderesplegal { get; set; }
        public virtual DbSet<Relentidadetrabalhador> Relentidadetrabalhador { get; set; }
        public virtual DbSet<Relperfilfuncionalidade> Relperfilfuncionalidade { get; set; }
        public virtual DbSet<Relprocessoconfigperfil> Relprocessoconfigperfil { get; set; }
        public virtual DbSet<Relprocessoconfigtarefa> Relprocessoconfigtarefa { get; set; }
        public virtual DbSet<Reltarefacomponente> Reltarefacomponente { get; set; }
        public virtual DbSet<Reltipodecontaorcamentoconfig> Reltipodecontaorcamentoconfig { get; set; }
        public virtual DbSet<Relutilizadordepartamento> Relutilizadordepartamento { get; set; }
        public virtual DbSet<Relutilizadorperfil> Relutilizadorperfil { get; set; }
        public virtual DbSet<Reservacredito> Reservacredito { get; set; }
        public virtual DbSet<Responsavellegal> Responsavellegal { get; set; }
        public virtual DbSet<Responsavellegalhist> Responsavellegalhist { get; set; }
        public virtual DbSet<Sectoractividade> Sectoractividade { get; set; }
        public virtual DbSet<Subclassificacao> Subclassificacao { get; set; }
        public virtual DbSet<Suco> Suco { get; set; }
        public virtual DbSet<Suspensoes> Suspensoes { get; set; }
        public virtual DbSet<Tarefa> Tarefa { get; set; }
        public virtual DbSet<Tarefaativo> Tarefaativo { get; set; }
        public virtual DbSet<Taxajuromensal> Taxajuromensal { get; set; }
        public virtual DbSet<Trabalhador> Trabalhador { get; set; }
        public virtual DbSet<Utilizador> Utilizador { get; set; }
        public virtual DbSet<Utilizadortoken> Utilizadortoken { get; set; }

        public virtual DbSet<Departamento> Departamento { get; set; }
        public virtual DbSet<Institution> Institution { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Latin1_General_CI_AS");

            modelBuilder.Entity<Actividadeeconomica>(entity =>
            {
                entity.HasKey(e => e.IdActivEconomica)
                    .HasName("PK__ACTIVIDA__1D8985AD7F5BF76E");

                entity.ToTable("ACTIVIDADEECONOMICA");

                entity.Property(e => e.IdActivEconomica).HasColumnName("idActivEconomica");

                entity.Property(e => e.Codigo)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("codigo");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.Descricao)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("descricao");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");
            });

            modelBuilder.Entity<Agrupamentoconfig>(entity =>
            {
                entity.ToTable("AGRUPAMENTOCONFIG");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Codigo)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("codigo");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.Designacao)
                    .IsRequired()
                    .HasMaxLength(75)
                    .IsUnicode(false)
                    .HasColumnName("designacao");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.ParentFk).HasColumnName("parent_fk");

                entity.Property(e => e.ReltipoDeContaOrcamentoConfigFk).HasColumnName("reltipoDeContaOrcamentoConfig_fk");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.HasOne(d => d.ParentFkNavigation)
                    .WithMany(p => p.InverseParentFkNavigation)
                    .HasForeignKey(d => d.ParentFk)
                    .HasConstraintName("parent_fk");

                entity.HasOne(d => d.ReltipoDeContaOrcamentoConfigFkNavigation)
                    .WithMany(p => p.Agrupamentoconfig)
                    .HasForeignKey(d => d.ReltipoDeContaOrcamentoConfigFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("reltipoDeContaOrcamentoConfig_fk");
            });

            modelBuilder.Entity<Aldeia>(entity =>
            {
                entity.HasKey(e => e.IdAldeia)
                    .HasName("PK__ALDEIA__BE9B4E81626DE74B");

                entity.ToTable("ALDEIA");

                entity.Property(e => e.IdAldeia).HasColumnName("idAldeia");

                entity.Property(e => e.AldeiaSucoFk).HasColumnName("aldeia_suco_fk");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("nome");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.HasOne(d => d.AldeiaSucoFkNavigation)
                    .WithMany(p => p.Aldeia)
                    .HasForeignKey(d => d.AldeiaSucoFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("aldeia_suco_fk");
            });

            modelBuilder.Entity<Camposeditaveis>(entity =>
            {
                entity.HasKey(e => e.IdCampoEditavel)
                    .HasName("PK__CAMPOSED__0DD13097F481FAE6");

                entity.ToTable("CAMPOSEDITAVEIS");

                entity.Property(e => e.IdCampoEditavel).HasColumnName("idCampoEditavel");

                entity.Property(e => e.CampoPaiFk).HasColumnName("campoPai_fk");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.DominioFk).HasColumnName("dominio_fk");

                entity.Property(e => e.DominioString)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("dominioString");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("nome");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.HasOne(d => d.CampoPaiFkNavigation)
                    .WithMany(p => p.InverseCampoPaiFkNavigation)
                    .HasForeignKey(d => d.CampoPaiFk)
                    .HasConstraintName("camposEditaveis_camposEditaveis_fk");

                entity.HasOne(d => d.DominioFkNavigation)
                    .WithMany(p => p.Camposeditaveis)
                    .HasForeignKey(d => d.DominioFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("camposEditaveis_dominio_fk");
            });

            modelBuilder.Entity<Centrocusto>(entity =>
            {
                entity.ToTable("CENTROCUSTO");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.DataFim)
                    .HasColumnType("datetime")
                    .HasColumnName("dataFim");

                entity.Property(e => e.DataInicio)
                    .HasColumnType("datetime")
                    .HasColumnName("dataInicio");

                entity.Property(e => e.Descricao)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("descricao");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.OrcamentoconfigFk).HasColumnName("orcamentoconfig_Fk");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.HasOne(d => d.OrcamentoconfigFkNavigation)
                    .WithMany(p => p.Centrocusto)
                    .HasForeignKey(d => d.OrcamentoconfigFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CentroCustoOrcamentoConfig");
            });

            modelBuilder.Entity<Classificacao>(entity =>
            {
                entity.ToTable("CLASSIFICACAO");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("nome");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");
            });

            modelBuilder.Entity<Codigoconta>(entity =>
            {
                entity.ToTable("CODIGOCONTA");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Codigo)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("codigo");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.Designacao)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("designacao");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.InitialValue).HasColumnType("decimal(20, 2)");

                entity.Property(e => e.InitialValueDate).HasColumnType("datetime");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.IsCredit).HasColumnName("isCredit");

                entity.Property(e => e.OrcamentoConfigFk).HasColumnName("orcamentoConfig_fk");

                entity.Property(e => e.ParentFk).HasColumnName("parent_fk");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.HasOne(d => d.OrcamentoConfigFkNavigation)
                    .WithMany(p => p.Codigoconta)
                    .HasForeignKey(d => d.OrcamentoConfigFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("codigoconta_orcamentoConfig_fk");

                entity.HasOne(d => d.ParentFkNavigation)
                    .WithMany(p => p.InverseParentFkNavigation)
                    .HasForeignKey(d => d.ParentFk)
                    .HasConstraintName("codigoconta_parent_fk");
            });

            modelBuilder.Entity<Componente>(entity =>
            {
                entity.ToTable("COMPONENTE");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.Descricao)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("descricao");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");
            });

            modelBuilder.Entity<Componenteaccoestarefa>(entity =>
            {
                entity.ToTable("COMPONENTEACCOESTAREFA");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ApelidoDaTarefa)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("apelidoDaTarefa");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.TarefaFk).HasColumnName("tarefa_fk");

                entity.Property(e => e.TarefaSeguirFk).HasColumnName("tarefa_seguir_fk");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.HasOne(d => d.TarefaFkNavigation)
                    .WithMany(p => p.ComponenteaccoestarefaTarefaFkNavigation)
                    .HasForeignKey(d => d.TarefaFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tarefa_accoes_fk");

                entity.HasOne(d => d.TarefaSeguirFkNavigation)
                    .WithMany(p => p.ComponenteaccoestarefaTarefaSeguirFkNavigation)
                    .HasForeignKey(d => d.TarefaSeguirFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tarefa_seguir_fk");
            });

            modelBuilder.Entity<ComponenteaccoestarefaRegisto>(entity =>
            {
                entity.ToTable("COMPONENTEACCOESTAREFA_REGISTO");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.TarefaAtivoFk).HasColumnName("tarefaAtivo_fk");

                entity.Property(e => e.TarefaSeguirFk).HasColumnName("tarefaSeguir_fk");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.HasOne(d => d.TarefaAtivoFkNavigation)
                    .WithMany(p => p.ComponenteaccoestarefaRegistoTarefaAtivoFkNavigation)
                    .HasForeignKey(d => d.TarefaAtivoFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("acoesTarefa_tarefaAtivo_fk");

                entity.HasOne(d => d.TarefaSeguirFkNavigation)
                    .WithMany(p => p.ComponenteaccoestarefaRegistoTarefaSeguirFkNavigation)
                    .HasForeignKey(d => d.TarefaSeguirFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("acoesTarefa_tarefaSeguir_fk");
            });

            modelBuilder.Entity<Componentecarregardocumento>(entity =>
            {
                entity.ToTable("COMPONENTECARREGARDOCUMENTO");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.DocumentoFk).HasColumnName("documento_fk");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.Obrigatorio).HasColumnName("obrigatorio");

                entity.Property(e => e.TarefaFk).HasColumnName("tarefa_fk");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.HasOne(d => d.DocumentoFkNavigation)
                    .WithMany(p => p.Componentecarregardocumento)
                    .HasForeignKey(d => d.DocumentoFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("documento_fk");

                entity.HasOne(d => d.TarefaFkNavigation)
                    .WithMany(p => p.Componentecarregardocumento)
                    .HasForeignKey(d => d.TarefaFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tarefa_carregarDocumento_fk");
            });

            modelBuilder.Entity<ComponenteclassificacaosubRegisto>(entity =>
            {
                entity.ToTable("COMPONENTECLASSIFICACAOSUB_REGISTO");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.SubClassificacaoFk).HasColumnName("subClassificacao_fk");

                entity.Property(e => e.TarefaAtivoFk).HasColumnName("tarefaAtivo_fk");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.HasOne(d => d.SubClassificacaoFkNavigation)
                    .WithMany(p => p.ComponenteclassificacaosubRegisto)
                    .HasForeignKey(d => d.SubClassificacaoFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("componenteClassific_subClassificacao_fk");

                entity.HasOne(d => d.TarefaAtivoFkNavigation)
                    .WithMany(p => p.ComponenteclassificacaosubRegisto)
                    .HasForeignKey(d => d.TarefaAtivoFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("classificacao_tarefaAtivo_fk");
            });

            modelBuilder.Entity<Componenteclassificacaosubclassific>(entity =>
            {
                entity.ToTable("COMPONENTECLASSIFICACAOSUBCLASSIFIC");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.SubclassificacaoFk).HasColumnName("subclassificacao_fk");

                entity.Property(e => e.TarefaFk).HasColumnName("tarefa_fk");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.HasOne(d => d.SubclassificacaoFkNavigation)
                    .WithMany(p => p.Componenteclassificacaosubclassific)
                    .HasForeignKey(d => d.SubclassificacaoFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("subclassificacao_fk");

                entity.HasOne(d => d.TarefaFkNavigation)
                    .WithMany(p => p.Componenteclassificacaosubclassific)
                    .HasForeignKey(d => d.TarefaFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tarefa_classificacao_fk");
            });

            modelBuilder.Entity<Componenteconciliacaomovimentos>(entity =>
            {
                entity.ToTable("COMPONENTECONCILIACAOMOVIMENTOS");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.PermissaoConciliar).HasColumnName("permissaoConciliar");

                entity.Property(e => e.PermissaoDesfazerConciliar).HasColumnName("permissaoDesfazerConciliar");

                entity.Property(e => e.PermissaoMovimentosBancarios).HasColumnName("permissaoMovimentosBancarios");

                entity.Property(e => e.PermissaoMovimentosConciliar).HasColumnName("permissaoMovimentosConciliar");

                entity.Property(e => e.PermissaoSelecionarMovimentos).HasColumnName("permissaoSelecionarMovimentos");

                entity.Property(e => e.PermissaoVerMovimentosAconciliar).HasColumnName("permissaoVerMovimentosAConciliar");

                entity.Property(e => e.TarefaFk).HasColumnName("tarefa_fk");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.HasOne(d => d.TarefaFkNavigation)
                    .WithMany(p => p.Componenteconciliacaomovimentos)
                    .HasForeignKey(d => d.TarefaFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ComponentConciliacaoMovimentos_tarefa");
            });

            modelBuilder.Entity<Componentecontroleacesso>(entity =>
            {
                entity.ToTable("COMPONENTECONTROLEACESSO");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.PerfilFk).HasColumnName("perfil_fk");

                entity.Property(e => e.TarefaFk).HasColumnName("tarefa_fk");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.Property(e => e.UtilizadorFk).HasColumnName("utilizador_fk");

                entity.HasOne(d => d.PerfilFkNavigation)
                    .WithMany(p => p.Componentecontroleacesso)
                    .HasForeignKey(d => d.PerfilFk)
                    .HasConstraintName("perfil_controloAcesso_fk");

                entity.HasOne(d => d.TarefaFkNavigation)
                    .WithMany(p => p.Componentecontroleacesso)
                    .HasForeignKey(d => d.TarefaFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tarefa_controloAcesso_fk");

                entity.HasOne(d => d.UtilizadorFkNavigation)
                    .WithMany(p => p.Componentecontroleacesso)
                    .HasForeignKey(d => d.UtilizadorFk)
                    .HasConstraintName("utilizador_controloAcesso_fk");
            });

            modelBuilder.Entity<Componentedespesa>(entity =>
            {
                entity.ToTable("COMPONENTEDESPESA");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.EmitirOrdemPagamento).HasColumnName("emitirOrdemPagamento");

                entity.Property(e => e.ExecutarPagamentos).HasColumnName("executarPagamentos");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.RegistarDespesa).HasColumnName("registarDespesa");

                entity.Property(e => e.TarefaFk).HasColumnName("tarefa_fk");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.Property(e => e.VisualiazarDespesaC).HasColumnName("visualiazarDespesaC");

                entity.Property(e => e.VisualizarDespesaA).HasColumnName("visualizarDespesaA");

                entity.Property(e => e.VisualizarDespesaAparaC).HasColumnName("visualizarDespesaAParaC");

                entity.Property(e => e.VisualizarDespesaComCompromisso).HasColumnName("visualizarDespesaComCompromisso");

                entity.Property(e => e.VisualizarDespesaR).HasColumnName("visualizarDespesaR");

                entity.Property(e => e.VisualizarDespesaRparaA).HasColumnName("visualizarDespesaRParaA");

                entity.Property(e => e.VisualizarExecucaoDespesaCabimentada).HasColumnName("visualizarExecucaoDespesaCabimentada");

                entity.HasOne(d => d.TarefaFkNavigation)
                    .WithMany(p => p.Componentedespesa)
                    .HasForeignKey(d => d.TarefaFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ComponentDespesa_tarefa");
            });

            modelBuilder.Entity<ComponentedespesaRegisto>(entity =>
            {
                entity.ToTable("COMPONENTEDESPESA_REGISTO");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AgrupamentoConfigFk).HasColumnName("agrupamentoConfig_fk");
                entity.Property(e => e.ActidadeFk).HasColumnName("actidade_fk");
                //entity.Property(e => e.EconomicFk).HasColumnName("economic_fk");
                entity.Property(e => e.FuncionalFk).HasColumnName("funcional_fk");

                entity.Property(e => e.CentroCustoFk).HasColumnName("centroCusto_fk");

                entity.Property(e => e.CodigoContaFk).HasColumnName("codigoConta_fk");

                entity.Property(e => e.ComponenteOrcamentoRegistoFk).HasColumnName("componenteOrcamentoRegistoFk");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.DepartamentoFk).HasColumnName("departamento_fk");

                entity.Property(e => e.Descricao)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("descricao");

                entity.Property(e => e.Estado).HasColumnName("estado");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.TarefaActivoFk).HasColumnName("tarefaActivo_fk");

                entity.Property(e => e.TipoContaFk).HasColumnName("tipoConta_fk");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.Property(e => e.Valor)
                    .HasColumnType("decimal(20, 2)")
                    .HasColumnName("valor");

                entity.HasOne(d => d.AgrupamentoConfigFkNavigation)
                    .WithMany(p => p.ComponentedespesaRegisto)
                    .HasForeignKey(d => d.AgrupamentoConfigFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_componentDespesaRegisto_agrupConfig");

                entity.HasOne(d => d.CentroCustoFkNavigation)
                    .WithMany(p => p.ComponentedespesaRegisto)
                    .HasForeignKey(d => d.CentroCustoFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_componentDespesaRegisto_centroCusto");

                entity.HasOne(d => d.CodigoContaFkNavigation)
                    .WithMany(p => p.ComponentedespesaRegisto)
                    .HasForeignKey(d => d.CodigoContaFk)
                    .HasConstraintName("FK_componentDespesaRegisto_codigoConta");

                entity.HasOne(d => d.ComponenteOrcamentoRegistoFkNavigation)
                    .WithMany(p => p.ComponentedespesaRegisto)
                    .HasForeignKey(d => d.ComponenteOrcamentoRegistoFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_componenteOrcamentoRegisto_despesa");

                entity.HasOne(d => d.DepartamentoFkNavigation)
                    .WithMany(p => p.ComponentedespesaRegisto)
                    .HasForeignKey(d => d.DepartamentoFk)
                    .HasConstraintName("FK_componentDespesaRegisto_departamento");

                entity.HasOne(d => d.EstadoNavigation)
                    .WithMany(p => p.ComponentedespesaRegistoEstadoNavigation)
                    .HasForeignKey(d => d.Estado)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_componentDespesaRegisto_estado");

                entity.HasOne(d => d.TarefaActivoFkNavigation)
                    .WithMany(p => p.ComponentedespesaRegisto)
                    .HasForeignKey(d => d.TarefaActivoFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_componentDespesaRegisto_tarefaAtivo");

                entity.HasOne(d => d.TipoContaFkNavigation)
                    .WithMany(p => p.ComponentedespesaRegistoTipoContaFkNavigation)
                    .HasForeignKey(d => d.TipoContaFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_componentDespesaRegisto_tipoConta");
            });

            modelBuilder.Entity<ComponentedocumentoRegisto>(entity =>
            {
                entity.ToTable("COMPONENTEDOCUMENTO_REGISTO");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.Documento)
                    .IsRequired()
                    .HasColumnName("documento");

                entity.Property(e => e.DocumentoFk).HasColumnName("documento_fk");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.Numero)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("numero");

                entity.Property(e => e.TarefaAtivoFk).HasColumnName("tarefaAtivo_fk");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.HasOne(d => d.DocumentoFkNavigation)
                    .WithMany(p => p.ComponentedocumentoRegisto)
                    .HasForeignKey(d => d.DocumentoFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("componenteDocumento_documento_fk");

                entity.HasOne(d => d.TarefaAtivoFkNavigation)
                    .WithMany(p => p.ComponentedocumentoRegisto)
                    .HasForeignKey(d => d.TarefaAtivoFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("documento_tarefaAtivo_fk");
            });

            modelBuilder.Entity<Componenteorcamento>(entity =>
            {
                entity.ToTable("COMPONENTEORCAMENTO");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.PermissaoAprovacao).HasColumnName("permissaoAprovacao");

                entity.Property(e => e.PermissaoDatas).HasColumnName("permissaoDatas");

                entity.Property(e => e.PermissaoDetalhes).HasColumnName("permissaoDetalhes");

                entity.Property(e => e.PermissaoInsercoes).HasColumnName("permissaoInsercoes");

                entity.Property(e => e.TarefaFk).HasColumnName("tarefa_fk");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.HasOne(d => d.TarefaFkNavigation)
                    .WithMany(p => p.Componenteorcamento)
                    .HasForeignKey(d => d.TarefaFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ComponentOrcamento_tarefa");
            });

            modelBuilder.Entity<ComponenteorcamentoRegisto>(entity =>
            {
                entity.ToTable("COMPONENTEORCAMENTO_REGISTO");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Aprovado).HasColumnName("aprovado");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.DataFim)
                    .HasColumnType("datetime")
                    .HasColumnName("dataFim");

                entity.Property(e => e.DataInicio)
                    .HasColumnType("datetime")
                    .HasColumnName("dataInicio");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.OrcamentoConfigFk).HasColumnName("orcamentoConfigFK");

                entity.Property(e => e.OrcamentoRetificadoFk).HasColumnName("orcamentoRetificado_fk");

                entity.Property(e => e.TarefaActivoFk).HasColumnName("tarefaActivo_fk");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.HasOne(d => d.OrcamentoConfigFkNavigation)
                    .WithMany(p => p.ComponenteorcamentoRegisto)
                    .HasForeignKey(d => d.OrcamentoConfigFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_componentORcamentoRegisto_orcamentoConfig");

                entity.HasOne(d => d.OrcamentoRetificadoFkNavigation)
                    .WithMany(p => p.InverseOrcamentoRetificadoFkNavigation)
                    .HasForeignKey(d => d.OrcamentoRetificadoFk)
                    .HasConstraintName("FK_componentOrcamentoRegisto_componentOrcamentoRegisto");

                entity.HasOne(d => d.TarefaActivoFkNavigation)
                    .WithMany(p => p.ComponenteorcamentoRegisto)
                    .HasForeignKey(d => d.TarefaActivoFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_componentORcamentoRegisto_tarefaActivo");
            });

            modelBuilder.Entity<Componenteorcamentovalor>(entity =>
            {
                entity.ToTable("COMPONENTEORCAMENTOVALOR");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AgrupamentoFk).HasColumnName("agrupamento_fk");
                entity.Property(e => e.ActidadeFk).HasColumnName("actidade_fk");
                //entity.Property(e => e.EconomicFk).HasColumnName("economic_fk");
                entity.Property(e => e.FuncionalFk).HasColumnName("funcional_fk");

                entity.Property(e => e.CentroCustoFk).HasColumnName("centroCusto_fk");

                entity.Property(e => e.ComponenteOrcamentoRegistoFk).HasColumnName("componenteOrcamentoRegisto_fk");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.DepartamentoFk).HasColumnName("departamento_fk");
                //entity.Property(e => e.InstitutionId).HasColumnName("InstitutionId");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.TipoContaFk).HasColumnName("tipoConta_fk");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.Property(e => e.Valor)
                    .HasColumnType("decimal(20, 2)")
                    .HasColumnName("valor");

                entity.HasOne(d => d.InstitutionFkNavigation)
                    .WithMany(p => p.Componenteorcamentovalor)
                    .HasForeignKey(d => d.InstitutionId)
                    .HasConstraintName("FK_componentOrcamentoRegisto_institution");


                entity.HasOne(d => d.AgrupamentoFkNavigation)
                    .WithMany(p => p.Componenteorcamentovalor)
                    .HasForeignKey(d => d.AgrupamentoFk)
                    .HasConstraintName("FK_ComponentOrcamentoRegisto_agrupamento");

                entity.HasOne(d => d.CentroCustoFkNavigation)
                    .WithMany(p => p.Componenteorcamentovalor)
                    .HasForeignKey(d => d.CentroCustoFk)
                    .HasConstraintName("FK_componentOrcamentoRegisto_centroCusto");

                entity.HasOne(d => d.ComponenteOrcamentoRegistoFkNavigation)
                    .WithMany(p => p.Componenteorcamentovalor)
                    .HasForeignKey(d => d.ComponenteOrcamentoRegistoFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_componenteOrcamentoValor_componenteOrcamentoRegisto");

                entity.HasOne(d => d.DepartamentoFkNavigation)
                    .WithMany(p => p.Componenteorcamentovalor)
                    .HasForeignKey(d => d.DepartamentoFk)
                    .HasConstraintName("FK_componentOrcamentoRegisto_departamento");

                entity.HasOne(d => d.TipoContaFkNavigation)
                    .WithMany(p => p.Componenteorcamentovalor)
                    .HasForeignKey(d => d.TipoContaFk)
                    .HasConstraintName("componenteOrcamento_tipoConta_fk");
            });

            modelBuilder.Entity<Componentereceita>(entity =>
            {
                entity.ToTable("COMPONENTERECEITA");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ClassificarMovSelecionados).HasColumnName("classificarMovSelecionados");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.SelecionarMovRecebidosParaRegisto).HasColumnName("selecionarMovRecebidosParaRegisto");

                entity.Property(e => e.TarefaFk).HasColumnName("tarefa_fk");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.Property(e => e.VerificarExecucaoOrcamentoEditarSelecao).HasColumnName("verificarExecucaoOrcamentoEditarSelecao");

                entity.HasOne(d => d.TarefaFkNavigation)
                    .WithMany(p => p.Componentereceita)
                    .HasForeignKey(d => d.TarefaFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ComponentReceita_tarefa");
            });

            modelBuilder.Entity<ComponentereceitaRegisto>(entity =>
            {
                entity.ToTable("COMPONENTERECEITA_REGISTO");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AgrupamentoConfigFk).HasColumnName("agrupamentoConfig_fk");

                entity.Property(e => e.CentroCustoFk).HasColumnName("centroCusto_fk");

                entity.Property(e => e.CodigoContaDebitoFk).HasColumnName("codigoContaDebito_fk");

                entity.Property(e => e.CodigoContaFk).HasColumnName("codigoConta_fk");

                entity.Property(e => e.ComponenteOrcamentoRegistoFk).HasColumnName("componenteOrcamentoRegistoFk");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.DepartamentoFk).HasColumnName("departamento_fk");

                entity.Property(e => e.Descricao)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("descricao");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.TarefaActivoFk).HasColumnName("tarefaActivo_fk");

                entity.Property(e => e.TipoContaFk).HasColumnName("tipoConta_fk");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.Property(e => e.Valor)
                    .HasColumnType("decimal(20, 2)")
                    .HasColumnName("valor");

                entity.HasOne(d => d.AgrupamentoConfigFkNavigation)
                    .WithMany(p => p.ComponentereceitaRegisto)
                    .HasForeignKey(d => d.AgrupamentoConfigFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_componentReceitaRegisto_agrupConfig");

                entity.HasOne(d => d.CentroCustoFkNavigation)
                    .WithMany(p => p.ComponentereceitaRegisto)
                    .HasForeignKey(d => d.CentroCustoFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_componentReceitaRegisto_centroCusto");

                entity.HasOne(d => d.CodigoContaDebitoFkNavigation)
                    .WithMany(p => p.ComponentereceitaRegistoCodigoContaDebitoFkNavigation)
                    .HasForeignKey(d => d.CodigoContaDebitoFk)
                    .HasConstraintName("componenteReceitaRegisto_codigoContaDebito_fk");

                entity.HasOne(d => d.CodigoContaFkNavigation)
                    .WithMany(p => p.ComponentereceitaRegistoCodigoContaFkNavigation)
                    .HasForeignKey(d => d.CodigoContaFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_componentReceitaRegisto_codigoConta");

                entity.HasOne(d => d.ComponenteOrcamentoRegistoFkNavigation)
                    .WithMany(p => p.ComponentereceitaRegisto)
                    .HasForeignKey(d => d.ComponenteOrcamentoRegistoFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_componenteOrcamentoRegisto_receita");

                entity.HasOne(d => d.DepartamentoFkNavigation)
                    .WithMany(p => p.ComponentereceitaRegisto)
                    .HasForeignKey(d => d.DepartamentoFk)
                    .HasConstraintName("FK_componentReceitaRegisto_departamento");

                entity.HasOne(d => d.TarefaActivoFkNavigation)
                    .WithMany(p => p.ComponentereceitaRegisto)
                    .HasForeignKey(d => d.TarefaActivoFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_componentReceitaRegisto_tarefaAtivo");

                entity.HasOne(d => d.TipoContaFkNavigation)
                    .WithMany(p => p.ComponentereceitaRegisto)
                    .HasForeignKey(d => d.TipoContaFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_componentReceitaRegisto_tipoConta");
            });

            modelBuilder.Entity<ComponentereceitaRegistoMovimentos>(entity =>
            {
                entity.ToTable("COMPONENTERECEITA_REGISTO_MOVIMENTOS");

                entity.Property(e => e.IndActivo)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.HasOne(d => d.ComponenteReceitaRegisto)
                    .WithMany(p => p.ComponentereceitaRegistoMovimentos)
                    .HasForeignKey(d => d.ComponenteReceitaRegistoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_COMPONENTERECEITA_REGISTO_MOVIMENTOS__COMPONENTERECEITA_REGISTO");

                entity.HasOne(d => d.RelMovimentosPorConciliarMovimentos)
                    .WithMany(p => p.ComponentereceitaRegistoMovimentos)
                    .HasForeignKey(d => d.RelMovimentosPorConciliarMovimentosId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_COMPONENTERECEITA_REGISTO_MOVIMENTOS__REL_MOVIMENTOSPORCONCILIAR_MOVIMENTOS");
            });

            modelBuilder.Entity<Componentetexto>(entity =>
            {
                entity.ToTable("COMPONENTETEXTO");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.Expandir1).HasColumnName("expandir1");

                entity.Property(e => e.Expandir2).HasColumnName("expandir2");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.Obrigatorio1).HasColumnName("obrigatorio1");

                entity.Property(e => e.Obrigatorio2).HasColumnName("obrigatorio2");

                entity.Property(e => e.ObrigatorioAoArquivar1).HasColumnName("obrigatorioAoArquivar1");

                entity.Property(e => e.ObrigatorioAoArquivar2).HasColumnName("obrigatorioAoArquivar2");

                entity.Property(e => e.QuantCaracteres1).HasColumnName("quantCaracteres1");

                entity.Property(e => e.QuantCaracteres2).HasColumnName("quantCaracteres2");

                entity.Property(e => e.TarefaFk).HasColumnName("tarefa_fk");

                entity.Property(e => e.Texto1).HasColumnName("texto1");

                entity.Property(e => e.Texto2).HasColumnName("texto2");

                entity.Property(e => e.Titulo1)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("titulo1");

                entity.Property(e => e.Titulo2)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("titulo2");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.HasOne(d => d.TarefaFkNavigation)
                    .WithMany(p => p.Componentetexto)
                    .HasForeignKey(d => d.TarefaFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tarefa_texto_fk");
            });

            modelBuilder.Entity<ComponentetextoRegisto>(entity =>
            {
                entity.ToTable("COMPONENTETEXTO_REGISTO");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.TarefaAtivoFk).HasColumnName("tarefaAtivo_fk");

                entity.Property(e => e.Texto)
                    .IsRequired()
                    .HasMaxLength(1000)
                    .IsUnicode(false)
                    .HasColumnName("texto");

                entity.Property(e => e.TituloTexto)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("tituloTexto");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.HasOne(d => d.TarefaAtivoFkNavigation)
                    .WithMany(p => p.ComponentetextoRegisto)
                    .HasForeignKey(d => d.TarefaAtivoFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("texto_tarefaAtivo_fk");
            });

            modelBuilder.Entity<Compromisso>(entity =>
            {
                entity.ToTable("COMPROMISSO");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ComponenteDespesaRegistoFk).HasColumnName("componenteDespesaRegisto_fk");

                entity.Property(e => e.Data)
                    .HasColumnType("datetime")
                    .HasColumnName("data");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("nome");

                entity.Property(e => e.TarefaAtivoFk).HasColumnName("tarefaAtivo_fk");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.Property(e => e.Valor)
                    .HasColumnType("decimal(20, 2)")
                    .HasColumnName("valor");

                entity.HasOne(d => d.ComponenteDespesaRegistoFkNavigation)
                    .WithMany(p => p.Compromisso)
                    .HasForeignKey(d => d.ComponenteDespesaRegistoFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Compromissos_despesaRegisto");

                entity.HasOne(d => d.TarefaAtivoFkNavigation)
                    .WithMany(p => p.Compromisso)
                    .HasForeignKey(d => d.TarefaAtivoFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Compromisso_tarefaAtivo");
            });

            modelBuilder.Entity<Contabancaria>(entity =>
            {
                entity.ToTable("CONTABANCARIA");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CampoPaiFk).HasColumnName("campoPai_fk");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.Descricao)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("descricao");

                entity.Property(e => e.EntidadeBancaria)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("entidadeBancaria");

                entity.Property(e => e.Iban)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("iban");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.Numero)
                    .IsRequired()
                    .HasMaxLength(21)
                    .IsUnicode(false)
                    .HasColumnName("numero");

                entity.Property(e => e.Swift)
                    .IsRequired()
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("swift");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");
            });

            modelBuilder.Entity<Contacorrente>(entity =>
            {
                entity.HasKey(e => e.IdContaCorrente)
                    .HasName("PK__CONTACOR__5CDA6508AA556055");

                entity.ToTable("CONTACORRENTE");

                entity.Property(e => e.IdContaCorrente).HasColumnName("idContaCorrente");

                entity.Property(e => e.ContaCorrenteEntidadeFk).HasColumnName("contaCorrente_entidade_fk");

                entity.Property(e => e.ContaCorrenteTaxaJuroFk).HasColumnName("contaCorrente_taxa_juro_fk");

                entity.Property(e => e.ContaCorrenteTrabalhadorFk).HasColumnName("contaCorrente_trabalhador_fk");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.DataVencimento)
                    .HasColumnType("datetime")
                    .HasColumnName("dataVencimento");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.MesAno)
                    .HasColumnType("datetime")
                    .HasColumnName("mesAno");

                entity.Property(e => e.PagoEm)
                    .HasColumnType("datetime")
                    .HasColumnName("pagoEm");

                entity.Property(e => e.SituacaoPagamento).HasColumnName("situacaoPagamento");

                entity.Property(e => e.TipoDivida).HasColumnName("tipoDivida");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.Property(e => e.ValorEntidade)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("valorEntidade");

                entity.Property(e => e.ValorJuros)
                    .HasColumnType("decimal(30, 2)")
                    .HasColumnName("valorJuros")
                    .HasComputedColumnSql("([dbo].[calcularJurosContaCorrente]([idContaCorrente]))", false);

                entity.Property(e => e.ValorTotal)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("valorTotal");

                entity.Property(e => e.ValorTrabalhador)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("valorTrabalhador");

                entity.HasOne(d => d.ContaCorrenteEntidadeFkNavigation)
                    .WithMany(p => p.Contacorrente)
                    .HasForeignKey(d => d.ContaCorrenteEntidadeFk)
                    .HasConstraintName("contaCorrente_entidade_fk");

                entity.HasOne(d => d.ContaCorrenteTaxaJuroFkNavigation)
                    .WithMany(p => p.Contacorrente)
                    .HasForeignKey(d => d.ContaCorrenteTaxaJuroFk)
                    .HasConstraintName("contaCorrente_taxa_juro_fk");

                entity.HasOne(d => d.ContaCorrenteTrabalhadorFkNavigation)
                    .WithMany(p => p.Contacorrente)
                    .HasForeignKey(d => d.ContaCorrenteTrabalhadorFk)
                    .HasConstraintName("contaCorrente_trabalhador_fk");

                entity.HasOne(d => d.SituacaoPagamentoNavigation)
                    .WithMany(p => p.ContacorrenteSituacaoPagamentoNavigation)
                    .HasForeignKey(d => d.SituacaoPagamento)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("situacaoPagamento");

                entity.HasOne(d => d.TipoDividaNavigation)
                    .WithMany(p => p.ContacorrenteTipoDividaNavigation)
                    .HasForeignKey(d => d.TipoDivida)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tipoDivida");
            });

            modelBuilder.Entity<Contacto>(entity =>
            {
                entity.HasKey(e => e.IdContacto)
                    .HasName("PK__CONTACTO__4B1329C7E57298CD");

                entity.ToTable("CONTACTO");

                entity.Property(e => e.IdContacto).HasColumnName("idContacto");

                entity.Property(e => e.ContactoEntidadeFk).HasColumnName("contacto_entidade_fk");

                entity.Property(e => e.ContactoTrabalhadorFk).HasColumnName("contacto_trabalhador_fk");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.FlagImportado).HasColumnName("flagImportado");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.Telemovel)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("telemovel");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.HasOne(d => d.ContactoEntidadeFkNavigation)
                    .WithMany(p => p.Contacto)
                    .HasForeignKey(d => d.ContactoEntidadeFk)
                    .HasConstraintName("contacto_entidade_fk");

                entity.HasOne(d => d.ContactoTrabalhadorFkNavigation)
                    .WithMany(p => p.Contacto)
                    .HasForeignKey(d => d.ContactoTrabalhadorFk)
                    .HasConstraintName("contacto_trabalhador_fk");
            });

            modelBuilder.Entity<Declaracaoremuneracao>(entity =>
            {
                entity.HasKey(e => e.IdDeclaracao)
                    .HasName("PK__DECLARAC__1AD507824523159D");

                entity.ToTable("DECLARACAOREMUNERACAO");

                entity.Property(e => e.IdDeclaracao).HasColumnName("idDeclaracao");

                entity.Property(e => e.ContaCorrenteFk).HasColumnName("conta_corrente_fk");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.DecimoTerceiro)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("decimoTerceiro");

                entity.Property(e => e.DeclaracaoRelEntidadeTrabalhadorFk).HasColumnName("declaracao_relEntidadeTrabalhador_FK");

                entity.Property(e => e.DiasContrato)
                    .HasColumnType("decimal(3, 1)")
                    .HasColumnName("diasContrato");

                entity.Property(e => e.DiasEfecTrabalhados)
                    .HasColumnType("decimal(3, 1)")
                    .HasColumnName("diasEfecTrabalhados");

                entity.Property(e => e.DiasParentalidade).HasColumnName("diasParentalidade");

                entity.Property(e => e.DiasTrabcontabSegSocial)
                    .HasColumnType("decimal(3, 1)")
                    .HasColumnName("diasTrabcontabSegSocial");

                entity.Property(e => e.FaltasInjustific).HasColumnName("faltasInjustific");

                entity.Property(e => e.FlagImportado).HasColumnName("flagImportado");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.MesAno)
                    .HasColumnType("datetime")
                    .HasColumnName("mesAno");

                entity.Property(e => e.NacionalidadeFk).HasDefaultValueSql("((11))");

                entity.Property(e => e.Oficioso).HasColumnName("oficioso");

                entity.Property(e => e.RegimeFk).HasColumnName("RegimeFK");

                entity.Property(e => e.RemunDeclarada)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("remunDeclarada");

                entity.Property(e => e.SexoFk)
                    .HasColumnName("SexoFK")
                    .HasDefaultValueSql("((9))");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.HasOne(d => d.ContaCorrenteFkNavigation)
                    .WithMany(p => p.Declaracaoremuneracao)
                    .HasForeignKey(d => d.ContaCorrenteFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("conta_corrente_fk");

                entity.HasOne(d => d.DeclaracaoRelEntidadeTrabalhadorFkNavigation)
                    .WithMany(p => p.Declaracaoremuneracao)
                    .HasForeignKey(d => d.DeclaracaoRelEntidadeTrabalhadorFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("declaracao_relEntidadeTrabalhador_FK");

                entity.HasOne(d => d.NacionalidadeFkNavigation)
                    .WithMany(p => p.DeclaracaoremuneracaoNacionalidadeFkNavigation)
                    .HasForeignKey(d => d.NacionalidadeFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__DECLARACA__Nacio__15DA3E5D");

                entity.HasOne(d => d.RegimeFkNavigation)
                    .WithMany(p => p.Declaracaoremuneracao)
                    .HasForeignKey(d => d.RegimeFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__DECLARACA__Regim__16CE6296");

                entity.HasOne(d => d.SexoFkNavigation)
                    .WithMany(p => p.DeclaracaoremuneracaoSexoFkNavigation)
                    .HasForeignKey(d => d.SexoFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__DECLARACA__SexoF__12FDD1B2");
            });

            modelBuilder.Entity<Departamento>(entity =>
            {
                entity.ToTable("DEPARTAMENTO");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("nome");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");
            });

            modelBuilder.Entity<Destinatario>(entity =>
            {
                entity.ToTable("DESTINATARIO");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.EntidadeFk).HasColumnName("entidade_fk");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.Morada)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("morada");

                entity.Property(e => e.Niss)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("NISS");

                entity.Property(e => e.Nome)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("nome");

                entity.Property(e => e.Tin)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("TIN");

                entity.Property(e => e.TrabalhadorFk).HasColumnName("trabalhador_fk");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.HasOne(d => d.EntidadeFkNavigation)
                    .WithMany(p => p.Destinatario)
                    .HasForeignKey(d => d.EntidadeFk)
                    .HasConstraintName("destinatario_entidade_fk");

                entity.HasOne(d => d.TrabalhadorFkNavigation)
                    .WithMany(p => p.Destinatario)
                    .HasForeignKey(d => d.TrabalhadorFk)
                    .HasConstraintName("destinatario_trabalhador_fk");
            });

            modelBuilder.Entity<Dispensacontributiva>(entity =>
            {
                entity.HasKey(e => e.IdDispContributiva)
                    .HasName("PK__DISPENSA__B0F8A33498308BA5");

                entity.ToTable("DISPENSACONTRIBUTIVA");

                entity.Property(e => e.IdDispContributiva).HasColumnName("idDispContributiva");

                entity.Property(e => e.Ano).HasColumnName("ano");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.Percentagem)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("percentagem");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");
            });

            modelBuilder.Entity<Documentoidentificacao>(entity =>
            {
                entity.HasKey(e => e.IdDocIdentificacao)
                    .HasName("PK__DOCUMENT__06B327213335BAFD");

                entity.ToTable("DOCUMENTOIDENTIFICACAO");

                entity.Property(e => e.IdDocIdentificacao).HasColumnName("idDocIdentificacao");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.DataEmissao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataEmissao");

                entity.Property(e => e.DataValidade)
                    .HasColumnType("datetime")
                    .HasColumnName("dataValidade");

                entity.Property(e => e.Documento)
                    .IsRequired()
                    .HasColumnName("documento");

                entity.Property(e => e.FlagImportado).HasColumnName("flagImportado");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.LocalEmissao)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("localEmissao");

                entity.Property(e => e.NomeDocumento)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("nomeDocumento");

                entity.Property(e => e.Numero)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("numero");

                entity.Property(e => e.RespLegalDocumentoFk).HasColumnName("respLegal_documento_fk");

                entity.Property(e => e.TpDocIdentificacao).HasColumnName("tpDocIdentificacao");

                entity.Property(e => e.TrabalhadorDocumetoFk).HasColumnName("trabalhador_documeto_fk");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.HasOne(d => d.RespLegalDocumentoFkNavigation)
                    .WithMany(p => p.Documentoidentificacao)
                    .HasForeignKey(d => d.RespLegalDocumentoFk)
                    .HasConstraintName("respLegal_documento_fk");

                entity.HasOne(d => d.TpDocIdentificacaoNavigation)
                    .WithMany(p => p.Documentoidentificacao)
                    .HasForeignKey(d => d.TpDocIdentificacao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tpDocIdentificacao");

                entity.HasOne(d => d.TrabalhadorDocumetoFkNavigation)
                    .WithMany(p => p.Documentoidentificacao)
                    .HasForeignKey(d => d.TrabalhadorDocumetoFk)
                    .HasConstraintName("trabalhador_documento_fk");
            });

            modelBuilder.Entity<Dominio>(entity =>
            {
                entity.HasKey(e => e.IdDominio)
                    .HasName("PK__DOMINIO__0C9D6C8F372DC123");

                entity.ToTable("DOMINIO");

                entity.Property(e => e.IdDominio).HasColumnName("idDominio");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.Descricao)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("descricao");

                entity.Property(e => e.Dominio1)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("dominio");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.Property(e => e.Valor).HasColumnName("valor");
            });

            modelBuilder.Entity<Entidadeempregadora>(entity =>
            {
                entity.HasKey(e => e.IdEntidadeEmpreg)
                    .HasName("PK__ENTIDADE__A4FAF38AF59837F4");

                entity.ToTable("ENTIDADEEMPREGADORA");

                entity.Property(e => e.IdEntidadeEmpreg).HasColumnName("idEntidadeEmpreg");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.DataFimActiv)
                    .HasColumnType("datetime")
                    .HasColumnName("dataFimActiv");

                entity.Property(e => e.DataInicioActiv)
                    .HasColumnType("datetime")
                    .HasColumnName("dataInicioActiv");

                entity.Property(e => e.DataInicioTrabServico)
                    .HasColumnType("datetime")
                    .HasColumnName("dataInicioTrabServico");

                entity.Property(e => e.DtHoraUltimoAcesso)
                    .HasColumnType("datetime")
                    .HasColumnName("dtHoraUltimoAcesso");

                entity.Property(e => e.DtInscricao)
                    .HasColumnType("datetime")
                    .HasColumnName("dtInscricao");

                entity.Property(e => e.EntidadeActEconomicaFk).HasColumnName("entidade_actEconomica_fk");

                entity.Property(e => e.EntidadeNatJuridicaFk).HasColumnName("entidade_natJuridica_fk");

                entity.Property(e => e.EntidadeSectorActFk).HasColumnName("entidade_sectorAct_fk");

                entity.Property(e => e.FlagImportado).HasColumnName("flagImportado");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.Niss)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("NISS");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("nome");

                entity.Property(e => e.NumTrabalhador).HasColumnName("numTrabalhador");

                entity.Property(e => e.SituacInscricao)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("situacInscricao")
                    .IsFixedLength(true);

                entity.Property(e => e.Tin)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("TIN");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.HasOne(d => d.EntidadeActEconomicaFkNavigation)
                    .WithMany(p => p.Entidadeempregadora)
                    .HasForeignKey(d => d.EntidadeActEconomicaFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("entidade_actEconomica_fk");

                entity.HasOne(d => d.EntidadeNatJuridicaFkNavigation)
                    .WithMany(p => p.Entidadeempregadora)
                    .HasForeignKey(d => d.EntidadeNatJuridicaFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("entidade_natJuridica_fk");

                entity.HasOne(d => d.EntidadeSectorActFkNavigation)
                    .WithMany(p => p.Entidadeempregadora)
                    .HasForeignKey(d => d.EntidadeSectorActFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("entidade_sectorAct_fk");
            });

            modelBuilder.Entity<Escalao>(entity =>
            {
                entity.HasKey(e => e.IdEscalao)
                    .HasName("PK__ESCALAO__A21A5AFCC036B54E");

                entity.ToTable("ESCALAO");

                entity.Property(e => e.IdEscalao).HasColumnName("idEscalao");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.DescNivelEscalao)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("descNivelEscalao");

                entity.Property(e => e.EscalaoRegimeFk).HasColumnName("escalao_regime_fk");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.Property(e => e.Valor)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("valor");

                entity.HasOne(d => d.EscalaoRegimeFkNavigation)
                    .WithMany(p => p.Escalao)
                    .HasForeignKey(d => d.EscalaoRegimeFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ESCALAO__escalao__1F63A897");
            });

            modelBuilder.Entity<ExcelImporterRel>(entity =>
            {
                entity.ToTable("EXCEL_IMPORTER_REL");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Data)
                    .HasMaxLength(4000)
                    .IsUnicode(false)
                    .HasColumnName("data");

                entity.Property(e => e.Date)
                    .HasColumnType("datetime")
                    .HasColumnName("date");

                entity.Property(e => e.ImportId).HasColumnName("importId");

                entity.Property(e => e.RelId).HasColumnName("rel_id");
            });

            modelBuilder.Entity<Funcionalidade>(entity =>
            {
                entity.ToTable("FUNCIONALIDADE");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.Descricao)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("descricao");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");
            });

            modelBuilder.Entity<Guiapagamento>(entity =>
            {
                entity.HasKey(e => e.IdGuia)
                    .HasName("PK__GUIAPAGA__9C667173F89C1547");

                entity.ToTable("GUIAPAGAMENTO");

                entity.Property(e => e.IdGuia).HasColumnName("idGuia");

                entity.Property(e => e.ComprovativoPag).HasColumnName("comprovativoPAG");

                entity.Property(e => e.ContaCorrenteId).HasColumnName("contaCorrenteId");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataComprovPag)
                    .HasColumnType("datetime")
                    .HasColumnName("dataComprovPag");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.Descricao)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("descricao");

                entity.Property(e => e.DtEmissao)
                    .HasColumnType("datetime")
                    .HasColumnName("dtEmissao");

                entity.Property(e => e.DtValidade)
                    .HasColumnType("datetime")
                    .HasColumnName("dtValidade");

                entity.Property(e => e.GuiaEntidadeFk).HasColumnName("guia_entidade_fk");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.IndPago).HasColumnName("indPago");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.MesAno)
                    .HasColumnType("datetime")
                    .HasColumnName("mesAno");

                entity.Property(e => e.NumDocumento)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("numDocumento");

                entity.Property(e => e.TipoGuia)
                    .HasColumnName("tipoGuia")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.Property(e => e.Valor)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("valor");

                entity.Property(e => e.ValorComprovPag)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("valorComprovPag");

                entity.Property(e => e.ValorJuros)
                    .HasColumnType("decimal(30, 2)")
                    .HasColumnName("valorJuros")
                    .HasComputedColumnSql("([dbo].[calcularJurosContaCorrente]([contaCorrenteId]))", false);

                entity.Property(e => e.ValorJurosFixo)
                    .HasColumnType("decimal(32, 2)")
                    .HasColumnName("valorJurosFixo");

                entity.HasOne(d => d.ContaCorrente)
                    .WithMany(p => p.Guiapagamento)
                    .HasForeignKey(d => d.ContaCorrenteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GUIAPAGAMENTO_CONTACORRENTE");

                entity.HasOne(d => d.GuiaEntidadeFkNavigation)
                    .WithMany(p => p.Guiapagamento)
                    .HasForeignKey(d => d.GuiaEntidadeFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("guia_entidade_fk");

                entity.HasOne(d => d.IndPagoNavigation)
                    .WithMany(p => p.GuiapagamentoIndPagoNavigation)
                    .HasForeignKey(d => d.IndPago)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("indPago");

                entity.HasOne(d => d.TipoGuiaNavigation)
                    .WithMany(p => p.GuiapagamentoTipoGuiaNavigation)
                    .HasForeignKey(d => d.TipoGuia)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__GUIAPAGAM__tipoG__2057CCD0");
            });

            modelBuilder.Entity<Inssestrangeiro>(entity =>
            {
                entity.HasKey(e => e.IdInssestrang)
                    .HasName("PK__INSSESTR__90A64927D9882654");

                entity.ToTable("INSSESTRANGEIRO");

                entity.Property(e => e.IdInssestrang).HasColumnName("idINSSEstrang");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.Documento).HasColumnName("documento");

                entity.Property(e => e.EstrangeiroEntidadeFk).HasColumnName("estrangeiro_entidade_fk");

                entity.Property(e => e.EstrangeiroPaisFk).HasColumnName("estrangeiro_pais_fk");

                entity.Property(e => e.EstrangeiroTrabalhadorFk).HasColumnName("estrangeiro_trabalhador_fk");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.IndBenfAtualmente).HasColumnName("indBenfAtualmente");

                entity.Property(e => e.IndDecontAtualmente).HasColumnName("indDecontAtualmente");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.Nissestrangeiro)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("NISSEstrangeiro");

                entity.Property(e => e.NomeDocumento)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("nomeDocumento");

                entity.Property(e => e.NomeSsestrangeiro)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("nomeSSEstrangeiro");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.HasOne(d => d.EstrangeiroEntidadeFkNavigation)
                    .WithMany(p => p.Inssestrangeiro)
                    .HasForeignKey(d => d.EstrangeiroEntidadeFk)
                    .HasConstraintName("estrangeiro_entidade_fk");

                entity.HasOne(d => d.EstrangeiroPaisFkNavigation)
                    .WithMany(p => p.Inssestrangeiro)
                    .HasForeignKey(d => d.EstrangeiroPaisFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("estrangeiro_pais_fk");

                entity.HasOne(d => d.EstrangeiroTrabalhadorFkNavigation)
                    .WithMany(p => p.Inssestrangeiro)
                    .HasForeignKey(d => d.EstrangeiroTrabalhadorFk)
                    .HasConstraintName("estrangeiro_trabalhador_fk");
            });

            modelBuilder.Entity<Morada>(entity =>
            {
                entity.HasKey(e => e.IdMorada)
                    .HasName("PK__MORADA__96A64147F89C146C");

                entity.ToTable("MORADA");

                entity.Property(e => e.IdMorada).HasColumnName("idMorada");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.EntidadeMoradaFk).HasColumnName("entidade_morada_fk");

                entity.Property(e => e.FlagImportado).HasColumnName("flagImportado");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.MoradaAldeiaFk).HasColumnName("morada_aldeia_fk");

                entity.Property(e => e.MoradaPaisFk).HasColumnName("morada_pais_fk");

                entity.Property(e => e.MoradaPrincipal).HasColumnName("moradaPrincipal");

                entity.Property(e => e.NumPorta)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("numPorta");

                entity.Property(e => e.Rua)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("rua");

                entity.Property(e => e.TrabalhadorMoradaFk).HasColumnName("trabalhador_morada_fk");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.HasOne(d => d.EntidadeMoradaFkNavigation)
                    .WithMany(p => p.Morada)
                    .HasForeignKey(d => d.EntidadeMoradaFk)
                    .HasConstraintName("entidade_morada_fk");

                entity.HasOne(d => d.MoradaAldeiaFkNavigation)
                    .WithMany(p => p.Morada)
                    .HasForeignKey(d => d.MoradaAldeiaFk)
                    .HasConstraintName("morada_aldeia_fk");

                entity.HasOne(d => d.MoradaPaisFkNavigation)
                    .WithMany(p => p.Morada)
                    .HasForeignKey(d => d.MoradaPaisFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("morada_pais_fk");

                entity.HasOne(d => d.TrabalhadorMoradaFkNavigation)
                    .WithMany(p => p.Morada)
                    .HasForeignKey(d => d.TrabalhadorMoradaFk)
                    .HasConstraintName("trabalhador_morada_fk");
            });

            modelBuilder.Entity<Movimentobancario>(entity =>
            {
                entity.ToTable("MOVIMENTOBANCARIO");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CampoPaiFk).HasColumnName("campoPai_fk");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.Descricao)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("descricao");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.TipoMovimento).HasColumnName("tipoMovimento");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.HasOne(d => d.TipoMovimentoNavigation)
                    .WithMany(p => p.Movimentobancario)
                    .HasForeignKey(d => d.TipoMovimento)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MovimentoBancario_dominio");
            });

            modelBuilder.Entity<Movimentosbancarios>(entity =>
            {
                entity.ToTable("MOVIMENTOSBANCARIOS");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CaixaFk).HasColumnName("caixa_fk");

                entity.Property(e => e.ContaFk).HasColumnName("conta_fk");

                entity.Property(e => e.Credito)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("credito");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.DataValor)
                    .HasColumnType("datetime")
                    .HasColumnName("dataValor");

                entity.Property(e => e.Debito)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("debito");

                entity.Property(e => e.Descricao)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("descricao");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.TarefaFk).HasColumnName("tarefa_fk");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.HasOne(d => d.CaixaFkNavigation)
                    .WithMany(p => p.Movimentosbancarios)
                    .HasForeignKey(d => d.CaixaFk)
                    .HasConstraintName("FK_MovimentosBancarios_caixa");

                entity.HasOne(d => d.ContaFkNavigation)
                    .WithMany(p => p.Movimentosbancarios)
                    .HasForeignKey(d => d.ContaFk)
                    .HasConstraintName("FK_MovimentosBancarios_conta");

                entity.HasOne(d => d.TarefaFkNavigation)
                    .WithMany(p => p.Movimentosbancarios)
                    .HasForeignKey(d => d.TarefaFk)
                    .HasConstraintName("FK_MovimentosBancarios_tarefa");
            });

            modelBuilder.Entity<Movimentosporconciliar>(entity =>
            {
                entity.ToTable("MOVIMENTOSPORCONCILIAR");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AgrupamentoConfigFk).HasColumnName("agrupamentoConfig_fk");

                entity.Property(e => e.CentroCustoFk).HasColumnName("centroCusto_fk");

                entity.Property(e => e.CodigoContaCreditoFk).HasColumnName("codigoContaCredito_fk");

                entity.Property(e => e.CodigoContaDebitoFk).HasColumnName("codigoContaDebito_fk");

                entity.Property(e => e.Comprovativo).HasColumnName("comprovativo");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.DepartamentoFk).HasColumnName("departamento_fk");

                entity.Property(e => e.IndActivo)
                    .IsRequired()
                    .HasColumnName("indActivo")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.IsReceita).HasColumnName("isReceita");

                entity.Property(e => e.MovimentoBancarioFk).HasColumnName("movimentoBancario_fk");

                entity.Property(e => e.NomeComprovativo)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("nomeComprovativo");

                entity.Property(e => e.NumeroDocumento)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("numeroDocumento");

                entity.Property(e => e.TarefaAtivoFk).HasColumnName("tarefaAtivo_fk");

                entity.Property(e => e.TipoContaFk).HasColumnName("tipoConta_fk");

                entity.Property(e => e.TipoDocumento)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("tipoDocumento");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.Property(e => e.Valor)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("valor");

                entity.HasOne(d => d.AgrupamentoConfigFkNavigation)
                    .WithMany(p => p.Movimentosporconciliar)
                    .HasForeignKey(d => d.AgrupamentoConfigFk)
                    .HasConstraintName("movimentosPorConciliar_agrupamentoConfig_fk");

                entity.HasOne(d => d.CentroCustoFkNavigation)
                    .WithMany(p => p.Movimentosporconciliar)
                    .HasForeignKey(d => d.CentroCustoFk)
                    .HasConstraintName("movimentosPorConciliar_centroCusto_fk");

                entity.HasOne(d => d.CodigoContaCreditoFkNavigation)
                    .WithMany(p => p.MovimentosporconciliarCodigoContaCreditoFkNavigation)
                    .HasForeignKey(d => d.CodigoContaCreditoFk)
                    .HasConstraintName("movimentosPorConciliar_codigoContaCredito_fk");

                entity.HasOne(d => d.CodigoContaDebitoFkNavigation)
                    .WithMany(p => p.MovimentosporconciliarCodigoContaDebitoFkNavigation)
                    .HasForeignKey(d => d.CodigoContaDebitoFk)
                    .HasConstraintName("movimentosPorConciliar_codigoContaDebito_fk");

                entity.HasOne(d => d.DepartamentoFkNavigation)
                    .WithMany(p => p.Movimentosporconciliar)
                    .HasForeignKey(d => d.DepartamentoFk)
                    .HasConstraintName("movimentosPorConciliar_departamento_fk");

                entity.HasOne(d => d.Guiapagamento)
                    .WithMany(p => p.Movimentosporconciliar)
                    .HasForeignKey(d => d.GuiapagamentoId)
                    .HasConstraintName("FK_MOVIMENTOSPORCONCILIAR_GUIAPAGAMENTO");

                entity.HasOne(d => d.MovimentoBancarioFkNavigation)
                    .WithMany(p => p.Movimentosporconciliar)
                    .HasForeignKey(d => d.MovimentoBancarioFk)
                    .HasConstraintName("FK_MOVIMENTOSPORCONCILIAR_movimentoBancario");

                entity.HasOne(d => d.ReservaCredito)
                    .WithMany(p => p.Movimentosporconciliar)
                    .HasForeignKey(d => d.ReservaCreditoId)
                    .HasConstraintName("FK_MOVIMENTOSPORCONCILIAR_RESERVACREDITO");

                entity.HasOne(d => d.TarefaAtivoFkNavigation)
                    .WithMany(p => p.Movimentosporconciliar)
                    .HasForeignKey(d => d.TarefaAtivoFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MOVIMENTOSPORCONCILIAR_tarefaAtivo");

                entity.HasOne(d => d.TipoContaFkNavigation)
                    .WithMany(p => p.Movimentosporconciliar)
                    .HasForeignKey(d => d.TipoContaFk)
                    .HasConstraintName("movimentosPorConciliar_tipoConta_fk");
            });

            modelBuilder.Entity<Municipio>(entity =>
            {
                entity.HasKey(e => e.IdMunicipio)
                    .HasName("PK__MUNICIPI__FD10E40008EFC357");

                entity.ToTable("MUNICIPIO");

                entity.Property(e => e.IdMunicipio).HasColumnName("idMunicipio");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("nome");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");
            });

            modelBuilder.Entity<Naturezajuridica>(entity =>
            {
                entity.HasKey(e => e.IdNatJuridica)
                    .HasName("PK__NATUREZA__6D0E610D3E26F897");

                entity.ToTable("NATUREZAJURIDICA");

                entity.Property(e => e.IdNatJuridica).HasColumnName("idNatJuridica");

                entity.Property(e => e.Codigo)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("codigo");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.Descricao)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("descricao");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");
            });

            modelBuilder.Entity<Orcamentoconfig>(entity =>
            {
                entity.ToTable("ORCAMENTOCONFIG");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.DataFim)
                    .HasColumnType("datetime")
                    .HasColumnName("dataFim");

                entity.Property(e => e.DataInicio)
                    .HasColumnType("datetime")
                    .HasColumnName("dataInicio");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");
            });

            modelBuilder.Entity<Pagamentosexecutados>(entity =>
            {
                entity.ToTable("PAGAMENTOSEXECUTADOS");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CodigoContaCreditoFk).HasColumnName("codigoContaCredito_fk");

                entity.Property(e => e.CodigoContaDebitoFk).HasColumnName("codigoContaDebito_fk");

                entity.Property(e => e.CompromissoFk).HasColumnName("compromisso_fk");

                entity.Property(e => e.CreditoExecucaoFk).HasColumnName("creditoExecucao_fk");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.DataExecucao).HasColumnType("datetime");

                entity.Property(e => e.DataObrigacao).HasColumnType("datetime");

                entity.Property(e => e.DebitoExecucaoFk).HasColumnName("debitoExecucao_fk");

                entity.Property(e => e.DestinatarioFk).HasColumnName("destinatario_fk");

                entity.Property(e => e.Estado).HasColumnName("estado");

                entity.Property(e => e.Iban)
                    .HasMaxLength(24)
                    .IsUnicode(false)
                    .HasColumnName("IBAN");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.NumeroConta)
                    .HasMaxLength(22)
                    .IsUnicode(false);

                entity.Property(e => e.NumeroPagamento)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("numeroPagamento");

                entity.Property(e => e.ProcessoAtivoFk).HasColumnName("processoAtivo_fk");

                entity.Property(e => e.Swift)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("swift");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.Property(e => e.ValorExecutado)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("valorExecutado");

                entity.HasOne(d => d.CodigoContaCreditoFkNavigation)
                    .WithMany(p => p.PagamentosexecutadosCodigoContaCreditoFkNavigation)
                    .HasForeignKey(d => d.CodigoContaCreditoFk)
                    .HasConstraintName("pagamentosExecutados_codigoContaCredito_fk");

                entity.HasOne(d => d.CodigoContaDebitoFkNavigation)
                    .WithMany(p => p.PagamentosexecutadosCodigoContaDebitoFkNavigation)
                    .HasForeignKey(d => d.CodigoContaDebitoFk)
                    .HasConstraintName("pagamentosExecutados_codigoContaDebito_fk");

                entity.HasOne(d => d.CompromissoFkNavigation)
                    .WithMany(p => p.Pagamentosexecutados)
                    .HasForeignKey(d => d.CompromissoFk)
                    .HasConstraintName("pagamentosExecutados_compromisso_fk");

                entity.HasOne(d => d.CreditoExecucaoFkNavigation)
                    .WithMany(p => p.PagamentosexecutadosCreditoExecucaoFkNavigation)
                    .HasForeignKey(d => d.CreditoExecucaoFk)
                    .HasConstraintName("pagamentosExecutados_creditoExecucao_fk");

                entity.HasOne(d => d.DebitoExecucaoFkNavigation)
                    .WithMany(p => p.PagamentosexecutadosDebitoExecucaoFkNavigation)
                    .HasForeignKey(d => d.DebitoExecucaoFk)
                    .HasConstraintName("pagamentosExecutados_debitoExecucao_fk");

                entity.HasOne(d => d.DestinatarioFkNavigation)
                    .WithMany(p => p.Pagamentosexecutados)
                    .HasForeignKey(d => d.DestinatarioFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("pagamentosExec_destinatario_fk");

                entity.HasOne(d => d.EstadoNavigation)
                    .WithMany(p => p.Pagamentosexecutados)
                    .HasForeignKey(d => d.Estado)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_pagamentosExec_estado");

                entity.HasOne(d => d.ProcessoAtivoFkNavigation)
                    .WithMany(p => p.Pagamentosexecutados)
                    .HasForeignKey(d => d.ProcessoAtivoFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("pagamentosExec_processoAtivo_fk");
            });

            modelBuilder.Entity<Pais>(entity =>
            {
                entity.HasKey(e => e.IdPais)
                    .HasName("PK__PAIS__BD2285E3A13F85D2");

                entity.ToTable("PAIS");

                entity.Property(e => e.IdPais).HasColumnName("idPais");

                entity.Property(e => e.Codigo)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("codigo");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Indicativo)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("indicativo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.Iso)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("iso");

                entity.Property(e => e.Iso3)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("iso3");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("nome");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");
            });

            modelBuilder.Entity<Perfil>(entity =>
            {
                entity.ToTable("PERFIL");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.Descricao)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("descricao");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");
            });

            modelBuilder.Entity<Postoadministrativo>(entity =>
            {
                entity.HasKey(e => e.IdPostoAdmin)
                    .HasName("PK__POSTOADM__EDCF9A5DE669CAB8");

                entity.ToTable("POSTOADMINISTRATIVO");

                entity.Property(e => e.IdPostoAdmin).HasColumnName("idPostoAdmin");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("nome");

                entity.Property(e => e.PostoAdminMunicipioFk).HasColumnName("postoAdmin_municipio_fk");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.HasOne(d => d.PostoAdminMunicipioFkNavigation)
                    .WithMany(p => p.Postoadministrativo)
                    .HasForeignKey(d => d.PostoAdminMunicipioFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("postoAdmin_municipio_fk");
            });

            modelBuilder.Entity<Processoativo>(entity =>
            {
                entity.ToTable("PROCESSOATIVO");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Arquivado).HasColumnName("arquivado");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.NumeroProcesso)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("numeroProcesso");

                entity.Property(e => e.ProcessoConfigFk).HasColumnName("processoConfig_fk");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.HasOne(d => d.ProcessoConfigFkNavigation)
                    .WithMany(p => p.Processoativo)
                    .HasForeignKey(d => d.ProcessoConfigFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("processoAtivo_processoConfig_fk");
            });

            modelBuilder.Entity<Processoconfig>(entity =>
            {
                entity.ToTable("PROCESSOCONFIG");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("nome");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");
            });

            modelBuilder.Entity<Regime>(entity =>
            {
                entity.HasKey(e => e.IdRegime)
                    .HasName("PK__REGIME__82DE393AEBDADAFF");

                entity.ToTable("REGIME");

                entity.Property(e => e.IdRegime).HasColumnName("idRegime");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.DataFim)
                    .HasColumnType("datetime")
                    .HasColumnName("dataFim");

                entity.Property(e => e.DataInicio)
                    .HasColumnType("datetime")
                    .HasColumnName("dataInicio");

                entity.Property(e => e.DataVencimento)
                    .HasColumnName("dataVencimento")
                    .HasDefaultValueSql("((20))");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.NomeRegime)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("nomeRegime");

                entity.Property(e => e.PercentEntidadeEmpreg)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("percentEntidadeEmpreg");

                entity.Property(e => e.PercentTrabalhador)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("percentTrabalhador");

                entity.Property(e => e.RegimePai).HasDefaultValueSql("((23))");

                entity.Property(e => e.TipoRegime).HasColumnName("tipoRegime");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.HasOne(d => d.RegimePaiNavigation)
                    .WithMany(p => p.RegimeRegimePaiNavigation)
                    .HasForeignKey(d => d.RegimePai)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__REGIME__RegimePa__3EDC53F0");

                entity.HasOne(d => d.TipoRegimeNavigation)
                    .WithMany(p => p.RegimeTipoRegimeNavigation)
                    .HasForeignKey(d => d.TipoRegime)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tipoRegime");
            });

            modelBuilder.Entity<RelMovimentosporconciliarMovimentos>(entity =>
            {
                entity.ToTable("REL_MOVIMENTOSPORCONCILIAR_MOVIMENTOS");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.Estado).HasColumnName("estado");

                entity.Property(e => e.GuiaPagamentoFk).HasColumnName("guiaPagamento_fk");

                entity.Property(e => e.IndActivo)
                    .IsRequired()
                    .HasColumnName("indActivo")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.MovimentoPorConciliarFk).HasColumnName("movimentoPorConciliar_fk");

                entity.Property(e => e.MovimentosBancariosFk).HasColumnName("movimentosBancarios_fk");

                entity.Property(e => e.PagamentosExecutadosFk).HasColumnName("pagamentosExecutados_fk");

                entity.Property(e => e.ReservaCreditoFk).HasColumnName("reservaCredito_fk");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.HasOne(d => d.GuiaPagamentoFkNavigation)
                    .WithMany(p => p.RelMovimentosporconciliarMovimentos)
                    .HasForeignKey(d => d.GuiaPagamentoFk)
                    .HasConstraintName("FK_REL_MOVIMENTOSPORCONCILIAR_MOVIMENTOS_guiaPagamento");

                entity.HasOne(d => d.MovimentoPorConciliarFkNavigation)
                    .WithMany(p => p.RelMovimentosporconciliarMovimentos)
                    .HasForeignKey(d => d.MovimentoPorConciliarFk)
                    .HasConstraintName("FK_REL_MOVIMENTOSPORCONCILIAR_MOVIMENTOS_movimentoPorConciliar");

                entity.HasOne(d => d.MovimentosBancariosFkNavigation)
                    .WithMany(p => p.RelMovimentosporconciliarMovimentos)
                    .HasForeignKey(d => d.MovimentosBancariosFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_REL_MOVIMENTOSPORCONCILIAR_MOVIMENTOS_movimentosBancarios");

                entity.HasOne(d => d.PagamentosExecutadosFkNavigation)
                    .WithMany(p => p.RelMovimentosporconciliarMovimentos)
                    .HasForeignKey(d => d.PagamentosExecutadosFk)
                    .HasConstraintName("FK_REL_MOVIMENTOSPORCONCILIAR_MOVIMENTOS_pagamentosExecutados");

                entity.HasOne(d => d.ReservaCreditoFkNavigation)
                    .WithMany(p => p.RelMovimentosporconciliarMovimentos)
                    .HasForeignKey(d => d.ReservaCreditoFk)
                    .HasConstraintName("FK_REL_MOVIMENTOSPORCONCILIAR_MOVIMENTOS_reservaCredito");
            });

            modelBuilder.Entity<Relcodigocontaagrupamentoconfig>(entity =>
            {
                entity.ToTable("RELCODIGOCONTAAGRUPAMENTOCONFIG");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AgrupamentoConfigFk).HasColumnName("agrupamentoConfig_fk");

                entity.Property(e => e.CodigocontaFk).HasColumnName("codigoconta_fk");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.HasOne(d => d.AgrupamentoConfigFkNavigation)
                    .WithMany(p => p.Relcodigocontaagrupamentoconfig)
                    .HasForeignKey(d => d.AgrupamentoConfigFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("relcodigocontaagrupamento_agrupamentoconfig_fk");

                entity.HasOne(d => d.CodigocontaFkNavigation)
                    .WithMany(p => p.Relcodigocontaagrupamentoconfig)
                    .HasForeignKey(d => d.CodigocontaFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("relcodigocontaagrupamento_codigocontaconfig_fk");
            });

            modelBuilder.Entity<Relentidaderesplegal>(entity =>
            {
                entity.HasKey(e => e.IdRel)
                    .HasName("PK__RELENTID__3C8791D25AFD69EB");

                entity.ToTable("RELENTIDADERESPLEGAL");

                entity.Property(e => e.IdRel).HasColumnName("idRel");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.DataFimFuncao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataFimFuncao");

                entity.Property(e => e.DataInicioFuncao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataInicioFuncao");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.RelEntidadeRespFk).HasColumnName("relEntidadeResp_fk");

                entity.Property(e => e.RelRespLegalEntFk).HasColumnName("relRespLegalEnt_fk");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.HasOne(d => d.RelEntidadeRespFkNavigation)
                    .WithMany(p => p.Relentidaderesplegal)
                    .HasForeignKey(d => d.RelEntidadeRespFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("relEntidadeResp_fk");

                entity.HasOne(d => d.RelRespLegalEntFkNavigation)
                    .WithMany(p => p.Relentidaderesplegal)
                    .HasForeignKey(d => d.RelRespLegalEntFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("relRespLegalEnt_fk");
            });

            modelBuilder.Entity<Relentidadetrabalhador>(entity =>
            {
                entity.HasKey(e => e.IdRel)
                    .HasName("PK__RELENTID__3C8791D2E6C2BD27");

                entity.ToTable("RELENTIDADETRABALHADOR");

                entity.Property(e => e.IdRel).HasColumnName("idRel");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.DiasSemana).HasColumnName("diasSemana");

                entity.Property(e => e.DtIniFimTrabalhador)
                    .HasColumnType("datetime")
                    .HasColumnName("dtIniFimTrabalhador");

                entity.Property(e => e.DtIniVincTrabalhador)
                    .HasColumnType("datetime")
                    .HasColumnName("dtIniVincTrabalhador");

                entity.Property(e => e.EntidadeFk).HasColumnName("entidade_fk");

                entity.Property(e => e.EscalaoFk).HasColumnName("escalao_fk");

                entity.Property(e => e.FlagImportado).HasColumnName("flagImportado");

                entity.Property(e => e.FuncPublico).HasColumnName("funcPublico");

                entity.Property(e => e.HorasSemana).HasColumnName("horasSemana");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.LeiLabAplicavel).HasColumnName("leiLabAplicavel");

                entity.Property(e => e.NaturezaContrato).HasColumnName("naturezaContrato");

                entity.Property(e => e.NumFuncPublico)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("numFuncPublico");

                entity.Property(e => e.Profissao).HasColumnName("profissao");

                entity.Property(e => e.ProfissaoOutro)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("profissaoOutro");

                entity.Property(e => e.RegimeFk).HasColumnName("regime_fk");

                entity.Property(e => e.TipoContrato).HasColumnName("tipoContrato");

                entity.Property(e => e.TrabalhadorFk).HasColumnName("trabalhador_fk");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.HasOne(d => d.EntidadeFkNavigation)
                    .WithMany(p => p.Relentidadetrabalhador)
                    .HasForeignKey(d => d.EntidadeFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("entidade_fk");

                entity.HasOne(d => d.EscalaoFkNavigation)
                    .WithMany(p => p.Relentidadetrabalhador)
                    .HasForeignKey(d => d.EscalaoFk)
                    .HasConstraintName("FK__RELENTIDA__escal__4959E263");

                entity.HasOne(d => d.LeiLabAplicavelNavigation)
                    .WithMany(p => p.RelentidadetrabalhadorLeiLabAplicavelNavigation)
                    .HasForeignKey(d => d.LeiLabAplicavel)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("leiLabAplicavel");

                entity.HasOne(d => d.NaturezaContratoNavigation)
                    .WithMany(p => p.RelentidadetrabalhadorNaturezaContratoNavigation)
                    .HasForeignKey(d => d.NaturezaContrato)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("naturezaContrato");

                entity.HasOne(d => d.ProfissaoNavigation)
                    .WithMany(p => p.RelentidadetrabalhadorProfissaoNavigation)
                    .HasForeignKey(d => d.Profissao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("profissao");

                entity.HasOne(d => d.RegimeFkNavigation)
                    .WithMany(p => p.RelentidadetrabalhadorRegimeFkNavigation)
                    .HasForeignKey(d => d.RegimeFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("regime_fk");

                entity.HasOne(d => d.TipoContratoNavigation)
                    .WithMany(p => p.RelentidadetrabalhadorTipoContratoNavigation)
                    .HasForeignKey(d => d.TipoContrato)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tipoContrato");

                entity.HasOne(d => d.TrabalhadorFkNavigation)
                    .WithMany(p => p.Relentidadetrabalhador)
                    .HasForeignKey(d => d.TrabalhadorFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("trabalhador_fk");
            });

            modelBuilder.Entity<Relperfilfuncionalidade>(entity =>
            {
                entity.ToTable("RELPERFILFUNCIONALIDADE");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Create).HasColumnName("create");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.Delete).HasColumnName("delete");

                entity.Property(e => e.FuncionalidadeFk).HasColumnName("funcionalidade_fk");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.PerfilFk).HasColumnName("perfil_fk");

                entity.Property(e => e.Read).HasColumnName("read");

                entity.Property(e => e.Update).HasColumnName("update");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.HasOne(d => d.FuncionalidadeFkNavigation)
                    .WithMany(p => p.Relperfilfuncionalidade)
                    .HasForeignKey(d => d.FuncionalidadeFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("funcionalidade_perfil_fk");

                entity.HasOne(d => d.PerfilFkNavigation)
                    .WithMany(p => p.Relperfilfuncionalidade)
                    .HasForeignKey(d => d.PerfilFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("perfil_funcionalidade_fk");
            });

            modelBuilder.Entity<Relprocessoconfigperfil>(entity =>
            {
                entity.ToTable("RELPROCESSOCONFIGPERFIL");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.PerfilFk).HasColumnName("perfil_fk");

                entity.Property(e => e.ProcessoConfigFk).HasColumnName("processoConfig_fk");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.HasOne(d => d.PerfilFkNavigation)
                    .WithMany(p => p.Relprocessoconfigperfil)
                    .HasForeignKey(d => d.PerfilFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("relPerfilProcessoCofig_fk");

                entity.HasOne(d => d.ProcessoConfigFkNavigation)
                    .WithMany(p => p.Relprocessoconfigperfil)
                    .HasForeignKey(d => d.ProcessoConfigFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("relprocessoConfigPerfil_fk");
            });

            modelBuilder.Entity<Relprocessoconfigtarefa>(entity =>
            {
                entity.ToTable("RELPROCESSOCONFIGTAREFA");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.ProcessoConfigFk).HasColumnName("processoConfig_fk");

                entity.Property(e => e.TarefaFk).HasColumnName("tarefa_fk");

                entity.Property(e => e.TarefaInicial).HasColumnName("tarefaInicial");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.HasOne(d => d.ProcessoConfigFkNavigation)
                    .WithMany(p => p.Relprocessoconfigtarefa)
                    .HasForeignKey(d => d.ProcessoConfigFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("processoConfig_fk");

                entity.HasOne(d => d.TarefaFkNavigation)
                    .WithMany(p => p.Relprocessoconfigtarefa)
                    .HasForeignKey(d => d.TarefaFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tarefaProcesso_fk");
            });

            modelBuilder.Entity<Reltarefacomponente>(entity =>
            {
                entity.ToTable("RELTAREFACOMPONENTE");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ComponenteFk).HasColumnName("componente_fk");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.Expandir).HasColumnName("expandir");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.Ordem).HasColumnName("ordem");

                entity.Property(e => e.TarefaFk).HasColumnName("tarefa_fk");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.HasOne(d => d.ComponenteFkNavigation)
                    .WithMany(p => p.Reltarefacomponente)
                    .HasForeignKey(d => d.ComponenteFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("componente_fk");

                entity.HasOne(d => d.TarefaFkNavigation)
                    .WithMany(p => p.Reltarefacomponente)
                    .HasForeignKey(d => d.TarefaFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tarefa_fk");
            });

            modelBuilder.Entity<Reltipodecontaorcamentoconfig>(entity =>
            {
                entity.ToTable("RELTIPODECONTAORCAMENTOCONFIG");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.OrcamentoConfigFk).HasColumnName("orcamentoConfig_fk");

                entity.Property(e => e.TipoContaFk).HasColumnName("tipoConta_fk");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.HasOne(d => d.OrcamentoConfigFkNavigation)
                    .WithMany(p => p.Reltipodecontaorcamentoconfig)
                    .HasForeignKey(d => d.OrcamentoConfigFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("orcamentoConfig_fk");

                entity.HasOne(d => d.TipoContaFkNavigation)
                    .WithMany(p => p.Reltipodecontaorcamentoconfig)
                    .HasForeignKey(d => d.TipoContaFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tipoConta_fk");
            });

            modelBuilder.Entity<Relutilizadordepartamento>(entity =>
            {
                entity.ToTable("RELUTILIZADORDEPARTAMENTO");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.DepartamentoFk).HasColumnName("departamento_fk");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.Property(e => e.UtilizadorFk).HasColumnName("utilizador_fk");

                entity.HasOne(d => d.DepartamentoFkNavigation)
                    .WithMany(p => p.Relutilizadordepartamento)
                    .HasForeignKey(d => d.DepartamentoFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("departamento_fk");

                entity.HasOne(d => d.UtilizadorFkNavigation)
                    .WithMany(p => p.Relutilizadordepartamento)
                    .HasForeignKey(d => d.UtilizadorFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("utilizador_fk");
            });

            modelBuilder.Entity<Relutilizadorperfil>(entity =>
            {
                entity.ToTable("RELUTILIZADORPERFIL");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.PerfilFk).HasColumnName("perfil_fk");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.Property(e => e.UtilizadorFk).HasColumnName("utilizador_fk");

                entity.HasOne(d => d.PerfilFkNavigation)
                    .WithMany(p => p.Relutilizadorperfil)
                    .HasForeignKey(d => d.PerfilFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("perfil_utilizador_fk");

                entity.HasOne(d => d.UtilizadorFkNavigation)
                    .WithMany(p => p.Relutilizadorperfil)
                    .HasForeignKey(d => d.UtilizadorFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("utilizador_perfil_fk");
            });

            modelBuilder.Entity<Reservacredito>(entity =>
            {
                entity.HasKey(e => e.IdReserva)
                    .HasName("PK__RESERVAC__94D104C83DCA6DDA");

                entity.ToTable("RESERVACREDITO");

                entity.Property(e => e.IdReserva).HasColumnName("idReserva");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.ReservaEntidadeFk).HasColumnName("reserva_entidade_fk");

                entity.Property(e => e.ReservaGuiaPagamentoFk).HasColumnName("reserva_guiaPagamento_fk");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.Property(e => e.Valor)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("valor");

                entity.HasOne(d => d.ReservaEntidadeFkNavigation)
                    .WithMany(p => p.Reservacredito)
                    .HasForeignKey(d => d.ReservaEntidadeFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("reserva_entidade_fk");

                entity.HasOne(d => d.ReservaGuiaPagamentoFkNavigation)
                    .WithMany(p => p.Reservacredito)
                    .HasForeignKey(d => d.ReservaGuiaPagamentoFk)
                    .HasConstraintName("reserva_guiaPagamento_fk");
            });

            modelBuilder.Entity<Responsavellegal>(entity =>
            {
                entity.HasKey(e => e.IdResponsavelLegal)
                    .HasName("PK__RESPONSA__4EBC17E791AD43B5");

                entity.ToTable("RESPONSAVELLEGAL");

                entity.Property(e => e.IdResponsavelLegal).HasColumnName("idResponsavelLegal");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.DataNasc)
                    .HasColumnType("datetime")
                    .HasColumnName("dataNasc");

                entity.Property(e => e.FlagImportado).HasColumnName("flagImportado");

                entity.Property(e => e.Funcao).HasColumnName("funcao");

                entity.Property(e => e.FuncaoOutro)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("funcaoOutro");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.IndFuncaoRem).HasColumnName("indFuncaoRem");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.Nacionalidade).HasColumnName("nacionalidade");

                entity.Property(e => e.Naturalidade)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("naturalidade");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("nome");

                entity.Property(e => e.RespLegalTabalhadorFk).HasColumnName("respLegal_tabalhador_fk");

                entity.Property(e => e.Sexo).HasColumnName("sexo");

                entity.Property(e => e.Tin)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("TIN");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.HasOne(d => d.FuncaoNavigation)
                    .WithMany(p => p.ResponsavellegalFuncaoNavigation)
                    .HasForeignKey(d => d.Funcao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("funcao");

                entity.HasOne(d => d.NacionalidadeNavigation)
                    .WithMany(p => p.ResponsavellegalNacionalidadeNavigation)
                    .HasForeignKey(d => d.Nacionalidade)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("nacionalidade");

                entity.HasOne(d => d.RespLegalTabalhadorFkNavigation)
                    .WithMany(p => p.Responsavellegal)
                    .HasForeignKey(d => d.RespLegalTabalhadorFk)
                    .HasConstraintName("respLegal_tabalhador_fk");

                entity.HasOne(d => d.SexoNavigation)
                    .WithMany(p => p.ResponsavellegalSexoNavigation)
                    .HasForeignKey(d => d.Sexo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("sexo");
            });

            modelBuilder.Entity<Responsavellegalhist>(entity =>
            {
                entity.HasKey(e => e.IdResPlegalHist)
                    .HasName("PK__RESPONSA__4B00BD7B85EDA35B");

                entity.ToTable("RESPONSAVELLEGALHIST");

                entity.Property(e => e.IdResPlegalHist).HasColumnName("idResPLegalHist");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.DataNasc)
                    .HasColumnType("datetime")
                    .HasColumnName("dataNasc");

                entity.Property(e => e.FlagImportado).HasColumnName("flagImportado");

                entity.Property(e => e.Funcao).HasColumnName("funcao");

                entity.Property(e => e.FuncaoOutro)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("funcaoOutro");

                entity.Property(e => e.IdEntidadeEmpreg).HasColumnName("idEntidadeEmpreg");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.IndAdesFacultInss).HasColumnName("indAdesFacultINSS");

                entity.Property(e => e.IndFuncaoRem).HasColumnName("indFuncaoRem");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.Nacionalidade).HasColumnName("nacionalidade");

                entity.Property(e => e.Naturalidade)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("naturalidade");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("nome");

                entity.Property(e => e.Sexo).HasColumnName("sexo");

                entity.Property(e => e.Tin)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("TIN");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");
            });

            modelBuilder.Entity<Sectoractividade>(entity =>
            {
                entity.HasKey(e => e.IdSectorActividade)
                    .HasName("PK__SECTORAC__898F1784B406BE4C");

                entity.ToTable("SECTORACTIVIDADE");

                entity.Property(e => e.IdSectorActividade).HasColumnName("idSectorActividade");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.Descricao)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("descricao");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");
            });

            modelBuilder.Entity<Subclassificacao>(entity =>
            {
                entity.ToTable("SUBCLASSIFICACAO");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ClassificacaoFk).HasColumnName("classificacao_fk");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("nome");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.HasOne(d => d.ClassificacaoFkNavigation)
                    .WithMany(p => p.Subclassificacao)
                    .HasForeignKey(d => d.ClassificacaoFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("classificacao_fk");
            });

            modelBuilder.Entity<Suco>(entity =>
            {
                entity.HasKey(e => e.IdSuco)
                    .HasName("PK__SUCO__C1F68DCF33BC352B");

                entity.ToTable("SUCO");

                entity.Property(e => e.IdSuco).HasColumnName("idSuco");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("nome");

                entity.Property(e => e.SucoPostoAdminFk).HasColumnName("suco_postoAdmin_fk");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.HasOne(d => d.SucoPostoAdminFkNavigation)
                    .WithMany(p => p.Suco)
                    .HasForeignKey(d => d.SucoPostoAdminFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("suco_postoAdmin_fk");
            });

            modelBuilder.Entity<Suspensoes>(entity =>
            {
                entity.HasKey(e => e.IdSuspensao)
                    .HasName("PK__SUSPENSO__EE6DF49C000C69EA");

                entity.ToTable("SUSPENSOES");

                entity.Property(e => e.IdSuspensao).HasColumnName("idSuspensao");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.DataFimSuspensao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataFimSuspensao");

                entity.Property(e => e.DataInicioSuspensao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataInicioSuspensao");

                entity.Property(e => e.EntidadeSuspensaoFk).HasColumnName("entidade_suspensao_fk");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.TrabalhadorSuspensaoFk).HasColumnName("trabalhador_suspensao_fk");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.HasOne(d => d.EntidadeSuspensaoFkNavigation)
                    .WithMany(p => p.Suspensoes)
                    .HasForeignKey(d => d.EntidadeSuspensaoFk)
                    .HasConstraintName("entidade_suspensao_fk");

                entity.HasOne(d => d.TrabalhadorSuspensaoFkNavigation)
                    .WithMany(p => p.Suspensoes)
                    .HasForeignKey(d => d.TrabalhadorSuspensaoFk)
                    .HasConstraintName("trabalhador_suspensao_fk");
            });

            modelBuilder.Entity<Tarefa>(entity =>
            {
                entity.ToTable("TAREFA");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BotaoArquivar).HasColumnName("botaoArquivar");

                entity.Property(e => e.CabecalhoProcesso).HasColumnName("cabecalhoProcesso");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.HistoricoDocumento).HasColumnName("historicoDocumento");

                entity.Property(e => e.HistoricoTexto).HasColumnName("historicoTexto");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("nome");

                entity.Property(e => e.NumeroTarefa)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("numeroTarefa");

                entity.Property(e => e.PrazoTarefa).HasColumnName("prazoTarefa");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");
            });

            modelBuilder.Entity<Tarefaativo>(entity =>
            {
                entity.ToTable("TAREFAATIVO");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.ProcessoAtivoFk).HasColumnName("processoAtivo_fk");

                entity.Property(e => e.TarefaconfigFk).HasColumnName("tarefaconfig_fk");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.Property(e => e.UtilizadorResponsavel).HasColumnName("utilizadorResponsavel");

                entity.HasOne(d => d.ProcessoAtivoFkNavigation)
                    .WithMany(p => p.Tarefaativo)
                    .HasForeignKey(d => d.ProcessoAtivoFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tarefaAtivo_procAtivo_fk");

                entity.HasOne(d => d.TarefaconfigFkNavigation)
                    .WithMany(p => p.Tarefaativo)
                    .HasForeignKey(d => d.TarefaconfigFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tarefaAtivo_tarefaConfig_fk");
            });

            modelBuilder.Entity<Taxajuromensal>(entity =>
            {
                entity.HasKey(e => e.IdTaxa)
                    .HasName("PK__TAXAJURO__C3E11D625510FA11");

                entity.ToTable("TAXAJUROMENSAL");

                entity.Property(e => e.IdTaxa).HasColumnName("idTaxa");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.DataFim).HasColumnType("datetime");

                entity.Property(e => e.DataInicio).HasColumnType("datetime");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.Percentagem)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("percentagem");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");
            });

            modelBuilder.Entity<Trabalhador>(entity =>
            {
                entity.HasKey(e => e.IdTrabalhador)
                    .HasName("PK__TRABALHA__37AD1D6CF54205F4");

                entity.ToTable("TRABALHADOR");

                entity.Property(e => e.IdTrabalhador).HasColumnName("idTrabalhador");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.DataNasc)
                    .HasColumnType("datetime")
                    .HasColumnName("dataNasc");

                entity.Property(e => e.EstadoCivil).HasColumnName("estadoCivil");

                entity.Property(e => e.FlagImportado).HasColumnName("flagImportado");

                entity.Property(e => e.IndDescNomeMae).HasColumnName("indDescNomeMae");

                entity.Property(e => e.IndDescNomePai).HasColumnName("indDescNomePai");

                entity.Property(e => e.Interno).HasColumnName("interno");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.NacionalidadeTrabalhador).HasColumnName("nacionalidadeTrabalhador");

                entity.Property(e => e.Naturalidade)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("naturalidade");

                entity.Property(e => e.Niss)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("NISS");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("nome");

                entity.Property(e => e.NomeMae)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("nomeMae");

                entity.Property(e => e.NomePai)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("nomePai");

                entity.Property(e => e.NumInscProvisoria)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("numInscProvisoria");

                entity.Property(e => e.SexoTrabalhador).HasColumnName("sexoTrabalhador");

                entity.Property(e => e.Tin)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("TIN");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.HasOne(d => d.EstadoCivilNavigation)
                    .WithMany(p => p.TrabalhadorEstadoCivilNavigation)
                    .HasForeignKey(d => d.EstadoCivil)
                    .HasConstraintName("estadoCivil");

                entity.HasOne(d => d.NacionalidadeTrabalhadorNavigation)
                    .WithMany(p => p.TrabalhadorNacionalidadeTrabalhadorNavigation)
                    .HasForeignKey(d => d.NacionalidadeTrabalhador)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("nacionalidadeTrabalhador");

                entity.HasOne(d => d.SexoTrabalhadorNavigation)
                    .WithMany(p => p.TrabalhadorSexoTrabalhadorNavigation)
                    .HasForeignKey(d => d.SexoTrabalhador)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("sexoTrabalhador");
            });

            modelBuilder.Entity<Utilizador>(entity =>
            {
                entity.HasKey(e => e.IdUtilizador)
                    .HasName("PK__UTILIZAD__9F6A66A35EBE01D2");

                entity.ToTable("UTILIZADOR");

                entity.Property(e => e.IdUtilizador).HasColumnName("idUtilizador");

                entity.Property(e => e.DataAlteracao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataAlteracao");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("datetime")
                    .HasColumnName("dataCriacao");

                entity.Property(e => e.IndActivo).HasColumnName("indActivo");

                entity.Property(e => e.Interno).HasColumnName("interno");

                entity.Property(e => e.Ipv6)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("ipv6");

                entity.Property(e => e.Locked).HasColumnName("locked");

                entity.Property(e => e.LoginAttempts).HasColumnName("loginAttempts");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(128)
                    .IsUnicode(false)
                    .HasColumnName("password");

                entity.Property(e => e.Salt)
                    .IsRequired()
                    .HasMaxLength(128)
                    .IsUnicode(false)
                    .HasColumnName("salt");

                entity.Property(e => e.TrabalhadorFk).HasColumnName("trabalhador_fk");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("username");

                entity.Property(e => e.UtilizadorAlteracao).HasColumnName("utilizadorAlteracao");

                entity.Property(e => e.UtilizadorCriacao).HasColumnName("utilizadorCriacao");

                entity.Property(e => e.UtilizadorEntidadeFk).HasColumnName("utilizador_entidade_fk");

                entity.HasOne(d => d.TrabalhadorFkNavigation)
                    .WithMany(p => p.Utilizador)
                    .HasForeignKey(d => d.TrabalhadorFk)
                    .HasConstraintName("utilizador_trabalhador_fk");

                entity.HasOne(d => d.UtilizadorEntidadeFkNavigation)
                    .WithMany(p => p.Utilizador)
                    .HasForeignKey(d => d.UtilizadorEntidadeFk)
                    .HasConstraintName("utilizador_entidade_fk");
            });

            modelBuilder.Entity<Utilizadortoken>(entity =>
            {
                entity.ToTable("UTILIZADORTOKEN");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IdUtilizador).HasColumnName("idUtilizador");

                entity.Property(e => e.IsRecover).HasColumnName("isRecover");

                entity.Property(e => e.Salt)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("salt");

                entity.Property(e => e.TokenString)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("tokenString");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
