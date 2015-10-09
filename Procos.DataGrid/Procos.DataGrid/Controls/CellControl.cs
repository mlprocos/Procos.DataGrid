using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

[assembly: InternalsVisibleTo("Procos.DataGrid.Droid")]

namespace Procos.DataGrid.Controls
{
    [ContentProperty("Content")]
    public class CellControl : ContentView
    {
        #region Properties

        #region BorderColor

        public static readonly BindableProperty BorderColorProperty =
            BindableProperty.Create<CellControl, Color>(p => p.BorderColor, Color.Black);

        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }

        #endregion

        #region BorderThickness

        public static readonly BindableProperty BorderThicknessProperty =
            BindableProperty.Create<CellControl, Thickness>(p => p.BorderThickness, new Thickness(1));

        public Thickness BorderThickness
        {
            get { return (Thickness)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        #endregion

        #region Margin

        public static readonly BindableProperty MarginProperty =
            BindableProperty.Create<CellControl, Thickness>(p => p.Margin, new Thickness(0), BindingMode.Default, null, MarginChanged);

        private static void MarginChanged(BindableObject bindable, Thickness oldValue, Thickness newValue)
        {
            var cellControl = bindable as CellControl;
            Debug.Assert(cellControl != null, "cellControl != null");

            cellControl.Padding = newValue;
        }

        public Thickness Margin
        {
            get { return (Thickness)GetValue(MarginProperty); }
            set { SetValue(MarginProperty, value); }
        }

        #endregion

        #region HasExpander

        public static readonly BindableProperty HasExpanderProperty =
            BindableProperty.Create<CellControl, bool>(p => p.HasExpander, false);

        public bool HasExpander
        {
            get { return (bool)GetValue(HasExpanderProperty); }
            set { SetValue(HasExpanderProperty, value); }
        }

        #endregion

        #endregion

        public CellControl()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            
        }
    }
}
