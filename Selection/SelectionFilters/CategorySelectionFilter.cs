using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace Selection.Revit.SelectionFilters
{
    // CATEGORY SELECTION FILTER
    public class CategorySelectionFilter : ISelectionFilter
    {
        private readonly ElementId _categoryId;


        public CategorySelectionFilter(
            ElementId categoryId)
        {
            _categoryId = categoryId;
        }


        public bool AllowElement(
            Element element)
        {
            if (element?.Category == null)
                return false;


            return element.Category.Id == _categoryId;
        }


        public bool AllowReference(
            Reference reference,
            XYZ position)
        {
            return false;
        }
    }
}
