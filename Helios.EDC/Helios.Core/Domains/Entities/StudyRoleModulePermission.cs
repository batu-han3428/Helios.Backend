using Helios.Core.Domains.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helios.Core.Domains.Entities
{
    public class StudyRoleModulePermission : EntityBase
    {
        public Guid StudyRoleId { get; set; }
        public Guid StudyVisitPageModuleId { get; set; }
        public bool Read { get; set; }
        public bool Write { get; set; }
        public bool SDV { get; set; }
        public bool Query { get; set; }
        public bool Freeze { get; set; }
        public bool Lock { get; set; }
        public StudyVisitPageModule StudyVisitPageModule { get; set; }
    }
}
