using System;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class DocumentoidentificacaoDto : BaseDto
    {
        [Mapper]
        public int IdDocIdentificacao { get; set; }

        [Mapper]
        public int? TrabalhadorDocumetoFk { get; set; }

        [Mapper]
        public int? RespLegalDocumentoFk { get; set; }

        [Mapper]
        public int TpDocIdentificacao { get; set; }

        [Mapper]
        public string Numero { get; set; }

        [Mapper]
        public string LocalEmissao { get; set; }

        [Mapper]
        public DateTime? DataEmissao { get; set; }

        [Mapper]
        public DateTime? DataValidade { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        [Mapper]
        public bool FlagImportado { get; set; }

        [Mapper]
        public byte[] Documento { get; set; }

        [Mapper]
        public string NomeDocumento { get; set; }

        public virtual ResponsavellegalDto RespLegalDocumento { get; set; }
        public virtual TrabalhadorDto TrabalhadorDocumeto { get; set; }
        public virtual ProcessoativoDto ProcessoAtivoFkNavigation { get; set; }
        public virtual TarefaativoDto TarefaAtivoFkNavigation { get; set; }
    }
}