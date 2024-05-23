using Helios.Common.Enums;

namespace Helios.Core.Domains.Entities
{
    public class StudyVisitRelation: EntityBase
    {
        public Int64 ElementId { get; set; }
        public ActionCondition ActionCondition { get; set; }
        public string ActionValue { get; set; }
        public string TargetPage { get; set; }
        public ActionType ActionType { get; set; }
        public Int64 StudyId { get; set; }
    }
}