using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class RelatorioProcessoDataContract
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string tipo { get; set; }

        [DataMember]
        public RelatorioProcessoDataContractUltimaTarefa ultimaTarefa { get; set; }

        [DataMember]
        public IEnumerable<string> perfis { get; set; }

        [DataMember]
        public bool arquivado { get; set; }
    }

    public class RelatorioProcessoDataContractUltimaTarefa
    {
        public int id { get; set; }
        public string nome { get; set; }
    }

    public class RelatorioTipoProcessoDataContract
    {
        public int id { get; set; }
        public string nome { get; set; }
    }
}