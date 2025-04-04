﻿using Helios.Common.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helios.Core.Domains.Entities
{
    public class ElementDetail : EntityBase
    {
        [ForeignKey("Element")]
        public Int64 ElementId { get; set; }
        public Int64? ParentId { get; set; }
        public int? RowIndex { get; set; }
        public int? ColunmIndex { get; set; }
        public string? MetaDataTags { get; set; }
        public string? ButtonText { get; set; }
        public string? DefaultValue { get; set; }
        public string? Unit { get; set; }
        public string? LowerLimit { get; set; }
        public string? UpperLimit { get; set; }
        public string? Extension { get; set; } //numeric description
        public string? Mask { get; set; }
        public AlignLayout? Layout { get; set; }

        //datagrid, table
        public int? RowCount { get; set; }
        public int? ColumnCount { get; set; }
        public string? DatagridAndTableProperties { get; set; }

        //datetime
        public int? StartDay { get; set; }
        public int? EndDay { get; set; }
        public int? StartMonth { get; set; }
        public int? EndMonth { get; set; }
        public int? EndYear { get; set; }
        public int? StartYear { get; set; }
        public bool? AddTodayDate { get; set; }

        //chkbox, radio, ddown vs
        public string? ElementOptions { get; set; } //json text value

        //rangeslider
        public string? LeftText { get; set; }
        public string? RightText { get; set; }

        //calculation
        public bool IsInCalculation { get; set; }
        public string? MainJs { get; set; }

        public string? RelationMainJs { get; set; }

        public int? AdverseEventType { get; set; }

        public Element Element { get; set; }

    }
}
