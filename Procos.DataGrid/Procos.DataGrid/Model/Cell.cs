using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Procos.DataGrid.Annotations;
using Xamarin.Forms;

namespace Procos.DataGrid
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


        public Cell(Column col, Row row)
        {
            Column = col;
            Row = row;

            Color = GetRandomColor();
        }

        public Color Color { get; set; }

        public Size Size
        {
            get { return new Size(Column.Width, Row.Height); }
        }


        private static readonly Random random = new Random();

        private Color GetRandomColor()
        {
            return Color.FromRgba(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
        }
    }

    public class CellRange : INotifyPropertyChanged
    {
        public static CellRange EmptyRange = new CellRange(null, null, null);

        public GridModel GridModel { get; set; }

        public Column Left { get; set; }
        public Column Right { get; set; }
        public Row Top { get; set; }
        public Row Bottom { get; set; }

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
            if (GridModel != null)
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


        public List<Cell> VisibleCells
        {
            get
            {
                var cellList = new List<Cell>();

                int left = GridModel.VisibleColumns.IndexOf(Left);
                int right = GridModel.VisibleColumns.IndexOf(Right);

                int top = GridModel.VisibleRows.IndexOf(Top);
                int bottom = GridModel.VisibleRows.IndexOf(Bottom);

                for (int j = top; j <= bottom; j++)
                {
                    for (int i = left; i <= right; i++)
                    {
                        cellList.Add(GridModel.Rows[j][GridModel.VisibleColumns[i]]);
                    }
                }

                return cellList;
            }
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