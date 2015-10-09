using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;

namespace Procos.DataGrid.Model
{
    public enum SelectionMode
    {
        None,
        Row,
        Cell
    }

    public class GridModel : BindableObject
    {
        #region Constructor

        public GridModel()
        {
        }

        #endregion


        private void ColumnsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyArgs)
        {
            switch (notifyArgs.Action)
            {
                default:
                    DoAction("OnPropertyChanged(nameof(Columns))");
                    DoAction("CalculateWidth");
                    DoAction("InitPanels(FixPoint)");
                    break;
            }
        }
        
        private void RowsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyArgs)
        {
            switch (notifyArgs.Action)
            {
                default:
                    DoAction("OnPropertyChanged(nameof(Rows))");
                    DoAction("CalculateHeight");
                    DoAction("InitPanels(FixPoint)");
                    break;
            }
        }

        public static GridModel CreateSample()
        {
            var random = new Random();

            var gridModel = new GridModel();

            gridModel.BeginTransformation();

            var colNum = 50;
            var rowNum = 150;

            for (var i = 0; i < colNum; i++)
            {
                gridModel.Columns.Add(new Column(gridModel) {Width = 120, Tag = (i + 1)}); //random.Next(80, 150)});
            }

            for (var i = 0; i < rowNum; i++)
            {
                var row = new Row(gridModel) {Height = 80, Tag = (i+1)};//random.Next(80, 120)};

                for (var j = 0; j < colNum; j++)
                {
                    var col = gridModel.Columns[j];
                    row[col] = new Cell(col, row, (j+1) + ":" + (i+1))
                    {
                        Color = Color.FromRgba(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255), 255)
                    };
                }

                gridModel.Rows.Add(row);
            }

