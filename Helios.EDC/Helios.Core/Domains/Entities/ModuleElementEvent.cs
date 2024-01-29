using Helios.Core.Domains.Base;
using Helios.Common.Enums;

namespace Helios.Core.Domains.Entities
{
    public class ModuleElementEvent : EntityBase
    {
        public Int64 ModuleId { get; set; }
        public EventType EventType { get; set; }
        public ActionType ActionType { get; set; }
        public Int64 SourceElementId { get; set; }
        public Int64 TargetElementId { get; set; }
        public ActionCondition ValueCondition { get; set; }
        public string ActionValue { get; set; }
        public Module Module { get; set; }
    }
}
