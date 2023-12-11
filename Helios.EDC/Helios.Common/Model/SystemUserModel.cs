using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helios.Common.Model
{
    public class SystemUserModel
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
    }
}
