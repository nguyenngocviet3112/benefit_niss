using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class FuncionalidadeListagemResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<FuncionalidadeDataContract> funcionalidade;

        [DataMember]
        public List<FuncionalidadeDataContract> perfilFuncionalidade;
    }
}