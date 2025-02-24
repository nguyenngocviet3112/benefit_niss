using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class MovimentosPorConciliarDto : BaseDto
    {
        [Mapper]
        public int Id { get; set; }

        [Mapper]
        public int TarefaAtivoFk { get; set; }

        [Mapper]
        public int MovimentoBancarioFk { get; set; }

        [Mapper]
        public bool IsReceita { get; set; }

        [Mapper]
        public decimal Valor { get; set; }

        [Mapper]
        public string TipoDocumento { get; set; }

        [Mapper]
        public string NumeroDocumento { get; set; }

        [Mapper]
        public byte[] Comprovativo { get; set; }

        [Mapper]
        public string NomeComprovativo { get; set; }

        [Mapper]
        public bool? IndActivo { get; set; }

        public virtual MovimentoBancarioDto MovimentoBancarioFkNavigation { get; set; }
        public virtual TarefaativoDto TarefaAtivoFkNavigation { get; set; }
    }
}