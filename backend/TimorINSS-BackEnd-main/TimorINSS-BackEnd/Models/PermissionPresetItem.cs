#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class PermissionPresetItem
    {
        public int Id { get; set; }
        public int PermissionPresetFk { get; set; }
        public string PermissionToken { get; set; }

        public virtual PermissionPreset PermissionPresetFkNavigation { get; set; }
    }
}
