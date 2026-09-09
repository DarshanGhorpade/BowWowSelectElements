using Autodesk.Windows;
using System;
using System.Windows;
using System.Windows.Controls;
using UIFramework;                     // Revit internal – usually available
using System.Windows.Media;

namespace Selection.Revit.Utils
{
    public static class RibbonController
    {
        private static ContentPresenter _panelPresenter;
        private static FrameworkElement _internalToolPanel;
        private static Grid _rootGrid;
        private static Window _revitWindow;

        public static bool IsActive { get; private set; }

        static RibbonController()
        {
            try
            {
                // Find the main ribbon root grid
                _rootGrid = VisualUtils.FindVisualParent<Grid>(
                    ComponentManager.Ribbon as FrameworkElement, "rootGrid")
                    ?? VisualUtils.FindVisualParent<Grid>(
                        ComponentManager.Ribbon as FrameworkElement, null);

                if (_rootGrid == null)
                    throw new InvalidOperationException("Could not find ribbon root grid.");

                // Revit's internal options / dialog bar
                _internalToolPanel = VisualUtils.FindVisualChild<DialogBarControl>(
                    _rootGrid, null) as FrameworkElement;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("RibbonController init failed: " + ex.Message);
            }
        }

        public static void ShowOptionsBar(FrameworkElement content, bool multiLine = false)
        {
            if (_rootGrid == null) return;

            if (_panelPresenter == null)
                _panelPresenter = CreateOptionsBar();

            _panelPresenter.Content = content;
            _panelPresenter.Visibility = Visibility.Visible;

            // Hide / collapse Revit's own internal bar so our content takes its place
            if (_internalToolPanel != null)
                _internalToolPanel.Height = multiLine ? 26 : 0;

            IsActive = true;

            // Keep reference to the main window for size changes (optional)
            _revitWindow = Window.GetWindow(content);
        }

        public static void HideOptionsBar()
        {
            if (_panelPresenter != null)
            {
                _panelPresenter.Visibility = Visibility.Collapsed;
                _panelPresenter.Content = null;
            }

            // Restore Revit's internal bar height
            if (_internalToolPanel != null)
                _internalToolPanel.Height = 26;

            IsActive = false;
            _revitWindow = null;
        }

        private static ContentPresenter CreateOptionsBar()
        {
            // Insert a new row for our options bar (same technique as the reference)
            _rootGrid.RowDefinitions.Insert(2, new RowDefinition
            {
                Height = new GridLength(1, GridUnitType.Auto)
            });

            // Shift existing children down
            foreach (UIElement child in _rootGrid.Children)
            {
                int row = Grid.GetRow(child);
                if (row >= 2)
                    Grid.SetRow(child, row + 1);
            }

            var presenter = new ContentPresenter();
            Grid.SetRow(presenter, 2);
            _rootGrid.Children.Add(presenter);

            return presenter;
        }
    }
}