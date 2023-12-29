using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helios.Common.DTO
{
    public class SystemAdminDTO
    {
        public Int64 Id { get; set; }
        public Int64 UserId { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public List<Int64>? RoleIds { get; set; }
        public List<Int64>? TenantIds { get; set; }
        public string? Password { get; set; }
        public string? Language { get; set; }
        public bool? isAddUser { get; set; }
    }
}
