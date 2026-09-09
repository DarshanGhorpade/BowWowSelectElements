using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Selection.Revit.Models;
using System.Collections.Generic;

namespace Selection.Revit.SelectionFilters
{
    internal class SelectionFilterVF : ISelectionFilter
    {
        private readonly ElementFilter _filter;

        public SelectionFilterVF(List<FilterInfo> selectedFilters)
        {
            var list = new List<ElementFilter>();

            foreach (var fi in selectedFilters)
            {
                if (fi.IsRuleBased)
                {
                    var pfe = fi.FilterElement as ParameterFilterElement;
                    if (pfe != null)
                        list.Add(pfe.GetElementFilter());
                }
                else
                {
                    var sfe = fi.FilterElement as SelectionFilterElement;
                    if (sfe != null)
                        list.Add(new ElementIdSetFilter(sfe.GetElementIds()));
                }
            }

            _filter = list.Count == 1
                ? list[0]
                : new LogicalAndFilter(list);
        }

        public bool AllowElement(Element elem) => _filter.PassesFilter(elem);

        public bool AllowReference(Reference reference, XYZ position) => false;
    }
}