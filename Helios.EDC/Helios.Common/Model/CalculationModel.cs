namespace Helios.Common.Model
{
    public class CalculationModel
    {
        public ElementFieldSelectedGroup elementFieldSelectedGroup { get; set; }
        public string variableName { get; set; }
    }

    public class ElementFieldSelectedGroup
    {
        public string label { get; set; }
        public Int64 value { get; set; }

    }
}
