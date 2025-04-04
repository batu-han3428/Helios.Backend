﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helios.Common.Model
{
    public class SiteModel: BaseModel
    {
        public Int64 Id { get; set; }
        public Int64 StudyId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public int MaxEnrolmentCount { get; set; }

    }
}
