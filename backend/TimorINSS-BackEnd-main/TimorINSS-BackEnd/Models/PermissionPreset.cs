using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class PermissionPreset
    {
        public PermissionPreset()
        {
            PermissionPresetItem = new HashSet<PermissionPresetItem>();
            UserPermission = new HashSet<UserPermission>();
        }

        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }

        public virtual ICollection<PermissionPresetItem> PermissionPresetItem { get; set; }
        public virtual ICollection<UserPermission> UserPermission { get; set; }
    }
}
