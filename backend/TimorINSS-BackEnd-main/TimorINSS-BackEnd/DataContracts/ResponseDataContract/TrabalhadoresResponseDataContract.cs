using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class TrabalhadorListagemResponse : ResponseBaseDataContract
    {
        [DataMember]
        public int rows { get; set; }

        [DataMember]
        public List<TrabalhadorListagem> trabalhadores { get; set; }
    }

    [DataContract]
    public class VincularTrabalhadorListagemResponse : ResponseBaseDataContract
    {
        [DataMember]
        public int rows { get; set; }

        [DataMember]
        public List<VincularTrabalhadorListagem> trabalhadores { get; set; }
    }

    [DataContract]
    public class TrabalhadorListagem
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string nome { get; set; }

        [DataMember]
        public string regime { get; set; }

        [DataMember]
        public string incricaoINSS { get; set; }

        [DataMember]
        public DateTime dtInicioDeVinculo { get; set; }

        [DataMember]
        public DateTime? dtFimDeVinculo { get; set; }

        [DataMember]
        public int? idRel { get; set; }
    }

    [DataContract]
    public class VincularTrabalhadorListagem
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string nome { get; set; }

        [DataMember]
        public string niss { get; set; }

        [DataMember]
        public string nissProvisorio { get; set; }
    }

    [DataContract]
    public class SingleTrabalhadorResponse : ResponseBaseDataContract
    {
        [DataMember]
        public TrabalhadorDataContract trabalhador { get; set; }
    }

    [DataContract]
    public class TrabalhadorViewResponse : ResponseBaseDataContract
    {
        [DataMember]
        public TrabalhadorDataContract Trabalhador { get; set; }

        [DataMember]
        public RelEntidadeTrabalhadorDataContract RelEntidadeTrabalhador { get; set; }

        [DataMember]
        public List<INSSEstrangeiroDataContract> InssEstrangeiro { get; set; }
    }
}