using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class ValorCamposEditaveis
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Nome { get; set; }

        [DataMember]
        public int? ParentId { get; set; }

        [DataMember]
        public List<ParametrosAdicionais> Parametros { get; set; }

        [DataMember]
        public bool NaoEditavelEliminavel { get; set; }

        [DataMember]
        public bool ParentHasInitialValue { get; set; }

        [DataMember]
        public bool HasKids { get; set; }
    }

    [DataContract]
    public class CamposEditaveisParents
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Nome { get; set; }

        [DataMember]
        public List<ValorCamposEditaveisParents> Valores { get; set; }
    }

    [DataContract]
    public class ValorCamposEditaveisParents
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Nome { get; set; }

        [DataMember]
        public bool IndActivo { get; set; }
    }

    [DataContract]
    public class ParametrosAdicionais
    {
        [DataMember]
        public string Valor { get; set; }

        [DataMember]
        public DateTime? DateValor { get; set; }

        [DataMember]
        public string Nome { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public string Size { get; set; }

        [DataMember]
        public string Suffix { get; set; }

        [DataMember]
        public bool Optional { get; set; }

        [DataMember]
        public List<MultipleSelectAdditionalParameter>? ValuesList { get; set; }

        [DataMember]
        public List<int>? SelectedValuesList { get; set; }

        [DataMember]
        public bool NaoVisivel { get; set; }

        [DataMember]
        public List<SelectDescription>? DropdownList { get; set; }

        [DataMember]
        public int? SelectedDropdown { get; set; }

        [DataMember]
        public bool Credit { get; set; }
    }

    [DataContract]
    public class MultipleSelectAdditionalParameter
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Nome { get; set; }
    }
}