using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helios.Common.DTO
{
    public class ApiResponse<T> where T : class?
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T? Values { get; set; }
    }
}
