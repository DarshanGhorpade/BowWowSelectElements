using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Selection.Revit.Forms;
using Selection.Revit.Models;
using Selection.Revit.SelectionFilters;
using Selection.Revit.Utils;
using Selection.Revit.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Interop;

namespace Selection.Revit
{
    internal class SelectByFilterCmd
    {
        public UIApplication UIApp { get; set; }
        public ExecuteType ExecuteType { get; set; }

        private SelectByFilterViewModel _viewModel;

        public Result Execute()
        {
            try
            {
                var uidoc = UIApp?.ActiveUIDocument;
                if (uidoc == null)
                {
                    TaskDialog.Show("Select by Filter", "No active document.");
                    return Result.Cancelled;
                }

                if (RibbonController.IsActive)
                {
                    RibbonController.HideOptionsBar();
                    return Result.Succeeded;
                }

                _viewModel = new SelectByFilterViewModel();

                var filters = FilterHelper.GetFilters(uidoc.Document);
                _viewModel.AvailableFilters = new ObservableCollection<FilterInfo>(filters);

                var hwndSource = HwndSource.FromHwnd(UIApp.MainWindowHandle);
                var revitWindow = hwndSource?.RootVisual as Window;

                var view = new SelectByFilterView(_viewModel, revitWindow);

                _viewModel.PostCommand = new RelayCommand(_ => OnStartSelection());

                RibbonController.ShowOptionsBar(view, multiLine: false);

                return Result.Succeeded;
            }
            catch (System.Exception ex)
            {
                TaskDialog.Show("ERROR", ex.Message);
                return Result.Failed;
            }
        }

        private void OnStartSelection()
        {
            try
            {
                RibbonController.HideOptionsBar();

                var selected = _viewModel.FilterItems
                    .Where(f => f.SelectedItem != null)
                    .Select(f => f.SelectedItem)
                    .ToList();

                if (selected.Count == 0)
                {
                    TaskDialog.Show("Select by Filter", "Please select at least one filter.");
                    return;
                }

                var uidoc = UIApp.ActiveUIDocument;

                switch (ExecuteType)
                {
                    case ExecuteType.GetElementsByUser_Filter:
                        DoInteractivePick(uidoc, selected);
                        break;

                    case ExecuteType.GetAllElements_Filter:
                        DoBulkSelect(uidoc, selected, null);
                        break;

                    case ExecuteType.GetAllElementsInActiveView_Filter:
                        var view = uidoc.ActiveGraphicalView ?? uidoc.ActiveView;
                        DoBulkSelect(uidoc, selected, view.Id);
                        break;
                }
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException) { }
            catch (System.Exception ex)
            {
                TaskDialog.Show("Selection Error", ex.Message);
            }
            finally
            {
                RibbonController.HideOptionsBar();
            }
        }

        private void DoInteractivePick(UIDocument uidoc, List<FilterInfo> selected)
        {
            var filter = new SelectionFilterVF(selected);
            var refs = uidoc.Selection.PickObjects(ObjectType.Element, filter, "Select elements by filter");

            var ids = refs.Where(r => r?.ElementId != null)
                          .Select(r => r.ElementId)
                          .ToList();

            uidoc.Selection.SetElementIds(ids);
        }

        private void DoBulkSelect(UIDocument uidoc, List<FilterInfo> selected, ElementId viewId)
        {
            var combined = BuildCombinedFilter(selected);

            FilteredElementCollector collector = viewId == null
                ? new FilteredElementCollector(uidoc.Document)
                : new FilteredElementCollector(uidoc.Document, viewId);

            var ids = collector
                .WhereElementIsNotElementType()
                .WherePasses(combined)
                .ToElementIds()
                .ToList();

            if (ids.Count == 0)
            {
                TaskDialog.Show("Select by Filter", "No elements found for the selected filter(s).");
                return;
            }

            uidoc.Selection.SetElementIds(ids);
        }

        private ElementFilter BuildCombinedFilter(List<FilterInfo> selected)
        {
            var filters = new List<ElementFilter>();

            foreach (var fi in selected)
            {
                if (fi.IsRuleBased)
                {
                    var pfe = fi.FilterElement as ParameterFilterElement;
                    if (pfe != null)
                        filters.Add(pfe.GetElementFilter());
                }
                else
                {
                    var sfe = fi.FilterElement as SelectionFilterElement;
                    if (sfe != null)
                        filters.Add(new ElementIdSetFilter(sfe.GetElementIds()));
                }
            }

            if (filters.Count == 0)
                return new ElementCategoryFilter(BuiltInCategory.INVALID); // never matches

            return filters.Count == 1
                ? filters[0]
                : new LogicalAndFilter(filters);
        }
    }
}