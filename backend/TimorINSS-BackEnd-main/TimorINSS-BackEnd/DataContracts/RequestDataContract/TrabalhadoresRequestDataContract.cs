using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class TrabalhadorListagemRequest : SearchFilterRequest
    {
        [DataMember(IsRequired = true)]
        public int id;
    }

    [DataContract]
    public class TrabalhadorRequest : RequestBaseDataContract
    {
        [DataMember]
        public TrabalhadorDataContract Trabalhador { get; set; }

        [DataMember]
        public MoradaDataContract Morada { get; set; }

        [DataMember]
        public ContactoDataContract Contacto { get; set; }

        [DataMember]
        [ContainsDocument]
        public DocumentoIdentificacaoDataContract DocumentoIdentificacao { get; set; }

        [DataMember]
        public RelEntidadeTrabalhadorDataContract RelEntidadeTrabalhador { get; set; }

        [DataMember]
        [ContainsDocument]
        public INSSEstrangeiroDataContract? InssEstrangeiro { get; set; }
    }

    [DataContract]
    public class EditTrabalhadorRequest : RequestBaseDataContract
    {
        [DataMember]
        public TrabalhadorDataContract Trabalhador { get; set; }
    }

    [DataContract]
    public class TrabalhadorListagemNissRequest : SearchFilterRequest
    {
        [DataMember(IsRequired = true)]
        public string niss;
    }
}