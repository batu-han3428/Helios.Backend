using Helios.Common.Enums;

namespace Helios.Common.Model
{
    public class QueryListModel
    {
        public string SubjectNo { get; set; }
        public string SiteName { get; set; }
        public string VisitName { get; set; }
        public string PageName { get; set; }
        public Int64 PageId { get; set; }
        public int OpenedDayNumber { get; set; }
        public Int64 AddedById { get; set; }
        public string FullName { get; set; }
        public Int64? QueryNo { get; set; }
        public string? LastMessageInQuery { get; set; }
        public CommentType Status { get; set; }
        public Int64 ElementId { get; set; }
        public string ElementName { get; set; }
        public string? ElementValue { get; set; }
        public Int64 SubjectId { get; set; }
    }
}
