using Autodesk.Revit.DB;
using Selection.Revit.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Selection.Revit.Utils
{
    internal static class CategoryHelper
    {
        // Simple cache per document hash – optional but useful
        private static string _lastDocHash;
        private static List<SelectedCategory> _cachedCategories;
        public static List<SelectedCategory> GetCategories(Document doc)
        {
            var list = new List<SelectedCategory>();

            foreach (BuiltInCategory bic in Enum.GetValues(typeof(BuiltInCategory)))
            {
                try
                {
                    Category cat = Category.GetCategory(doc, bic);
                    if (cat == null) continue;
                    if (cat.Parent != null) continue;                 // only top-level
                    if (cat.CategoryType != CategoryType.Model) continue;

                    list.Add(new SelectedCategory
                    {
                        Id = cat.Id,
                        Name = cat.Name
                    });
                }
                catch
                {
                    // some BuiltInCategories throw – ignore them
                }
            }

            return list.OrderBy(c => c.Name).ToList();
        }
        public static void LoadCategories(Document doc, System.Windows.Controls.ComboBox cb)
        {
            string hash = doc.GetHashCode().ToString();

            if (_cachedCategories == null || _lastDocHash != hash)
            {
                _cachedCategories = BuildCategoryList(doc);
                _lastDocHash = hash;
            }

            cb.Items.Clear();
            foreach (var cat in _cachedCategories)
                cb.Items.Add(cat);

            if (cb.Items.Count > 0)
                cb.SelectedIndex = 0;
        }

        private static List<SelectedCategory> BuildCategoryList(Document doc)
        {
            // Faster approach: walk BuiltInCategory instead of every element
            var list = new List<SelectedCategory>();

            foreach (BuiltInCategory bic in Enum.GetValues(typeof(BuiltInCategory)))
            {
                try
                {
                    Category cat = Category.GetCategory(doc, bic);
                    if (cat == null || cat.Parent != null) continue;          // only top-level
                    if (cat.CategoryType != CategoryType.Model) continue;

                    list.Add(new SelectedCategory { Id = cat.Id, Name = cat.Name });
                }
                catch { /* some BuiltInCategories throw */ }
            }

            return list.OrderBy(c => c.Name).ToList();
        }
    }
}
