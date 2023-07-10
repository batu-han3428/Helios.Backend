using Helios.Core.Domains.Base;

namespace Helios.Core.Domains.Entities
{
    public class StudyUserRole : EntityBase
    {
        public Guid StudyUserId { get; set; }
        public Guid StudyRoleId { get; set; }
        public StudyUser StudyUser{ get; set; }
        public StudyRole StudyRole { get; set; }

    }
}
