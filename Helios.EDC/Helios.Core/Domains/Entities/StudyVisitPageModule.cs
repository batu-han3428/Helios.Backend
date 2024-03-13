using System.ComponentModel.DataAnnotations.Schema;

namespace Helios.Core.Domains.Entities
{
    public class StudyVisitPageModule : EntityBase
    {
        public Int64 StudyVisitPageId { get; set; }
        public string Name { get; set; }
        public Int64 ReferenceKey { get; set; }
        public Int64 VersionKey { get; set; }
        public int Order { get; set; }
        public StudyVisitPage StudyVisitPage { get; set; }
        public ICollection<StudyVisitPageModuleElement> StudyVisitPageModuleElements { get; set; }
    }
}
