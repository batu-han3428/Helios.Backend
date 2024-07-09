using System.ComponentModel.DataAnnotations;

namespace Helios.Common.Model
{
    public class UserPermissionRoleModel
    {
        public Int64 Id { get; set; }

        [Required]
        public string RoleName { get; set; }
    }
}