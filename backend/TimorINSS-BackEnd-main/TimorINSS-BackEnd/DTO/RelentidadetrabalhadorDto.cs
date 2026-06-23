using System;
using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class RelentidadetrabalhadorDto : BaseDto
    {
        [Mapper]
        public int IdRel { get; set; }

        [Mapper]
        public int EntidadeFk { get; set; }

        [Mapper]
        public int TrabalhadorFk { get; set; }

        [Mapper]
        public int TipoContrato { get; set; }

        [Mapper]
        public int NaturezaContrato { get; set; }

        [Mapper]
        public int LeiLabAplicavel { get; set; }

        [Mapper]
        public int? HorasSemana { get; set; }

        [Mapper]
        public int? DiasSemana { get; set; }

        [Mapper]
        public DateTime DtIniVincTrabalhador { get; set; }

        [Mapper]
        public DateTime? DtIniFimTrabalhador { get; set; }

        [Mapper]
        public bool FuncPublico { get; set; }

        [Mapper]
        public string NumFuncPublico { get; set; }

        [Mapper]
        public bool FlagImportado { get; set; }

        [Mapper]
        public int RegimeFk { get; set; }

        [Mapper]
        public int? EscalaoFk { get; set; }

        [Mapper]
        public int? Profissao { get; set; }

        [Mapper]
        public string ProfissaoOutro { get; set; }

        public virtual EntidadeempregadoraDto Entidade { get; set; }
        public virtual EscalaoDto EscalaoFkNavigation { get; set; }
        public virtual DominioDto LeiLabAplicavelNavigation { get; set; }
        public virtual DominioDto NaturezaContratoNavigation { get; set; }
        public virtual DominioDto Regime { get; set; }
        public virtual DominioDto TipoContratoNavigation { get; set; }
        public virtual TrabalhadorDto Trabalhador { get; set; }
        public virtual ICollection<DeclaracaoremuneracaoDto> Declaracaoremuneracao { get; set; }
    }
}