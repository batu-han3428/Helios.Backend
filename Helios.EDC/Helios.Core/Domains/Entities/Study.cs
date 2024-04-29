namespace Helios.Core.Domains.Entities
{
    public class Study : EntityBase
    {
        public Guid ReferenceKey { get; set; }
        public Int64 VersionKey { get; set; }
        public Int64? EquivalentStudyId { get; set; }
        public string? StudyState { get; set; }
        public int StudyType { get; set; }
        public bool AskSubjectInitial { get; set; }
        public bool IsDemo { get; set; }
        public bool ReasonForChange { get; set; }
        public string? ProtocolCode { get; set; }
        public string StudyLink { get; set; }
        public string? Description { get; set; }
        public string? SubDescription { get; set; }
        public string? StudyLogoPath { get; set; }
        public string? CompanyLogoPath { get; set; }
        public int StudyLanguage { get; set; }
        public string StudyName { get; set; }
        public int SubjectNumberDigitCount { get; set; }
        public bool IsLock { get; set; }
        public Study EquivalentStudy { get; set; }
        public ICollection<Site> Sites { get; set; }
        public ICollection<StudyVisit> StudyVisits { get; set; }
        public ICollection<StudyRole> StudyRoles { get; set; }
        public ICollection<StudyUser> StudyUsers { get; set; }
        public ICollection<Subject> Subjects { get; set; }

    }
}
