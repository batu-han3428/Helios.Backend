using Helios.Common.Enums;

namespace Helios.Common
{
    public class StudyVisitAnnotatedCrfModel
    {
        public StudyAnnotatedCrfModel StudyModel { get; set; }
        public List<VisitAnnotatedCrfModel> VisitModel { get; set; }
    }

    public class StudyAnnotatedCrfModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string SubDescription { get; set; }
        public string ProtocolCode { get; set; }
    }

    public class VisitAnnotatedCrfModel
    {
        public string Title { get; set; }
        public ElementType? Input { get; set; }
        public string Description { get; set; }
        public bool IsRequired { get; set; }
        public string ElementOptions { get; set; }
        public Dictionary<string, string> DatagridAndTableValue { get; set; }
        public string LowerLimit { get; set; }
        public string UpperLimit { get; set; }
        public List<VisitAnnotatedCrfModel>? Children { get; set; }
    }
}
