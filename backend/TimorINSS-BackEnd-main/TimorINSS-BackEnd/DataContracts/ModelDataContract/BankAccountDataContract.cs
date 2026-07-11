using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    // New-mode dedicated screen for the existing Contabancaria table (reused
    // as-is, no new table) — lets Cấu hình hệ thống manage the bank-account
    // list that Receita/Pagamento pick from (mirrors the Idioma/Organization
    // dedicated-screen pattern rather than the old generic Campos Editáveis
    // engine).
    [DataContract]
    public class BankAccountDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string EntidadeBancaria { get; set; }

        [DataMember]
        public string Descricao { get; set; }

        [DataMember]
        public string Swift { get; set; }

        [DataMember]
        public string Iban { get; set; }

        [DataMember]
        public string Numero { get; set; }
    }
}
