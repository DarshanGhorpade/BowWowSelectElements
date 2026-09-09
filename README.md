# Revit Select Elements

A Revit add-in that provides a convenient way to select elements by **category** or **filter** directly from the Revit Options Bar.

The add-in is designed to make element selection faster and easier without opening an additional dialog window.

## Features

* Select elements manually from the Revit model.
* Select all elements belonging to one or more specified categories.
* Select elements from the **active view** only.
* Select elements from the **entire Revit model**.
* Select elements using a specified **filter**.
* Multiple category selection.
* Integrated directly into the Revit Options Bar.
* No additional dialog window is required.
* Supports Revit dark theme on Revit 2024 & later.

## Selection Modes

The add-in supports the following selection workflows:

### 1. Manual Selection

Allows the user to select elements directly from the Revit model.

This mode uses Revit's normal element selection workflow while providing the add-in's selection controls in the Options Bar.

### 2. Active View Selection

Automatically selects all matching elements from the currently active Revit view.

For example, if the user selects:

```text
Category: Walls
Scope: Active View
```

the add-in selects all walls visible/available in the active view that satisfy the specified selection criteria.

### 3. Entire Model Selection

Automatically searches the complete Revit model and selects all elements matching the specified categories or filter.

For example:

```text
Category: Doors
Scope: Entire Model
```

will select matching door elements throughout the model rather than limiting the search to the active view.

## Filters

The add-in can also work with Revit filters to provide more specific element selection.

A filter can be used when selecting an entire category is not sufficient.

For example, instead of selecting every wall, a filter could be used to select only walls satisfying particular conditions.

Typical workflow:

```text
Category
    ↓
Filter
    ↓
Selection Scope
    ↓
Execute
    ↓
Revit Selection
```

## User Interface

The selection controls are integrated into the Revit Options Bar.

A typical workflow is:

1. Open the Revit project.
2. Start the Select Elements command.
3. Choose one or more categories.
4. Optionally select a filter.
5. Select the desired selection scope.
6. Execute the selection.
7. Matching elements are selected in Revit.

## Project Structure

A typical project structure is:

```text
Selection.Revit
│
├── Models
│   └── Selection models
│
├── ViewModels
│   └── Selection view models
│
├── Views
│   └── Selection UI
│
├── Commands
│   └── Revit external commands
│
├── Filters
│   └── Revit filter logic
│
├── Services
│   └── Revit API services
│
├── Resources
│   └── Styles / resources
│
└── Selection.Revit.addin
```

The exact structure can be adjusted depending on the architecture of the implementation.

## Technologies

* C#
* .NET
* Autodesk Revit API
* WPF
* MVVM
* Revit ExternalCommand
* Revit Selection API
* Revit Filter API

## Revit API Concepts

The add-in primarily works with Revit's selection and filtering APIs.

Important Revit API classes/concepts include:

```csharp
UIDocument
Document
Selection
Element
FilteredElementCollector
ElementCategoryFilter
ElementClassFilter
ElementParameterFilter
LogicalAndFilter
LogicalOrFilter
```

For category-based selection, `FilteredElementCollector` can be used to find matching elements.

Example:

```csharp
FilteredElementCollector collector =
    new FilteredElementCollector(document);

IList<Element> elements =
    collector
        .OfCategory(BuiltInCategory.OST_Walls)
        .WhereElementIsNotElementType()
        .ToElements();
```

The resulting element IDs can then be supplied to Revit's selection API.

```csharp
uidoc.Selection.SetElementIds(
    elements.Select(x => x.Id).ToList()
);
```

## Active View vs Entire Model

One of the important differences in the implementation is the collector scope.

### Active View

For active-view selection:

```csharp
FilteredElementCollector collector =
    new FilteredElementCollector(
        document,
        document.ActiveView.Id);
```

This limits the search to elements associated with the active view.

### Entire Model

For model-wide selection:

```csharp
FilteredElementCollector collector =
    new FilteredElementCollector(document);
```

This searches the complete Revit document.

## Multiple Categories

Multiple categories can be combined to create a single selection.

For example:

```text
Walls
Doors
Windows
Floors
```

can be selected together.

The implementation can build a logical filter using `LogicalOrFilter` or collect elements category-by-category and combine their IDs.

Example:

