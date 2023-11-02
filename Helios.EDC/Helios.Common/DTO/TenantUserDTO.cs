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
        public Guid StudyUserId { get; set; }
        public Guid AuthUserId { get; set; }
        public Guid StudyId { get; set; }
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
    }
}
