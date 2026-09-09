using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Selection.Revit
{
    public class ProjectAvailability : IExternalCommandAvailability
    {
        public bool IsCommandAvailable(
            UIApplication applicationData,
            CategorySet selectedCategories)
        {
            return applicationData.ActiveUIDocument != null;
        }
    }
}