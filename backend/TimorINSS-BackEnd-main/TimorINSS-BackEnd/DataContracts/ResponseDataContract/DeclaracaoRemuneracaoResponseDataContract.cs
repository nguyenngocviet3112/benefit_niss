using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class GetDeclaracaoByEntidadeAndFilterResponse : ResponseBaseDataContract
    {
        [DataMember]
        public int rows;

        [DataMember]
        public List<DeclaracaoListagem> declaracoes { get; set; }
    }

    [DataContract]
    public class RelatorioDeclaracaoRenumeracaoListagemResponse : ResponseBaseDataContract
    {
        [DataMember]
        public int rows;

        [DataMember]
        public List<RelatorioDeclaracaoRenumeracaoDataContract> declaracoes;
    }

    #region initial page table

    [DataContract]
    public class DeclaracaoListagem
    {
        [DataMember]
        public DeclaracaoRemuneracaoDataContract declaracao { get; set; }

        [DataMember]
        public DeclaracaoTrabalhadorInfo trabalhadorInfo { get; set; }
    }

    [DataContract]
    public class DeclaracaoTrabalhadorInfo
    {
        [DataMember]
        public string niss { get; set; }

        [DataMember]
        public string nome { get; set; }

        [DataMember]
        public string nacionalidade { get; set; }

        [DataMember]
        public string regime { get; set; }

        [DataMember]
        public string sexo { get; set; }

        [DataMember]
        public string tipoRegime { get; set; }
    }

    #endregion initial page table

    #region resumo

    [DataContract]
    public class ResumoDeclaracaoResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<NacionalidadeResumoDeclaracao> nacionalidades { get; set; }

        [DataMember]
        public NacionalidadeResumoDeclaracao totalNacionalidades { get; set; }

        [DataMember]
        public List<RegimesResumoDeclaracao> regimes { get; set; }

        [DataMember]
        public TotalResumoDeclaracao total { get; set; }
    }

    [DataContract]
    public class NacionalidadeResumoDeclaracao
    {
        [DataMember]
        public decimal total { get; set; }

        [DataMember]
        public int trabalhadores { get; set; }

        [DataMember]
        public string nacionalidade { get; set; }
    }

    [DataContract]
    public class RegimesResumoDeclaracao
    {
        [DataMember]
        public decimal remuneracoes { get; set; }

        [DataMember]
        public decimal taxaTrabalhador { get; set; }

        [DataMember]
        public decimal taxaEntidade { get; set; }

        [DataMember]
        public decimal quotizacoes { get; set; }

        [DataMember]
        public decimal contribuicoes { get; set; }

        [DataMember]
        public string regime { get; set; }

        [DataMember]
        public decimal total { get; set; }
    }

    [DataContract]
    public class TotalResumoDeclaracao
    {
        [DataMember]
        public decimal remuneracoes { get; set; }

        [DataMember]
        public decimal quotizacoes { get; set; }

        [DataMember]
        public decimal contribuicoes { get; set; }

        [DataMember]
        public decimal total { get; set; }
    }

    #endregion resumo
}