namespace Helios.Common.Model
{
    public class RelationModel
    {
        public RelationFieldsSelectedGroup relationFieldsSelectedGroup { get; set; }
        public string variableName { get; set; }
    }

    public class RelationFieldsSelectedGroup
    {
        public string label { get; set; }
        public Int64 value { get; set; }

    }
}
