using Helios.Common.Enums;

namespace Helios.Core.Domains.Entities
{
    public class StudyVisitPageModuleElementEvent : EntityBase
    {
        public Int64 StudyVisitPageModuleId { get; set; }
        public EventType EventType { get; set; }
        public ActionType ActionType { get; set; }
        public Int64 SourceElementId { get; set; }
        public Int64 TargetElementId { get; set; }
        public ActionCondition ValueCondition { get; set; }
        public string? ActionValue { get; set; }
        public string? VariableName { get; set; }
        public Guid ReferenceKey { get; set; }
        public StudyVisitPageModule StudyVisitPageModule { get; set; }
        public StudyVisitPageModuleElement TargetElement { get; set; }
    }
}
