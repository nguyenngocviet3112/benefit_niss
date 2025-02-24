using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class ComponentereceitaRegistoMovimentos
    {
        public int Id { get; set; }
        public int ComponenteReceitaRegistoId { get; set; }
        public int RelMovimentosPorConciliarMovimentosId { get; set; }
        public bool? IndActivo { get; set; }

        public virtual ComponentereceitaRegisto ComponenteReceitaRegisto { get; set; }
        public virtual RelMovimentosporconciliarMovimentos RelMovimentosPorConciliarMovimentos { get; set; }
    }
}
