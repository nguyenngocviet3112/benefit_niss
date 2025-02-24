using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Responsavellegalhist
    {
        public int IdResPlegalHist { get; set; }
        public int IdEntidadeEmpreg { get; set; }
        public string Nome { get; set; }
        public string Tin { get; set; }
        public DateTime DataNasc { get; set; }
        public int Nacionalidade { get; set; }
        public string Naturalidade { get; set; }
        public int Sexo { get; set; }
        public bool IndFuncaoRem { get; set; }
        public bool IndAdesFacultInss { get; set; }
        public bool IndActivo { get; set; }
        public bool FlagImportado { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public string Ipv6 { get; set; }
        public int Funcao { get; set; }
        public string FuncaoOutro { get; set; }
    }
}
