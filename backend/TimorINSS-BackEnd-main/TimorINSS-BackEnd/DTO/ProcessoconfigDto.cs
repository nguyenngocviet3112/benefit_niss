using System;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class ProcessoconfigDto : BaseDto
    {
        [Mapper]
        public int Id { get; set; }

        [Mapper]
        public string Nome { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        [Mapper]
        public DateTime Data { get; set; }
    }
}