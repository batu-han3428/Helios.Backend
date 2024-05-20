namespace Helios.Common.Model
{
    public class SubjectDetailMenuModel
    {
        public Int64 Id { get; set; }
        public string Title { get; set; }
        public List<SubjectDetailMenuModel>? Children { get; set; }
    }
}
