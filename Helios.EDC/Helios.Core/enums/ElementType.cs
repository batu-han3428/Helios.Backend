﻿using System.ComponentModel;

namespace Helios.Core.Enums
{
    public enum ElementType
    {
        [Description("Label")]
        Label = 1,

        [Description("Text")]
        Text = 2,

        [Description("Hidden")]
        Hidden = 3,

        [Description("Numeric")]
        Numeric = 4,

        [Description("Textarea")]
        Textarea = 5,

        [Description("Date")]
        DateOption = 6,

        [Description("Calculation")]
        Calculated = 7,

        [Description("Radiobutton")]
        RadioList = 8,

        [Description("CheckList")]
        CheckList = 9,

        [Description("Drop-Down")]
        DropDown = 10,

        [Description("Drop-Down checklist")]
        DropDownMulti = 11,

        [Description("File attachment")]
        File = 12,

        [Description("Range slider")]
        RangeSlider = 13,

        [Description("Concomitant medication")]
        ConcomitantMedication = 14,

        [Description("Table")]
        Table = 15,

        [Description("Datagrid")]
        DataGrid = 16,

        [Description("Adverse event")]
        AdversEventElement = 17,
    }
}
