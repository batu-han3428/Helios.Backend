using System.ComponentModel.DataAnnotations.Schema;

namespace Helios.Common.DTO
{
    public class SubjectListFilterDTO
    {
        public Int64 StudyId { get; set; }
        public Int64 UserId { get; set; }
        public bool ShowArchivedSubjects { get; set; }

    }
}
