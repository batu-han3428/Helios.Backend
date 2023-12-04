using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helios.Common.DTO
{
    public class EmailTemplateTagDTO
    {
        public Guid? Id { get; set; }
        public Guid UserId { get; set; }
        public Guid TenantId { get; set; }
        public string? Tag { get; set; }
        public int? TemplateType { get; set; }
    }
}
