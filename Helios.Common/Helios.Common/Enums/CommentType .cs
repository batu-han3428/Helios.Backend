using System.ComponentModel;

namespace Helios.Common.Enums
{
    public enum CommentType : byte
    {
        [Description("Subject Element Comment")]
        SubjectElementComment = 1,
        [Description("Subject Change Element Comment")]
        SubjectChangeElementComment = 2,
        [Description("Closed")]
        Query_Closed = 3,
        [Description("Unanswered")]
        Query_Unanswered = 4,
        [Description("Answered")]
        Query_Answered = 5,
        [Description("Data change after query")]
        Query_DataChangeAfterQuery = 6,
        [Description("Last auto query message")]
        Query_LastAutoQueryMessage = 7,
    }
}
