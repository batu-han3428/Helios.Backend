using Helios.Core.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Helios.Core.Domains.Entities
{
    public class ElementDetail
    {
        public Guid ElementId { get; set; }
        public int RowIndex { get; set; }
        public int ColunmIndex { get; set; }
        public Guid StudyVisitModuleId { get; set; }
        public Guid VisitPageId { get; set; }
        public Guid ElementQueryId { get; set; }
        public bool IsQuery { get; set; }
        public bool IsRemoteSdv { get; set; }
        public SdvState SdvState { get; set; }
        public bool IsComment { get; set; }
        public QueryState QueryState { get; set; }
        public bool IsMarkedNull { get; set; }
        public bool HasDataEntryPermission { get; set; }
        public int ParentRowIndex { get; set; }
        public int ParentColumnIndex { get; set; }
        public Guid ParentId { get; set; }
        public ElementType ParentElementType { get; set; }
        public int ParentElementEProPageNumber { get; set; }
        public bool CanRemoteSdv { get; set; }
        public bool CanSdv { get; set; }
        public bool CanQuery { get; set; }
        public bool CanInputAuditView { get; set; }
        public bool CanMarkAsNull { get; set; }
        public bool CanRandomize { get; set; }
        public bool ViewRandomize { get; set; }
        public bool DoubleEntryCanQuery { get; set; }
        public bool DoubleEntryCanAnswerQuery { get; set; }
        public bool IsDependentField { get; }
        public int PageOrder { get; set; }
        public bool IsShowOnScreen { get; set; }
        public bool CanUNKSee { get; set; }
        public CodingLibrary MedraCoding { get; set; }
        public Guid LabId { get; set; }
        public Guid CustomCodingTagId { get; set; }
        //public bool CanUploadWithOutRequest { get; set; }//??
        //public List<SelectListItem> CustomCodingTags { get; set; }//**??
        public Guid SelectedLabGuid { get; set; }
        public string MetaDataUID { get; set; }
        public string MetaDataTags { get; set; }
        public int EProPageNumber { get; set; }
        public string ButtonText { get; set; }
        public string DefaultValue { get; set; }
        public string UserValue { get; set; }
        public bool IsHidden { get; set; }
        public bool HasPdfForm { get; set; } = false;
        public string LabParamName { get; set; } = "";
        public bool MarkedAsNull { get; set; }
        public string Icon { get; set; }

        public Element Element { get; set; }
    }
}
