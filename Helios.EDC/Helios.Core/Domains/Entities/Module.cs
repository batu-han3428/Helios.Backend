namespace Helios.Core.Domains.Entities
{
    public class Module : EntityBase
    {
        public string Name { get; set; }
        public List<Element> Elements { get; set; }
    }
}
