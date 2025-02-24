using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class TarefaDto : BaseDto
    {
        [Mapper]
        public int Id { get; set; }

        [Mapper]
        public string Nome { get; set; }

        [Mapper]
        public int PrazoTarefa { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        [Mapper]
        public bool? HistoricoTexto { get; set; }

        [Mapper]
        public bool? HistoricoDocumento { get; set; }

        [Mapper]
        public bool? CabecalhoProcesso { get; set; }

        [Mapper]
        public string NumeroTarefa { get; set; }

        [Mapper]
        public bool BotaoArquivar { get; set; }

        public virtual ICollection<ComponenteAccoesTarefaDto> ComponenteaccoestarefaTarefaFkNavigation { get; set; }
        public virtual ICollection<ComponenteAccoesTarefaDto> ComponenteaccoestarefaTarefaSeguirFkNavigation { get; set; }
        public virtual ICollection<ComponenteCarregarDocumentoDto> Componentecarregardocumento { get; set; }
        public virtual ICollection<ComponenteClassificacaoSubClassificDto> Componenteclassificacaosubclassific { get; set; }
        public virtual ICollection<ComponenteconciliacaomovimentosDto> Componenteconciliacaomovimentos { get; set; }
        public virtual ICollection<ComponenteControleAcessoDto> Componentecontroleacesso { get; set; }
        public virtual ICollection<ComponenteOrcamentoDto> Componenteorcamento { get; set; }
        public virtual ICollection<ComponenteTextoDto> Componentetexto { get; set; }
        public virtual ICollection<Relprocessoconfigtarefadto> Relprocessoconfigtarefa { get; set; }
        public virtual ICollection<RelTarefaComponenteDto> Reltarefacomponente { get; set; }
        public virtual ICollection<TarefaativoDto> Tarefaativo { get; set; }
    }
}