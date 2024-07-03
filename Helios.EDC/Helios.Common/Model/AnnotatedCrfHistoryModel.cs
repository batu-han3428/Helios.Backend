namespace Helios.Common.Model
{
    public class AnnotatedCrfHistoryModel
    {
        public Int64 Id { get; set; }
        public string Version { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}
