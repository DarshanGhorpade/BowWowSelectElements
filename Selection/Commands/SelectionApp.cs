using Autodesk.Revit.UI;

using Selection.Revit.Utils;

namespace Selection.Revit
{
    public class SelectionApp : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            try
            {
                // 1. create ribbon tab
                string tabName = "My Tab";
                application.CreateRibbonTab(tabName);
                // Get executing assembly
                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                // 2. create selByCat panel
                RibbonPanel SelByCatPanel = application.CreateRibbonPanel(tabName, "Select By Categories");
                // 3. create splitbuttondata for SelByCat
                SplitButtonData selByCatSplitBtnData = new SplitButtonData("selByCatSplitBtnData", "Select elements");
                SplitButton selByCatSplitBtn = SelByCatPanel.AddItem(selByCatSplitBtnData) as SplitButton;
                selByCatSplitBtn.ToolTip = "Selection by Categories in project";
                //selByCatSplitBtn.Image = ImageUtils.PngImageSource("Selection.Revit.Resources.selectionByCategories_light_16.png");
                //selByCatSplitBtn.LargeImage = ImageUtils.PngImageSource("Selection.Revit.Resources.selectionByCategories_light_32.png");
                selByCatSplitBtn.Image = ImageUtils.ThemeImage("Selection.Revit.Resources.selectionByCategories_light_16.png", "Selection.Revit.Resources.selectionByCategories_dark_16.png");
                selByCatSplitBtn.LargeImage = ImageUtils.ThemeImage("Selection.Revit.Resources.selectionByCategories_light_32.png", "Selection.Revit.Resources.selectionByCategories_dark_32.png");
                // 4. create pushbuttondata
                PushButtonData SelEleInCurrentViewByCategoryButtonData = new PushButtonData("SelEleInCurrentViewButtonData", "Select elements", assembly.Location, "Selection.Revit.SelEleInCurrentViewByCategoryCommand");
                //SelEleInCurrentViewButtonData.Image = ImageUtils.PngImageSource("Selection.Revit.Resources.selectionByCategories_light_16.png");
                //SelEleInCurrentViewButtonData.LargeImage = ImageUtils.PngImageSource("Selection.Revit.Resources.selectionByCategories_light_32.png");
                SelEleInCurrentViewByCategoryButtonData.Image = ImageUtils.ThemeImage("Selection.Revit.Resources.selectionByCategories_light_16.png", "Selection.Revit.Resources.selectionByCategories_dark_16.png");
                SelEleInCurrentViewByCategoryButtonData.LargeImage = ImageUtils.ThemeImage("Selection.Revit.Resources.selectionByCategories_light_32.png", "Selection.Revit.Resources.selectionByCategories_dark_32.png");
                SelEleInCurrentViewByCategoryButtonData.AvailabilityClassName = "Selection.Revit.ProjectAvailability";
                PushButtonData SelAllEleInActiveViewByCategoryButtonData = new PushButtonData("SelAllEleInActiveViewButtonData", "All elements in Active view", assembly.Location, "Selection.Revit.SelAllEleInActiveViewByCategoryCommand");
                //SelAllEleInActiveViewButtonData.Image = ImageUtils.PngImageSource("Selection.Revit.Resources.selectionByCategories_light_16.png");
                //SelAllEleInActiveViewButtonData.LargeImage = ImageUtils.PngImageSource("Selection.Revit.Resources.selectionByCategories_light_32.png");
                SelAllEleInActiveViewByCategoryButtonData.Image = ImageUtils.ThemeImage("Selection.Revit.Resources.selectionByCategories_light_16.png", "Selection.Revit.Resources.selectionByCategories_dark_16.png");
                SelAllEleInActiveViewByCategoryButtonData.LargeImage = ImageUtils.ThemeImage("Selection.Revit.Resources.selectionByCategories_light_32.png", "Selection.Revit.Resources.selectionByCategories_dark_32.png");
                SelAllEleInActiveViewByCategoryButtonData.AvailabilityClassName = "Selection.Revit.ProjectAvailability";
                SelAllEleInActiveViewByCategoryButtonData.AvailabilityClassName = "Selection.Revit.ProjectAvailability";
                PushButtonData SelAllEleByCategoryButtonData = new PushButtonData("SelAllEleButtonData", "All elements", assembly.Location, "Selection.Revit.SelAllEleByCategoryCommand");
                SelAllEleByCategoryButtonData.Image = ImageUtils.ThemeImage("Selection.Revit.Resources.selectionByCategories_light_16.png", "Selection.Revit.Resources.selectionByCategories_dark_16.png");
                SelAllEleByCategoryButtonData.LargeImage = ImageUtils.ThemeImage("Selection.Revit.Resources.selectionByCategories_light_32.png", "Selection.Revit.Resources.selectionByCategories_dark_32.png");
                SelAllEleByCategoryButtonData.AvailabilityClassName = "Selection.Revit.ProjectAvailability";
                // 5. add pushbuttondata to splitbutton
                selByCatSplitBtn.AddPushButton(SelEleInCurrentViewByCategoryButtonData);
                selByCatSplitBtn.AddPushButton(SelAllEleInActiveViewByCategoryButtonData);
                selByCatSplitBtn.AddPushButton(SelAllEleByCategoryButtonData);
                // 6. create selByFilt panel
                RibbonPanel SelByFiltPanel = application.CreateRibbonPanel(tabName, "Select By Filters");
                // 7. create splitbuttondata for SelByFilt
                SplitButtonData selByFiltSplitBtnData = new SplitButtonData("selByCatSplitBtnData", "Select elements");
                SplitButton selByFiltSplitBtn = SelByFiltPanel.AddItem(selByFiltSplitBtnData) as SplitButton;
                selByFiltSplitBtn.ToolTip = "Selection by Filters in project";
                selByFiltSplitBtn.Image = ImageUtils.PngImageSource("Selection.Revit.Resources.selectionByFilters_light_16.png");
                selByFiltSplitBtn.LargeImage = ImageUtils.PngImageSource("Selection.Revit.Resources.selectionByFilters_light_32.png");
                selByFiltSplitBtn.Image = ImageUtils.ThemeImage("Selection.Revit.Resources.selectionByFilters_light_16.png", "Selection.Revit.Resources.selectionByFilters_dark_16.png");
                selByFiltSplitBtn.LargeImage = ImageUtils.ThemeImage("Selection.Revit.Resources.selectionByFilters_light_32.png", "Selection.Revit.Resources.selectionByFilters_dark_32.png");
                // 8. create pushbuttondata
                PushButtonData SelEleInCurrentViewByFiltButtonData = new PushButtonData("SelEleInCurrentViewByFiltButtonData", "Select elements", assembly.Location, "Selection.Revit.SelEleInCurrentViewByFilterCommand");
                //SelEleInCurrentViewByFiltButtonData.Image = ImageUtils.PngImageSource("Selection.Revit.Resources.selectionByFilters_light_16.png");
                //SelEleInCurrentViewByFiltButtonData.LargeImage = ImageUtils.PngImageSource("Selection.Revit.Resources.selectionByFilters_light_32.png");
                SelEleInCurrentViewByFiltButtonData.Image = ImageUtils.ThemeImage("Selection.Revit.Resources.selectionByFilters_light_16.png", "Selection.Revit.Resources.selectionByFilters_dark_16.png");
                SelEleInCurrentViewByFiltButtonData.LargeImage = ImageUtils.ThemeImage("Selection.Revit.Resources.selectionByFilters_light_32.png", "Selection.Revit.Resources.selectionByFilters_dark_32.png");
                SelEleInCurrentViewByFiltButtonData.AvailabilityClassName = "Selection.Revit.ProjectAvailability";
                PushButtonData SelAllEleInActiveViewByFiltButtonData = new PushButtonData("SelAllEleInActiveViewByFiltButtonData", "All elements in Active view", assembly.Location, "Selection.Revit.SelAllEleInActiveViewByFilterCommand");
                //SelAllEleInActiveViewByFiltButtonData.Image = ImageUtils.PngImageSource("Selection.Revit.Resources.selectionByFilters_light_16.png");
                //SelAllEleInActiveViewByFiltButtonData.LargeImage = ImageUtils.PngImageSource("Selection.Revit.Resources.selectionByFilters_light_32.png");
                SelAllEleInActiveViewByFiltButtonData.Image = ImageUtils.ThemeImage("Selection.Revit.Resources.selectionByFilters_light_16.png", "Selection.Revit.Resources.selectionByFilters_dark_16.png");
                SelAllEleInActiveViewByFiltButtonData.LargeImage = ImageUtils.ThemeImage("Selection.Revit.Resources.selectionByFilters_light_32.png", "Selection.Revit.Resources.selectionByFilters_dark_32.png");
                SelAllEleInActiveViewByFiltButtonData.AvailabilityClassName = "Selection.Revit.ProjectAvailability";
                PushButtonData SelAllEleByFiltButtonData = new PushButtonData("SelAllEleByFiltButtonData", "All elements", assembly.Location, "Selection.Revit.SelAllEleByFilterCommand");
                //SelAllEleByFiltButtonData.Image = ImageUtils.PngImageSource("Selection.Revit.Resources.selectionByFilters_light_16.png");
                //SelAllEleByFiltButtonData.LargeImage = ImageUtils.PngImageSource("Selection.Revit.Resources.selectionByFilters_light_32.png");
                SelAllEleByFiltButtonData.Image = ImageUtils.ThemeImage("Selection.Revit.Resources.selectionByFilters_light_16.png", "Selection.Revit.Resources.selectionByFilters_dark_16.png");
                SelAllEleByFiltButtonData.LargeImage = ImageUtils.ThemeImage("Selection.Revit.Resources.selectionByFilters_light_32.png", "Selection.Revit.Resources.selectionByFilters_dark_32.png");
                SelAllEleByFiltButtonData.AvailabilityClassName = "Selection.Revit.ProjectAvailability";
                // 9. add pushbuttondata to splitbutton
                selByFiltSplitBtn.AddPushButton(SelEleInCurrentViewByFiltButtonData);
                selByFiltSplitBtn.AddPushButton(SelAllEleInActiveViewByFiltButtonData);
                selByFiltSplitBtn.AddPushButton(SelAllEleByFiltButtonData);
                return Result.Succeeded;
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR: {ex.Message}\nStackTrace:\n{ex.StackTrace}");
                return Result.Failed;
            }
        }
    }
}
