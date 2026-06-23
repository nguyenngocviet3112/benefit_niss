using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class MoradaListagemResponse : ResponseBaseDataContract
    {
        [DataMember]
        public int rows;

        [DataMember]
        public List<MoradaListagem> morada;
    }

    [DataContract]
    public class MoradaListagem
    {
        [DataMember]
        public int idMorada { get; set; }

        [DataMember]
        public int idEntidadeEmpreg { get; set; }

        [DataMember]
        public string rua { get; set; }

        [DataMember]
        public string? municipio { get; set; }

        [DataMember]
        public int? idMunicipio { get; set; }

        [DataMember]
        public string? postoAdministrativo { get; set; }

        [DataMember]
        public int? idPostoAdministrativo { get; set; }

        [DataMember]
        public String? suco { get; set; }

        [DataMember]
        public int? idSuco { get; set; }

        [DataMember]
        public String? aldeia { get; set; }

        [DataMember]
        public int? idAldeia { get; set; }

        [DataMember]
        public String pais { get; set; }

        [DataMember]
        public int idPais { get; set; }

        [DataMember]
        public bool moradaPrincipal { get; set; }

        [DataMember]
        public String numPorta { get; set; }

        [DataMember]
        public String ruaNumPorta { get; set; }
    }
}