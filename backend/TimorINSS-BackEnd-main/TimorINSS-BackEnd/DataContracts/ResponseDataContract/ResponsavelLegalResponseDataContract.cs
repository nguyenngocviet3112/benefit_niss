using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class SingleResponsavelLegalResponse : ResponseBaseDataContract
    {
        [DataMember]
        public DateTime DataInicioFuncao { get; set; }

        [DataMember]
        public DateTime? DataFimFuncao { get; set; }

        [DataMember]
        public string Niss { get; set; }

        [DataMember]
        public ResponsavelLegalDataContract responsavelLegal { get; set; }

        [DataMember]
        public List<DocumentoIdentificacaoDataContract> DocumentoIdentificacao { get; set; }
    }

    [DataContract]
    public class ResponsavelLegalListagemResponse : ResponseBaseDataContract
    {
        [DataMember]
        public int rows { get; set; }

        [DataMember]
        public List<ResponsavelLegalListagem> responsavelLegal { get; set; }
    }

    [DataContract]
    public class ResponsavelLegalListagem
    {
        [DataMember]
        public int IdResponsavelLegal { get; set; }

        [DataMember]
        public string Nome { get; set; }

        [DataMember]
        public string Tin { get; set; }

        [DataMember]
        public DateTime DataNascimento { get; set; }

        [DataMember]
        public int Sexo { get; set; }

        [DataMember]
        public int Nacionalidade { get; set; }

        [DataMember]
        public string Naturalidade { get; set; }

        [DataMember]
        public int Funcao { get; set; }

        [DataMember]
        public string FuncaoString { get; set; }

        [DataMember]
        public string FuncaoOutro { get; set; }

        [DataMember]
        public bool IndFuncaoRem { get; set; }

        [DataMember]
        public int? RespLegalTabalhadorFk { get; set; }
    }
}