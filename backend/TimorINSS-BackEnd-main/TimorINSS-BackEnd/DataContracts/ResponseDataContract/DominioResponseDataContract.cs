using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class DominioDescricaoStringResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<DominioDescricaoString> dominios { get; set; }
        [DataMember]
        public List<SelectDescription> institutions { get; set; }
    }

    [DataContract]
    public class ListagemRegimesResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<RegimeDescricaoString> regimes { get; set; }
    }

    [DataContract]
    public class SingleDominioDescricaoStringResponse : ResponseBaseDataContract
    {
        [DataMember]
        public DominioDescricaoString dominio { get; set; }
    }

    [DataContract]
    public class DominiosComGruposResponse : ResponseBaseDataContract
    {
        [DataMember]
        public DominiosComGrupos dominio { get; set; }
    }

    [DataContract]
    public class DominioDescricaoString
    {
        [DataMember]
        public long id { get; set; }

        [DataMember]
        public long value { get; set; }

        [DataMember]
        public string descricao { get; set; }

        [DataMember]
        public string descricaoEn { get; set; }

        [DataMember]
        public bool? indActivo { get; set; }
    }

    [DataContract]
    public class RegimeDescricaoString
    {
        [DataMember]
        public long id { get; set; }

        [DataMember]
        public long value { get; set; }

        [DataMember]
        public string descricao { get; set; }

        [DataMember]
        public bool? indActivo { get; set; }

        [DataMember]
        public string tipoRegime { get; set; }
    }

    [DataContract]
    public class DominiosComGrupos
    {
        [DataMember]
        public List<Group> groups { get; set; }
    }

    [DataContract]
    public class Group
    {
        [DataMember]
        public string name { get; set; }

        [DataMember]
        public string? disabled { get; set; }

        [DataMember]
        public List<DominioDescricaoString> values { get; set; }
    }
}