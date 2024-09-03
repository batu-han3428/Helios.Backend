using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helios.Common.DTO
{
    public class SubjectMultiDTO
    {
        public Int64 Id { get; set; }
        public string FormName { get; set; }
        public string FormNo { get; set; }
        [Column(TypeName = "datetime")]
        public DateTimeOffset CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTimeOffset UpdatedAt { get; set; }
        public int Status { get; set; }
        public int FormStatus { get; set; }
    }
}
