using Autodesk.Revit.DB;

namespace Selection.Revit.Models
{
    public class FilterInfo
    {
        public string Name { get; set; }
        public FilterElement FilterElement { get; set; }
        public bool IsRuleBased { get; set; }   // true = ParameterFilterElement, false = SelectionFilterElement
        public override string ToString() => Name;
    }
}
