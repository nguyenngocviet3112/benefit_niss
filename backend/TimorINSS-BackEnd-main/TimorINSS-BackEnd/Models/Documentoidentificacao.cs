using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Documentoidentificacao
    {
        public int IdDocIdentificacao { get; set; }
        public int? TrabalhadorDocumetoFk { get; set; }
        public int? RespLegalDocumentoFk { get; set; }
        public int TpDocIdentificacao { get; set; }
        public string Numero { get; set; }
        public string LocalEmissao { get; set; }
        public DateTime? DataEmissao { get; set; }
        public DateTime? DataValidade { get; set; }
        public bool IndActivo { get; set; }
        public bool FlagImportado { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }
        public byte[] Documento { get; set; }
        public string NomeDocumento { get; set; }

        public virtual Responsavellegal RespLegalDocumentoFkNavigation { get; set; }
        public virtual Dominio TpDocIdentificacaoNavigation { get; set; }
        public virtual Trabalhador TrabalhadorDocumetoFkNavigation { get; set; }
    }
}
