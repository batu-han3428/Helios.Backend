﻿using Helios.Common.Domains.Core.Base;

namespace Helios.Common.Domains.Core.Entities
{
    public class StudyVisitPageModuleCalculationElementDetails : EntityBase
    {
        public Int64 StudyVisitPageModuleId { get; set; }
        public Int64 CalculationElementId { get; set; }
        public Int64 TargetElementId { get; set; }
        public string VariableName { get; set; }
        public StudyVisitPageModule StudyVisitPageModule { get; set; }
    }
}
