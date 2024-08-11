using System.ComponentModel.DataAnnotations;

namespace Helios.Common.DTO
{
    public class UserPermissionDTO
    {
        public Int64 Id { get; set; }
        public Int64 StudyId { get; set; }

        [Required]
        public string RoleName { get; set; }
        public IEnumerable<int> RolePermissions { get; set; }
    }
}
