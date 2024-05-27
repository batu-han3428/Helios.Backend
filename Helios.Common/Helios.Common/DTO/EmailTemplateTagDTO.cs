using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helios.Common.DTO
{
    public class EmailTemplateTagDTO
    {
        public Int64? Id { get; set; }
        public Int64 UserId { get; set; }
        public Int64 TenantId { get; set; }
        public string? Tag { get; set; }
        public int? TemplateType { get; set; }
    }
}
