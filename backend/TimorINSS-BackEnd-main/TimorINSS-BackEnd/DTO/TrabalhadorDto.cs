using System;
using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class TrabalhadorDto : BaseDto
    {
        [Mapper]
        public int IdTrabalhador { get; set; }

        [Mapper]
        public string Nome { get; set; }

        [Mapper]
        public string Niss { get; set; }

        [Mapper]
        public string Tin { get; set; }

        [Mapper]
        public string NumInscProvisoria { get; set; }

        [Mapper]
        public DateTime DataNasc { get; set; }

        [Mapper]
        public string NomeMae { get; set; }

        [Mapper]
        public bool IndDescNomeMae { get; set; }

        [Mapper]
        public string NomePai { get; set; }

        [Mapper]
        public bool IndDescNomePai { get; set; }

        [Mapper]
        public int? EstadoCivil { get; set; }

        [Mapper]
        public string Naturalidade { get; set; }

        [Mapper]
        public bool FlagImportado { get; set; }

        [Mapper]
        public int SexoTrabalhador { get; set; }

        [Mapper]
        public int NacionalidadeTrabalhador { get; set; }

        [Mapper]
        public bool Interno { get; set; }

        public virtual DominioDto EstadoCivilNavigation { get; set; }
        public virtual DominioDto NacionalidadeTrabalhadorNavigation { get; set; }
        public virtual DominioDto SexoTrabalhadorNavigation { get; set; }
        public virtual MoradaDto TrabalhadorMoradaFkNavigation { get; set; }
        public virtual ICollection<ContacorrenteDto> Contacorrente { get; set; }
        public virtual ICollection<ContactoDto> Contacto { get; set; }
        public virtual ICollection<DocumentoidentificacaoDto> Documentoidentificacao { get; set; }
        public virtual ICollection<InssestrangeiroDto> Inssestrangeiro { get; set; }
        public virtual ICollection<RelentidadetrabalhadorDto> Relentidadetrabalhador { get; set; }
        public virtual ICollection<ResponsavellegalDto> Responsavellegal { get; set; }
        public virtual ICollection<SuspensoesDto> Suspensoes { get; set; }
    }
}