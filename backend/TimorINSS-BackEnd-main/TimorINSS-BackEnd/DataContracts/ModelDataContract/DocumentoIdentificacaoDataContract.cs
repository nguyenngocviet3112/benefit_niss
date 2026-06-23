using System;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class DocumentoIdentificacaoDataContract
    {
        [DataMember]
        public int IdDocumento { get; set; }

        [DataMember]
        public int? IdTrabalhador { get; set; }

        [DataMember]
        public int? IdResponsavelLegal { get; set; }

        [DataMember]
        public int TpDocIdentificacao { get; set; }

        [DataMember]
        public string Numero { get; set; }

        [DataMember]
        public string LocalEmissao { get; set; }

        [DataMember]
        public DateTime? DataValidade { get; set; }

        [DataMember]
        public DateTime? DataEmissao { get; set; }

        [DataMember]
        [Document]
        public string Documento { get; set; }

        [DataMember]
        public string NomeDocumento { get; set; }
    }
}