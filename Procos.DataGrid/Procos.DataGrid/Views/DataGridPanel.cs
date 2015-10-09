using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Procos.DataGrid.Controls;
using Procos.DataGrid.Model;
using Xamarin.Forms;
using Cell = Procos.DataGrid.Model.Cell;

namespace Procos.DataGrid.Views
{
    internal class DataGridPanel : Layout<View>
    {
        private readonly BoxView _scrollVerticalView = new BoxView {Color = Color.White, Opacity = 1};
        private readonly BoxView _scrollHorizontalView = new BoxView {Color = Color.White, Opacity = 1};
//        private readonly Label _statisticLabel = new Label {BackgroundColor = Color.Black, TextColor = Color.White};

        private readonly Dictionary<Cell, View> _cellViewDictionary = new Dictionary<Cell, View>();
        private readonly Stack<View> _freeViews = new Stack<View>();
        
        #region Properties

        #region GridModel

        public static readonly BindableProperty GridModelProperty =
            BindableProperty.Create<DataGridPanel, GridModel>(p => p.GridModel, null);

        public GridModel GridModel
        {
            get { return (GridModel) GetValue(GridModelProperty); }
            set { SetValue(GridModelProperty, value); }
        }

        #endregion

        #region GridOffset

        public static readonly BindableProperty GridOffsetProperty =
            BindableProperty.Create<DataGridPanel, Point>(p => p.GridOffset, Point.Zero, BindingMode.OneWay, null,
                GridOffsetChanged);

        private static void GridOffsetChanged(BindableObject bindable, Point oldValue, Point newValue)
        {
            var dataGridPanel = bindable as DataGridPanel;
            Debug.Assert(dataGridPanel != null, "dataGridPanel != null");

            dataGridPanel.PanelOffset = newValue;
        }

        public Point GridOffset
        {
            get { return (Point) GetValue(GridOffsetProperty); }
            set { SetValue(GridOffsetProperty, value); }
        }

        #endregion

        #region PanelOffset

        public static readonly BindableProperty PanelOffsetProperty =
            BindableProperty.Create<DataGridPanel, Point>(p => p.PanelOffset, Point.Zero, BindingMode.OneWay, null,
                PanelOffsetChanged, null, CoercePanelOffset);

        private static Point CoercePanelOffset(BindableObject bindable, Point value)
        {
            var dataGridPanel = bindable as DataGridPanel;
            Debug.Assert(dataGridPanel != null, "dataGridPanel != null");

            var gridPanel = dataGridPanel.GridPanelRectangle;

            value.X += gridPanel.X;
            value.Y += gridPanel.Y;

            var viewPanel = new Rectangle(value.X, value.Y, dataGridPanel.Width, dataGridPanel.Height);

            if (!gridPanel.Contains(viewPanel))
            {
                if (viewPanel.Width > gridPanel.Right)
                    value.X = gridPanel.X;
                else if (viewPanel.Right > gridPanel.Right)
                    value.X = gridPanel.Right - viewPanel.Width;

                if (viewPanel.Height > gridPanel.Bottom)
                    value.Y = gridPanel.Y;
                else if (viewPanel.Bottom > gridPanel.Bottom)
                    value.Y = gridPanel.Bottom - viewPanel.Height;
            }

            return value;
        }

        private static void PanelOffsetChanged(BindableObject bindable, Point oldValue, Point newValue)
        {
            var dataGridPanel = bindable as DataGridPanel;
            Debug.Assert(dataGridPanel != null, "dataGridPanel != null");

            dataGridPanel.RefreshLayout();
        }

        public Point PanelOffset
        {
            get { return (Point) GetValue(PanelOffsetProperty); }
            set { SetValue(PanelOffsetProperty, value); }
        }

        #endregion

        #region ViewRange

        private CellRange _viewRange;

