namespace Helios.Common.DTO
{
    public class SubjectMissingDataDTO
    {
        public Int64 ElementId { get; set; }
        public string Value { get; set; }
        public string? Comment { get; set; }
        public Int64? CommentType { get; set; }
    }
}
