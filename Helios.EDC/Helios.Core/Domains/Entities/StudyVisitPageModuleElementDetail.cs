using Helios.Core.Domains.Base;
using Helios.Core.enums;
using System.ComponentModel.DataAnnotations;

namespace Helios.Core.Domains.Entities
{
    public class StudyVisitPageModuleElementDetail : EntityBase
    {
        public Int64 StudyVisitPageModuleElementId { get; set; }
        public Int64 ParentId { get; set; }
        public int RowIndex { get; set; }
        public int ColunmIndex { get; set; }
        public bool CanQuery { get; set; }
        public bool CanSdv { get; set; }
        public bool CanRemoteSdv { get; set; }
        public bool CanComment { get; set; }
        public bool CanDataEntry { get; set; }
        public int ParentElementEProPageNumber { get; set; }
        public string MetaDataTags { get; set; }
        public int EProPageNumber { get; set; }
        public string ButtonText { get; set; }
        public string DefaultValue { get; set; }
        public string Unit { get; set; }
        public string LowerLimit { get; set; }
        public string UpperLimit { get; set; }
        public string Mask { get; set; }
        public AlignLayout Layout { get; set; }
        public string Options { get; set; }
        public StudyVisitPageModuleElement StudyVisitPageModuleElement { get; set; }
    }
}
