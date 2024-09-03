namespace Helios.Common.DTO
{
    public class SubjectCommentDTO
    {
        public Int64 Id { get; set; }
        public Int64 ElementId { get; set; }
        public string Comment { get; set; }
        public Int64 CommentType { get; set; }
    }
}
