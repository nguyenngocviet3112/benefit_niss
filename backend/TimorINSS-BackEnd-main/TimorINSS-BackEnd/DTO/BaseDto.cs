using System;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class BaseDto
    {
        [Mapper]
        public int UtilizadorCriacao { get; set; }

        [Mapper]
        public DateTime DataCriacao { get; set; }

        [Mapper]
        public int? UtilizadorAlteracao { get; set; }

        [Mapper]
        public DateTime? DataAlteracao { get; set; }

        [Mapper]
        public string Ipv6 { get; set; }
    }
}