using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class MovimentoBancarioDto : BaseDto
    {
        [Mapper]
        public int Id { get; set; }

        [Mapper]
        public string Descricao { get; set; }

        [Mapper]
        public int TipoMovimento { get; set; }
    }
}