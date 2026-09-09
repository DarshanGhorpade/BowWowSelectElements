using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Selection.Revit.Utils;

namespace Selection.Revit
{
    [Transaction(TransactionMode.Manual)]
    public class SelAllEleByCategoryCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            return new SelectByCategoryCmd
            {
                UIApp = commandData.Application,
                ExecuteType = ExecuteType.GetAllElements_Category
            }.Execute();
        }
    }
}