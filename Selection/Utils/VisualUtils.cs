using System.Windows;
using System.Windows.Media;

namespace Selection.Revit.Utils
{
    public static class VisualUtils
    {
        public static T FindVisualParent<T>(FrameworkElement element, string name) where T : FrameworkElement
        {
            var parent = VisualTreeHelper.GetParent(element) as FrameworkElement;

            while (parent != null)
            {
                if (parent is T typed && (string.IsNullOrEmpty(name) || parent.Name == name))
                    return typed;

                parent = VisualTreeHelper.GetParent(parent) as FrameworkElement;
            }
            return null;
        }

        public static T FindVisualChild<T>(FrameworkElement element, string name) where T : Visual
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var child = VisualTreeHelper.GetChild(element, i) as FrameworkElement;
                if (child == null) continue;

                if (child is T typed && (string.IsNullOrEmpty(name) || child.Name == name))
                    return typed;

                var result = FindVisualChild<T>(child, name);
                if (result != null) return result;
            }
            return null;
        }
    }
}