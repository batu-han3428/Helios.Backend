﻿using Helios.Common.Enums;

namespace Helios.Common.DTO
{
    public class VisitDTO
    {
        public Int64? Id { get; set; }
        public Int64 UserId { get; set; }
        public Int64 StudyId { get; set; }
        public Int64? ParentId { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public VisitType? VisitType { get; set; }
        public int Order { get; set; }
    }
}
