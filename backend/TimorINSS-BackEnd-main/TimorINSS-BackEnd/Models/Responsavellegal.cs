using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Responsavellegal
    {
        public Responsavellegal()
        {
            Documentoidentificacao = new HashSet<Documentoidentificacao>();
            Relentidaderesplegal = new HashSet<Relentidaderesplegal>();
        }

        public int IdResponsavelLegal { get; set; }
        public string Nome { get; set; }
        public string Tin { get; set; }
        public DateTime DataNasc { get; set; }
        public int Nacionalidade { get; set; }
        public string Naturalidade { get; set; }
        public int Sexo { get; set; }
        public bool IndFuncaoRem { get; set; }
        public bool IndActivo { get; set; }
        public bool FlagImportado { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }
        public int? RespLegalTabalhadorFk { get; set; }
        public int Funcao { get; set; }
        public string FuncaoOutro { get; set; }

        public virtual Dominio FuncaoNavigation { get; set; }
        public virtual Dominio NacionalidadeNavigation { get; set; }
        public virtual Trabalhador RespLegalTabalhadorFkNavigation { get; set; }
        public virtual Dominio SexoNavigation { get; set; }
        public virtual ICollection<Documentoidentificacao> Documentoidentificacao { get; set; }
        public virtual ICollection<Relentidaderesplegal> Relentidaderesplegal { get; set; }
    }
}