        private CellRange ViewRange
        {
            get
            {
                if (GridRange == null || !GridRange.IsValid)
                    return CellRange.EmptyRange;

                if (_viewRange == null || !_viewRange.IsValid)
                {
                    _viewRange = CellRange.EmptyRange;
                    _viewRange.GridModel = GridModel;

                    foreach (var visibleColumn in GridRange.VisibleColumns)
                    {
                        var columnRectangle = new Rectangle(new Point(visibleColumn.Offset, 0),
                            new Size(visibleColumn.Width, double.MaxValue));

                        if (GridPanelRectangle.IntersectsWith(columnRectangle))
                        {
                            if (_viewRange.Left == null)
                                _viewRange.Left = _viewRange.Right = visibleColumn;
                            else
                                _viewRange.Right = visibleColumn;
                        }
                    }

                    foreach (var visibleRow in GridRange.VisibleRows)
                    {
                        var rowRectangle = new Rectangle(new Point(0, visibleRow.Offset),
                            new Size(double.MaxValue, visibleRow.Height));

                        if (GridPanelRectangle.IntersectsWith(rowRectangle))
                        {
                            if (_viewRange.Top == null)
                                _viewRange.Top = _viewRange.Bottom = visibleRow;
                            else
                                _viewRange.Bottom = visibleRow;
                        }
                    }
                }

                return _viewRange;
            }
        }

        #endregion

        #region GridRange

        public static readonly BindableProperty GridRangeProperty =
            BindableProperty.Create<DataGridPanel, CellRange>(p => p.GridRange, CellRange.EmptyRange,
                BindingMode.Default, null, GridRangeChanged);

        private static void GridRangeChanged(BindableObject bindable, CellRange oldValue, CellRange newValue)
        {
            var dataGridPanel = bindable as DataGridPanel;
            Debug.Assert(dataGridPanel != null, "dataGridPanel != null");

            if (newValue.IsValid)
                dataGridPanel.RefreshLayout();
        }

        public CellRange GridRange
        {
            get { return (CellRange) GetValue(GridRangeProperty); }
            set { SetValue(GridRangeProperty, value); }
        }

        #endregion

        #region ShowScrollPosition

        public static readonly BindableProperty ShowScrollPositionProperty =
            BindableProperty.Create<DataGridPanel, bool>(p => p.ShowScrollPosition, false, BindingMode.Default);

        public bool ShowScrollPosition
        {
            get { return (bool) GetValue(ShowScrollPositionProperty); }
            set { SetValue(ShowScrollPositionProperty, value); }
        }

        #endregion

        #region GridPanelRectangle

        private Rectangle _gridPanelRectangle = Rectangle.Zero;

        private Rectangle GridPanelRectangle
        {
            get
            {
                if (GridRange.IsValid
                    && _gridPanelRectangle == Rectangle.Zero)
                {
                    double leftOffset = GridRange.Left.Offset;
                    double topOffset = GridRange.Top.Offset;

                    double width = GridRange.Right.Offset + GridRange.Right.Width - leftOffset;
                    double height = GridRange.Bottom.Offset + GridRange.Bottom.Height - topOffset;

                    _gridPanelRectangle = new Rectangle(leftOffset, topOffset, width, height);
                }

                return _gridPanelRectangle;
            }
        }

        #endregion

        #endregion

        #region Configuration

        protected override bool ShouldInvalidateOnChildAdded(View child) => false;

        protected override bool ShouldInvalidateOnChildRemoved(View child) => false;

        public bool CallOnChildMeasureInvalidated { get; set; } = false;

        protected override void OnChildMeasureInvalidated()
        {
            if (CallOnChildMeasureInvalidated)
                base.OnChildMeasureInvalidated();
        }
        
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            PanelOffset = GridOffset;
        }

        #endregion

        #region Constructor

        public DataGridPanel()
        {
            IsClippedToBounds = true;
        }

        #endregion

        #region Private Methods
        
