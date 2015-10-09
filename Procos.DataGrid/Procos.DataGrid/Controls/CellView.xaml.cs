using Procos.DataGrid.Model;
using Procos.DataGrid.Views;
using Xamarin.Forms;

namespace Procos.DataGrid.Controls
{
    public partial class CellView
    {
        #region Properties

        #region BorderColor

        public static readonly BindableProperty BorderColorProperty =
            BindableProperty.Create<CellView, Color>(p => p.BorderColor, Color.Black);

        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }

        #endregion

        #endregion

        #region Constructor

        public CellView()
        {
            InitializeComponent();
        }

        #endregion
    }
}
