using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helios.Common.Model
{
    public class SSOUserStudyModel: BaseModel
    {
        public Guid StudyId { get; set; }
        public string StudyName { get; set; }
        public string UserRoleName { get; set; }
        public string Statu { get; set; }
    }
}
