using Helios.Core.Domains.Base;
using Helios.Core.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Helios.Core.Domains.Entities
{
    public class ModuleElementEvent : EntityBase
    {
        public Int64 ModuleId { get; set; }
        public ActionType ActionType { get; set; }
        public Int64 SourceElementId { get; set; }
        public Int64 TargetElementId { get; set; }
        public ActionCondition ValueCondition { get; set; }
        public string ActionValue { get; set; }
        public Module Module { get; set; }
    }
}
