using Helios.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helios.Common.Model
{
    public class ElementModel
    {
        public Int64 Id { get; set; }
        public Int64 ModuleId { get; set; }
        public Int64 TenantId { get; set; }
        public Int64 UserId { get; set; }
        public Int64 ElementDetailId { get; set; }
        public Int64 ParentId { get; set; }
        public ElementType ElementType { get; set; } = 0;
        public string ElementName { get; set; } = "";
        public string Title { get; set; } = "";
        public bool IsTitleHidden { get; set; }
        public int Order { get; set; } = 0;
        public string Description { get; set; } = "";
        public GridLayout Width { get; set; } = 0;
        public bool IsHidden { get; set; }
        public bool IsRequired { get; set; }
        public bool IsDependent { get; set; }
        public bool IsRelated { get; set; }
        public bool IsReadonly { get; set; }
        public bool CanMissing { get; set; }

        //public string ParentId { get; set; }
        
        //public bool CanQuery { get; set; }
        //public bool CanSdv { get; set; }
        //public bool CanRemoteSdv { get; set; }
        //public bool CanComment { get; set; }
        //public bool CanDataEntry { get; set; }
        //public int ParentElementEProPageNumber { get; set; }
        //public string MetaDataTags { get; set; }
        //public int EProPageNumber { get; set; }
        //public string ButtonText { get; set; }
        public string? DefaultValue { get; set; } = "";
        public string? Unit { get; set; } = "";
        public string? Mask { get; set; } = "";
        public string? LowerLimit { get; set; } = "";
        public string? UpperLimit { get; set; } = "";
        public AlignLayout? Layout { get; set; } = 0;
        //public string Extension { get; set; } //numeric description
        public int? StartDay { get; set; } = 0;
        public int? EndDay { get; set; } = 0;
        public int? StartMonth { get; set; } = 0;
        public int? EndMonth { get; set; } = 0;
        public int? StartYear { get; set; } = 0;
        public int? EndYear { get; set; } = 0;
        public bool? AddTodayDate { get; set; }

        ////chkbox, radio, ddown vs
        public string? ElementOptions { get; set; } = ""; //json text value

        ////hidden
        //public string TargetElementId { get; set; }

        //rangeslider
        public string? LeftText { get; set; } = "";
        public string? RightText { get; set; } = "";

        public string? CalculationSourceInputs { get; set; } = "";
        public string? MainJs { get; set; } = "";

        //datagrid, table
        public int? RowCount { get; set; } = 0;
        public int? ColumnCount { get; set; } = 0;
        public string? DatagridProperties { get; set; } = "";
        public int? RowIndex { get; set; }
        public int? ColumnIndex { get; set; }

        ////dependent
        public Int64 DependentSourceFieldId { get; set; }
        public Int64 DependentTargetFieldId { get; set; }
        public int DependentCondition { get; set; } = 0;
        public int DependentAction { get; set; } = 0;
        public string DependentFieldValue { get; set; } = "";

        ////relation
        public string RelationSourceInputs { get; set; } = "";
        public string RelationMainJs { get; set; } = "";

        public bool HasChildren { get; set; } = false;
        public List<ElementModel>? ChildElements { get; set; } = new List<ElementModel>();
    }

    public class ElementShortModel
    {
        public Int64 Id { get; set; }
        public Int64 UserId { get; set; }
    }
}
