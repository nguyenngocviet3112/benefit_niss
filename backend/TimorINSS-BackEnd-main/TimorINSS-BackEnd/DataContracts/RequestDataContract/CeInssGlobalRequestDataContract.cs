using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    // CE_INSS_Global — global Receita+Despesa summary grouped only by Classificação Económica
    // (no Programa/Atividade/Regime breakdown). See finance-brd §6.15.
    [DataContract]
    public class CeInssGlobalRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = true)]
        public int year { get; set; }

        // 1-12; null = whole year (Jan..Dec accumulated)
        [DataMember]
        public int? month { get; set; }

        // null = Both (INSS+FRSS combined, displayed as "INSS Global"); otherwise a specific Institution.Id
        [DataMember]
        public int? institution { get; set; }
    }
}
