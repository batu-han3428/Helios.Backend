using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helios.Common.Model
{
    public class TagModel
    {
        public Int64 Id { get; set; }
        public string TagKey { get; set; }
        public string TagName { get; set; }
        public string TagValue { get; set; }
    }
}
