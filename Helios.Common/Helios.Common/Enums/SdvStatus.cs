using System.ComponentModel;

namespace Helios.Common.Enums
{
    public enum SdvStatus
    {
        [Description("SDV done")]
        SdvDone = 1,
        [Description("Ready for SDV")]
        SdvReady = 2,
        [Description("Partial SDV")]
        SdvPartial = 3
    }
}