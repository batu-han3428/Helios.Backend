using Helios.Common.Enums;

namespace Helios.Common.Model
{
    public class SdvModel
    {
        public string SubjectNo { get; set; }
        public string SiteName { get; set; }
        public string VisitName { get; set; }
        public string PageName { get; set; }
        public Int64 PageId { get; set; }
        public Int64 SubjectId { get; set; }
        public SdvStatus SdvStatus { get; set; }
    }
}
