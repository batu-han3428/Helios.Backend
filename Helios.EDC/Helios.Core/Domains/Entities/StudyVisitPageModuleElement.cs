using Helios.Core.Domains.Base;
using Helios.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Helios.Core.Domains.Entities
{
    public class StudyVisitPageModuleElement : EntityBase
    {
        public Guid StudyVisitPageModuleId { get; set; }
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
        public bool CanMissing { get; set; }
        public StudyVisitPageModule StudyVisitPageModule { get; set; }
        public ICollection<StudyVisitPageModuleElementDetail> StudyVisitPageModuleElementDetails { get; set; }

    }
}
