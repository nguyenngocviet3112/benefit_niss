using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class DepartamentoListagemResponse : ResponseBaseDataContract
    {
        [DataMember]
        public int rows;

        [DataMember]
        public List<DepartamentoListagem> departamento;
    }

    public class DepartamentoListagem
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string nome { get; set; }
    }
}