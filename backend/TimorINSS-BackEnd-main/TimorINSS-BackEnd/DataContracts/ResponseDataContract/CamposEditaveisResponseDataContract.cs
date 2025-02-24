using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class CamposEditaveisListagemResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<CamposEditaveisListagem> Campos { get; set; }
    }

    [DataContract]
    public class ValueCampoEditavelListagemResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<ValorCamposEditaveis> ValuesCampo { get; set; }

        [DataMember]
        public int CountValuesCampo { get; set; }

        [DataMember]
        public CamposEditaveisParents ValuesCampoParents { get; set; }

        [DataMember]
        public List<ParametrosAdicionais> Parametros { get; set; }
    }

    [DataContract]
    public class CamposEditaveisListagem
    {
        [DataMember]
        public int IdCampoEditavel { get; set; }

        [DataMember]
        public string Nome { get; set; }

        [DataMember]
        public int DominioFk { get; set; }

        [DataMember]
        public int? CampoPaiFk { get; set; }

        [DataMember]
        public bool NomeNaoEditavel { get; set; }

        [DataMember]
        public bool NaoEliminavel { get; set; }

        [DataMember]
        public bool NaoPesquisavel { get; set; }

        [DataMember]
        public List<ParametrosAdicionais> Parametros { get; set; }

        [DataMember]
        public bool NaoEditavel { get; set; }

        [DataMember]
        public bool Unico { get; set; }

        [DataMember]
        public int NomeSize { get; set; }
    }
}