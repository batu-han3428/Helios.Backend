using Helios.Core.Domains.Base;

namespace Helios.Core.Domains.Entities
{
    public class Study : EntityBase
    {
        public Guid ReferansKey { get; set; }
        public Guid VersiyonKey { get; set; }
        public Guid? EquivalentStudyId { get; set; }
        public int StudyType { get; set; }
        public bool AskInitial { get; set; }
        public bool IsDemo { get; set; }
        public string Code { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public string SubDescription { get; set; }
        public string StudyLogoPath { get; set; }
        public string CompanyLogoPath { get; set; }
        public string Language { get; set; }
        public Study EquivalentStudy { get; set; }
    }
}
