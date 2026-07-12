using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class MeuPerfilResponse : ResponseBaseDataContract
    {
        [DataMember] public string Username { get; set; }
        [DataMember] public string Nome { get; set; }
        [DataMember] public string Email { get; set; }
        [DataMember] public int? DepartamentoFk { get; set; }
        [DataMember] public string DepartamentoNome { get; set; }
    }
}
