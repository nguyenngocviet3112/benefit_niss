using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class TarefaativoDto : BaseDto
    {
        [Mapper]
        public int Id { get; set; }

        [Mapper]
        public int ProcessoAtivoFk { get; set; }

        [Mapper]
        public int TarefaconfigFk { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        [Mapper]
        public int UtilizadorResponsavel { get; set; }

        public virtual ProcessoativoDto ProcessoAtivoFkNavigation { get; set; }
        public virtual TarefaDto TarefaconfigFkNavigation { get; set; }
        public virtual ICollection<ComponenteaccoestarefaRegistoDto> ComponenteaccoestarefaRegistoTarefaAtivoFkNavigation { get; set; }
        public virtual ICollection<ComponenteaccoestarefaRegistoDto> ComponenteaccoestarefaRegistoTarefaSeguirFkNavigation { get; set; }
        public virtual ICollection<ComponenteclassificacaosubRegistoDto> ComponenteclassificacaosubRegisto { get; set; }
        public virtual ICollection<ComponentedocumentoRegistoDto> ComponentedocumentoRegisto { get; set; }
        public virtual ICollection<ComponenteOrcamentoRegistoDto> ComponenteorcamentoRegisto { get; set; }
        public virtual ICollection<ComponentetextoRegistoDto> ComponentetextoRegisto { get; set; }
        public virtual ICollection<DocumentoidentificacaoDto> Documentoidentificacao { get; set; }
    }
}