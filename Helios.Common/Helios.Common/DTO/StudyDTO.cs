using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helios.Common.DTO
{
    public class StudyDTO
    {
        public Int64 Id { get; set; }
        public Int64? EquivalentStudyId { get; set; }
        public Int64 UserId { get; set; }
        public string StudyName { get; set; }
        public string? ProtocolCode { get; set; }
        public bool AskSubjectInitial { get; set; }     
        [Column(TypeName = "datetime")]
        public DateTimeOffset UpdatedAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public int StudyLanguage { get; set; }
        public string? Description { get; set; }
        public string? SubDescription { get; set; }
        public int SubjectNumberDigist { get; set; }
        public bool DoubleDataEntry { get; set; }
        public bool ReasonForChange { get; set; }
        public bool IsDemo { get; set; }
        public bool IsLock { get; set; }
    }
}
