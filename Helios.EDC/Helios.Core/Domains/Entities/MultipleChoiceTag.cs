using Helios.Core.Domains.Base;

namespace Helios.Core.Domains.Entities
{
    public class MultipleChoiceTag : EntityBase
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
