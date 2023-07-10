using Helios.Core.Domains.Base;

namespace Helios.Core.Domains.Entities
{
    public class StudyRoleModulePermission : EntityBase
    {
        public Guid StudyId { get; set; }
        public bool Read { get; set; }
        public bool Write { get; set; }
        public bool SDV { get; set; }
        public bool Query { get; set; }
        public bool Freeze { get; set; }
        public bool Lock { get; set; }
        public Study Study { get; set; }
    }
}
