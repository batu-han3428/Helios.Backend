using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helios.Common.DTO
{
    public class EmailTemplateDTO
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public Guid UserId { get; set; }
        public Guid StudyId { get; set; }
        public string Name { get; set; }
        public string Editor { get; set; }
        public int TemplateType { get; set; }
        public List<string> ExternalMails { get; set; }
        public List<Guid> Roles { get; set; }
    }
}
