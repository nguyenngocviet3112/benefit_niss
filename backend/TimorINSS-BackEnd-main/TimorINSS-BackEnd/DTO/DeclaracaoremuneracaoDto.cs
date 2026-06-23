using System;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class DeclaracaoremuneracaoDto : BaseDto
    {
        [Mapper]
        public int IdDeclaracao { get; set; }

        [Mapper]
        public decimal DiasContrato { get; set; }

        [Mapper]
        public decimal DiasEfecTrabalhados { get; set; }

        [Mapper]
        public int FaltasInjustific { get; set; }

        [Mapper]
        public int DiasParentalidade { get; set; }

        [Mapper]
        public decimal DiasTrabcontabSegSocial { get; set; }

        [Mapper]
        public decimal RemunDeclarada { get; set; }

        [Mapper]
        public decimal DecimoTerceiro { get; set; }

        [Mapper]
        public DateTime MesAno { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        [Mapper]
        public bool FlagImportado { get; set; }

        [Mapper]
        public int ContaCorrenteFk { get; set; }

        [Mapper]
        public int DeclaracaoRelEntidadeTrabalhadorFk { get; set; }

        [Mapper]
        public bool Oficioso { get; set; }

        [Mapper]
        public int NacionalidadeFk { get; set; }

        [Mapper]
        public int RegimeFk { get; set; }

        [Mapper]
        public int SexoFk { get; set; }

        public virtual ContacorrenteDto ContaCorrente { get; set; }
        public virtual RelentidadetrabalhadorDto DeclaracaoRelEntidadeTrabalhador { get; set; }
        public virtual DominioDto Nacionalidade { get; set; }
        public virtual RegimeDto Regime { get; set; }
        public virtual DominioDto Sexo { get; set; }
    }
}