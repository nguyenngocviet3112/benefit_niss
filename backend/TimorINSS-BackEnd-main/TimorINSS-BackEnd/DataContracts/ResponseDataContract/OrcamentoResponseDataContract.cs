using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class OrcamentoBatchResponse : ResponseBaseDataContract
    {
        [DataMember]
        public OrcamentoBatchDataContract Batch { get; set; }
    }

    // 1 dòng trong file Excel đã được đối chiếu với DB — dùng cho cả bước
    // preview (trả về FE) và giữ nguyên gửi lại ở bước confirm (chỉ FE đổi
    // Action). Xem CLAUDE.md §6.
    [DataContract]
    public class OrcamentoImportRowDataContract
    {
        [DataMember] public int RowNum { get; set; }
        [DataMember] public string AtividadeCodigo { get; set; }
        [DataMember] public string AtividadeDesignacao { get; set; }
        [DataMember] public string EconomicClassificationCodigo { get; set; }
        [DataMember] public string EconomicClassificationDesignacao { get; set; }
        [DataMember] public string OrganizationNome { get; set; }
        [DataMember] public decimal Valor { get; set; }
        // "New" | "Exists" | "Error"
        [DataMember] public string Status { get; set; }
        [DataMember] public string ErrorMessage { get; set; }
        [DataMember] public int? ExistingOrcamentoLinhaId { get; set; }
        [DataMember] public decimal? ExistingValor { get; set; }
        [DataMember] public int? AtividadeFk { get; set; }
        [DataMember] public int? EconomicClassificationFk { get; set; }
        [DataMember] public int? OrganizationFk { get; set; }
    }

    [DataContract]
    public class ImportOrcamentoPreviewResponse : ResponseBaseDataContract
    {
        [DataMember] public List<OrcamentoImportRowDataContract> Rows { get; set; } = new List<OrcamentoImportRowDataContract>();
        [DataMember] public int TotalNew { get; set; }
        [DataMember] public int TotalExists { get; set; }
        [DataMember] public int TotalErrors { get; set; }
    }
}
