using Helios.Core.Domains.Base;
using Helios.Common.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helios.Core.Domains.Entities
{
    public class Element : EntityBase
    {
        public Int64 ModuleId { get; set; }
        [ForeignKey("ElementDetail")]
        public Int64? ElementDetailId { get; set; }
        public ElementType ElementType { get; set; }
        public string ElementName{ get; set; }
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
        public Module Module { get; set; }
        public ElementDetail? ElementDetail { get; set; }

    }
}
