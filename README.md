<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/520791644/22.1.4%2B)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
<!-- default badges end -->

# Blazor Grid - Show the Context Menu

You can show a Context Menu when you right-click a Grid element. In this example, a click on a column header or row invokes the Context Menu.

![Grid with Context Menu for a column](result.png)

Add the **oncontextmenu:preventDefault** to disable the standard browser context menu. In the Grid's [CustomizeElement](https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxGrid.CustomizeElement) event handler, subscribe for the **contextmenu** event that shows the custom Context Menu.

The [GridContextMenuContainer](./CS/Pages/GridContextMenuContainer.razor) component contains Context Menu components. The [GridContextMenuHelper](./CS/Data/GridContextMenuHelper.cs) class implements Context Menu's item generation, their state, and click handler that use the [Grid's API](https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxGrid._methods) to execute commands.

In this example, the [Column Chooser](https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxGrid.ShowColumnChooser(System.String)) is shown next to the Grid component. To do this, assign a CSS class to the Grid and use it to [arrange](./CS/Data/GridContextMenuHelper.cs#L85) the Column Chooser.

## Files to Look At

- [Index.razor](./CS/Pages/Index.razor)
- [GridContextMenuHelper.cs](./CS/Data/GridContextMenuHelper.cs)
- [GridContextMenuContainer.razor](./CS/Pages/GridContextMenuContainer.razor)

## Documentation

- [DxContextMenu](https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxContextMenu)
- [DxGrid](https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxGrid)

## More Examples

- [Blazor Data Grid - How to edit/delete the selected row by clicking on external buttons](https://github.com/DevExpress-Examples/blazor-DxDataGrid-edit-selected-row-by-clicking-on-external-button)
