using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class MoradaDataContract
    {
        [DataMember]
        public int IdMorada { get; set; }

        [DataMember]
        public int? IdEntidadeEmpreg { get; set; }

        [DataMember]
        public int? IdTrabalhador { get; set; }

        [DataMember]
        public int? MoradaAldeiaFk { get; set; }

        [DataMember]
        public string Rua { get; set; }

        [DataMember]
        public string NumPorta { get; set; }

        [DataMember]
        public int MoradaPaisFk { get; set; }

        [DataMember]
        public bool MoradaPrincipal { get; set; }
    }
}