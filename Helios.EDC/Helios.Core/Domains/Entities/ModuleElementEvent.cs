using Helios.Core.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Helios.Core.Domains.Entities
{
    public class ModuleElementEvent
    {
        [Key]
        public Guid ModuleId { get; set; }
        public ActionType ActionType { get; set; }
        public Guid SourceElementKey { get; set; }
        public Guid TargetElementKey { get; set; }
        public string JavascriptCode { get; set; }
        public string MainJsCode { get; set; }
        public ValueCondition ValueCondition { get; set; }
        public bool ActionResult { get; set; } 
        public Module Module { get; set; }
    }
}
