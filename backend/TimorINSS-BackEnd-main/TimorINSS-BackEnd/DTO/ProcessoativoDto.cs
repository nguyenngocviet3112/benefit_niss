using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class ProcessoativoDto : BaseDto
    {
        [Mapper]
        public int Id { get; set; }

        [Mapper]
        public int ProcessoConfigFk { get; set; }

        [Mapper]
        public bool Arquivado { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        [Mapper]
        public string NumeroProcesso { get; set; }

        public virtual ProcessoconfigDto ProcessoConfigFkNavigation { get; set; }
        public virtual ICollection<DocumentoidentificacaoDto> Documentoidentificacao { get; set; }
        public virtual ICollection<TarefaativoDto> Tarefaativo { get; set; }
    }
}