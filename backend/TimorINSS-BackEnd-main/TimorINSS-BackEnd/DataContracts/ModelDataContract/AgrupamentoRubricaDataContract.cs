using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    // "Mapeamento Rubricas" — KHÁC với AgrupamentoConfigDataContract (dùng cho màn cũ/
    // mục đích khác). Màn này chỉ quản lý 4 loại TipoConta phục vụ đúng mapping
    // Codigoconta -> dòng Balanço/DR (Receita/Despesa/Neutro Receita/Neutro Despesa) —
    // xem AgrupamentoRubricaController để biết lý do loại trừ Actidade/Funcional
    // (2 loại đó trùng lặp với ProgramActivity/FunctionalClassification đã có).
    [DataContract]
    public class AgrupamentoRubricaDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Codigo { get; set; }

        [DataMember]
        public string Designacao { get; set; }

        [DataMember]
        public int? ParentFk { get; set; }

        [DataMember]
        public int OrcamentoConfigFk { get; set; }

        [DataMember]
        public string TipoConta { get; set; }

        [DataMember]
        public bool IndActivo { get; set; }

        [DataMember]
        public bool HasKids { get; set; }
    }
}
