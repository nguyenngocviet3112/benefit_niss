using System;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class AttachmentConfig
    {
        public int Id { get; set; }
        public int MaxFileSizeMb { get; set; }
        public bool AdObrigatorio { get; set; }
        public bool CabimentoObrigatorio { get; set; }
        public bool CompromissoObrigatorio { get; set; }
        public bool ObrigacaoObrigatorio { get; set; }
        public bool PagamentoObrigatorio { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
    }
}
