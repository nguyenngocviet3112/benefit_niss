using System;
using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class CompromissoDto : BaseDto
    {
        [Mapper]
        public int Id { get; set; }
        [Mapper]
        public int TarefaAtivoFk { get; set; }
        [Mapper]
        public int ComponenteDespesaRegistoFk { get; set; }
        [Mapper]
        public string Nome { get; set; }
        [Mapper]
        public decimal Valor { get; set; }
        [Mapper]
        public DateTime Data { get; set; }
        [Mapper]
        public bool IndActivo { get; set; }

        public virtual ComponenteDespesaRegistoDto ComponenteDespesaRegistoFkNavigation { get; set; }
        public virtual TarefaativoDto TarefaAtivoFkNavigation { get; set; }
        public virtual ICollection<PagamentosExecutadosDto> Pagamentosexecutados { get; set; }



    }
}
