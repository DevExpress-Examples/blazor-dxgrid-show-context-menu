using DevExpress.Blazor;

namespace GridWithContextMenu.Data {
    public enum GridContextMenuItemType {
        FullExpand, FullCollapse,
        ShowGroupPanel,
        ShowColumnChooser,
        ClearFilter,
        ShowFilterRow, ShowFooter,

        ExpandRow, CollapseRow,
        ExpandDetailRow, CollapseDetailRow,
        NewRow, EditRow, DeleteRow,

        SaveUpdates, CancelUpdates,

        ExportXls, ExportXlsx, ExportPdf, ExportDocx
    }

    public class ContextMenuItem {
        public GridContextMenuItemType ItemType { get; set; }
        public string Text { get; set; }
        public bool Enabled { get; set; }
        public bool Visible { get; set; }
        public bool BeginGroup { get; set; }
        public string CssClass { get; set; }
        public string IconCssClass { get; set; }
    }

    public class GridContextMenuHelper {
        static List<ContextMenuItem> CreateCustomContextMenuItems() {
            return new List<ContextMenuItem> {
                new ContextMenuItem { ItemType = GridContextMenuItemType.FullExpand, Text = "Expand All", IconCssClass="grid-context-menu-item-full-expand" },
                new ContextMenuItem { ItemType = GridContextMenuItemType.FullCollapse, Text = "Collapse All", IconCssClass="grid-context-menu-item-full-collapse" },
                new ContextMenuItem { ItemType = GridContextMenuItemType.ShowGroupPanel, Text = "Group Panel", IconCssClass="grid-context-menu-item-show-group-panel" },
                new ContextMenuItem { ItemType = GridContextMenuItemType.ShowColumnChooser, Text = "Column Chooser", IconCssClass="grid-context-menu-item-column-chooser" },
                new ContextMenuItem { ItemType = GridContextMenuItemType.ClearFilter, Text = "Clear Filter", BeginGroup = true, IconCssClass="grid-context-menu-item-clear-filter" },
                new ContextMenuItem { ItemType = GridContextMenuItemType.ShowFilterRow, Text = "Filter Row", IconCssClass="grid-context-menu-item-filter-row" },
                new ContextMenuItem { ItemType = GridContextMenuItemType.ShowFooter, Text = "Footer", IconCssClass="grid-context-menu-item-footer" }
            };
        }
        static List<ContextMenuItem> CreateRowContextMenuItems() {
            return new List<ContextMenuItem> {
                new ContextMenuItem { ItemType = GridContextMenuItemType.SaveUpdates, Text = "Save", IconCssClass="grid-context-menu-item-edit-row" },
                new ContextMenuItem { ItemType = GridContextMenuItemType.CancelUpdates, Text = "Cancel", IconCssClass="grid-context-menu-item-delete-row" },
                new ContextMenuItem { ItemType = GridContextMenuItemType.ExpandRow, Text = "Expand", IconCssClass="grid-context-menu-item-expand-row" },
                new ContextMenuItem { ItemType = GridContextMenuItemType.CollapseRow, Text = "Collapse", IconCssClass="grid-context-menu-item-collapse-row" },
                new ContextMenuItem { ItemType = GridContextMenuItemType.ExpandDetailRow, Text = "Expand Detail", BeginGroup = true, IconCssClass="grid-context-menu-item-expand-detail-row" },
                new ContextMenuItem { ItemType = GridContextMenuItemType.CollapseDetailRow, Text = "Collapse Detail", IconCssClass="grid-context-menu-item-collapse-detail-row" },
                new ContextMenuItem { ItemType = GridContextMenuItemType.NewRow, Text = "New", BeginGroup = true, IconCssClass="grid-context-menu-item-new-row" },
                new ContextMenuItem { ItemType = GridContextMenuItemType.EditRow, Text = "Edit", IconCssClass="grid-context-menu-item-edit-row" },
                new ContextMenuItem { ItemType = GridContextMenuItemType.DeleteRow, Text = "Delete", IconCssClass="grid-context-menu-item-delete-row" }
            };
        }

        public static bool IsContextMenuElement(GridElementType elementType) {
            return IsCustomContextMenuElement(elementType) || IsRowContextMenuElement(elementType);
        }
        public static bool IsCustomContextMenuElement(GridElementType elementType) {
            switch(elementType) {
                case GridElementType.ToolbarContainer:
                case GridElementType.PagerContainer:
                    return true;
            }
            return false;
        }
        public static bool IsRowContextMenuElement(GridElementType elementType) {
            switch(elementType) {
                case GridElementType.EditRow:
                    return true;
            }
            return false;
        }

