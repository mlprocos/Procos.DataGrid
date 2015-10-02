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
                p => p.Rows, new ObservableCollection<Row>());

        public ObservableCollection<Row> Rows
        {
            get { return (ObservableCollection<Row>) GetValue(RowsProperty); }
            set
            {
                SetValue(RowsProperty, value);
                Rows.CollectionChanged += RowsOnCollectionChanged;
            }
        }

        private void RowsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyArgs)
        {
            switch (notifyArgs.Action)
            {
                default:
                    OnPropertyChanged(nameof(Rows));
                    break;
            }
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

        #endregion

        #region Public Methods

        public List<Column> VisibleColumns
        {
            get
            {
                return Columns.Where(column => column.IsVisible).ToList();
            }
        }

        public List<Row> VisibleRows
        {
            get
            {
                var visibleRows = new List<Row>();

                foreach (var row in Rows)
                    if (row.IsVisible)
                    {
                        visibleRows.Add(row);

                        if (row.IsExpanded)
                            visibleRows.AddRange(row.AllVisibleChildRows);
                    }

                return visibleRows;
            }
        }

        public double Width
        {
            get
            {
                return VisibleColumns.Sum(visibleColumn => visibleColumn.Width);
            }
        }

        public double Height
        {
            get
            {
                return VisibleRows.Sum(visibleRow => visibleRow.Height);
            }
        }

        #endregion

        public static GridModel CreateSample()
        {
            var gridModel = new GridModel();

            int colNum = 5;
            int rowNum = 10;

            for (int i = 0; i < colNum; i++)
            {
                gridModel.Columns.Add(new Column());
            }

            for (int i = 0; i < rowNum; i++)
            {
                var row = new Row();

                for (int j = 0; j < colNum; j++)
                {
                    var col = gridModel.Columns[j];
                    row[col] = new Cell(col, row);
                }

                gridModel.Rows.Add(row);

            }

            return gridModel;
        }
    }
}