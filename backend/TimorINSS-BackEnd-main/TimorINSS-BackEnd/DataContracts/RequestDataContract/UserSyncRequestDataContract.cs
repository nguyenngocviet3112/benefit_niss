using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    // Đồng bộ 1 user internal của hệ thống cũ vào mode mới: chỉ tạo cổng vào
    // (UserModeAccess) + hồ sơ (UserProfile) — KHÔNG tự gán quyền
    // (UserPermission), admin phải tự làm bước đó sau ở màn Quản lý User &
    // Phân quyền, giống hệt như tạo user hoàn toàn mới (user quyết định
    // 2026-07-12: không đoán quyền, tránh gán nhầm).
    [DataContract]
    public class SyncInternalUserRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Nome { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public int? DepartamentoFk { get; set; }
    }
}
