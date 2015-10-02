using System;
using Xamarin.Forms;

namespace Procos.DataGrid.Views
{
    class DataGridLayout : Layout<View>
    {

        public GridModel GridModel { get; set; }

        private CellRange ViewRange { get; set; }

        private CellRange GridRange { get; set; }


        public Func<View, Rectangle> GetBox { get; set; }

        public DataGridLayout()
        {
        }

        public void LayoutOneChild(View v)
        {
            Rectangle r = GetBox(v);
            v.Layout(r);
        }

        public void LayoutAllChildren()
        {
            foreach (View v in Children)
            {
                LayoutOneChild(v);
            }
        }

        protected override bool ShouldInvalidateOnChildAdded(View child)
        {
            return false;
        }

        protected override bool ShouldInvalidateOnChildRemoved(View child)
        {
            return false;
        }

        public bool CallOnChildMeasureInvalidated { get; set; } = false;
        protected override void OnChildMeasureInvalidated()
        {
            if (CallOnChildMeasureInvalidated)
                base.OnChildMeasureInvalidated();
        }

        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            // TODO consider a flag here to suspend/resume.
            // maybe implement this by requiring all children insertions to be done
            // through a method?
            LayoutAllChildren();
        }
    }

}
