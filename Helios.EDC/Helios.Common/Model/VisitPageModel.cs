using Helios.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helios.Common.Model
{
    public class VisitPageModel
    {
        public Int64 Id { get; set; }
        public string PageName { get; set; }
        public int Order { get; set; }

        [Column(TypeName = "datetime")]
        public DateTimeOffset UpdatedAt { get; set; }
        public List<VisitPageModuleModel> VisitPageModuleModels { get; set; }
    }
}
