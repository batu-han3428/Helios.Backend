using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helios.Common.DTO
{
    public class UserPermissionRoleDTO
    {
        public string Role { get; set; }
        public List<Guid> UserIds { get; set; }
    }
}
