using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helios.Common.DTO
{
    public class TenantUserDTO
    {
        public Int64 StudyUserId { get; set; }
        public Int64 AuthUserId { get; set; }
        public Int64 StudyId { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public bool IsActive { get; set; }
        public string Email { get; set; }
        public string StudyName { get; set; }
        public bool StudyDemoLive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTimeOffset CreatedOn { get; set; }
        [Column(TypeName = "datetime")]
        public DateTimeOffset LastUpdatedOn { get; set; }
        public string UserRoleName { get; set; }
    }
    public class TenantUserListDTO
    {
        public List<TenantUserDTO> TenantUserList { get; set; }
        public int TenantUserLimit { get; set; }
    }
}
