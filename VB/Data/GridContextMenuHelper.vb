Imports DevExpress.Blazor

Namespace GridWithContextMenu.Data

    Public Class GridContextMenuHelper

        Public Shared Function IsContextMenuElement(ByVal elementType As GridElementType) As Boolean
            Return GridContextMenuHelper.IsColumnContextMenuElement(elementType) OrElse GridContextMenuHelper.IsRowContextMenuElement(elementType)
        End Function

        Public Shared Function IsColumnContextMenuElement(ByVal elementType As GridElementType) As Boolean
            Select Case elementType
                Case GridElementType.HeaderCell, GridElementType.HeaderCommandCell, GridElementType.HeaderSelectionCell, GridElementType.GroupPanelHeader
                    Return True
            End Select

            Return False
        End Function

        Public Shared Function IsRowContextMenuElement(ByVal elementType As GridElementType) As Boolean
            Select Case elementType
                Case GridElementType.DataRow, GridElementType.GroupRow
                    Return True
            End Select

            Return False
        End Function

        Public Shared Sub ProcessColumnMenuItemClick(ByVal item As ContextMenuItem, ByVal column As IGridColumn, ByVal grid As IGrid)
            Dim dataColumn = TryCast(column, IGridDataColumn)
            grid.BeginUpdate()
            Select Case item.ItemType
                Case GridContextMenuItemType.FullExpand
                    grid.ExpandAllGroupRows()
                Case GridContextMenuItemType.FullCollapse
                    grid.CollapseAllGroupRows()
                Case GridContextMenuItemType.SortAscending
                    If dataColumn.SortOrder IsNot GridColumnSortOrder.Ascending Then
                        Dim newSortIndex = If(dataColumn.SortIndex > -1, dataColumn.SortIndex, grid.GetSortedColumns().Count)
                        grid.SortBy(dataColumn.FieldName, GridColumnSortOrder.Ascending, newSortIndex)
                    End If

                Case GridContextMenuItemType.SortDescending
                    If dataColumn.SortOrder IsNot GridColumnSortOrder.Descending Then
                        Dim newSortIndex = If(dataColumn.SortIndex > -1, dataColumn.SortIndex, grid.GetSortedColumns().Count)
                        grid.SortBy(dataColumn.FieldName, GridColumnSortOrder.Descending, newSortIndex)
                    End If

                Case GridContextMenuItemType.ClearSorting
                    grid.SortBy(dataColumn.FieldName, GridColumnSortOrder.Descending, -1)
                Case GridContextMenuItemType.GroupByColumn
                    grid.GroupBy(dataColumn.FieldName, grid.GetGroupCount())
                Case GridContextMenuItemType.UngroupColumn
                    grid.GroupBy(dataColumn.FieldName, -1)
                Case GridContextMenuItemType.ClearGrouping
                    grid.ClearSort()
                Case GridContextMenuItemType.ShowGroupPanel
                    grid.ShowGroupPanel = Not grid.ShowGroupPanel
                Case GridContextMenuItemType.ShowFilterRow
                    grid.ShowFilterRow = Not grid.ShowFilterRow
                Case GridContextMenuItemType.ShowFooter
                    Dim isFooterVisible = grid.FooterDisplayMode Is GridFooterDisplayMode.Always OrElse grid.FooterDisplayMode Is GridFooterDisplayMode.Auto AndAlso grid.GetTotalSummaryItems().Count > 0
                    grid.FooterDisplayMode = If(isFooterVisible, GridFooterDisplayMode.Never, GridFooterDisplayMode.Always)
                Case GridContextMenuItemType.HideColumn
                    column.Visible = False
                Case GridContextMenuItemType.ShowColumnChooser
                    If String.IsNullOrEmpty(grid.CssClass) Then Throw New Exception("Specify DxGrid.CssClass property.")
                    Dim gridCssSelector = String.Join(String.Empty, grid.CssClass.Split(" ").Where(Function(i) Not String.IsNullOrWhiteSpace(i)).[Select](Function(i) "." & i.Trim()))
                    Dim positionTarget = $"{gridCssSelector} .dxbs-grid-header-row"
                    grid.ShowColumnChooser(positionTarget)
                Case GridContextMenuItemType.ClearFilter
                    grid.ClearFilter()
            End Select

            grid.EndUpdate()
        End Sub

        Public Shared Async Function ProcessRowMenuItemClickAsync(ByVal item As ContextMenuItem, ByVal visibleIndex As Integer, ByVal grid As IGrid) As Task
            Select Case item.ItemType
                Case GridContextMenuItemType.ExpandRow
                    grid.ExpandGroupRow(visibleIndex)
                Case GridContextMenuItemType.CollapseRow
                    grid.CollapseGroupRow(visibleIndex)
                Case GridContextMenuItemType.ExpandDetailRow
                    grid.ExpandDetailRow(visibleIndex)
                Case GridContextMenuItemType.CollapseDetailRow
                    grid.CollapseDetailRow(visibleIndex)
                Case GridContextMenuItemType.NewRow
                    Await grid.StartEditNewRowAsync()
                Case GridContextMenuItemType.EditRow
                    Await grid.StartEditRowAsync(visibleIndex)
                Case GridContextMenuItemType.DeleteRow
                    grid.ShowRowDeleteConfirmation(visibleIndex)
            End Select
        End Function

        Public Shared Function GetColumnItems(ByVal e As GridCustomizeElementEventArgs) As List(Of ContextMenuItem)
            Dim items = CreateColumnContextMenuItems()
            Dim applyBeginGroupForNextVisibleItem = False
            For Each item In items
                item.Visible = IsColumnMenuItemVisible(e, item.ItemType)
                If Not item.Visible AndAlso item.BeginGroup Then applyBeginGroupForNextVisibleItem = True
                If item.Visible AndAlso applyBeginGroupForNextVisibleItem Then
                    item.BeginGroup = True
                    applyBeginGroupForNextVisibleItem = False
                End If

                item.Enabled = IsColumnMenuItemEnabled(e, item.ItemType)
                Dim isSelected = IsColumnMenuItemSelected(e, item.ItemType)
                If item.Enabled AndAlso isSelected Then item.CssClass = "menu-item-selected"
            Next

            Return items
        End Function

        Public Shared Function GetRowItems(ByVal e As GridCustomizeElementEventArgs) As List(Of ContextMenuItem)
            Dim items = CreateRowContextMenuItems()
            For Each item In items
                item.Visible = IsRowMenuItemVisible(e, item.ItemType)
                item.Enabled = IsRowMenuItemEnabled(e, item.ItemType)
            Next

            Return items
        End Function

        Private Shared Function IsColumnMenuItemVisible(ByVal e As GridCustomizeElementEventArgs, ByVal itemType As GridContextMenuItemType) As Boolean
            Dim dataColumn = TryCast(e.Column, IGridDataColumn)
            Dim allowSort = GetAllowSort(e.Column, e.Grid)
            Dim allowGroup = GetAllowGroup(e.Column, e.Grid)
            Select Case itemType
                Case GridContextMenuItemType.FullExpand, GridContextMenuItemType.FullCollapse
                    Return e.ElementType Is GridElementType.GroupPanelHeader
                Case GridContextMenuItemType.SortAscending, GridContextMenuItemType.SortDescending
                    Return allowSort
                Case GridContextMenuItemType.ClearSorting
                    Return allowSort AndAlso dataColumn.GroupIndex < 0
                Case GridContextMenuItemType.GroupByColumn
                    Return allowGroup AndAlso dataColumn.GroupIndex < 0
                Case GridContextMenuItemType.UngroupColumn
                    Return allowGroup AndAlso dataColumn.GroupIndex > -1
                Case GridContextMenuItemType.ClearGrouping
                    Return e.Grid.AllowGroup
                Case GridContextMenuItemType.ShowGroupPanel, GridContextMenuItemType.ShowFilterRow, GridContextMenuItemType.ShowFooter, GridContextMenuItemType.HideColumn, GridContextMenuItemType.ShowColumnChooser, GridContextMenuItemType.ClearFilter
                    Return True
            End Select

            Return False
        End Function

        Private Shared Function IsColumnMenuItemSelected(ByVal e As GridCustomizeElementEventArgs, ByVal itemType As GridContextMenuItemType) As Boolean
            Dim dataColumn = TryCast(e.Column, IGridDataColumn)
            Dim isSorted = dataColumn IsNot Nothing AndAlso dataColumn.SortIndex > -1
            Dim isGrouped = dataColumn IsNot Nothing AndAlso dataColumn.GroupIndex > -1
            Dim sortOrder = GridColumnSortOrder.None
            If isSorted OrElse isGrouped Then
                sortOrder = dataColumn.SortOrder
                If sortOrder Is GridColumnSortOrder.None Then sortOrder = GridColumnSortOrder.Ascending
            End If

            Select Case itemType
                Case GridContextMenuItemType.SortAscending
                    Return sortOrder Is GridColumnSortOrder.Ascending
                Case GridContextMenuItemType.SortDescending
                    Return sortOrder Is GridColumnSortOrder.Descending
                Case GridContextMenuItemType.ShowGroupPanel
                    Return e.Grid.ShowGroupPanel
                Case GridContextMenuItemType.ShowFilterRow
                    Return e.Grid.ShowFilterRow
                Case GridContextMenuItemType.ShowFooter
                    Return e.Grid.FooterDisplayMode Is GridFooterDisplayMode.Always OrElse e.Grid.FooterDisplayMode Is GridFooterDisplayMode.Auto AndAlso e.Grid.GetTotalSummaryItems().Count > 0
            End Select

            Return False
        End Function

        Private Shared Function IsColumnMenuItemEnabled(ByVal e As GridCustomizeElementEventArgs, ByVal itemType As GridContextMenuItemType) As Boolean
            Dim dataColumn = TryCast(e.Column, IGridDataColumn)
            Dim allowSort = GetAllowSort(e.Column, e.Grid)
            Dim allowGroup = GetAllowGroup(e.Column, e.Grid)
            Select Case itemType
                Case GridContextMenuItemType.FullExpand, GridContextMenuItemType.FullCollapse, GridContextMenuItemType.SortAscending, GridContextMenuItemType.SortDescending, GridContextMenuItemType.GroupByColumn, GridContextMenuItemType.UngroupColumn, GridContextMenuItemType.ShowGroupPanel, GridContextMenuItemType.ShowFilterRow, GridContextMenuItemType.ShowFooter, GridContextMenuItemType.HideColumn, GridContextMenuItemType.ShowColumnChooser
                    Return True
                Case GridContextMenuItemType.ClearSorting
                    Return allowSort AndAlso (dataColumn.SortIndex > -1 OrElse dataColumn.GroupIndex > -1)
                Case GridContextMenuItemType.ClearGrouping
                    Return e.Grid.AllowGroup AndAlso e.Grid.GetGroupCount() > 1
                Case GridContextMenuItemType.ClearFilter
                    Return e.Grid.GetDataColumns().Any(Function(i) i.FilterRowValue IsNot Nothing)
            End Select

            Return False
        End Function

        Private Shared Function IsRowMenuItemVisible(ByVal e As GridCustomizeElementEventArgs, ByVal itemType As GridContextMenuItemType) As Boolean
            Dim isGroupRow = e.Grid.IsGroupRow(e.VisibleIndex)
            Dim hasDetailButton = Not isGroupRow AndAlso e.Grid.DetailRowTemplate IsNot Nothing AndAlso e.Grid.DetailRowDisplayMode Is GridDetailRowDisplayMode.Auto AndAlso e.Grid.DetailExpandButtonDisplayMode Is GridDetailExpandButtonDisplayMode.Auto
            Select Case itemType
                Case GridContextMenuItemType.ExpandRow, GridContextMenuItemType.CollapseRow
                    Return isGroupRow
                Case GridContextMenuItemType.ExpandDetailRow, GridContextMenuItemType.CollapseDetailRow
                    Return hasDetailButton
                Case GridContextMenuItemType.NewRow
                    Return True
                Case GridContextMenuItemType.EditRow, GridContextMenuItemType.DeleteRow
                    Return Not isGroupRow
            End Select

            Return False
        End Function

        Private Shared Function IsRowMenuItemEnabled(ByVal e As GridCustomizeElementEventArgs, ByVal itemType As GridContextMenuItemType) As Boolean
            Dim isGroupRow = e.Grid.IsGroupRow(e.VisibleIndex)
            Dim isGroupRowExpanded = e.Grid.IsGroupRowExpanded(e.VisibleIndex)
            Dim hasDetailButton = Not isGroupRow AndAlso e.Grid.DetailRowTemplate IsNot Nothing AndAlso e.Grid.DetailRowDisplayMode Is GridDetailRowDisplayMode.Auto AndAlso e.Grid.DetailExpandButtonDisplayMode Is GridDetailExpandButtonDisplayMode.Auto
            Dim isDetailRowExpanded = e.Grid.IsDetailRowExpanded(e.VisibleIndex)
            Select Case itemType
                Case GridContextMenuItemType.ExpandRow
                    Return isGroupRow AndAlso Not isGroupRowExpanded
                Case GridContextMenuItemType.CollapseRow
                    Return isGroupRow AndAlso isGroupRowExpanded
                Case GridContextMenuItemType.ExpandDetailRow
                    Return hasDetailButton AndAlso Not isDetailRowExpanded
                Case GridContextMenuItemType.CollapseDetailRow
                    Return hasDetailButton AndAlso isDetailRowExpanded
                Case GridContextMenuItemType.NewRow, GridContextMenuItemType.EditRow, GridContextMenuItemType.DeleteRow
                    Return True
            End Select

            Return False
        End Function

        Private Shared Function GetAllowSort(ByVal column As IGridColumn, ByVal grid As IGrid) As Boolean
            Dim dataColumn As IGridDataColumn = Nothing
            If CSharpImpl.__Assign(dataColumn, TryCast(column, IGridDataColumn)) IsNot Nothing Then Return If(dataColumn.AllowSort, grid.AllowSort)
            Return False
        End Function

        Private Shared Function GetAllowGroup(ByVal column As IGridColumn, ByVal grid As IGrid) As Boolean
            Dim dataColumn As IGridDataColumn = Nothing
            If CSharpImpl.__Assign(dataColumn, TryCast(column, IGridDataColumn)) IsNot Nothing Then Return If(dataColumn.AllowGroup, grid.AllowGroup)
            Return False
        End Function

        Private Shared Function CreateColumnContextMenuItems() As List(Of ContextMenuItem)
            Return New List(Of ContextMenuItem) From {New ContextMenuItem With {.ItemType = GridContextMenuItemType.FullExpand, .Text = "Expand All", .IconCssClass = "grid-context-menu-item-full-expand"}, New ContextMenuItem With {.ItemType = GridContextMenuItemType.FullCollapse, .Text = "Collapse All", .IconCssClass = "grid-context-menu-item-full-collapse"}, New ContextMenuItem With {.ItemType = GridContextMenuItemType.SortAscending, .Text = "Sort Ascending", .BeginGroup = True, .IconCssClass = "grid-context-menu-item-sort-ascending"}, New ContextMenuItem With {.ItemType = GridContextMenuItemType.SortDescending, .Text = "Sort Descending", .IconCssClass = "grid-context-menu-item-sort-descending"}, New ContextMenuItem With {.ItemType = GridContextMenuItemType.ClearSorting, .Text = "Clear Sorting"}, New ContextMenuItem With {.ItemType = GridContextMenuItemType.GroupByColumn, .Text = "Group By This Column", .BeginGroup = True, .IconCssClass = "grid-context-menu-item-group-by-column"}, New ContextMenuItem With {.ItemType = GridContextMenuItemType.UngroupColumn, .Text = "Ungroup", .IconCssClass = "grid-context-menu-item-ungroup-column"}, New ContextMenuItem With {.ItemType = GridContextMenuItemType.ClearGrouping, .Text = "Clear Grouping", .IconCssClass = "grid-context-menu-item-clear-grouping"}, New ContextMenuItem With {.ItemType = GridContextMenuItemType.ShowGroupPanel, .Text = "Group Panel", .IconCssClass = "grid-context-menu-item-show-group-panel"}, New ContextMenuItem With {.ItemType = GridContextMenuItemType.HideColumn, .Text = "Hide Column", .BeginGroup = True}, New ContextMenuItem With {.ItemType = GridContextMenuItemType.ShowColumnChooser, .Text = "Column Chooser", .IconCssClass = "grid-context-menu-item-column-chooser"}, New ContextMenuItem With {.ItemType = GridContextMenuItemType.ClearFilter, .Text = "Clear Filter", .BeginGroup = True, .IconCssClass = "grid-context-menu-item-clear-filter"}, New ContextMenuItem With {.ItemType = GridContextMenuItemType.ShowFilterRow, .Text = "Filter Row", .IconCssClass = "grid-context-menu-item-filter-row"}, New ContextMenuItem With {.ItemType = GridContextMenuItemType.ShowFooter, .Text = "Footer", .IconCssClass = "grid-context-menu-item-footer"}}
        End Function

        Private Shared Function CreateRowContextMenuItems() As List(Of ContextMenuItem)
            Return New List(Of ContextMenuItem) From {New ContextMenuItem With {.ItemType = GridContextMenuItemType.ExpandRow, .Text = "Expand", .IconCssClass = "grid-context-menu-item-expand-row"}, New ContextMenuItem With {.ItemType = GridContextMenuItemType.CollapseRow, .Text = "Collapse", .IconCssClass = "grid-context-menu-item-collapse-row"}, New ContextMenuItem With {.ItemType = GridContextMenuItemType.ExpandDetailRow, .Text = "Expand Detail", .BeginGroup = True, .IconCssClass = "grid-context-menu-item-expand-detail-row"}, New ContextMenuItem With {.ItemType = GridContextMenuItemType.CollapseDetailRow, .Text = "Collapse Detail", .IconCssClass = "grid-context-menu-item-collapse-detail-row"}, New ContextMenuItem With {.ItemType = GridContextMenuItemType.NewRow, .Text = "New", .BeginGroup = True, .IconCssClass = "grid-context-menu-item-new-row"}, New ContextMenuItem With {.ItemType = GridContextMenuItemType.EditRow, .Text = "Edit", .IconCssClass = "grid-context-menu-item-edit-row"}, New ContextMenuItem With {.ItemType = GridContextMenuItemType.DeleteRow, .Text = "Delete", .IconCssClass = "grid-context-menu-item-delete-row"}}
        End Function

        Private Class CSharpImpl

            <System.Obsolete("Please refactor calling code to use normal Visual Basic assignment")>
            Shared Function __Assign(Of T)(ByRef target As T, value As T) As T
                target = value
                Return value
            End Function
        End Class
    End Class

    Public Class ContextMenuItem

        Public Property ItemType As GridContextMenuItemType

        Public Property Text As String

        Public Property Enabled As Boolean

        Public Property Visible As Boolean

        Public Property BeginGroup As Boolean

        Public Property CssClass As String

        Public Property IconCssClass As String
    End Class

    Public Enum GridContextMenuItemType
        FullExpand
        FullCollapse
        SortAscending
        SortDescending
        ClearSorting
        GroupByColumn
        UngroupColumn
        ClearGrouping
        ShowGroupPanel
        HideColumn
        ShowColumnChooser
        ClearFilter
        ShowFilterRow
        ShowFooter
        ExpandRow
        CollapseRow
        ExpandDetailRow
        CollapseDetailRow
        NewRow
        EditRow
        DeleteRow
    End Enum
End Namespace
