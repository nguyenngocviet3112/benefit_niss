using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Relentidadetrabalhador
    {
        public Relentidadetrabalhador()
        {
            Declaracaoremuneracao = new HashSet<Declaracaoremuneracao>();
        }

        public int IdRel { get; set; }
        public int EntidadeFk { get; set; }
        public int TrabalhadorFk { get; set; }
        public int TipoContrato { get; set; }
        public int NaturezaContrato { get; set; }
        public int LeiLabAplicavel { get; set; }
        public int? HorasSemana { get; set; }
        public int? DiasSemana { get; set; }
        public DateTime DtIniVincTrabalhador { get; set; }
        public DateTime? DtIniFimTrabalhador { get; set; }
        public bool FuncPublico { get; set; }
        public string NumFuncPublico { get; set; }
        public bool FlagImportado { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }
        public int RegimeFk { get; set; }
        public int? EscalaoFk { get; set; }
        public int Profissao { get; set; }
        public string ProfissaoOutro { get; set; }

        public virtual Entidadeempregadora EntidadeFkNavigation { get; set; }
        public virtual Escalao EscalaoFkNavigation { get; set; }
        public virtual Dominio LeiLabAplicavelNavigation { get; set; }
        public virtual Dominio NaturezaContratoNavigation { get; set; }
        public virtual Dominio ProfissaoNavigation { get; set; }
        public virtual Dominio RegimeFkNavigation { get; set; }
        public virtual Dominio TipoContratoNavigation { get; set; }
        public virtual Trabalhador TrabalhadorFkNavigation { get; set; }
        public virtual ICollection<Declaracaoremuneracao> Declaracaoremuneracao { get; set; }
    }
}
