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
    internal class SelectByCategoryCmd
    {
        public UIApplication UIApp { get; set; }
        public ExecuteType ExecuteType { get; set; }

        private SelectByCategoriesViewModel _viewModel;

        public Result Execute()
        {
            try
            {
                var uidoc = UIApp?.ActiveUIDocument;
                if (uidoc == null)
                {
                    TaskDialog.Show("Select by Categories", "No active document.");
                    return Result.Cancelled;
                }

                // Toggle behaviour
                if (RibbonController.IsActive)
                {
                    RibbonController.HideOptionsBar();
                    return Result.Succeeded;
                }

                // 1. Create ViewModel
                _viewModel = new SelectByCategoriesViewModel();

                // 2. Load available categories once
                var categories = CategoryHelper.GetCategories(uidoc.Document);
                _viewModel.AvailableCategories = new ObservableCollection<SelectedCategory>(categories);

                // 3. Get Revit main window (needed for width calculation)
                var hwndSource = HwndSource.FromHwnd(UIApp.MainWindowHandle);
                var revitWindow = hwndSource?.RootVisual as Window;

                // 4. Create the View
                var view = new SelectByCategoryView(_viewModel, revitWindow);

                // 5. Wire the real "Start Selection" action
                _viewModel.PostCommand = new RelayCommand(_ => OnStartSelection());

                // ---------- TEMPORARY DIAGNOSTIC - START ----------
                var ribbon = Autodesk.Windows.ComponentManager.Ribbon as FrameworkElement;
                System.Diagnostics.Debug.WriteLine("=== Ribbon Visual Tree ===");
                DumpVisualTree(ribbon, 0);

                void DumpVisualTree(DependencyObject parent, int indent)
                {
                    if (parent == null) return;
                    string name = (parent as FrameworkElement)?.Name ?? "";
                    string type = parent.GetType().Name;
                    System.Diagnostics.Debug.WriteLine(new string(' ', indent * 2) + type + (string.IsNullOrEmpty(name) ? "" : " [" + name + "]"));

                    for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent); i++)
                        DumpVisualTree(System.Windows.Media.VisualTreeHelper.GetChild(parent, i), indent + 1);
                }

                // 6. Show the options bar
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

                // Collect all selected categories from the ViewModel
                var selectedCategories = _viewModel.CatItems
                    .Where(c => c.SelectedItem != null)
                    .Select(c => c.SelectedItem)
                    .ToList();

                if (selectedCategories.Count == 0)
                {
                    TaskDialog.Show("Select by Categories", "Please select at least one category.");
                    return;
                }

                var uidoc = UIApp.ActiveUIDocument;

                switch (ExecuteType)
                {
                    case ExecuteType.GetElementsByUser_Category:
                        // For interactive pick we currently support only the first category
                        DoInteractivePick(uidoc, selectedCategories.First());
                        break;

                    case ExecuteType.GetAllElements_Category:
                        DoBulkSelect(uidoc, selectedCategories, viewId: null);
                        break;

                    case ExecuteType.GetAllElementsInActiveView_Category:
                        var view = uidoc.ActiveGraphicalView ?? uidoc.ActiveView;
                        DoBulkSelect(uidoc, selectedCategories, view.Id);
                        break;
                }
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                // user cancelled
            }
            catch (System.Exception ex)
            {
                TaskDialog.Show("Selection Error", ex.Message);
            }
            finally
            {
                RibbonController.HideOptionsBar();
            }
        }

        private void DoInteractivePick(UIDocument uidoc, SelectedCategory selected)
        {
            var filter = new CategorySelectionFilter(selected.Id);
            IList<Reference> refs = uidoc.Selection.PickObjects(
                ObjectType.Element, filter, $"Select {selected.Name} elements.");

            var ids = refs
                .Where(r => r?.ElementId != null)
                .Select(r => r.ElementId)
                .ToList();

            uidoc.Selection.SetElementIds(ids);
        }

        private void DoBulkSelect(UIDocument uidoc, List<SelectedCategory> selectedCategories, ElementId viewId)
        {
            // Build a LogicalOrFilter of all selected categories
            var filters = selectedCategories
                .Select(c => (ElementFilter)new ElementCategoryFilter(c.Id))
                .ToList();

            ElementFilter categoryFilter = filters.Count == 1
                ? filters[0]
                : new LogicalOrFilter(filters);

            FilteredElementCollector collector = viewId == null
                ? new FilteredElementCollector(uidoc.Document)
                : new FilteredElementCollector(uidoc.Document, viewId);

            var ids = collector
                .WhereElementIsNotElementType()
                .WherePasses(categoryFilter)
                .ToElementIds()
                .ToList();

            if (ids.Count == 0)
            {
                string names = string.Join(", ", selectedCategories.Select(c => c.Name));
                TaskDialog.Show("Select by Categories",
                    $"No elements of the selected categor{(selectedCategories.Count > 1 ? "ies" : "y")} found.\n({names})");
                return;
            }

            uidoc.Selection.SetElementIds(ids);
        }
    }
}