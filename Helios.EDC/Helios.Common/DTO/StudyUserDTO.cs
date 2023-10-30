using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helios.Common.DTO
{
    public class StudyUserDTO
    {
        public Guid StudyUserId { get; set; }
        public Guid AuthUserId { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public bool IsActive { get; set; }
        public string Email { get; set; }
        public string? RoleName { get; set; }
        public string Password { get; set; }
        public Guid? RoleId { get; set; }
        public List<SiteDTO> Sites { get; set; }
        [Column(TypeName = "datetime")]
        public DateTimeOffset CreatedOn { get; set; }
        [Column(TypeName = "datetime")]
        public DateTimeOffset LastUpdatedOn { get; set; }
    }
}
