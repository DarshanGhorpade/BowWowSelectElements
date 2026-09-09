using Autodesk.Revit.DB;

namespace Selection.Revit.Models
{
    // CATEGORY MODEL
    public class SelectedCategory
    {
        public ElementId Id { get; set; }
        public string Name { get; set; }
        public override string ToString() { return Name; }
    }
}
