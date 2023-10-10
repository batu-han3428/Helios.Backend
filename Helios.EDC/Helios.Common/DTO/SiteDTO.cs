using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helios.Common.DTO
{
    public class SiteDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public string SiteFullName { get; set; }
        public string SiteNameWithCountry { get; set; }
        public int MaxEnrolmentCount { get; set; }
        public string SystemTimeZoneName { get; set; }
        [Column(TypeName = "datetime")]
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
