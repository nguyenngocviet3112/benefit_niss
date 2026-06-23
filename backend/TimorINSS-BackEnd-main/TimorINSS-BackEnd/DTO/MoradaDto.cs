using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class MoradaDto : BaseDto
    {
        [Mapper]
        public int IdMorada { get; set; }

        [Mapper]
        public int? MoradaAldeiaFk { get; set; }

        [Mapper]
        public string Rua { get; set; }

        [Mapper]
        public string NumPorta { get; set; }

        [Mapper]
        public int MoradaPaisFk { get; set; }

        [Mapper]
        public bool FlagImportado { get; set; }

        [Mapper]
        public int? TrabalhadorMoradaFk { get; set; }

        [Mapper]
        public int? EntidadeMoradaFk { get; set; }

        [Mapper]
        public bool MoradaPrincipal { get; set; }

        public virtual EntidadeempregadoraDto EntidadeMoradaFkNavigation { get; set; }
        public virtual AldeiaDto MoradaAldeiaFkNavigation { get; set; }
        public virtual PaisDto MoradaPaisFkNavigation { get; set; }
        public virtual TrabalhadorDto TrabalhadorMoradaFkNavigation { get; set; }
    }
}