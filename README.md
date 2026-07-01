<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/520791644/25.2.5%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/T1106945)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
[![](https://img.shields.io/badge/💬_Leave_Feedback-feecdd?style=flat-square)](#does-this-example-address-your-development-requirementsobjectives)
<!-- default badges end -->

# Blazor Grid - Customize Context Menu

This example enables, customizes, and implements context menus in Blazor Grid and Toolbar components. It demonstrates the following tasks:
* Enables built-in context menus for predefined Grid areas: header, footer, data row, group panel, group row, and group footer.
* Customizes context menu items for header, footer, and data rows using Grid APIs.
* Defines custom context menus for areas that do not include built-in context menus.

![Grid with Context Menu for a column](result.png)

## Implementation Details

### Enable Context Menus for All Predefined Areas

Set the [ContextMenus](https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxGrid.ContextMenus) property to `All` to allow users to use built-in context menus in the following Grid areas: header, footer, data row, group panel, group row, and group footer.

```
<DxGrid @ref="Grid"
        ...
        ContextMenus="GridContextMenus.All">
</DxGrid>
```

### Customize Context Menu Items Using Grid APIs

The `DxGrid` component includes built-in APIs used to customize context menu items. This scenario affects the following Grid areas: header, footer, and data rows.

In the [CustomizeContextMenu](https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxGrid.CustomizeContextMenu) event handler, call the helper method defined in the [GridContextMenuHelper.cs](./CS/GridWithContextMenu/Data/GridContextMenuHelper.cs#L298-L365) class to customize built-in context menu items.

```
<DxGrid @ref="Grid"
        CustomizeContextMenu="Grid_CustomizeContextMenu"
        ... >
        <!-- ... -->
</DxGrid>

@code {
    IGrid Grid { get; set; }
    // ...

    void Grid_CustomizeContextMenu(GridCustomizeContextMenuEventArgs args) => GridContextMenuHelper.CustomizeContextMenu(args);
}
```

### Add Custom Context Menus

To display custom context menus for the toolbar component, Grid's edit row and pager, you must:

1. In the [GridContextMenuContainer](./CS/GridWithContextMenu/Pages/GridContextMenuContainer.razor#L4-L28) component, define context menu components.
2. In the [GridContextMenuHelper](./CS/GridWithContextMenu/Data/GridContextMenuHelper.cs#L31-L59) class, implement context menu item generation, state management, and click handlers. The Blazor [Grid API](https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxGrid._methods) calls these helper methods to execute commands.
3. Add the [oncontextmenu:preventDefault](./CS/GridWithContextMenu/Pages/Index.razor#L14) directive to disable the standard browser context menu.
4. In the [CustomizeElement](https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxGrid.CustomizeElement) event handler, subscribe to the **contextmenu** event to display a custom context menu.

```
<DxGrid @ref="Grid"
        ...
        CustomizeElement="Grid_CustomizeElement"
        @oncontextmenu:preventDefault>
        @* ... *@
</DxGrid>

<GridContextMenuContainer Grid="Grid" @ref="ContextMenuContainer" />

@code {
    IGrid Grid { get; set; }
    GridContextMenuContainer ContextMenuContainer { get; set; }
    // ...

    void Grid_CustomizeElement(GridCustomizeElementEventArgs e) {
        if(GridContextMenuHelper.IsContextMenuElement(e.ElementType)) {
        	e.Attributes["oncontextmenu"] = EventCallback.Factory.Create<MouseEventArgs>(
         		this,
        		async mArgs => await ContextMenuContainer.Grid_ContextMenu(e, mArgs)
        	);
        }
    }
}
```

#### Video

- [Adding a Context Menu to a Grid](https://www.youtube.com/watch?v=TfBR77ARnf8&t)

## Files to Review

- [Index.razor](./CS/GridWithContextMenu/Pages/Index.razor)
- [GridContextMenuHelper.cs](./CS/GridWithContextMenu/Data/GridContextMenuHelper.cs)
- [GridContextMenuContainer.razor](./CS/GridWithContextMenu/Pages/GridContextMenuContainer.razor)

## Documentation

- [DxContextMenu](https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxContextMenu)
- [DxGrid](https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxGrid)
- [DxGrid.CustomizeElement](https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxGrid.CustomizeElement)
- [DxGrid.CustomizeContextMenu](https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxGrid.CustomizeContextMenu)

## More Examples

- [Blazor Data Grid - How to edit/delete the selected row by clicking on external buttons](https://github.com/DevExpress-Examples/blazor-DxDataGrid-edit-selected-row-by-clicking-on-external-button)
<!-- feedback -->
## Does This Example Address Your Development Requirements/Objectives?

[<img src="https://www.devexpress.com/support/examples/i/yes-button.svg"/>](https://www.devexpress.com/support/examples/survey.xml?utm_source=github&utm_campaign=blazor-grid-show-context-menu&~~~was_helpful=yes) [<img src="https://www.devexpress.com/support/examples/i/no-button.svg"/>](https://www.devexpress.com/support/examples/survey.xml?utm_source=github&utm_campaign=blazor-grid-show-context-menu&~~~was_helpful=no)

(you will be redirected to DevExpress.com to submit your response)
<!-- feedback end -->
