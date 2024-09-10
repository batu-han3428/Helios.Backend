using Helios.Common.Enums;

namespace Helios.Common.Model
{
    public class SubjectQueryModel
    {
        public Int64 Id { get; set; }
        public string Comment { get; set; }
        public DateTimeOffset CommentTime { get; set; }
        public string SenderName { get; set; }
        public Int64 SenderId { get; set; }
        public Int64? No { get; set; }
        public CommentType CommentType { get; set; }
    }
}