        public static void ProcessCustomMenuItemClick(ContextMenuItem item, IGrid grid) {
            grid.BeginUpdate();
            switch(item.ItemType) {
                case GridContextMenuItemType.FullExpand:
                    grid.ExpandAllGroupRows();
                    break;
                case GridContextMenuItemType.FullCollapse:
                    grid.CollapseAllGroupRows();
                    break;
                case GridContextMenuItemType.ShowGroupPanel:
                    grid.ShowGroupPanel = !grid.ShowGroupPanel;
                    break;
                case GridContextMenuItemType.ShowFilterRow:
                    grid.ShowFilterRow = !grid.ShowFilterRow;
                    break;
                case GridContextMenuItemType.ShowFooter:
                    var isFooterVisible = grid.FooterDisplayMode == GridFooterDisplayMode.Always
                        || grid.FooterDisplayMode == GridFooterDisplayMode.Auto && grid.GetTotalSummaryItems().Count > 0;
                    grid.FooterDisplayMode = isFooterVisible ? GridFooterDisplayMode.Never : GridFooterDisplayMode.Always;
                    break;            
                case GridContextMenuItemType.ShowColumnChooser:
                    grid.ShowColumnChooser();
                    break;
                case GridContextMenuItemType.ClearFilter:
                    grid.ClearFilter();
                    break;
            }
            grid.EndUpdate();
        }
        public static async Task ProcessRowMenuItemClickAsync(ContextMenuItem item, int visibleIndex, IGrid grid) {
            switch(item.ItemType) {
                case GridContextMenuItemType.ExpandRow:
                    grid.ExpandGroupRow(visibleIndex);
                    break;
                case GridContextMenuItemType.CollapseRow:
                    grid.CollapseGroupRow(visibleIndex);
                    break;
                case GridContextMenuItemType.ExpandDetailRow:
                    grid.ExpandDetailRow(visibleIndex);
                    break;
                case GridContextMenuItemType.CollapseDetailRow:
                    grid.CollapseDetailRow(visibleIndex);
                    break;
                case GridContextMenuItemType.NewRow:
                    await grid.StartEditNewRowAsync();
                    break;
                case GridContextMenuItemType.EditRow:
                    await grid.StartEditRowAsync(visibleIndex);
                    break;
                case GridContextMenuItemType.DeleteRow:
                    grid.ShowRowDeleteConfirmation(visibleIndex);
                    break;
                case GridContextMenuItemType.SaveUpdates:
                    await grid.SaveChangesAsync();
                    break;
                case GridContextMenuItemType.CancelUpdates:
                    await grid.CancelEditAsync();
                    break;
            }
        }
        public static List<ContextMenuItem> GetCustomItems(GridCustomizeElementEventArgs e) {
            var items = CreateCustomContextMenuItems();
            var applyBeginGroupForNextVisibleItem = false;
            foreach(var item in items) {
                item.Visible = IsCustomMenuItemVisible(e, item.ItemType);
                if(!item.Visible && item.BeginGroup)
                    applyBeginGroupForNextVisibleItem = true;
                if(item.Visible && applyBeginGroupForNextVisibleItem) {
                    item.BeginGroup = true;
                    applyBeginGroupForNextVisibleItem = false;
                }
                item.Enabled = IsCustomMenuItemEnabled(e, item.ItemType);
                var isSelected = IsCustomMenuItemSelected(e, item.ItemType);
                if(item.Enabled && isSelected)
                    item.CssClass = "menu-item-selected";
            }
            return items;
        }
        public static List<ContextMenuItem> GetRowItems(GridCustomizeElementEventArgs e) {
            var items = CreateRowContextMenuItems();
            foreach(var item in items) {
                item.Visible = IsRowMenuItemVisible(e, item.ItemType);
                item.Enabled = IsRowMenuItemEnabled(e, item.ItemType);
            }
            return items;
        }

