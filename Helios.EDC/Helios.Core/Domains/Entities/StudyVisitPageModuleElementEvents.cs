using Helios.Core.Domains.Base;
using Helios.Core.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helios.Core.Domains.Entities
{
    public class StudyVisitPageModuleElementEvents : EntityBase
    {
        public Int64 StudyVisitPageModuleElementId { get; set; }
        public ActionType ActionType { get; set; }
        public Int64 SourceElementId { get; set; }
        public Int64 TargetElementId { get; set; }
        public ActionCondition ValueCondition { get; set; }
        public string ActionValue { get; set; }
        public StudyVisitPageModuleElement StudyVisitPageModuleElement { get; set; }

    }
}
