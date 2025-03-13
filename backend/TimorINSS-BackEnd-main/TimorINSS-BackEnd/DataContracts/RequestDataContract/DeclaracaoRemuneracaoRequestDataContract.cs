using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class GetDeclaracaoByEntidadeAndFilterRequest : SearchFilterRequest
    {
        [DataMember(IsRequired = true)]
        public int IdEntidade { get; set; }
    }

    [DataContract]
    public class GetResumoDeclaracaoRequest : RequestBaseDataContract
    {
        [DataMember]
        public List<DeclaracaoRemuneracaoDataContract> declaracoes { get; set; }
    }

    [DataContract]
    public class SaveDeclaracoesRequest : RequestBaseDataContract
    {
        [DataMember]
        public List<DeclaracaoRemuneracaoDataContract> declaracoes { get; set; }

        [DataMember]

        public DateTime data { get; set; }


        [DataMember]
        public int entidadeId { get; set; }
    }
}