using Helios.Common.Domains.Core.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helios.Common.Domains.Core.Entities
{
    public class StudyRoleModulePermission : EntityBase
    {
        [ForeignKey("StudyRole")]
        public Int64 StudyRoleId { get; set; }
        public Int64 StudyVisitPageModuleId { get; set; }
        public bool Read { get; set; }
        public bool Write { get; set; }
        public bool SDV { get; set; }
        public bool Query { get; set; }
        public bool Freeze { get; set; }
        public bool Lock { get; set; }
        public StudyRole StudyRole { get; set; }
        public StudyVisitPageModule StudyVisitPageModule { get; set; }
    }
}
