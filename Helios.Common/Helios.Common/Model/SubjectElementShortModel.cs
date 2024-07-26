using Helios.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helios.Common.Model
{
    public class SubjectElementShortModel
    {
        public Int64 Id { get; set; }
        public string Value { get; set; } = "";
        public ElementType Type { get; set; }
    }

    public class SubjectVisitPageModuleElementModel
    {
        public Int64 SubjectVisitPageModuleId { get; set; }
        public Int64 StudyVisitPageModuleElementId { get; set; }
        public int DataGridRowId { get; set; }
    }
}
