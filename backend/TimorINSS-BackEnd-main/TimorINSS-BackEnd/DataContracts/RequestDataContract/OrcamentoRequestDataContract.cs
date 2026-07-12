using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class GetActiveOrcamentoBatchRequest : RequestBaseDataContract
    {
        [DataMember]
        public int OrcamentoConfigFk { get; set; }
    }

    [DataContract]
    public class StartNewOrcamentoBatchRequest : RequestBaseDataContract
    {
        [DataMember]
        public int OrcamentoConfigFk { get; set; }
    }

    [DataContract]
    public class SaveOrcamentoLinhaRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int OrcamentoConfigFk { get; set; }

        [DataMember]
        public int AtividadeFk { get; set; }

        [DataMember]
        public int EconomicClassificationFk { get; set; }

        [DataMember]
        public int? FunctionalClassificationFk { get; set; }

        [DataMember]
        public int OrganizationFk { get; set; }

        [DataMember]
        public decimal Valor { get; set; }
    }

    [DataContract]
    public class DeleteOrcamentoLinhaRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }
    }

    [DataContract]
    public class SubmitOrcamentoBatchRequest : RequestBaseDataContract
    {
        [DataMember]
        public int OrcamentoConfigFk { get; set; }
    }

    [DataContract]
    public class ReviewOrcamentoBatchRequest : RequestBaseDataContract
    {
        [DataMember]
        public int BatchId { get; set; }

        [DataMember]
        public bool Approve { get; set; }

        [DataMember]
        public string Comment { get; set; }
    }

    [DataContract]
    public class ApproveOrcamentoBatchRequest : RequestBaseDataContract
    {
        [DataMember]
        public int BatchId { get; set; }

        [DataMember]
        public bool Approve { get; set; }

        [DataMember]
        public string Comment { get; set; }
    }

    [DataContract]
    public class ImportOrcamentoRequest : RequestBaseDataContract
    {
        public IFormFile File { get; set; }
        public int OrcamentoConfigFk { get; set; }
    }

    // Bước 1/2 của luồng import Excel — chỉ đọc file + đối chiếu, KHÔNG ghi
    // DB (xem CLAUDE.md §6 "Excel import screens must preview before
    // committing"). Cùng shape với ImportOrcamentoRequest, tách tên riêng
    // cho rõ ý nghĩa route.
    [DataContract]
    public class ImportOrcamentoPreviewRequest : RequestBaseDataContract
    {
        public IFormFile File { get; set; }
        public int OrcamentoConfigFk { get; set; }
    }

    [DataContract]
    public class OrcamentoImportRowConfirmRequest
    {
        [DataMember] public int RowNum { get; set; }
        [DataMember] public int AtividadeFk { get; set; }
        [DataMember] public int EconomicClassificationFk { get; set; }
        [DataMember] public int OrganizationFk { get; set; }
        [DataMember] public decimal Valor { get; set; }
        // "Insert" | "Overwrite" | "Skip" — do người dùng chọn ở bước preview,
        // KHÔNG tự suy luận lại ở backend (tránh lệch với những gì user đã
        // thấy/xác nhận trên màn preview).
        [DataMember] public string Action { get; set; }
        [DataMember] public int? ExistingOrcamentoLinhaId { get; set; }
    }

    // Bước 2/2 — áp dụng đúng quyết định (Insert/Overwrite/Skip) người dùng
    // đã chọn cho từng dòng ở bước preview. Không nhận lại file — nhận danh
    // sách dòng đã được preview trả về (FE giữ nguyên, chỉ đổi Action).
    [DataContract]
    public class ConfirmOrcamentoImportRequest : RequestBaseDataContract
    {
        [DataMember] public int OrcamentoConfigFk { get; set; }
        [DataMember] public List<OrcamentoImportRowConfirmRequest> Rows { get; set; }
    }
}
