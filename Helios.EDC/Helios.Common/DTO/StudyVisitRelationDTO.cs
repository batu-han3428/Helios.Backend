using Helios.Common.Enums;

namespace Helios.Common.DTO
{
    public class StudyVisitRelationDTO
    {
        public Guid Key { get; set; }
        public Int64 Id { get; set; }
        public Int64 ElementId { get; set; }
        public ActionCondition ActionCondition { get; set; }
        public string ActionValue { get; set; }
        public string TargetPage { get; set; }
        public ActionType ActionType { get; set; }
    }
}
