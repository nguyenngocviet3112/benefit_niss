using System;
using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class ResponsavellegalDto : BaseDto
    {
        [Mapper]
        public int IdResponsavelLegal { get; set; }

        [Mapper]
        public string Nome { get; set; }

        [Mapper]
        public string Tin { get; set; }

        [Mapper]
        public DateTime DataNasc { get; set; }

        [Mapper]
        public int Nacionalidade { get; set; }

        [Mapper]
        public string Naturalidade { get; set; }

        [Mapper]
        public int Sexo { get; set; }

        [Mapper]
        public bool IndFuncaoRem { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        [Mapper]
        public bool FlagImportado { get; set; }

        [Mapper]
        public int? RespLegalTabalhadorFk { get; set; }

        [Mapper]
        public int Funcao { get; set; }

        [Mapper]
        public string? FuncaoOutro { get; set; }

        public virtual TrabalhadorDto Trabalhador { get; set; }
        public virtual ICollection<DocumentoidentificacaoDto> Documentoidentificacao { get; set; }
        public virtual ICollection<EntidadeempregadoraDto> EntidadeEmpregadora { get; set; }
    }
}