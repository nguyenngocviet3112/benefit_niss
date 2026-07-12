using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class PermissionTokenDataContract
    {
        [DataMember]
        public string Token { get; set; }

        [DataMember]
        public string Label { get; set; }
    }

    [DataContract]
    public class PermissionGroupDataContract
    {
        [DataMember]
        public string Codigo { get; set; }

        [DataMember]
        public string Nome { get; set; }

        [DataMember]
        public List<PermissionTokenDataContract> Tokens { get; set; } = new List<PermissionTokenDataContract>();
    }

    [DataContract]
    public class PermissionPresetDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Codigo { get; set; }

        [DataMember]
        public string Nome { get; set; }

        [DataMember]
        public List<string> Tokens { get; set; } = new List<string>();
    }

    // Fixed permission-token catalog — domain × action, per the granular RBAC
    // design locked for the new-mode "Quản lý User & Phân quyền" screen.
    // AD/CABIMENTO/COMPROMISSO/OBRIGACAO/PAG split & approval-gate counts match
    // the legal chain in ReuniaoSS_2024.pptx (see customer-provided-data-findings
    // memory, 2026-07-11) — supersedes the earlier flatter DESP_*/execution-only
    // draft. Intentionally NOT a DB table: this list only changes with a code
    // deploy, never by an admin at runtime.
    public static class PermissionCatalog
    {
        public static readonly List<PermissionGroupDataContract> Groups = new List<PermissionGroupDataContract>
        {
            new PermissionGroupDataContract
            {
                Codigo = "ORC",
                Nome = "Orçamento",
                Tokens = new List<PermissionTokenDataContract>
                {
                    new PermissionTokenDataContract { Token = "ORC_SUBMIT", Label = "Gửi duyệt Orçamento" },
                    new PermissionTokenDataContract { Token = "ORC_REVIEW", Label = "Kiểm tra Orçamento" },
                    new PermissionTokenDataContract { Token = "ORC_APPROVE", Label = "Phê duyệt Orçamento" },
                }
            },
            new PermissionGroupDataContract
            {
                Codigo = "AD",
                Nome = "AD - Autorização de Despesa",
                Tokens = new List<PermissionTokenDataContract>
                {
                    new PermissionTokenDataContract { Token = "AD_SUBMIT", Label = "Gửi duyệt AD" },
                    new PermissionTokenDataContract { Token = "AD_REVIEW", Label = "Kiểm tra AD (Diretor Depto)" },
                    new PermissionTokenDataContract { Token = "AD_APPROVE", Label = "Phê duyệt AD (Diretor Executivo)" },
                }
            },
            new PermissionGroupDataContract
            {
                Codigo = "CABIMENTO",
                Nome = "Cabimento (DIC)",
                Tokens = new List<PermissionTokenDataContract>
                {
                    new PermissionTokenDataContract { Token = "CABIMENTO_SUBMIT", Label = "Gửi duyệt Cabimento" },
                    new PermissionTokenDataContract { Token = "CABIMENTO_APPROVE", Label = "Phê duyệt Cabimento (Diretor DF)" },
                }
            },
            new PermissionGroupDataContract
            {
                Codigo = "COMPROMISSO",
                Nome = "Compromisso",
                Tokens = new List<PermissionTokenDataContract>
                {
                    new PermissionTokenDataContract { Token = "COMPROMISSO_SUBMIT", Label = "Gửi duyệt Compromisso" },
                    new PermissionTokenDataContract { Token = "COMPROMISSO_REVIEW", Label = "Kiểm tra Compromisso (Diretor DF)" },
                    new PermissionTokenDataContract { Token = "COMPROMISSO_APPROVE", Label = "Phê duyệt Compromisso (Diretor Executivo)" },
                }
            },
            new PermissionGroupDataContract
            {
                Codigo = "OBRIGACAO",
                Nome = "Obrigação",
                Tokens = new List<PermissionTokenDataContract>
                {
                    new PermissionTokenDataContract { Token = "OBRIGACAO_SUBMIT", Label = "Gửi duyệt Obrigação" },
                    new PermissionTokenDataContract { Token = "OBRIGACAO_APPROVE", Label = "Phê duyệt & Liquidação Obrigação (Diretor DF)" },
                }
            },
            new PermissionGroupDataContract
            {
                Codigo = "REC",
                Nome = "Receita",
                Tokens = new List<PermissionTokenDataContract>
                {
                    new PermissionTokenDataContract { Token = "REC_SUBMIT", Label = "Gửi duyệt Receita" },
                    new PermissionTokenDataContract { Token = "REC_REVIEW", Label = "Kiểm tra Receita" },
                    new PermissionTokenDataContract { Token = "REC_APPROVE", Label = "Phê duyệt Receita" },
                }
            },
            new PermissionGroupDataContract
            {
                Codigo = "ABE",
                Nome = "Saldos de Abertura",
                Tokens = new List<PermissionTokenDataContract>
                {
                    new PermissionTokenDataContract { Token = "ABE_SUBMIT", Label = "Nhập Saldos de Abertura" },
                    new PermissionTokenDataContract { Token = "ABE_APPROVE", Label = "Khoá/Xác nhận Saldos de Abertura" },
                }
            },
            new PermissionGroupDataContract
            {
                Codigo = "PAG",
                Nome = "Pagamento",
                Tokens = new List<PermissionTokenDataContract>
                {
                    new PermissionTokenDataContract { Token = "PAG_SUBMIT", Label = "Gửi duyệt Autorização de Pagamento" },
                    new PermissionTokenDataContract { Token = "PAG_APPROVE", Label = "Phê duyệt Autorização de Pagamento (Diretor DF)" },
                    new PermissionTokenDataContract { Token = "PAG_EXECUTE", Label = "Thực hiện Realização de Pagamento" },
                }
            },
            new PermissionGroupDataContract
            {
                Codigo = "EXEC",
                Nome = "Thao tác khác",
                Tokens = new List<PermissionTokenDataContract>
                {
                    new PermissionTokenDataContract { Token = "BANCO_CONCILIAR", Label = "Conciliação de Movimentos bancários" },
                }
            },
            new PermissionGroupDataContract
            {
                Codigo = "DASH",
                Nome = "Dashboard",
                Tokens = new List<PermissionTokenDataContract>
                {
                    new PermissionTokenDataContract { Token = "DASHBOARD_VIEW", Label = "Xem Dashboard tổng quan" },
                }
            },
            new PermissionGroupDataContract
            {
                Codigo = "SYS",
                Nome = "Hệ thống",
                Tokens = new List<PermissionTokenDataContract>
                {
                    new PermissionTokenDataContract { Token = "MASTERDATA_MANAGE", Label = "Quản lý Master Data" },
                    new PermissionTokenDataContract { Token = "REPORT_VIEW", Label = "Xem báo cáo" },
                    new PermissionTokenDataContract { Token = "USER_MANAGE", Label = "Quản lý User & Phân quyền" },
                    new PermissionTokenDataContract { Token = "ADMIN", Label = "Quản trị hệ thống (toàn quyền - bỏ qua mọi kiểm tra quyền khác)" },
                }
            },
        };

        public static readonly HashSet<string> AllTokens = Groups.SelectMany(g => g.Tokens.Select(t => t.Token)).ToHashSet();

        public static bool IsValidToken(string token) => AllTokens.Contains(token);
    }
}
