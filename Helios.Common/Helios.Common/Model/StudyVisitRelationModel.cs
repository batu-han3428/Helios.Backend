using Helios.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helios.Common.Model
{
    public class StudyVisitRelationModel
    {
        public List<StudyVisitRelationSourcePageModel> studyVisitRelationSourcePageModels { get; set; }
        public List<VisitRelationModel> visitRelationModels { get; set; }
        public object fieldOperationData { get; set; }
    }

    public class StudyVisitRelationSourcePageModel
    {
        public Int64 Id { get; set; }
        public string Label { get; set; }
        public List<StudyVisitRelationSourcePageModel>? options { get; set; }
    }

    public class VisitRelationModel
    {
        public Guid Key { get; set; }
        public Int64 Id { get; set; }
        public Int64 SourcePageId { get; set; }
        public Int64 ElementId { get; set; }
        public ActionCondition ActionCondition { get; set; }
        public List<string> ActionValue { get; set; }
        public List<Int64> TargetPage { get; set; }
        public ActionType ActionType { get; set; }
    }
}
