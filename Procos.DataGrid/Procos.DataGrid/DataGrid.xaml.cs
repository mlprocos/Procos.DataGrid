using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

[assembly: InternalsVisibleTo("Procos.DataGrid.Droid")]

namespace Procos.DataGrid
{
    public partial class DataGrid
    {
        #region Properties

        public GridModel GridModel { get; set; } = GridModel.CreateSample();

        #region ContentOffset

        public static readonly BindableProperty ContentOffsetProperty =
            BindableProperty.Create<DataGrid, Point>(p => p.ContentOffset, Point.Zero, BindingMode.Default, null,
                ContentOffsetChanged, null, ContentOffsetCoerce);

        private static Point ContentOffsetCoerce(BindableObject bindable, Point value)
        {
            var dataGrid = bindable as DataGrid;
            Debug.Assert(dataGrid != null, "dataGrid != null");

            var gridModel = new Rectangle(0, 0, dataGrid.GridModel.Width, dataGrid.GridModel.Height);
            var mainPanel = new Rectangle(value.X, value.Y, dataGrid.MainPanel.Width, dataGrid.MainPanel.Height);

            if (!gridModel.Contains(mainPanel))
            {
                if (value.X < 0) value.X = 0;
                if (value.Y < 0) value.Y = 0;

                if (mainPanel.Width > gridModel.Right)
                    value.X = 0;
                else if (mainPanel.Right > gridModel.Right)
                    value.X = gridModel.Right - mainPanel.Width;

                if (mainPanel.Height > gridModel.Bottom)
                    value.Y = 0;
                else if (mainPanel.Bottom > gridModel.Bottom)
                    value.Y = gridModel.Bottom - mainPanel.Height;
            }

            return value;
        }

        private static void ContentOffsetChanged(BindableObject bindable, Point oldValue, Point newValue)
        {
            var dataGrid = bindable as DataGrid;
            Debug.Assert(dataGrid != null, "dataGrid != null");

            dataGrid.RefreshLayout();
        }

        public Point ContentOffset
        {
            get { return (Point) GetValue(ContentOffsetProperty); }
            set { SetValue(ContentOffsetProperty, value); }
        }

        #endregion

        #endregion

        #region Constructor

        public DataGrid()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        private void OnPropertyChanging(object sender, PropertyChangingEventArgs e)
        {
//            throw new NotImplementedException();
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
//            throw new NotImplementedException();
        }

        #endregion

        #region Private Methods

        private Point CellOffset(Cell cell)
        {
            return new Point(ColumnOffest(cell.Column) -ContentOffset.X, RowOffest(cell.Row)-ContentOffset.Y);
        }

        private double ColumnOffest(Column column)
        {
            var visibleColumns = GridModel.VisibleColumns;

            if (visibleColumns.Contains(column))
            {
                double offset = 0;

                foreach (var visibleColumn in visibleColumns)
                    if (visibleColumn == column)
                        return offset;
                    else
                        offset += visibleColumn.Width;
            }

            return -1;
        }

        private double RowOffest(Row row)
        {
            var visibleRows = GridModel.VisibleRows;

            if (visibleRows.Contains(row))
            {
                double offset = 0;

                foreach (var visibleRow in visibleRows)
                    if (visibleRow == row)
                        return offset;
                    else
                        offset += visibleRow.Height;
            }

            return -1;
        }

        #endregion




        private Dictionary<Cell, View> cellViewDictionary = new Dictionary<Cell, View>(); 

        internal void RefreshLayout()
        {
            var rectangle = AbsoluteLayout.GetLayoutBounds(MainPanel);

            rectangle.X = 10 - ContentOffset.X;
            rectangle.Y = 10 - ContentOffset.Y;

//            rectangle.Width = rectangle.Width + ContentOffset.X;
//            rectangle.Height = rectangle.Height + ContentOffset.Y;

            AbsoluteLayout.SetLayoutBounds(MainPanel, rectangle);

            var viewRange = CellRange.EmptyRange;
            viewRange.GridModel = GridModel;

            var viewRectangle = new Rectangle(ContentOffset, new Size(CompleteLayout.Width, CompleteLayout.Height));

            foreach (var visibleColumn in GridModel.VisibleColumns)
            {
                var columnRectangle = new Rectangle(new Point(ColumnOffest(visibleColumn), 0), new Size(visibleColumn.Width, MainPanel.Height));

                if (viewRectangle.IntersectsWith(columnRectangle))
                {
                    if (viewRange.Left == null)
                        viewRange.Left = visibleColumn;
                    else
                        viewRange.Right = visibleColumn;
                }
            }

            foreach (var visibleRow in GridModel.VisibleRows)
            {
                var rowRectangle = new Rectangle(new Point(RowOffest(visibleRow), 0), new Size(visibleRow.Height, MainPanel.Width));

                if (viewRectangle.IntersectsWith(rowRectangle))
                {
                    if (viewRange.Top == null)
                        viewRange.Top = visibleRow;
                    else
                        viewRange.Bottom = visibleRow;
                }
            }

            var visibleCells = viewRange.VisibleCells;

            foreach (var visibleCell in visibleCells)
            {
                if (!cellViewDictionary.ContainsKey(visibleCell))
                {
                    cellViewDictionary[visibleCell] = GetFreeView();

                    cellViewDictionary[visibleCell].BindingContext = visibleCell;

                    CompleteLayout.Children.Add(cellViewDictionary[visibleCell]);
                }

                var posRectangle = new Rectangle(CellOffset(visibleCell), visibleCell.Size);
                AbsoluteLayout.SetLayoutBounds(cellViewDictionary[visibleCell], posRectangle);

                cellViewDictionary[visibleCell].Layout(posRectangle);
            }


            foreach (var cell in cellViewDictionary.Keys.ToArray())
            {
                if (!visibleCells.Contains(cell))
                {
                    CompleteLayout.Children.Remove(cellViewDictionary[cell]);
                    freeViews.Push(cellViewDictionary[cell]);
                    cellViewDictionary.Remove(cell);
                }
            }

        }


        private Stack<View> freeViews = new Stack<View>(); 
        private View GetFreeView()
        {
            if (freeViews.Count > 0)
                return freeViews.Pop();

            var box = new BoxView();

            box.SetBinding(BoxView.ColorProperty, "Color");
            box.SetBinding(WidthRequestProperty, "Column.Width");
            box.SetBinding(HeightRequestProperty, "Row.Height");

            AbsoluteLayout.SetLayoutFlags(box, AbsoluteLayoutFlags.None);

            return box;
        }

        internal bool SingleTap(double x, double y)
        {
            // TODO pretend that there is a setting which binds the "select row" command
            // to the "single tap " action.

//            if (SelMode.Row == SelectionMode)
//            {
//                var x2 = x - MainPanel.X + ContentOffset.X;
//                if ((x2 < 0) || (x2 > _cachedTotalWidth))
//                {
//                    return false;
//                }
//                var y2 = y - MainPanel.Y + ContentOffset.Y;
//                int row = (int)(y2 / RowHeightPlusSpacing);
//                if ((row < 0) || (row >= NumberOfRows))
//                {
//                    return false;
//                }
//                // when we get a tap on the row that is already selected, we unselect it.
//                // TODO this could be a matter of policy
//                if (row == SelectedRowIndex)
//                {
//                    SelectedRowIndex = -1;
//                }
//                else
//                {
//                    SelectedRowIndex = row;
//                }
//                return true;
//            }
            return false;
        }
    }
}