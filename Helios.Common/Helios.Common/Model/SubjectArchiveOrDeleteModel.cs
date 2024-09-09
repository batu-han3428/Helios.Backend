using Helios.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helios.Common.Model
{
    public class SubjectArchiveOrDeleteModel
    {
        public Int64 SubjectId { get; set; }
        public bool IsDelete { get; set; }
        public bool IsArchived { get; set; }
        public string Comment { get; set; } = "";
    }
    
    public class SubjectMultiFormArchiveOrDeleteModel
    {
        public Int64 SubjectId { get; set; }
        public Int64 SubjectVisitId { get; set; }
        public int RowIndex { get; set; }
        public bool IsArchived { get; set; }
        public string Comment { get; set; } = "";
    }
}
