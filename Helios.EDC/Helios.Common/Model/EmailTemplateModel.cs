using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helios.Common.Model
{
    public class EmailTemplateModel
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string? Name { get; set; }
        public string? TemplateBody { get; set; }
        public Guid? MailTemplateTagId { get; set; }
        public int TemplateType { get; set; }
        public Guid StudyId { get; set; }
        public List<string>? ExternalMails { get; set; }
        public List<Guid> Roles { get; set; }
        [Column(TypeName = "datetime")]
        public DateTimeOffset CreatedAt { get; set; }

        [Column(TypeName = "datetime")]
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
