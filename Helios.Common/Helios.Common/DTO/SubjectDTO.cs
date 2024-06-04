﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Helios.Common.DTO
{
    public class SubjectDTO
    {
        public Int64 StudyId { get; set; }
        public Int64 SiteId { get; set; }
        public Int64 Id { get; set; }
        public Int64 FirstPageId { get; set; }
        public string InitialName { get; set; }
        public string SubjectNumber { get; set; }
        [Column(TypeName = "datetime")]
        public DateTimeOffset CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTimeOffset UpdatedAt { get; set; }
        public string Country { get; set; }
        public string SiteName { get; set; }
        public string RandomData { get; set; }
        public Int64 AddedById { get; set; }
        public string AddedByName { get; set; }
        public bool SDV { get; set; }
        public int Query { get; set; }
       
    }
}
