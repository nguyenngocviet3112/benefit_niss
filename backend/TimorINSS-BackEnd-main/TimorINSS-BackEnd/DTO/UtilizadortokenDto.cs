using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class UtilizadortokenDto : BaseDto
    {
        [Mapper]
        public int Id { get; set; }

        [Mapper]
        public int IdUtilizador { get; set; }

        [Mapper]
        public string TokenString { get; set; }

        [Mapper]
        public string Salt { get; set; }
    }
}