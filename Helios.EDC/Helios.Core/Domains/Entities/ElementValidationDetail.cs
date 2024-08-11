using Helios.Common.Enums;

namespace Helios.Core.Domains.Entities
{
    public class ElementValidationDetail : EntityBase
    {
        public Int64 ElementId { get; set; }
        public Int64 ModuleId { get; set; }
        public ValidationActionType ActionType { get; set; }
        public ActionCondition ValueCondition { get; set; }
        public string? Value { get; set; }
        public string? Message { get; set; }
        public Module Module { get; set; }
        public Element Element { get; set; }
    }
}
