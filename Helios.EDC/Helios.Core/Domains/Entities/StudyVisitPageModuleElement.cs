using Helios.Common.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helios.Core.Domains.Entities
{
    public class StudyVisitPageModuleElement : EntityBase
    {
        public Int64 StudyVisitPageModuleId { get; set; }
        [ForeignKey("StudyVisitPageModuleElementDetail")]
        public Int64? StudyVisitPageModuleElementDetailId { get; set; }
        public Int64 ReferenceKey { get; set; }
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
        public StudyVisitPageModule StudyVisitPageModule { get; set; }
        public StudyVisitPageModuleElementDetail? StudyVisitPageModuleElementDetail { get; set; }
        public ICollection<StudyVisitPageModuleCalculationElementDetail>? studyVisitPageModuleCalculationElementDetails { get; set; }
        public ICollection<StudyVisitPageModuleElementEvent>? StudyVisitPageModuleElementEvents { get; set; }

        [NotMapped]
        public Int64 ElementId { get; set; }

    }
}
