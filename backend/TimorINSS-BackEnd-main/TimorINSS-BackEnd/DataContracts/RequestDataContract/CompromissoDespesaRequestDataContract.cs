using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class GetCompromissoDespesaListRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Ano { get; set; }
    }

    [DataContract]
    public class GetCabimentosDisponiveisRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Ano { get; set; }
    }

    [DataContract]
    public class CreateCompromissoDespesaRequest : RequestBaseDataContract
    {
        [DataMember]
        public int CabimentoFk { get; set; }

        [DataMember]
        public string Descritivo { get; set; }

        [DataMember]
        public decimal ValorCompromissoGlobal { get; set; }

        [DataMember]
        public decimal ValorCompromissoAno { get; set; }

        [DataMember]
        public int Mes { get; set; }

        [DataMember]
        public int Ano { get; set; }
    }

    [DataContract]
    public class SaveCompromissoDespesaPlurianualidadeRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int CompromissoDespesaFk { get; set; }

        [DataMember]
        public int Ano { get; set; }

        [DataMember]
        public decimal Valor { get; set; }
    }

    [DataContract]
    public class SubmitCompromissoDespesaRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }
    }

    [DataContract]
    public class ReviewCompromissoDespesaRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public bool Approve { get; set; }

        [DataMember]
        public string Comment { get; set; }
    }

    [DataContract]
    public class ApproveCompromissoDespesaRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public bool Approve { get; set; }

        [DataMember]
        public string Comment { get; set; }
    }
}
