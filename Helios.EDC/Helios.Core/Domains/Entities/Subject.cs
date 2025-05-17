using Helios.Common.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helios.Core.Domains.Entities
{
    public class Subject : EntityBase
    {
        public Int64 SiteId { get; set; }
        public Int64 StudyId { get; set; }
        public string? InitialName { get; set; }
        public string SubjectNumber { get; set; }
        public DataStatus DataStatus { get; set; }
        public SubjectValidationStatus ValidationStatus { get; set; }
        public GeneralStatus SubjectStatus { get; set; }
        public bool Signature { get; set; }
        public bool Lock { get; set; }
        public bool Freeze { get; set; }
        public string? RandomData { get; set; }
        [Column(TypeName = "datetime")]
        public DateTimeOffset? RandomDataDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTimeOffset? UserValueUpdateDate { get; set; }
        public string? Comment { get; set; }
        public Site Site { get; set; }
        public Study Study { get; set; }
        public ICollection<SubjectVisit> SubjectVisits { get; set; }
    }
}
