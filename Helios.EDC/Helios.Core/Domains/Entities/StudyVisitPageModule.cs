using Helios.Core.Domains.Base;

namespace Helios.Core.Domains.Entities
{
    public class StudyVisitPageModule : EntityBase
    {
        public Guid StudyVisitPageId { get; set; }
        public Guid ReferenceKey { get; set; }
        public Guid VersionKey { get; set; }
        public int Order {get;set;}
        public StudyVisitPage StudyVisitPage { get; set; }
        public ICollection<StudyVisitPageModuleElement> StudyVisitPageModuleElements { get; set; }

        ///*
        /// add module dediğimizde module ve elements tablosundakı her şeyi bu tarafa aktaracağız yeni keylerle
        /// */
    }
}
