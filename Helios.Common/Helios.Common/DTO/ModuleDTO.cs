using Helios.Common.Enums;

namespace Helios.Common.DTO
{
    public class ModuleDTO
    {
        public Int64 StudyVisitPageId { get; set; }
        public string Name { get; set; }
        public List<ElementDTO> StudyVisitPageModuleElements { get; set; }        
        public Guid ReferenceKey { get; set; }
        public Int64 VersionKey { get; set; }
        public int Order { get; set; }
        public Int64 TenantId { get; set; }
    }

    public class ElementDTO
    {
        public Int64 Id { get; set; }
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
        public List<CalculatationElementDetailDTO>? StudyVisitPageModuleCalculationElementDetails { get; set; } = new List<CalculatationElementDetailDTO>();
        public List<ModuleElementEventDTO>? StudyVisitPageModuleElementEvents { get; set; }
        public List<ElementValidationDTO>? StudyVisitPageModuleElementValidationDetails { get; set; }
    }

    public class ElementDetailDTO
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

    }

    public class CalculatationElementDetailDTO
    {
        public Int64 CalculationElementId { get; set; }
        public Int64 TargetElementId { get; set; }
        public string VariableName { get; set; }
    }

    public class ModuleElementEventDTO
    {
        public EventType EventType { get; set; }
        public ActionType ActionType { get; set; }
        public Int64 SourceElementId { get; set; }
        public Int64 TargetElementId { get; set; }
        public ActionCondition ValueCondition { get; set; }
        public string? ActionValue { get; set; }
        public string? VariableName { get; set; }
    }

public class ElementValidationDTO
    {
        public ValidationActionType ValidationActionType { get; set; } = 0;
        public ActionCondition ValidationCondition { get; set; } = 0;
        public string ValidationValue { get; set; } = "";
        public string ValidationMessage { get; set; } = "";
    }

}
