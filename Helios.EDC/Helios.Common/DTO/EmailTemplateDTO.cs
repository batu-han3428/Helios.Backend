using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helios.Common.DTO
{
    public class EmailTemplateDTO
    {
        public Int64 Id { get; set; }
        public Int64 TenantId { get; set; }
        public Int64 UserId { get; set; }
        public Int64 StudyId { get; set; }
        public string Name { get; set; }
        public string Editor { get; set; }
        public int TemplateType { get; set; }
        public List<string> ExternalMails { get; set; }
        public List<Int64> Roles { get; set; }
    }
}
