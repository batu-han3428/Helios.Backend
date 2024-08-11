namespace Helios.Core.Domains.Entities
{
    public class SubjectVisitPageModuleElement : EntityBase
    {
        public Int64 StudyVisitPageModuleElementId { get; set; }
        public Int64 SubjectVisitModuleId { get; set; }
        public int? DataGridRowId { get; set; }
        public string? UserValue { get; set; }
        public bool ShowOnScreen { get; set; }
        public bool MissingData { get; set; }
        public bool Sdv { get; set; }
        public bool Query { get; set; }
        public StudyVisitPageModuleElement StudyVisitPageModuleElement { get; set; }
        public SubjectVisitPageModule SubjectVisitModule { get; set; }
        public List<SubjectVisitPageModuleElementComments> SubjectVisitPageModuleElementComments { get; set; }
    }
}