        static bool IsCustomMenuItemVisible(GridCustomizeElementEventArgs e, GridContextMenuItemType itemType) {
            
            switch(itemType) {
                case GridContextMenuItemType.FullExpand:
                case GridContextMenuItemType.FullCollapse:     
                case GridContextMenuItemType.ShowGroupPanel:
                case GridContextMenuItemType.ShowFilterRow:
                case GridContextMenuItemType.ShowFooter:
                case GridContextMenuItemType.ShowColumnChooser:
                case GridContextMenuItemType.ClearFilter:
                    return true;
            }
            return false;
        }
        static bool IsCustomMenuItemSelected(GridCustomizeElementEventArgs e, GridContextMenuItemType itemType) {
            switch(itemType) {
                case GridContextMenuItemType.ShowGroupPanel:
                    return e.Grid.ShowGroupPanel;
                case GridContextMenuItemType.ShowFilterRow:
                    return e.Grid.ShowFilterRow;
                case GridContextMenuItemType.ShowFooter:
                    return e.Grid.FooterDisplayMode == GridFooterDisplayMode.Always
                        || e.Grid.FooterDisplayMode == GridFooterDisplayMode.Auto && e.Grid.GetTotalSummaryItems().Count > 0;
            }
            return false;
        }
        static bool IsCustomMenuItemEnabled(GridCustomizeElementEventArgs e, GridContextMenuItemType itemType) {
            switch(itemType) {
                case GridContextMenuItemType.FullExpand:
                case GridContextMenuItemType.FullCollapse:
                case GridContextMenuItemType.ShowGroupPanel:
                case GridContextMenuItemType.ShowFilterRow:
                case GridContextMenuItemType.ShowFooter:
                case GridContextMenuItemType.ShowColumnChooser:
                    return true;
                case GridContextMenuItemType.ClearFilter:
                    return e.Grid.GetFilterCriteria() != null ? true : false;
            }
            return false;
        }

        static bool IsRowMenuItemVisible(GridCustomizeElementEventArgs e, GridContextMenuItemType itemType) {
            var isGroupRow = e.Grid.IsGroupRow(e.VisibleIndex);
            var hasDetailButton = !isGroupRow
                        && e.Grid.DetailRowTemplate != null
                        && e.Grid.DetailRowDisplayMode == GridDetailRowDisplayMode.Auto
                        && e.Grid.DetailExpandButtonDisplayMode == GridDetailExpandButtonDisplayMode.Auto;
            switch(itemType) {
                case GridContextMenuItemType.ExpandRow:
                case GridContextMenuItemType.CollapseRow:
                    return isGroupRow;
                case GridContextMenuItemType.ExpandDetailRow:
                case GridContextMenuItemType.CollapseDetailRow:
                    return hasDetailButton;
                case GridContextMenuItemType.NewRow:
                    return true;
                case GridContextMenuItemType.EditRow:
                case GridContextMenuItemType.DeleteRow:
                    return !isGroupRow;
                case GridContextMenuItemType.SaveUpdates:
                case GridContextMenuItemType.CancelUpdates:
                    return e.Grid.IsEditing();
            }
            return false;
        }
        static bool IsRowMenuItemEnabled(GridCustomizeElementEventArgs e, GridContextMenuItemType itemType) {
            var isGroupRow = e.Grid.IsGroupRow(e.VisibleIndex);
            var isGroupRowExpanded = e.Grid.IsGroupRowExpanded(e.VisibleIndex);
            var hasDetailButton = !isGroupRow
                        && e.Grid.DetailRowTemplate != null
                        && e.Grid.DetailRowDisplayMode == GridDetailRowDisplayMode.Auto
                        && e.Grid.DetailExpandButtonDisplayMode == GridDetailExpandButtonDisplayMode.Auto;
            var isDetailRowExpanded = e.Grid.IsDetailRowExpanded(e.VisibleIndex);
            switch(itemType) {
                case GridContextMenuItemType.ExpandRow:
                    return isGroupRow && !isGroupRowExpanded;
                case GridContextMenuItemType.CollapseRow:
                    return isGroupRow && isGroupRowExpanded;
                case GridContextMenuItemType.ExpandDetailRow:
                    return hasDetailButton && !isDetailRowExpanded;
                case GridContextMenuItemType.CollapseDetailRow:
                    return hasDetailButton && isDetailRowExpanded;
                case GridContextMenuItemType.NewRow:
                case GridContextMenuItemType.EditRow:
                case GridContextMenuItemType.DeleteRow:
                    return !e.Grid.IsEditing();
                case GridContextMenuItemType.SaveUpdates:
                case GridContextMenuItemType.CancelUpdates:
                    return e.Grid.IsEditing();
            }
            return false;
        }
    }
}
