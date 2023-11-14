using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helios.Common.DTO
{
    public class StudyLockDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public bool IsLock { get; set; }
    }
}
