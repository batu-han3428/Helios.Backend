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
        public Int64 Id { get; set; }
        public Int64 TenantId { get; set; }
        public string? Name { get; set; }
        public string? TemplateBody { get; set; }
        public Int64? MailTemplateTagId { get; set; }
        public int TemplateType { get; set; }
        public Int64 StudyId { get; set; }
        public List<string>? ExternalMails { get; set; }
        public List<Int64> Roles { get; set; }
        [Column(TypeName = "datetime")]
        public DateTimeOffset CreatedAt { get; set; }

        [Column(TypeName = "datetime")]
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
