using Autodesk.Revit.DB;
using Selection.Revit.Models;
using System.Collections.Generic;
using System.Linq;

namespace Selection.Revit.Utils
{
    internal static class FilterHelper
    {
        public static List<FilterInfo> GetFilters(Document doc)
        {
            var list = new List<FilterInfo>();

            // Rule-based filters (ParameterFilterElement)
            foreach (ParameterFilterElement f in new FilteredElementCollector(doc)
                .OfClass(typeof(ParameterFilterElement))
                .Cast<ParameterFilterElement>())
            {
                list.Add(new FilterInfo
                {
                    Name = f.Name,
                    FilterElement = f,
                    IsRuleBased = true
                });
            }

            // Selection filters (SelectionFilterElement)
            foreach (SelectionFilterElement f in new FilteredElementCollector(doc)
                .OfClass(typeof(SelectionFilterElement))
                .Cast<SelectionFilterElement>())
            {
                list.Add(new FilterInfo
                {
                    Name = f.Name,
                    FilterElement = f,
                    IsRuleBased = false
                });
            }

            return list.OrderBy(f => f.Name).ToList();
        }
    }
}
