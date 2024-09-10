using Helios.Common.Enums;

namespace Helios.Common.DTO
{
    public class SubjectQueryDTO
    {
        public Int64 Id { get; set; }
        public Int64 ElementId { get; set; }
        public string Comment { get; set; }
        public CommentType CommentType { get; set; }
        public Int64 No { get; set; }
        public Int64 SubjectId { get; set; }
    }
}
