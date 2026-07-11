using System;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class UserPermission
    {
        public int Id { get; set; }
        public int UtilizadorFk { get; set; }
        public string PermissionToken { get; set; }
        public int? SourcePresetFk { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }

        public virtual Utilizador UtilizadorFkNavigation { get; set; }
        public virtual PermissionPreset SourcePresetFkNavigation { get; set; }
    }
}
