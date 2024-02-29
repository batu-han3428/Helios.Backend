using Helios.Common.Domains.Core.Base;

namespace Helios.Common.Domains.Core.Entities
{
    public class Module : EntityBase
    {
        public string Name { get; set; }
        public List<Element> Elements { get; set; }
        public List<CalculatationElementDetail> CalculatationElementDetails { get; set; }
        public List<ModuleElementEvent> ModuleElementEvents { get; set; }
    }
}
