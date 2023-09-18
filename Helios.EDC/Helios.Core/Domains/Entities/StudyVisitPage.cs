using Helios.Core.Domains.Base;

namespace Helios.Core.Domains.Entities
{
    public class StudyVisitPage : EntityBase
    {
        public Guid StudyVisitId { get; set; }
        public Guid ReferenceKey { get; set; }
        public Guid VersionKey { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public StudyVisit StudyVisit { get; set; }
        public ICollection<StudyVisitPageModule> StudyVisitPageModules { get; set; }
    }
}
