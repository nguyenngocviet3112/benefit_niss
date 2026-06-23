using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class UtilizadorListagemResponse : ResponseBaseDataContract
    {
        [DataMember]
        public int rows;

        [DataMember]
        public List<UtilizadorListagem> utilizador;
    }

    [DataContract]
    public class UtilizadorListagem
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public int idTrabalhador { get; set; }

        [DataMember]
        public string utilizador { get; set; }

        [DataMember]
        public string departamento { get; set; }

        [DataMember]
        public string perfil { get; set; }

        [DataMember]
        public List<int> idPerfil { get; set; }
    }

    [DataContract]
    public class DadosUtilizadorResponse : ResponseBaseDataContract
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public int idTrabalhador { get; set; }

        [DataMember]
        public string nomeTrabalhador { get; set; }

        [DataMember]
        public string documentoIdentificacao { get; set; }

        [DataMember]
        public string numeroDocumento { get; set; }

        [DataMember]
        public string? dataValidade { get; set; }

        [DataMember]
        public string dataNascimento { get; set; }

        [DataMember]
        public string email { get; set; }

        [DataMember]
        public string telefone { get; set; }
    }

    [DataContract]
    public class UtilizadoresAcessoListagemResponse : ResponseBaseDataContract
    {
        [DataMember]
        public int rows;

        [DataMember]
        public List<UtilizadoresAcessoListagem> utilizador;
    }

    [DataContract]
    public class UtilizadoresAcessoListagem
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string nome { get; set; }

        [DataMember]
        public string utilizador { get; set; }

        [DataMember]
        public string departamento { get; set; }

        [DataMember]
        public string perfil { get; set; }

        [DataMember]
        public bool interno { get; set; }

        [DataMember]
        public bool locked { get; set; }
    }
}