using System.ComponentModel;

namespace Helios.Core.Enums
{
    public enum ElementType
    {
        [Description("Label")]
        Label = 1,

        [Description("Text")]
        Text = 2,

        [Description("Integer")]
        Integer = 3,

        [Description("Decimal")]
        Decimal = 4,

        [Description("Table")]
        Table = 6,

        [Description("Radiobutton")]
        RadioList = 7,

        [Description("CheckList")]
        CheckList = 8,

        [Description("Drop-Down")]
        DropDown = 9,

        [Description("Drop-Down checklist")]
        DropDownMulti = 10,

        [Description("Panel")]
        Panel = 11,

        [Description("File attachment")]
        File = 12,

        [Description("Datagrid")]
        DataGrid = 13,

        [Description("Date")]
        DateOption = 14,

        [Description("Calculation")]
        Calculated = 15,

        [Description("Hidden")]
        Hidden = 16,

        [Description("Textarea")]
        Textarea = 17,

        [Description("Randomization")]
        RandomizeButton = 18,

        [Description("Range slider")]
        RangeSlider = 19,

        [Description("Signature")]
        Signature = 20,

        [Description("Numeric")]
        Numeric = 21,

        [Description("Adverse event")]
        AdversEventElement = 22,

        [Description("Source document")]
        SourceDocument = 23,

        [Description("Concomitant medication")]
        ConcomitantMedication = 24,

        [Description("Local Laboratory  ")]
        Labs = 25,
        //Coding = 26,
    }
}
