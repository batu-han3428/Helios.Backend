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
        public Guid StudyVisitPageModuleElementId { get; set; }
        public ActionType ActionType { get; set; }
        public Guid SourceElementId { get; set; }
        public Guid TargetElementId { get; set; }
        public ActionCondition ValueCondition { get; set; }
        public string ActionValue { get; set; }
        public StudyVisitPageModuleElement StudyVisitPageModuleElement { get; set; }

    }
}
