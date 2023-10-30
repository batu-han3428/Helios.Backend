using Helios.Core.Enums;

namespace Helios.eCRF.Models
{
    public class ElementModel
    {
        public string Id { get; set; }
        public string ModuleId { get; set; }
        public string UserId { get; set; }
        public string ElementDetailId { get; set; }
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
        public bool IsReadonly { get; set; }
        public bool CanMissing { get; set; }

        //public string ParentId { get; set; }
        //public int RowIndex { get; set; }
        //public int ColunmIndex { get; set; }
        //public bool CanQuery { get; set; }
        //public bool CanSdv { get; set; }
        //public bool CanRemoteSdv { get; set; }
        //public bool CanComment { get; set; }
        //public bool CanDataEntry { get; set; }
        //public int ParentElementEProPageNumber { get; set; }
        //public string MetaDataTags { get; set; }
        //public int EProPageNumber { get; set; }
        //public string ButtonText { get; set; }
        //public string DefaultValue { get; set; }
        public string Unit { get; set; }
        public string LowerLimit { get; set; }
        public string UpperLimit { get; set; }
        //public string Extension { get; set; } //numeric description
        //public string Mask { get; set; }
        //public int StartDay { get; set; }
        //public int EndDay { get; set; }
        //public int StartMonth { get; set; }
        //public int EndMonth { get; set; }
        //public int EndYear { get; set; }
        //public int StartYear { get; set; }
        //public bool AddTodayDate { get; set; }

        ////chkbox, radio, ddown vs
        //public string ElementOptions { get; set; } //json text value

        ////hidden
        //public string TargetElementId { get; set; }

        ////rangeslider
        //public string LeftText { get; set; }
        //public string RightText { get; set; }

        ////dependent
        public string DependentSourceFieldId { get; set; }
        public string DependentTargetFieldId { get; set; }
        public int DependentCondition { get; set; }
        public int DependentAction { get; set; }
        public string DependentFieldValue { get; set; }
    }
}
