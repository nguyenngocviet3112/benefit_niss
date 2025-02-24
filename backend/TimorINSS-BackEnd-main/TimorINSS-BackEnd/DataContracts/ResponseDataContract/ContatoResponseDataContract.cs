using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class ContatoListagemResponse : ResponseBaseDataContract
    {
        [DataMember]
        public int rows { get; set; }

        [DataMember]
        public List<ContatoListagem> contato { get; set; }
    }

    [DataContract]
    public class ContatoListagem
    {
        [DataMember]
        public int idContato { get; set; }

        [DataMember]
        public int? idTrabalhador { get; set; }

        [DataMember]
        public int? idEntidade { get; set; }

        [DataMember]
        public string telemovel { get; set; }

        [DataMember]
        public string email { get; set; }
    }
}