﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helios.Common.Model
{
    public class SystemUserModel
    {
        public Int64 Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public IEnumerable<dynamic> Tenants { get; set; }
        public IEnumerable<dynamic> Roles { get; set; }
    }
}
