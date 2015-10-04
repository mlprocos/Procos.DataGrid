using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Xamarin.Forms;

namespace Procos.DataGrid
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
            Rows = new ObservableCollection<Row>();
            Rows.CollectionChanged += RowsOnCollectionChanged;
        }

        #endregion

        public static GridModel CreateSample()
        {
            var gridModel = new GridModel();

            var colNum = 5;
            var rowNum = 2000;

            for (var i = 0; i < colNum; i++)
            {
                gridModel.Columns.Add(new Column());
            }

            for (var i = 0; i < rowNum; i++)
            {
                var row = new Row();

                for (var j = 0; j < colNum; j++)
                {
                    var col = gridModel.Columns[j];
                    row[col] = new Cell(col, row);
                }

                gridModel.Rows.Add(row);
            }

            return gridModel;
        }

        #region Properties

        #region Columns

        public static readonly BindableProperty ColumnsProperty =
            BindableProperty.Create<GridModel, ObservableCollection<Column>>(
                p => p.Columns, new ObservableCollection<Column>());

        public ObservableCollection<Column> Columns
        {
            get { return (ObservableCollection<Column>) GetValue(ColumnsProperty); }
            set
            {
                SetValue(ColumnsProperty, value);
                Columns.CollectionChanged += ColumnsOnCollectionChanged;
            }
        }

        private void ColumnsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyArgs)
        {
            switch (notifyArgs.Action)
            {
                default:
                    OnPropertyChanged(nameof(Columns));
                    break;
            }
        }

        #endregion

        #region Rows

        public static readonly BindableProperty RowsProperty =
            BindableProperty.Create<GridModel, ObservableCollection<Row>>(
                p => p.Rows, null);

        public ObservableCollection<Row> Rows
        {
            get { return (ObservableCollection<Row>) GetValue(RowsProperty); }
            private set { SetValue(RowsProperty, value); }
        }

        private void RowsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyArgs)
        {
            switch (notifyArgs.Action)
            {
                default:
                    if (InTransformation)
                    {
                        TransformationList.Add(()=>OnPropertyChanged(nameof(Rows)));
                    }
                    else
                    {
                        
                    }
                    OnPropertyChanged(nameof(Rows));
                    break;
            }
        }

        #endregion



        public bool InTransformation { get; private set; }

        private List<Action> TransformationList { get; } = new List<Action>();  

        public void BeginTransformation() => InTransformation = true;

        public void EndTransformation()
        {
            if (InTransformation)
            {
                foreach (var action in TransformationList)
                {
                    action.Invoke();
                }

                InTransformation = false;
                TransformationList.Clear();
            }
        }


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

        #endregion

        #region Public Methods

        public List<Column> VisibleColumns
        {
            get { return Columns.Where(column => column.IsVisible).ToList(); }
        }

        private List<Row> _visibleRows;

        public List<Row> VisibleRows
        {
            get
            {
                if (_visibleRows == null)
                {
                    _visibleRows = new List<Row>();

                    foreach (var row in Rows)
                        if (row.IsVisible)
                        {
                            _visibleRows.Add(row);

                            if (row.IsExpanded)
                                _visibleRows.AddRange(row.AllVisibleChildRows);
                        }
                }
                return _visibleRows;
            }
        }

        public double Width
        {
            get { return VisibleColumns.Sum(visibleColumn => visibleColumn.Width); }
        }

        public double Height
        {
            get { return VisibleRows.Sum(visibleRow => visibleRow.Height); }
        }

        #endregion
    }
}