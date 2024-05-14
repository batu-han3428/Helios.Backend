namespace Helios.Common.Model
{
    public class ElementRankingModel
    {
        public string Key { get; set; }
        public Int64 Id { get; set; }
        public string Title { get; set; }
        public string Order { get; set; }
        public List<ElementRankingModel>? Children { get; set; }
    }
}
