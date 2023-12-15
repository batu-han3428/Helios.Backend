using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helios.Common.DTO
{
    public class AspNetUserDTO
    {
        public Int64 Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public bool IsActive { get; set; }
        public string? Email { get; set; }
    }
}
