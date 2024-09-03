using System.ComponentModel;

namespace Helios.Common.Enums
{
    public enum CommentType : byte
    {
        [Description("Subject Element Comment")]
        SubjectElementComment = 1,
        [Description("Subject Change Element Comment")]
        SubjectChangeElementComment = 2,
    }
}
