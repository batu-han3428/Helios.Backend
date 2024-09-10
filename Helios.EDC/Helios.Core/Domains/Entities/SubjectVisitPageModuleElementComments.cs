using Helios.Common.Enums;

namespace Helios.Core.Domains.Entities
{
    public class SubjectVisitPageModuleElementComments : EntityBase
    {
        public Int64 SubjectVisitPageModuleElementId { get; set; }
        public Int64? StudyId { get; set; }
        public Int64 SubjectId { get; set; }
        public Int64? No { get; set; }
        public string Comment { get; set; }
        public CommentType CommentType { get; set; }
        public SubjectVisitPageModuleElement SubjectVisitPageModuleElement { get; set; }
    }
}
