namespace Helios.Common.DTO
{
    public class AnnotatedDTO
    {
        public bool IsPage { get; set; }
        public bool IsCalculated { get; set; }
        public bool IsLabel { get; set; }
        public bool IsDesc { get; set; }
        public bool IsAdditional { get; set; }
        public bool IsHiddenElement { get; set; }
        public bool IsHiddenFields { get; set; }
        public string? IsVersion { get; set; }
    }
}
