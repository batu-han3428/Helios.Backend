using Helios.Common.Domains.Core.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helios.Common.Domains.Core.Entities
{
    public class StudyVisitPageModule : EntityBase
    {
        public Int64 StudyVisitPageId { get; set; }
        [ForeignKey("StudyRoleModulePermission")]
        public Int64 StudyRoleModulePermissionId { get; set; }
        public string Name { get; set; }
        public Int64 ReferenceKey { get; set; }
        public Int64 VersionKey { get; set; }
        public int Order { get; set; }
        public bool CanFreeze { get; set; }
        public bool CanLock { get; set; }
        public bool CanSign { get; set; }
        public bool CanVerify { get; set; }
        public bool CanSdv { get; set; }
        public bool CanQuery { get; set; }
        public StudyVisitPage StudyVisitPage { get; set; }
        public StudyRoleModulePermission StudyRoleModulePermission { get; set; }
        public ICollection<StudyVisitPageModuleElement> StudyVisitPageModuleElements { get; set; }
        public List<StudyVisitPageModuleCalculationElementDetails> studyVisitPageModuleCalculationElementDetails { get; set; }
        public List<StudyVisitPageModuleElementEvent> StudyVisitPageModuleElementEvent { get; set; }

        ///*
        /// add module dediğimizde module ve elements tablosundakı her şeyi bu tarafa aktaracağız yeni keylerle
        /// */
    }
}
