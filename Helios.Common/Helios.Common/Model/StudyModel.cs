
namespace Helios.Common.Model
{
    public class StudyModel: BaseModel
    {
        public Int64 StudyId { get; set; }
        public Int64 DemoStudyId { get; set; }
        public Int64? CopyStudyId { get; set; }
        public bool AskSubjectInitial { get; set; }
        public string Description { get; set; }
        public bool DoubleDataEntry { get; set; }
        public string ProtocolCode { get; set; }
        public bool ReasonForChange { get; set; }
        public string StudyLink { get; set; }
        public string StudyName { get; set; }
        public string SubDescription { get; set; }
        public int SubjectNumberDigist { get; set; }
        public int StudyLanguage { get; set; }
        public string? StudyLimit { get; set; }
    }
}
