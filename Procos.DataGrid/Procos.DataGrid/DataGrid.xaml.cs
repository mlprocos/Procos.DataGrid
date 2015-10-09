using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Procos.DataGrid.Model;
using Xamarin.Forms;

[assembly: InternalsVisibleTo("Procos.DataGrid.Droid")]

namespace Procos.DataGrid
{
    public partial class DataGrid : ContentView
    {
        #region Properties

        public GridModel GridModel { get; set; } = GridModel.CreateSample();

        #region GridOffset

        public static readonly BindableProperty GridOffsetProperty =
            BindableProperty.Create<DataGrid, Point>(p => p.GridOffset, Point.Zero, BindingMode.Default, null,
                null, null, CoerceGridOffset);

        private static Point CoerceGridOffset(BindableObject bindable, Point value)
        {
            var dataGrid = bindable as DataGrid;
            Debug.Assert(dataGrid != null, "dataGrid != null");

            var gridModel = new Rectangle(0, 0, dataGrid.GridModel.Width, dataGrid.GridModel.Height);
            var viewPanel = new Rectangle(value.X, value.Y, dataGrid.Width, dataGrid.Height);
            
            if (!gridModel.Contains(viewPanel))
            {
                if (value.X < 0) value.X = 0;
                if (value.Y < 0) value.Y = 0;

                if (viewPanel.Width > gridModel.Right)
                    value.X = 0;
                else if (viewPanel.Right > gridModel.Right)
                    value.X = gridModel.Right - viewPanel.Width;

                if (viewPanel.Height > gridModel.Height)
                    value.Y = 0;
                else if (viewPanel.Bottom > gridModel.Bottom)
                    value.Y = gridModel.Bottom - viewPanel.Height;
            }
            
            return value;
        }

        public Point GridOffset
        {
            get { return (Point) GetValue(GridOffsetProperty); }
            set { SetValue(GridOffsetProperty, value); }
        }

        #endregion

        #endregion

        #region Constructor

        public DataGrid()
        {
            InitializeComponent();

            BindingContext = this;
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