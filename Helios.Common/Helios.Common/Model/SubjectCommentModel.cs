namespace Helios.Common.Model
{
    public class SubjectCommentModel
    {
        public Int64 Id { get; set; }
        public string Comment { get; set; }
        public DateTimeOffset CommentTime { get; set; }
        public string SenderName { get; set; }
        public Int64 SenderId { get; set; }
    }
}
