using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class ComponenteTextoDto : BaseDto
    {
        [Mapper]
        public int Id { get; set; }

        [Mapper]
        public int TarefaFk { get; set; }

        [Mapper]
        public bool? Texto1 { get; set; }

        [Mapper]
        public bool? Expandir1 { get; set; }

        [Mapper]
        public string Titulo1 { get; set; }

        [Mapper]
        public int? QuantCaracteres1 { get; set; }

        [Mapper]
        public bool? Obrigatorio1 { get; set; }

        [Mapper]
        public bool? ObrigatorioAoArquivar1 { get; set; }

        [Mapper]
        public bool? Texto2 { get; set; }

        [Mapper]
        public bool? Expandir2 { get; set; }

        [Mapper]
        public string Titulo2 { get; set; }

        [Mapper]
        public int? QuantCaracteres2 { get; set; }

        [Mapper]
        public bool? Obrigatorio2 { get; set; }

        [Mapper]
        public bool? ObrigatorioAoArquivar2 { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        public virtual TarefaDto TarefaFkNavigation { get; set; }
    }
}