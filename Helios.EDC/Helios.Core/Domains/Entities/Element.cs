using Helios.Core.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helios.Core.Domains.Entities
{
    public class Element
    {
        public Guid ModuleId { get; set; }
        [Key]
        public Guid ElementKey { get; set; }
        public ElementType ElementType { get; set; }
        public string ElementName{ get; set; }
        public string Title { get; set; }
        public bool IsTitleHidden { get; set; }
        public int Order { get; set; }
        public string Description { get; set; }
        public string CustomClass { get; set; }
        public string DataAttributes { get; set; }
        public string Value { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public byte Border { get; set; }
        public string Padding { get; set; }
        public string Margin { get; set; }
        public string TextColor { get; set; }
        public string BackColor { get; set; }
        public AlignLayout AlignLayout { get; set; }
        public GridLayout GridLayout { get; set; }
        public GridOffset GridOffset { get; set; }
        public TextAlign TextAlign { get; set; }
        public byte FontSize { get; set; }
        public int MainBorder { get; set; }
        public BorderStyle MainBorderStyle { get; set; }
        public string MainBorderColor { get; set; }

        //public bool CanUploadWithOutRequest { get; set; }//??
        //public List<SelectListItem> CustomCodingTags { get; set; }//**??

        public Module Module { get; set; }

    }
}
