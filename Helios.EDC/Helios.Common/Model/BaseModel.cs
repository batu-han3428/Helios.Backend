﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helios.Common.Model
{
    public class BaseModel
    {
        public Guid UserId { get; set; }
        public Guid TenantId { get; set; }
    }
}
