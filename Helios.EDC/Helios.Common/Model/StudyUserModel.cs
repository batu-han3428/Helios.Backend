using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helios.Common.Model
{
    public class StudyUserModel: BaseModel
    {
        public Guid StudyUserId { get; set; }
        public Guid AuthUserId { get; set; }
        public Guid StudyId { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string ResearchName { get; set; }
        public int ResearchLanguage { get; set; }
        public bool FirstAddition { get; set; } = false;
        public bool IsActive { get; set; }
        public string Email { get; set; }
        public Guid? RoleId { get; set; }
        public List<Guid> SiteIds { get; set; }
        public List<Guid> ResponsiblePersonIds { get; set; }
    }
}
