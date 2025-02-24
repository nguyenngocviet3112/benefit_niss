using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Trabalhador
    {
        public Trabalhador()
        {
            Contacorrente = new HashSet<Contacorrente>();
            Contacto = new HashSet<Contacto>();
            Destinatario = new HashSet<Destinatario>();
            Documentoidentificacao = new HashSet<Documentoidentificacao>();
            Inssestrangeiro = new HashSet<Inssestrangeiro>();
            Morada = new HashSet<Morada>();
            Relentidadetrabalhador = new HashSet<Relentidadetrabalhador>();
            Responsavellegal = new HashSet<Responsavellegal>();
            Suspensoes = new HashSet<Suspensoes>();
            Utilizador = new HashSet<Utilizador>();
        }

        public int IdTrabalhador { get; set; }
        public string Nome { get; set; }
        public string Niss { get; set; }
        public string Tin { get; set; }
        public string NumInscProvisoria { get; set; }
        public DateTime DataNasc { get; set; }
        public string NomeMae { get; set; }
        public bool IndDescNomeMae { get; set; }
        public string NomePai { get; set; }
        public bool IndDescNomePai { get; set; }
        public int? EstadoCivil { get; set; }
        public string Naturalidade { get; set; }
        public bool FlagImportado { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }
        public int SexoTrabalhador { get; set; }
        public int NacionalidadeTrabalhador { get; set; }
        public bool Interno { get; set; }

        public virtual Dominio EstadoCivilNavigation { get; set; }
        public virtual Dominio NacionalidadeTrabalhadorNavigation { get; set; }
        public virtual Dominio SexoTrabalhadorNavigation { get; set; }
        public virtual ICollection<Contacorrente> Contacorrente { get; set; }
        public virtual ICollection<Contacto> Contacto { get; set; }
        public virtual ICollection<Destinatario> Destinatario { get; set; }
        public virtual ICollection<Documentoidentificacao> Documentoidentificacao { get; set; }
        public virtual ICollection<Inssestrangeiro> Inssestrangeiro { get; set; }
        public virtual ICollection<Morada> Morada { get; set; }
        public virtual ICollection<Relentidadetrabalhador> Relentidadetrabalhador { get; set; }
        public virtual ICollection<Responsavellegal> Responsavellegal { get; set; }
        public virtual ICollection<Suspensoes> Suspensoes { get; set; }
        public virtual ICollection<Utilizador> Utilizador { get; set; }
    }
}
