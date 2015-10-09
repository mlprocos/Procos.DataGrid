using System.Diagnostics;
using Xamarin.Forms;

namespace Procos.DataGrid.Model
{
    public class Column : BindableObject
    {
        #region Properties

        #region WidthRequest

        public static readonly BindableProperty WidthRequestProperty =
            BindableProperty.Create<Column, double>(
                p => p.WidthRequest, 120, BindingMode.Default, ValidateWidthRequest);

        private static bool ValidateWidthRequest(BindableObject bindable, double value)
        {
            var col = bindable as Column;
            Debug.Assert(col != null, "col != null");

            // TODO are this all invalid values?
            return value >= 0 && value <= col.MaxWidth;
        }

        public double WidthRequest
        {
            get { return (double)GetValue(WidthRequestProperty); }
            set { SetValue(WidthRequestProperty, value); }
        }

        #endregion

        #region Width

        public static readonly BindableProperty WidthProperty =
            BindableProperty.Create<Column, double>(
                p => p.Width, 120, BindingMode.Default, ValidateWidth);

        private static bool ValidateWidth(BindableObject bindable, double value)
        {
            var col = bindable as Column;
            Debug.Assert(col != null, "col != null");

            // TODO are this all invalid values?
            return value >= 0 && value <= col.MaxWidth;
        }

        public double Width
        {
            get { return (double)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        #endregion

        #region MaxWidth

        public static readonly BindableProperty MaxWidthProperty =
            BindableProperty.Create<Column, double>(
                p => p.MaxWidth, 500, BindingMode.Default, ValidateMaxWidth);

        private static bool ValidateMaxWidth(BindableObject bindable, double value)
        {
            // TODO are this all invalid values?
            return value >= 0;
        }

        public double MaxWidth
        {
            get { return (double)GetValue(MaxWidthProperty); }
            set { SetValue(MaxWidthProperty, value); }
        }

        #endregion

        #region Template

        public static readonly BindableProperty TemplateProperty =
            BindableProperty.Create<Column, DataTemplate>(
                p => p.Template, null);

        public DataTemplate Template
        {
            get { return (DataTemplate)GetValue(TemplateProperty); }
            set { SetValue(TemplateProperty, value); }
        }

        #endregion

        #region HeaderView

        public static readonly BindableProperty HeaderViewProperty =
            BindableProperty.Create<Column, View>(
                p => p.HeaderView, null);

        public View HeaderView
        {
            get { return (View)GetValue(HeaderViewProperty); }
            set { SetValue(HeaderViewProperty, value); }
        }

        #endregion

        #region IsVisible

        public static readonly BindableProperty IsVisibleProperty =
            BindableProperty.Create<Column, bool>(
                p => p.IsVisible, true);

        public bool IsVisible
        {
            get { return (bool)GetValue(IsVisibleProperty); }
            set { SetValue(IsVisibleProperty, value); }
        }

        #endregion

        #region GridModel

        public static readonly BindableProperty GridModelProperty =
            BindableProperty.Create<Row, GridModel>(
                p => p.GridModel, null, BindingMode.Default, null, GridModelChanged);

        private static void GridModelChanged(BindableObject bindable, GridModel oldValue, GridModel newValue)
        {
            var col = bindable as Column;

            oldValue?.Unregister(col);
            newValue.Register(col);
        }

        public GridModel GridModel
        {
            get { return (GridModel)GetValue(GridModelProperty); }
            set { SetValue(GridModelProperty, value); }
        }

        #endregion


        public double Offset { get; set; } = 0;

        #endregion

        #region Constructor

        public Column(GridModel gridModel)
        {
            GridModel = gridModel;
        }

        #endregion

        public int Tag { get; set; }
    }
}
