using Helios.Common.Enums;

namespace Helios.Common.Model
{
    public class SubjectMenuModel
    {
        public Int64 StudyId { get; set; }
        public List<SubjectDetailMenuModel> DetailMenuModels{ get; set; }
    }

    public class SubjectDetailMenuModel
    {
        public Int64 Id { get; set; }
        public string Title { get; set; }
        public VisitType Type { get; set; }
        public List<SubjectDetailMenuModel>? Children { get; set; }
    }
}
