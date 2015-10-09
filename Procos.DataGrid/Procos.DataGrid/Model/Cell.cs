using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Procos.DataGrid.Annotations;
using Xamarin.Forms;

namespace Procos.DataGrid.Model
{
    public class Cell : BindableObject
    {
        #region Column

        public static readonly BindableProperty ColumnProperty =
            BindableProperty.Create<Cell, Column>(p => p.Column, null);

        public Column Column
        {
            get { return (Column) GetValue(ColumnProperty); }
            set { SetValue(ColumnProperty, value); }
        }

        #endregion

        #region Row

        public static readonly BindableProperty RowProperty =
            BindableProperty.Create<Cell, Row>(p => p.Row, null);

        public Row Row
        {
            get { return (Row) GetValue(RowProperty); }
            set { SetValue(RowProperty, value); }
        }

        #endregion

        #region Template

        public static readonly BindableProperty TemplateProperty =
            BindableProperty.Create<Cell, DataTemplate>(
                p => p.Template, null);

        public DataTemplate Template
        {
            get { return (DataTemplate) GetValue(TemplateProperty); }
            set { SetValue(TemplateProperty, value); }
        }

        #endregion

        public bool IsVisible => Column.IsVisible && Row.IsVisible;

        public bool FirstVisibleColumn => true;


        public Cell(Column col, Row row, string data)
        {
            Column = col;
            Row = row;
            Data = data;
        }

        public string Data { get; set; }

        public Color Color { get; set; }

        public Size Size
        {
            get { return new Size(Column.Width, Row.Height); }
        }

        public Point Offset
        {
            get { return new Point(Column.Offset, Row.Offset); }
        }
    }

    public class CellRange : INotifyPropertyChanged
    {
        public static CellRange EmptyRange
        {
            get { return new CellRange(null, null, null); }
        }

        public GridModel GridModel { get; set; }

        
        public Column Left { get; set; }
        public Column Right { get; set; }
        public Row Top { get; set; }
        public Row Bottom { get; set; }


        #region Width

        public double Width
        {
            get
            {
                return IsValid ? VisibleColumns.Sum(visibleColumn => visibleColumn.Width) : 0; 
            }
        }

        #endregion

        #region Height
        public double Height
        {
            get
            {
                return IsValid ? VisibleRows.Sum(visibleRow => visibleRow.Height) : 0; 
                
            }
        }

        #endregion

        public bool IsValid
            => GridModel != null
               && GridModel.Columns.Contains(Left)
               && GridModel.Columns.Contains(Right)
               && GridModel.Rows.Contains(Top)
               && GridModel.Rows.Contains(Bottom);

        public bool IsSingleCell
            => IsValid && Left == Right && Top == Bottom;

        public CellRange(GridModel gridModel, Column leftCol, Row topRow, Column rightCol, Row bottomRow)
        {
            if (gridModel != null)
            {
                GridModel = gridModel;

                int startCol = GridModel.Columns.IndexOf(leftCol);
                int endCol = GridModel.Columns.IndexOf(rightCol);

                Left = startCol <= endCol ? leftCol : rightCol;
                Right = startCol <= endCol ? rightCol : leftCol;

                int startRow = GridModel.Rows.IndexOf(topRow);
                int endRow = GridModel.Rows.IndexOf(bottomRow);

                Top = startRow <= endRow ? topRow : bottomRow;
                Bottom = startRow <= endRow ? bottomRow : topRow;
            }
        }

        public CellRange(GridModel gridModel, Column col, Row row) : this(gridModel, col, row, col, row)
        {
        }

        public List<Column> VisibleColumns
        {
            get
            {
                return PartialList(GridModel.VisibleColumns, Left, Right);
            }
        }

        public List<Row> VisibleRows
        {
            get
            {
                return PartialList(GridModel.VisibleRows, Top, Bottom);
            }
        }

        public List<Cell> VisibleCells
        {
            get
            {
                var cellList = new List<Cell>();

                foreach (var row in VisibleRows)
                    foreach (var col in VisibleColumns)
                        cellList.Add(row[col]);

                return cellList;
            }
        }

        private List<T> PartialList<T>(LinkedList<T> list, T start, T end)
        {
            List<T> returnList = new List<T>();

            var listNode = list.Find(start);

            if (listNode != null)
            {
                do
                {
                    returnList.Add(listNode.Value);
                    listNode = listNode.Next;
                } while (listNode != null
                         && !Equals(listNode.Previous.Value, end));
            }

            return returnList;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}