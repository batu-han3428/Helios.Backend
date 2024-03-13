using Helios.Common.Enums;

namespace Helios.Core.Domains.Entities
{
    public class StudyVisitPageModuleElementDetail : EntityBase
    {
        public Int64 StudyVisitPageModuleElementId { get; set; }
        public Int64? ParentId { get; set; }
        public int? RowIndex { get; set; }
        public int? ColunmIndex { get; set; }
        public bool? CanQuery { get; set; }
        public bool? CanSdv { get; set; }
        public bool? CanRemoteSdv { get; set; }
        public bool? CanComment { get; set; }
        public bool? CanDataEntry { get; set; }
        public int? ParentElementEProPageNumber { get; set; }
        public string? MetaDataTags { get; set; }
        public int? EProPageNumber { get; set; }
        public string? ButtonText { get; set; }
        public string? DefaultValue { get; set; }
        public string? Unit { get; set; }
        public string? LowerLimit { get; set; }
        public string? UpperLimit { get; set; }
        public string? Mask { get; set; }
        public AlignLayout? Layout { get; set; }
        public int? StartDay { get; set; }
        public int? EndDay { get; set; }
        public int? StartMonth { get; set; }
        public int? EndMonth { get; set; }
        public int? StartYear { get; set; }
        public int? EndYear { get; set; }
        public bool? AddTodayDate { get; set; }
        public string? ElementOptions { get; set; }
        public Int64? TargetElementId { get; set; }
        public string? LeftText { get; set; }
        public string? RightText { get; set; }
        public bool IsInCalculation { get; set; }
        public string? MainJs { get; set; }
        public string? RelationMainJs { get; set; }
        public int? RowCount { get; set; }
        public int? ColumnCount { get; set; }
        public string? DatagridAndTableProperties { get; set; }
        public int? AdverseEventType { get; set; }
        public StudyVisitPageModuleElement StudyVisitPageModuleElement { get; set; }
    }
}
