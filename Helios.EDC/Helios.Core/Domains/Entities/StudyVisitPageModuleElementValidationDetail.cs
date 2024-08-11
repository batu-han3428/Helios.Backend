using Helios.Common.Enums;

namespace Helios.Core.Domains.Entities
{
    public class StudyVisitPageModuleElementValidationDetail : EntityBase
    {
        public Int64 StudyVisitPageModuleElementId { get; set; }
        public Guid ReferenceKey { get; set; }
        public ValidationActionType ActionType { get; set; }
        public ActionCondition ValueCondition { get; set; }
        public string? Value { get; set; }
        public string? Message { get; set; }
        public StudyVisitPageModuleElement StudyVisitPageModuleElement { get; set; }
    }
}
