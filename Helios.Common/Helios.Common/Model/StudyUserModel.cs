﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helios.Common.Model
{
    public class StudyUserModel: BaseModel
    {
        public Int64 StudyUserId { get; set; }
        public Int64 AuthUserId { get; set; }
        public Int64 StudyId { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string ResearchName { get; set; }
        public int ResearchLanguage { get; set; }
        public bool FirstAddition { get; set; } = false;
        public bool IsActive { get; set; }
        public string Email { get; set; }
        public Int64? RoleId { get; set; }
        public List<Int64> SiteIds { get; set; }
        public List<Int64> ResponsiblePersonIds { get; set; }
    }
    public class ResetUserProfileViewModel : BaseModel
    {
       
        public Int64 AuthUserId { get; set; }        
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }

    }
}
