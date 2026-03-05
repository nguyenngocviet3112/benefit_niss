using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class ComponenteOrcamentoValorDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int ComponenteOrcamentoRegistoFk { get; set; }

        [DataMember]
        public int? DepartamentoFk { get; set; }
        [DataMember]
        public int? ActidadeFk { get; set; }
        [DataMember]
       
        public int? FuncionalFk { get; set; }

        [DataMember]
        public int? InstitutionId { get; set; }

        [DataMember]
        public int? CentroCustoFk { get; set; }

        [DataMember]
        public int AgrupamentoFk { get; set; }

        [DataMember]
        public decimal Valor { get; set; }

        [DataMember]
        public int? TipoContaFk { get; set; }
    }

    [DataContract]
    public class ComponenteOrcamentoValorFullDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int ComponenteOrcamentoRegistoFk { get; set; }

        [DataMember]
        public int? DepartamentoFk { get; set; }

        [DataMember]
        public int? InstitutionId { get; set; }

        [DataMember]
        public int? FuncionalFk { get; set; }


        [DataMember]
        public string InstitutionDescricao { get; set; }

        [DataMember]
        public int? ActidadeFk { get; set; }

        [DataMember]
        public string DepartamentoDescricao { get; set; }

        [DataMember]
        public string ActidadeDescricao { get; set; }

        //[DataMember]
        //public string EconomicDescricao { get; set; }

        [DataMember]
        public string FuncionalDescricao { get; set; }

  
        [DataMember]
        public int? CentroCustoFk { get; set; }

        [DataMember]
        public string CentroCustoDescricao { get; set; }

        [DataMember]
        public int? AgrupamentoFk { get; set; }

        [DataMember]
        public decimal Valor { get; set; }

        [DataMember]
        public int TipoDeConta { get; set; }

        [DataMember]
        public string TipoDeContaDescricao { get; set; }

        [DataMember]
        public string Codigo { get; set; }

        [DataMember]
        public string Descricao { get; set; }

        [DataMember]
        public bool Editavel { get; set; }
    }

    [DataContract]
    public class ComponenteOrcamentoValorSearch
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public List<int> Departamentos { get; set; }

        [DataMember]
        public List<int> CentrosDeCusto { get; set; }

        [DataMember]
        public List<int> TiposDeConta { get; set; }
    }
}