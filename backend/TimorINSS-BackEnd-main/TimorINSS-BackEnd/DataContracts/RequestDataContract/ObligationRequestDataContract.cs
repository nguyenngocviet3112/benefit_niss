using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class GetObligationListRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Ano { get; set; }
    }

    [DataContract]
    public class GetCompromissosComSaldoRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Ano { get; set; }
    }

    [DataContract]
    public class CreateObligationRequest : RequestBaseDataContract
    {
        [DataMember]
        public string DescritivoObrigacao { get; set; }

        [DataMember]
        public int Mes { get; set; }

        [DataMember]
        public int Ano { get; set; }
    }

    [DataContract]
    public class AddObligationItemRequest : RequestBaseDataContract
    {
        [DataMember]
        public int ObligationFk { get; set; }

        [DataMember]
        public int CompromissoDespesaFk { get; set; }

        [DataMember]
        public decimal Value { get; set; }
    }

    [DataContract]
    public class RemoveObligationItemRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }
    }

    [DataContract]
    public class SubmitObligationRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }
    }

    [DataContract]
    public class ApproveObligationRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public bool Approve { get; set; }

        [DataMember]
        public string Comment { get; set; }
    }
}
