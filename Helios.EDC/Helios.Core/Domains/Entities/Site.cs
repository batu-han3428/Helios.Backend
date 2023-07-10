using Helios.Core.Domains.Base;

namespace Helios.Core.Domains.Entities
{
    public class Site : EntityBase
    {
        public Guid StudyId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public int MaxEnrolmentCount { get; set; }
        public Study Study { get; set; }

        public string FullName
        {
            get
            {
                var result = CountryCode + Code + " - " + Name;
                return result;
            }
        }

        public string CodeFull
        {
            get
            {
                var result = CountryCode + Code;
                return result;
            }
        }
    }
}
