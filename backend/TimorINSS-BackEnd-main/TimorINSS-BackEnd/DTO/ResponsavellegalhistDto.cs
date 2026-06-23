using System;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class ResponsavellegalhistDto : BaseDto
    {
        [Mapper]
        public int IdResPlegalHist { get; set; }

        [Mapper]
        public int IdEntidadeEmpreg { get; set; }

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
        public bool IndAdesFacultInss { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        [Mapper]
        public bool FlagImportado { get; set; }

        [Mapper]
        public int Funcao { get; set; }

        [Mapper]
        public string FuncaoOutro { get; set; }
    }
}