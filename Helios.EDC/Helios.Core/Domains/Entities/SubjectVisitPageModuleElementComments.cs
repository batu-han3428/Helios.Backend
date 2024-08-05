namespace Helios.Core.Domains.Entities
{
    public class SubjectVisitPageModuleElementComments : EntityBase
    {
        public Int64 SubjectVisitPageModuleElementId { get; set; }
        public string Comment { get; set; }
        public SubjectVisitPageModuleElement SubjectVisitPageModuleElement { get; set; }
    }
}
