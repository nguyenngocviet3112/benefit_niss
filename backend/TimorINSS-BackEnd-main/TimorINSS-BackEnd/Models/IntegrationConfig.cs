using System;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class IntegrationConfig
    {
        public int Id { get; set; }
        public bool BenefitApiEnabled { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
    }
}
