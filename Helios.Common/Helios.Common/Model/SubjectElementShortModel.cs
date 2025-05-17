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
        public string ElementName { get; set; }
        public string Value { get; set; } = "";
        public ElementType Type { get; set; }
        public string? Comment { get; set; }
        public CommentType? CommentType { get; set; }
    }
}
