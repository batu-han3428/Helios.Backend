using System.ComponentModel;

namespace Helios.Common.Enums
{
    public enum TransferChangeType
    {
        [Description("None")]
        None,
        [Description("Insert")]
        Insert,
        [Description("Delete")]
        Delete,
        [Description("Update")]
        Update,
        Back
    }
}