//            gridModel.FixPoint = new Point(1,1);

            

            gridModel.EndTransformation();

            return gridModel;
        }

        #region Properties

        #region Columns

        public static readonly BindableProperty ColumnsProperty =
            BindableProperty.Create<GridModel, ObservableCollection<Column>>(
                p => p.Columns, new ObservableCollection<Column>(), BindingMode.Default, null, ColumnsChanged, null, null, DefaultColumnsCreator);

        private static ObservableCollection<Column> DefaultColumnsCreator(GridModel gridModel)
        {
            var columns = new ObservableCollection<Column>();
            columns.CollectionChanged += gridModel.ColumnsOnCollectionChanged;

            return columns;
        }

        private static void ColumnsChanged(BindableObject bindable, ObservableCollection<Column> oldValue, ObservableCollection<Column> newValue)
        {
            var gridModel = bindable as GridModel;
            Debug.Assert(gridModel != null, "gridModel != null");

            newValue.CollectionChanged += gridModel.ColumnsOnCollectionChanged;
        }

        public ObservableCollection<Column> Columns
        {
            get { return (ObservableCollection<Column>) GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }
        
        #endregion

        #region Rows

        public static readonly BindableProperty RowsProperty =
            BindableProperty.Create<GridModel, ObservableCollection<Row>>(
                p => p.Rows, null, BindingMode.Default, null, RowsChanged, null, null, DefaultRowsCreator);

        private static ObservableCollection<Row> DefaultRowsCreator(GridModel gridModel)
        {
            var rows = new ObservableCollection<Row>();
            rows.CollectionChanged += gridModel.RowsOnCollectionChanged;

            return rows;
        }

        private static void RowsChanged(BindableObject bindable, ObservableCollection<Row> oldValue, ObservableCollection<Row> newValue)
        {
            var gridModel = bindable as GridModel;
            Debug.Assert(gridModel != null, "gridModel != null");

            newValue.CollectionChanged += gridModel.RowsOnCollectionChanged;
        }

        public ObservableCollection<Row> Rows
        {
            get { return (ObservableCollection<Row>) GetValue(RowsProperty); }
            private set { SetValue(RowsProperty, value); }
        }

        #endregion

        #region SelectionMode

        public static readonly BindableProperty SelectionModeProperty =
            BindableProperty.Create<GridModel, SelectionMode>(
                p => p.SelectionMode, SelectionMode.None);

        public SelectionMode SelectionMode
        {
            get { return (SelectionMode) GetValue(SelectionModeProperty); }
            set { SetValue(RowsProperty, value); }
        }

        #endregion

        #region Width

        public static readonly BindableProperty WidthProperty =
            BindableProperty.Create<Column, double>(
                p => p.Width, 0);

        public double Width
        {
            get { return (double) GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        #endregion

        #region Height

        public static readonly BindableProperty HeightProperty =
            BindableProperty.Create<Row, double>(
                p => p.Height, 0);

        public double Height
        {
            get { return (double) GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        #endregion

        #region CornerPanel

        public static readonly BindableProperty CornerPanelProperty =
            BindableProperty.Create<GridModel, CellRange>(
                p => p.CornerPanel, CellRange.EmptyRange);

        public CellRange CornerPanel
        {
            get { return (CellRange)GetValue(CornerPanelProperty); }
            set { SetValue(CornerPanelProperty, value); }
        }

        #endregion

        #region HeaderPanel

        public static readonly BindableProperty HeaderPanelProperty =
            BindableProperty.Create<GridModel, CellRange>(
                p => p.HeaderPanel, CellRange.EmptyRange);

        public CellRange HeaderPanel
        {
            get { return (CellRange)GetValue(HeaderPanelProperty); }
            set { SetValue(HeaderPanelProperty, value); }
        }

        #endregion

        #region FixedColumnsPanel

        public static readonly BindableProperty FixedColumnsPanelProperty =
            BindableProperty.Create<GridModel, CellRange>(
                p => p.FixedColumnsPanel, CellRange.EmptyRange);

        public CellRange FixedColumnsPanel
        {
            get { return (CellRange)GetValue(FixedColumnsPanelProperty); }
            set { SetValue(FixedColumnsPanelProperty, value); }
        }

        #endregion

        #region MainPanel

        public static readonly BindableProperty MainPanelProperty =
            BindableProperty.Create<GridModel, CellRange>(
                p => p.MainPanel, CellRange.EmptyRange);

        public CellRange MainPanel
        {
            get { return (CellRange)GetValue(MainPanelProperty); }
            set { SetValue(MainPanelProperty, value); }
        }

        #endregion
        
        #region FixPoint

        public static readonly BindableProperty FixPointProperty =
            BindableProperty.Create<GridModel, Point>(
                p => p.FixPoint, Point.Zero, BindingMode.Default, null, FixPointChanged, null, CoerceFixPoint);

//        private static Point DefaultFixPointCreator(GridModel gridModel)
//        {
//            gridModel.InitPanels(Point.Zero);
//
//            return Point.Zero;
//        }

        private static Point CoerceFixPoint(BindableObject bindable, Point value)
        {
            var gridModel = bindable as GridModel;
            Debug.Assert(gridModel != null, "gridModel != null");

            value = value.Round();

            int visibleColumns = gridModel.VisibleColumns.Count - 1;
            if (value.X > visibleColumns)
                value.X = visibleColumns;

            int visibleRows = gridModel.VisibleRows.Count - 1;
            if (value.Y > visibleRows)
                value.Y = visibleRows;

            return value;
        }

        private static void FixPointChanged(BindableObject bindable, Point oldValue, Point newValue)
        {
            var gridModel = bindable as GridModel;
            Debug.Assert(gridModel != null, "gridModel != null");

            gridModel.InitPanels(newValue);
        }

        private void InitPanels(Point fixPoint)
        {
            int colNum = VisibleColumns.Count - 1;
            int rowNum = VisibleRows.Count - 1;

            if (colNum<0 || rowNum<0)
                return;

            if (fixPoint.X == 0 || fixPoint.Y == 0)
            {
                CornerPanel = CellRange.EmptyRange;
            }
            else
            {
                CornerPanel = new CellRange(this, Columns[0], Rows[0],
                    Columns[(int) (fixPoint.X - 1)], Rows[(int) (fixPoint.Y - 1)]);
            }

            if (fixPoint.Y == 0)
            {
                HeaderPanel = CellRange.EmptyRange;
            }
            else
            {
                HeaderPanel = new CellRange(this, Columns[(int)(fixPoint.X)], Rows[0], Columns[colNum], Rows[(int)(fixPoint.Y - 1)]);
            }


            if (fixPoint.X == 0)
            {
                FixedColumnsPanel = CellRange.EmptyRange;
            }
            else
            {
                FixedColumnsPanel = new CellRange(this, Columns[0],
                    Rows[(int) (fixPoint.Y)], Columns[(int) (fixPoint.X - 1)],
                    Rows[rowNum]);
            }

            MainPanel = new CellRange(this, Columns[(int)(fixPoint.X)], Rows[(int)(fixPoint.Y)], Columns[colNum], Rows[rowNum]);
        }

        public Point FixPoint
        {
            get { return (Point)GetValue(FixPointProperty); }
            set { SetValue(FixPointProperty, value); }
        }

        #endregion

        private LinkedList<Column> _visibleColumns;
        public LinkedList<Column> VisibleColumns
        {
            get
            {
                return _visibleColumns ??
                       (_visibleColumns = new LinkedList<Column>(Columns.Where(column => column.IsVisible).ToList()));
            }
        }

        private LinkedList<Row> _visibleRows;
        public LinkedList<Row> VisibleRows
        {
            get
            {
                if (_visibleRows == null)
                {
                    _visibleRows = new LinkedList<Row>();

                    foreach (var row in Rows)
                        if (row.IsVisible)
                        {
                            _visibleRows.AddLast(row);

                            if (row.IsExpanded)
                                foreach (var childRow in row.AllVisibleChildRows)
                                    _visibleRows.AddLast(childRow);
                        }
                }
                return _visibleRows;
            }
        }

        #endregion

        #region Private Methods

        private void CalculateWidth()
        {
            Width = 0;

            foreach (var visibleColumn in VisibleColumns)
            {
                visibleColumn.Offset = Width;
                Width += visibleColumn.Width;
            }
        }

        private void CalculateHeight()
        {
            Height = 0;

            foreach (var visibleRow in VisibleRows)
            {
                visibleRow.Offset = Height;
                Height += visibleRow.Height;
            }
        }

        #endregion

        #region Action Collection

        private void DoAction(String function)
        {
            if (InTransformation)
            {
                if (!TransformationList.Contains(function))
                    TransformationList.Add(function);
            }
            else
            {
                InvokeFunction(function);
            }
        }

        private void InvokeFunction(String function)
        {
            switch (function)
            {
                case "OnPropertyChanged(nameof(Columns))":
                    OnPropertyChanged(nameof(Columns));
                    break;
                case "OnPropertyChanged(nameof(Rows))":
                    OnPropertyChanged(nameof(Rows));
                    break;
                case "CalculateWidth":
                    CalculateWidth();
                    break;
                case "CalculateHeight":
                    CalculateHeight();
                    break;
                case "InitPanels(FixPoint)":
                    InitPanels(FixPoint);
                    break;
            }
        }
        
        public bool InTransformation { get; private set; }

        private List<String> TransformationList { get; } = new List<String>();

        public void BeginTransformation() => InTransformation = true;

        public void EndTransformation()
        {
            if (InTransformation)
            {
                foreach (var action in TransformationList)
                    InvokeFunction(action);

                InTransformation = false;
                TransformationList.Clear();
            }
        }

        #endregion

        public void Register(BindableObject bindable)
        {
            bindable.PropertyChanged += BindableOnPropertyChanged;
        }

        public void Unregister(BindableObject bindable)
        {
            bindable.PropertyChanged -= BindableOnPropertyChanged;
        }

        private void BindableOnPropertyChanged(object sender, PropertyChangedEventArgs eventArgs)
        {
            if (sender is Row)
                RowOnPropertyChanged(sender as Row, eventArgs);
            else if (sender is Column)
                ColumnOnPropertyChanged(sender as Column, eventArgs);
        }

        private void ColumnOnPropertyChanged(Column col, PropertyChangedEventArgs eventArgs)
        {
            switch (eventArgs.PropertyName)
            {
                case "Width":
                    DoAction("CalculateWidth");
                    break;
            }
        }

        private void RowOnPropertyChanged(Row row, PropertyChangedEventArgs eventArgs)
        {
            switch (eventArgs.PropertyName)
            {
                case "Height":
                    DoAction("CalculateHeight");
                    break;
            }
        }
    }
}