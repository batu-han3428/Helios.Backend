namespace Helios.Core.Domains.Entities
{
    public class AnnotatedVersions: EntityBase
    {
        public Int64 StudyId { get; set; }
        public string Version { get; set; }
        public string Pdf { get; set; }
    }
}
