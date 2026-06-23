using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class CamposEditaveisDto : BaseDto
    {
        [Mapper]
        public int IdCampoEditavel { get; set; }

        [Mapper]
        public string Nome { get; set; }

        [Mapper]
        public int DominioFk { get; set; }

        [Mapper]
        public string DominioString { get; set; }

        [Mapper]
        public int? CampoPaiFk { get; set; }

        public virtual DominioDto Dominio { get; set; }
        public virtual CamposEditaveisDto CampoPai { get; set; }
        public virtual ICollection<CamposEditaveisDto> InverseCampoPai { get; set; }
    }
}