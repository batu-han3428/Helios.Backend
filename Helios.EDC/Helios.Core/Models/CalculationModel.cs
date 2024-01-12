namespace Helios.Core.Models
{
    public class CalculationModel
    {
        public ElementFieldSelectedGroup elementFieldSelectedGroup { get; set; }
        public string variableName { get; set; }
    }

    public class ElementFieldSelectedGroup
    {
        public string label { get; set; }
        public string value { get; set; }

    }
}
