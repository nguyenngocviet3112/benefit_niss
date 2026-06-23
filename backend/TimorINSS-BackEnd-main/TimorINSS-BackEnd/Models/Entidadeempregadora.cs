using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Entidadeempregadora
    {
        public Entidadeempregadora()
        {
            Contacorrente = new HashSet<Contacorrente>();
            Contacto = new HashSet<Contacto>();
            Destinatario = new HashSet<Destinatario>();
            Guiapagamento = new HashSet<Guiapagamento>();
            Inssestrangeiro = new HashSet<Inssestrangeiro>();
            Morada = new HashSet<Morada>();
            Relentidaderesplegal = new HashSet<Relentidaderesplegal>();
            Relentidadetrabalhador = new HashSet<Relentidadetrabalhador>();
            Reservacredito = new HashSet<Reservacredito>();
            Suspensoes = new HashSet<Suspensoes>();
            Utilizador = new HashSet<Utilizador>();
        }

        public int IdEntidadeEmpreg { get; set; }
        public string Nome { get; set; }
        public string Niss { get; set; }
        public string Tin { get; set; }
        public DateTime DtInscricao { get; set; }
        public string SituacInscricao { get; set; }
        public DateTime DataInicioActiv { get; set; }
        public DateTime? DataFimActiv { get; set; }
        public DateTime DataInicioTrabServico { get; set; }
        public int NumTrabalhador { get; set; }
        public int EntidadeNatJuridicaFk { get; set; }
        public int EntidadeActEconomicaFk { get; set; }
        public int EntidadeSectorActFk { get; set; }
        public DateTime DtHoraUltimoAcesso { get; set; }
        public bool FlagImportado { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }

        public virtual Actividadeeconomica EntidadeActEconomicaFkNavigation { get; set; }
        public virtual Naturezajuridica EntidadeNatJuridicaFkNavigation { get; set; }
        public virtual Sectoractividade EntidadeSectorActFkNavigation { get; set; }
        public virtual ICollection<Contacorrente> Contacorrente { get; set; }
        public virtual ICollection<Contacto> Contacto { get; set; }
        public virtual ICollection<Destinatario> Destinatario { get; set; }
        public virtual ICollection<Guiapagamento> Guiapagamento { get; set; }
        public virtual ICollection<Inssestrangeiro> Inssestrangeiro { get; set; }
        public virtual ICollection<Morada> Morada { get; set; }
        public virtual ICollection<Relentidaderesplegal> Relentidaderesplegal { get; set; }
        public virtual ICollection<Relentidadetrabalhador> Relentidadetrabalhador { get; set; }
        public virtual ICollection<Reservacredito> Reservacredito { get; set; }
        public virtual ICollection<Suspensoes> Suspensoes { get; set; }
        public virtual ICollection<Utilizador> Utilizador { get; set; }
    }
}
