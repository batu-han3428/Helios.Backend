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
}
