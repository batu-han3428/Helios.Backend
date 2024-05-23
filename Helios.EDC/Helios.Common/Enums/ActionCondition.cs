using System.ComponentModel;

namespace Helios.Common.Enums
{
    public enum ActionCondition
    {
        [Description("Less than")]
        Less = 1,
        [Description("More than")]
        More = 2,
        [Description("Equal to")]
        Equal = 3,
        [Description("More than or equal to")]
        MoreAndEqual = 4,
        [Description("Less than or equal to")]
        LessAndEqual = 5,
        [Description("Not equal to")]
        NotEqual = 6,
    }
}
