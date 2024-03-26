﻿namespace Helios.Core.Domains.Entities
{
    public class StudyVisitPageModuleCalculationElementDetail : EntityBase
    {
        public Int64 StudyVisitPageModuleId { get; set; }
        public Int64 CalculationElementId { get; set; }
        public Int64 TargetElementId { get; set; }
        public Int64 ReferenceKey { get; set; }
        public string VariableName { get; set; }
        public StudyVisitPageModule StudyVisitPageModule { get; set; }
        public StudyVisitPageModuleElement CalculationElement { get; set; }
    }
}
