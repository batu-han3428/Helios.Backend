using Helios.Common.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helios.Common.DTO
{
    public class ModuleDTO
    {
        public Int64 Id { get; set; }
        public Int64 StudyVisitPageId { get; set; }
        public string Name { get; set; }
        public Int64 StudyRoleModulePermissionId { get; set; }
        public List<ElementDTO> StudyVisitPageModuleElements { get; set; }
        public List<CalculatationElementDetailDTO> studyVisitPageModuleCalculationElementDetails { get; set; }
        public List<ModuleElementEventDTO> StudyVisitPageModuleElementEvent { get; set; }
        public StudyRoleModulePermissionDTO StudyRoleModulePermission { get; set; }
        public Int64 ReferenceKey { get; set; }
        public Int64 VersionKey { get; set; }
        public int Order { get; set; }
        public bool CanFreeze { get; set; }
        public bool CanLock { get; set; }
        public bool CanSign { get; set; }
        public bool CanVerify { get; set; }
        public bool CanSdv { get; set; }
        public bool CanQuery { get; set; }
    }

    public class ElementDTO
    {
        public Int64 Id { get; set; }
        public Int64 ModuleId { get; set; }
        public Int64? ElementDetailId { get; set; }
        public ElementType ElementType { get; set; }
        public string ElementName { get; set; }
        public string Title { get; set; }
        public bool IsTitleHidden { get; set; }
        public int Order { get; set; }
        public string Description { get; set; }
        public GridLayout Width { get; set; }
        public bool IsHidden { get; set; }
        public bool IsRequired { get; set; }
        public bool IsDependent { get; set; }
        public bool IsRelated { get; set; }
        public bool IsReadonly { get; set; }
        public bool CanMissing { get; set; }
        public ElementDetailDTO? StudyVisitPageModuleElementDetails { get; set; }
    }

    public class ElementDetailDTO
    {
        public Int64 Id { get; set; }
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
        public string? CalculationSourceInputs { get; set; }
        public string? RelationSourceInputs { get; set; }
        public bool IsInCalculation { get; set; }
        public string? MainJs { get; set; }
        public string? RelationMainJs { get; set; }
        public int? RowCount { get; set; }
        public int? ColumnCount { get; set; }
        public string? DatagridAndTableProperties { get; set; }
    }

    public class CalculatationElementDetailDTO
    {
        public Int64 Id { get; set; }
        public Int64 ModuleId { get; set; }
        public Int64 CalculationElementId { get; set; }
        public Int64 TargetElementId { get; set; }
        public string VariableName { get; set; }
    }

    public class ModuleElementEventDTO
    {
        public Int64 Id { get; set; }
        public Int64 ModuleId { get; set; }
        public EventType EventType { get; set; }
        public ActionType ActionType { get; set; }
        public Int64 SourceElementId { get; set; }
        public Int64 TargetElementId { get; set; }
        public ActionCondition ValueCondition { get; set; }
        public string? ActionValue { get; set; }
        public string? VariableName { get; set; }
    }

    public class StudyRoleModulePermissionDTO
    {
        public Int64 StudyRoleId { get; set; }
        public Int64 StudyVisitPageModuleId { get; set; }
        public bool Read { get; set; }
        public bool Write { get; set; }
        public bool SDV { get; set; }
        public bool Query { get; set; }
        public bool Freeze { get; set; }
        public bool Lock { get; set; }
    }
}
