using System.Diagnostics;
using Xamarin.Forms;

namespace Procos.DataGrid
{
    public class Column : BindableObject
    {
        #region ActualWidth

        public static readonly BindableProperty ActualWidthProperty =
            BindableProperty.Create<Column, double>(
                p => p.ActualWidth, 120, BindingMode.Default, ValidateActualWidth);

        private static bool ValidateActualWidth(BindableObject bindable, double value)
        {
            var col = bindable as Column;
            Debug.Assert(col != null, "col != null");

            // TODO are this all invalid values?
            return value >= 0 && value <= col.MaxWidth;
        }

        public double ActualWidth
        {
            get { return (double)GetValue(ActualWidthProperty); }
            set { SetValue(ActualWidthProperty, value); }
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
    }
}