        private void RefreshLayout()
        {
            if (ViewRange == null || !ViewRange.IsValid)
                return;

            RefreshViewRange();

            var visibleCells = ViewRange.VisibleCells;

            foreach (var cell in _cellViewDictionary.Keys.ToArray())
            {
                if (!visibleCells.Contains(cell))
                {
//                    Children.Remove(_cellViewDictionary[cell]);
                    _freeViews.Push(_cellViewDictionary[cell]);
                    _cellViewDictionary.Remove(cell);
                }
            }

            foreach (var visibleCell in visibleCells)
            {
                View cellView;

                if (!_cellViewDictionary.ContainsKey(visibleCell))
                {
                    cellView = GetFreeView();
                    cellView.BindingContext = visibleCell;
                    Children.Add(cellView);
                    _cellViewDictionary[visibleCell] = cellView;
                }
                else
                {
                    cellView = _cellViewDictionary[visibleCell];
                }

                cellView.Layout(LayoutCell(visibleCell));
            }

            foreach (var freeView in _freeViews)
                if (!_cellViewDictionary.ContainsValue(freeView)
                    && Children.Contains(freeView))
                    Children.Remove(freeView);

            if (ShowScrollPosition)
            {
                if (Children.Contains(_scrollVerticalView))
                    Children.Remove(_scrollVerticalView);

                double scrollLength = Height/GridRange.Height;

                if (scrollLength < 1)
                {
                    _scrollVerticalView.Opacity = 1;

                    Children.Add(_scrollVerticalView);

                    double scrollStart = GridOffset.Y/GridRange.Height*Height;
                    scrollLength *= Height;

                    _scrollVerticalView.Layout(new Rectangle(new Point(Width - 10, scrollStart),
                        new Size(5, (scrollLength > 25) ? scrollLength : 25)));

                    _scrollVerticalView.FadeTo(0, 1000, Easing.SpringIn);
                }

                if (Children.Contains(_scrollHorizontalView))
                    Children.Remove(_scrollHorizontalView);

                scrollLength = Width/GridRange.Width;

                if (scrollLength < 1)
                {
                    _scrollHorizontalView.Opacity = 1;

                    Children.Add(_scrollHorizontalView);

                    double scrollStart = GridOffset.X/GridRange.Width*Width;
                    scrollLength *= Width;

                    _scrollHorizontalView.Layout(new Rectangle(new Point(scrollStart, Height - 10),
                        new Size((scrollLength > 25) ? scrollLength : 25, 5)));

                    _scrollHorizontalView.FadeTo(0, 1000, Easing.SpringIn);
                }
            }

//            if (Children.Contains(_statisticLabel))
//                Children.Remove(_statisticLabel);
//
//            Children.Add(_statisticLabel);
//
//            _statisticLabel.Text = _callCount + Environment.NewLine;
//            _statisticLabel.Layout(new Rectangle(new Point(5, Height - 125), new Size(180, 120)));
        }
        
        private void RefreshViewRange()
        {
            var leftNode = GridModel.VisibleColumns.Find(ViewRange.Left);
            ViewRange.Left = GetNode(leftNode, PanelOffset.X, "Width");

            var rightNode = GridModel.VisibleColumns.Find(ViewRange.Right);
            ViewRange.Right = GetNode(rightNode, PanelOffset.X + Width, "Width");

            var topNode = GridModel.VisibleRows.Find(ViewRange.Top);
            ViewRange.Top = GetNode(topNode, PanelOffset.Y, "Height");

            var bottomNode = GridModel.VisibleRows.Find(ViewRange.Bottom);
            ViewRange.Bottom = GetNode(bottomNode, PanelOffset.Y + Height, "Height");
        }

        private T GetNode<T>(LinkedListNode<T> linkedNode, double offset, string property)
        {
            int between = Between(offset, linkedNode, property);

            while (between != 0)
            {
                var node = (between == 1) ? linkedNode.Next : linkedNode.Previous;

                if (node == null)
                    between = 0;
                else
                {
                    linkedNode = node;
                    between = Between(offset, linkedNode, property);
                }
            }

            return linkedNode.Value;
        }

        private int Between<T>(double value, LinkedListNode<T> node, string property)
        {
            double offset = Convert.ToDouble(node.Value.GetType().GetRuntimeProperty("Offset").GetValue(node.Value));
            double length = Convert.ToDouble(node.Value.GetType().GetRuntimeProperty(property).GetValue(node.Value));

            if (value < offset) return -1;
            if (value >= offset + length) return 1;

            return 0;
        }

        private Rectangle LayoutCell(Cell cell)
        {
            var cellOffset = cell.Offset;
            return new Rectangle(new Point(cellOffset.X - PanelOffset.X, cellOffset.Y - PanelOffset.Y), cell.Size);
        }

        private View GetFreeView()
        {
            if (_freeViews.Count > 0)
                return _freeViews.Pop();

            return CreateCellView();
        }

        private View CreateCellView()
        {
            var grid = new CellControl();

            grid.SetBinding(CellControl.BorderColorProperty, "Color");
            grid.SetBinding(WidthRequestProperty, "Column.Width");
            grid.SetBinding(HeightRequestProperty, "Row.Height");

            var label = new Label();
            label.SetBinding(Label.TextProperty, "Data");
            grid.Content = label;

            AbsoluteLayout.SetLayoutFlags(grid, AbsoluteLayoutFlags.None);

            return grid;
        }

        #endregion

        #region Layout Members

        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            RefreshLayout();
        }

        #endregion
    }
}