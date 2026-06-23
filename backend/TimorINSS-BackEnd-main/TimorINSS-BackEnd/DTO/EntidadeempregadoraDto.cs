using System;
using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class EntidadeempregadoraDto : BaseDto
    {
        [Mapper]
        public int IdEntidadeEmpreg { get; set; }

        [Mapper]
        public string Nome { get; set; }

        [Mapper]
        public string Niss { get; set; }

        [Mapper]
        public string Tin { get; set; }

        [Mapper]
        public DateTime DtInscricao { get; set; }

        [Mapper]
        public string SituacInscricao { get; set; }

        [Mapper]
        public DateTime DataInicioActiv { get; set; }

        [Mapper]
        public DateTime? DataFimActiv { get; set; }

        [Mapper]
        public DateTime DataInicioTrabServico { get; set; }

        [Mapper]
        public int NumTrabalhador { get; set; }

        [Mapper]
        public int EntidadeNatJuridicaFk { get; set; }

        [Mapper]
        public int EntidadeActEconomicaFk { get; set; }

        [Mapper]
        public int EntidadeSectorActFk { get; set; }

        [Mapper]
        public DateTime DtHoraUltimoAcesso { get; set; }

        [Mapper]
        public bool FlagImportado { get; set; }

        public ActividadeeconomicaDto Actividadeeconomica { get; set; }
        public NaturezajuridicaDto Naturezajuridica { get; set; }
        public SectoractividadeDto Sectoractividade { get; set; }
        public ICollection<ContacorrenteDto> Contacorrente { get; set; }
        public ICollection<ContactoDto> Contacto { get; set; }
        public ICollection<GuiapagamentoDto> Guiapagamento { get; set; }
        public ICollection<InssestrangeiroDto> Inssestrangeiro { get; set; }
        public ICollection<MoradaDto> Morada { get; set; }
        public ICollection<ResponsavellegalDto> ResponsavelLegal { get; set; }
        public ICollection<SuspensoesDto> Suspensoes { get; set; }
        public ICollection<RelentidadetrabalhadorDto> Relentidadetrabalhador { get; set; }
        public ICollection<UtilizadorDto> Utilizador { get; set; }
    }
}