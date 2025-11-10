using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class ResponsavelLegalRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = true)]
        public int IdEntidade { get; set; }

        [DataMember]
        public long IdTrabalhador { get; set; }

        [DataMember(IsRequired = true)]
        public DateTime DataInicioFuncao { get; set; }

        [DataMember]
        public DateTime? DataFimFuncao { get; set; }

        [DataMember]
        public string Niss { get; set; }

        [DataMember(IsRequired = true)]
        public ResponsavelLegalDataContract ResponsavelLegal { get; set; }

        [DataMember(IsRequired = false)]
        [ContainsDocument]
        public List<DocumentoIdentificacaoDataContract>? DocumentoIdentificacao { get; set; }
    }

    [DataContract]
    public class ResponsavelLegalListagemRequest : SearchFilterRequest
    {
        [DataMember(IsRequired = true)]
        public int id;
        [DataMember(IsRequired = true)]
        public string idStr;
    }

    [DataContract]
    public class ResponsavelLegalDeleteRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = true)]
        public int id;
        [DataMember(IsRequired = true)]
        public string idStr;
    }

    [DataContract]
    public class AddResponsavelLegalHistRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = true)]
        public int IdEntidade { get; set; }

        [DataMember]
        public long IdTrabalhador { get; set; }

        [DataMember(IsRequired = true)]
        public DateTime DataInicioFuncao { get; set; }

        [DataMember]
        public DateTime? DataFimFuncao { get; set; }

        [DataMember]
        public string Niss { get; set; }

        [DataMember(IsRequired = true)]
        public ResponsavelLegalDataContract ResponsavelLegal { get; set; }

        [DataMember(IsRequired = true)]
        [ContainsDocument]
        public List<DocumentoIdentificacaoDataContract> DocumentoIdentificacao { get; set; }
    }
}