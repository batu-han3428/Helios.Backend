﻿using Helios.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helios.Common.Model
{
    public class StudyModel: BaseModel
    {
        public Guid StudyId { get; set; }
        public Guid DemoStudyId { get; set; }
        public bool AskSubjectInitial { get; set; }
        public string Description { get; set; }
        public bool DoubleDataEntry { get; set; }
        public string ProtocolCode { get; set; }
        public bool ReasonForChange { get; set; }
        public string StudyLink { get; set; }
        public string StudyName { get; set; }
        public string SubDescription { get; set; }
        public int SubjectNumberDigist { get; set; }
        public int StudyLanguage { get; set; }

    }
}
