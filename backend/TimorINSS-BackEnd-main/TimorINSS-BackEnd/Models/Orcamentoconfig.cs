using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Orcamentoconfig
    {
        public Orcamentoconfig()
        {
            Centrocusto = new HashSet<Centrocusto>();
            Codigoconta = new HashSet<Codigoconta>();
            ComponenteorcamentoRegisto = new HashSet<ComponenteorcamentoRegisto>();
            Reltipodecontaorcamentoconfig = new HashSet<Reltipodecontaorcamentoconfig>();
        }

        public int Id { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        // Ano/Tipo added 2026-07-11 (System Settings — Kỳ ngân sách) — see
        // db_migrations/2026-07-11i_system_settings.sql. Tipo = PRINCIPAL | SUPLEMENTAR;
        // only PRINCIPAL is created via the admin screen for now.
        public int Ano { get; set; }
        public string Tipo { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }

        public virtual ICollection<Centrocusto> Centrocusto { get; set; }
        public virtual ICollection<Codigoconta> Codigoconta { get; set; }
        public virtual ICollection<ComponenteorcamentoRegisto> ComponenteorcamentoRegisto { get; set; }
        public virtual ICollection<Reltipodecontaorcamentoconfig> Reltipodecontaorcamentoconfig { get; set; }
    }
}
