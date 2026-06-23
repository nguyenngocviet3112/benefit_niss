using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class ValorCamposEditaveisRequest : RequestBaseDataContract
    {
        [DataMember]
        public int CampoEditavelId { get; set; }

        [DataMember]
        public SearchFilter Filter { get; set; }
    }

    [DataContract]
    public class ValorCamposEditaveisSaveRequest : RequestBaseDataContract
    {
        [DataMember]
        public ValorCamposEditaveis ValorCampo { get; set; }

        [DataMember]
        public int IdCampo { get; set; }
    }

    [DataContract]
    public class RegimeCampoEditaveisDeleteRequest : RequestBaseDataContract
    {
        [DataMember]
        public ValorCamposEditaveis ValorCampo { get; set; }
    }

    [DataContract]
    public class ValorCamposEditaveisDeleteRequest : RequestBaseDataContract
    {
        [DataMember]
        public int IdValorCampo { get; set; }

        [DataMember]
        public int IdCampo { get; set; }
    }
}