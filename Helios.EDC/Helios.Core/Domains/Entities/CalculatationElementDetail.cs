using Helios.Core.Domains.Base;

namespace Helios.Core.Domains.Entities
{
    public class CalculatationElementDetail : EntityBase
    {
        public Int64 ModuleId { get; set; }
        public Int64 CalculationElementId { get; set; }
        public Int64 TargetElementId { get; set; }
        public string? VariableName { get; set; }
        public Module Module { get; set; }
    }
}
