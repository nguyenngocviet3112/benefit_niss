using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class ComponentesListagemResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<Componentes> componentes;
    }

    [DataContract]
    public class Componentes
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string descricao { get; set; }

        [DataMember]
        public bool expandir { get; set; }

        [DataMember]
        public bool select { get; set; }

        [DataMember]
        public int? ordem { get; set; }
    }
}