```csharp
var filters = categories
    .Select(category =>
        new ElementCategoryFilter(category))
    .ToList();

var logicalFilter = new LogicalOrFilter(filters);
```

## MVVM

The WPF interface can follow the MVVM pattern.

### Model

Contains data representing categories, filters and selection options.

### ViewModel

Responsible for:

* Loading categories.
* Managing selected categories.
* Managing the selected filter.
* Managing selection scope.
* Executing selection commands.

### View

Contains the WPF controls used by the user.

Example:

```text
ComboBox
    ↓
Selected Category

ComboBox
    ↓
Selected Filter

ComboBox
    ↓
Selection Scope

Button
    ↓
Execute Selection
```

## Installation

Build the project in Visual Studio.

Copy the generated DLL and `.addin` manifest into the Revit Addins directory.

Typical location:

```text
%AppData%\Autodesk\Revit\Addins\<RevitVersion>\
```

For example:

```text
%AppData%\Autodesk\Revit\Addins\2025\
```

The `.addin` file should reference the compiled assembly.

Example:

```xml
<?xml version="1.0" encoding="utf-8" standalone="no"?>
<RevitAddIns>

  <AddIn Type="Command">

    <Name>Selection.Revit</Name>

    <Assembly>
      C:\Path\To\Selection.Revit.dll
    </Assembly>

    <AddInId>
      00000000-0000-0000-0000-000000000000
    </AddInId>

    <FullClassName>
      Selection.Revit.Commands.SelectElementsCommand
    </FullClassName>

    <VendorId>
      YOUR_ID
    </VendorId>

    <VendorDescription>
      Revit element selection tools
    </VendorDescription>

  </AddIn>

</RevitAddIns>
```

Replace the assembly path, GUID and class name with the values from your project.

## Development

### Requirements

* Autodesk Revit
* Visual Studio
* C#
* .NET version compatible with the target Revit version
* Revit API assemblies

Add references to:

```text
RevitAPI.dll
RevitAPIUI.dll
```

These assemblies are normally located in the Revit installation directory.

## Important Notes

The add-in interacts directly with the Revit UI and Revit API. Therefore, Revit API operations must be executed according to Revit's API context requirements.

If the implementation uses an `IExternalCommand`, Revit API operations can normally be performed directly inside the command's `Execute()` method.

For model modifications, use a Revit `Transaction`.

Example:

```csharp
using (Transaction transaction =
       new Transaction(document, "Modify Elements"))
{
    transaction.Start();

    // Revit modifications

    transaction.Commit();
}
```

Pure element selection does not normally require a transaction.

## Known Considerations

### Options Bar Conflicts

Other Revit add-ins that modify or extend the Options Bar may potentially conflict with this add-in.

### Display Resolution

Changes to display resolution while the add-in is running may affect the visibility or layout of controls, particularly when several ComboBoxes or controls are displayed.

Restarting Revit can resolve UI layout issues.

## Error Handling

Revit API operations should be wrapped with appropriate error handling.

Example:

```csharp
try
{
    // Revit API operation
}
catch (Exception ex)
{
    TaskDialog.Show(
        "Select Elements",
        ex.Message);
}
```

Avoid exposing raw exceptions to end users in production. Consider logging exceptions and displaying a user-friendly message instead.

## Future Improvements

Possible improvements include:

* Searchable category ComboBox.
* Searchable filter ComboBox.
* Recent selections.
* Saved selection presets.
* Exclude categories.
* Parameter-based selection.
* Selection by family.
* Selection by family type.
* Selection by workset.
* Selection by level.
* Selection by phase.
* Selection by design option.
* Selection from linked models.
* Advanced AND/OR filter combinations.
* Export/import selection presets.
* Improved dark/light theme support.

## License

This project is an independent C# implementation.

Autodesk, Revit, and related trademarks are property of Autodesk, Inc.

If this project is intended for public distribution, add your own license here, for example:

```text
MIT License
```

or

```text
Proprietary
```

depending on your project's requirements.

## Disclaimer

This project is not affiliated with or endorsed by Autodesk unless explicitly stated.

The implementation is intended for educational, development, and workflow-automation purposes.

---

## References

* Autodesk Revit API
* Autodesk Revit
* Autodesk Platform / Marketplace

Original reference application:

**BowWow Select Elements — BowWowApps**

The referenced Marketplace application provides category/filter-based element selection directly from the Revit Options Bar.
