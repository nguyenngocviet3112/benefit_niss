using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class EntidadeRelatorioDataContract
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string nome { get; set; }

        [DataMember]
        public string niss { get; set; }

        [DataMember]
        public string tin { get; set; }

        [DataMember]
        public DateTime dtInscricao { get; set; }

        [DataMember]
        public bool ativo { get; set; }

        [DataMember]
        public int totalTrabalhadores { get; set; }

        [DataMember]
        public int totalMasculino { get; set; }

        [DataMember]
        public int totalFeminino { get; set; }
    }

    [DataContract]
    public class SituacaoContributivaEmpresaDataContract
    {
        [DataMember]
        public int idEntidade { get; set; }

        [DataMember]
        public string nomeEmpregador { get; set; }

        [DataMember]
        public string niss { get; set; }

        [DataMember]
        public DateTime? ultimoMesPago { get; set; }

        [DataMember]
        public List<DateTime> mesesEmDivida { get; set; } = new List<DateTime>();

        [DataMember]
        public decimal totalDivida { get; set; }
    }

    [DataContract]
    public class ContribuicoesTrendMesDataContract
    {
        [DataMember]
        public DateTime mesAno { get; set; }

        [DataMember]
        public decimal valorPago { get; set; }

        [DataMember]
        public decimal valorDivida { get; set; }

        [DataMember]
        public int novosRegistos { get; set; }
    }

    [DataContract]
    public class RubricaOrcamentoDataContract
    {
        [DataMember]
        public int agrupamentoConfigFk { get; set; }

        [DataMember]
        public string descricao { get; set; }

        [DataMember]
        public string departamento { get; set; }

        [DataMember]
        public decimal valorOrcado { get; set; }

        [DataMember]
        public decimal valorReservado { get; set; }

        [DataMember]
        public decimal saldoDisponivel { get; set; }
    }

    [DataContract]
    public class DespesaPipelineEstagioDataContract
    {
        [DataMember]
        public string estagio { get; set; }

        [DataMember]
        public int ordem { get; set; }

        [DataMember]
        public int total { get; set; }
    }
}
