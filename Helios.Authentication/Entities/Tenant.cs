using Helios.Authentication.Entities.Base;

namespace Helios.Authentication.Entities
{
    public class Tenant : EntityBase
    {
        public Guid? AddedById { get; set; }

        public string Name { get; set; }
    }
}
