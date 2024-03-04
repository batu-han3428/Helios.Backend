using Helios.Common.Domains.Core.Base;
using Helios.Common.Enums;

namespace Helios.Common.Domains.Core.Entities
{
    public class ModuleElementEvent : EntityBase
    {
        public Int64 ModuleId { get; set; }
        public EventType EventType { get; set; }
        public ActionType ActionType { get; set; }
        public Int64 SourceElementId { get; set; }
        public Int64 TargetElementId { get; set; }
        public ActionCondition ValueCondition { get; set; }
        public string? ActionValue { get; set; }
        public string? VariableName { get; set; }
        public Module Module { get; set; }
    }
}
