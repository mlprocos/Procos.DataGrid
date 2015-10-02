using Android.Views;
using Procos.DataGrid;
using Procos.DataGrid.Droid;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
// ReSharper disable BitwiseOperatorOnEnumWithoutFlags

[assembly: ExportRenderer(typeof(DataGrid), typeof(DataGridRenderer))]

namespace Procos.DataGrid.Droid
{
    public class DataGridRenderer : VisualElementRenderer<DataGrid>
    {
        private int _mTouchSlop;
        bool mIsBeingDragged;
        private Point _mLastMotion;
        private Point _mFirstMotion;
        private Point _mBegan;

        #region Constructor

        public DataGridRenderer()
        {
            ViewConfiguration configuration = ViewConfiguration.Get(Context);
            _mTouchSlop = configuration.ScaledTouchSlop;
        }

        #endregion

        #region Events

        public override bool OnInterceptTouchEvent(MotionEvent e)
        {
            //Android.Util.Log.Debug("OnInterceptTouchEvent", "OnInterceptTouchEvent " + e.ToString());

            if ((e.Action == MotionEventActions.Move) && (mIsBeingDragged))
                return true;

            switch (e.Action & MotionEventActions.Mask)
            {
                case MotionEventActions.Move:
                    {
                        float y = e.GetY();
                        float x = e.GetX();
                        int yDiff = (int)Math.Abs(y - _mLastMotion.Y);
                        int xDiff = (int)Math.Abs(x - _mLastMotion.X);
                        if (yDiff > _mTouchSlop || xDiff > _mTouchSlop)
                        {
                            mIsBeingDragged = true;
                            _mLastMotion.X = x;
                            _mLastMotion.Y = y;
                        }
                        break;
                    }

                case MotionEventActions.Down:
                {
                    _mLastMotion = _mFirstMotion = new Point(e.GetX(), e.GetY());
                        
                        _mBegan = Element.ContentOffset;

                        break;
                    }

                case MotionEventActions.Cancel:
                case MotionEventActions.Up:
                    /* Release the drag */
                    mIsBeingDragged = false;
                    break;
            }

            /*
            * The only time we want to intercept motion events is if we are in the
            * drag mode.
            */
            return mIsBeingDragged;
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            //Android.Util.Log.Debug("OnTouchEvent", "OnTouchEvent " + e.ToString());

            switch (e.Action & MotionEventActions.Mask)
            {
                case MotionEventActions.Down:
                    {
                        // Remember where the motion event started
                        _mLastMotion.Y = e.GetY();
                        _mLastMotion.X = e.GetX();

                        _mBegan = Element.ContentOffset;

                        break;
                    }
                case MotionEventActions.Move:

                    float y = e.GetY();
                    float x = e.GetX();
                    Point delta = new Point(_mLastMotion.X - x, _mLastMotion.Y - y);
                    if (!mIsBeingDragged 
                        && (Math.Abs(delta.X) > _mTouchSlop
                            || Math.Abs(delta.Y) > _mTouchSlop))
                    {
                        mIsBeingDragged = true;
                    }

                    if (mIsBeingDragged)
                    {
                        //Actually perform the scroll
                        double tr_x = Context.FromPixels(x - _mFirstMotion.X);
                        double tr_y = Context.FromPixels(y - _mFirstMotion.Y);

                        double newx = _mBegan.X - tr_x;
                        double newy = _mBegan.Y - tr_y;

                        //Android.Util.Log.Debug("ActualScroll", "deltaX " + deltaX + " deltaY " + deltaY + " newx " + newx + " newy " + newy);
                        //Android.Util.Log.Debug("ActualScroll", "began_x " + _began_x + " tr_x " + tr_x +" began_y " + _began_y + " tr_y " + tr_y);
                        //Android.Util.Log.Debug("ActualScroll", "mLastMotionX " + mLastMotionX + " x " + x + " mLastMotionY " + mLastMotionY + " y " + y);

                        _mLastMotion = new Point(x, y);

                        Element.ContentOffset = new Point(newx, newy);
                    }
                    break;
                case MotionEventActions.Up:
                    if (mIsBeingDragged)
                    {
                        mIsBeingDragged = false;
                        _mLastMotion = Point.Zero;
                    }
                    else
                    {
                        //If they are releasing the touch, they didn't drag, and none of our children caught it, call it a tap event.
                        double touch_x = Context.FromPixels(e.GetX());
                        double touch_y = Context.FromPixels(e.GetY());

                        Element.SingleTap(touch_x, touch_y); // TODO shouldn't we use the return value of this call?
                    }
                    break;
                case MotionEventActions.Cancel:
                    if (mIsBeingDragged)
                    {
                        mIsBeingDragged = false;
                        _mLastMotion = Point.Zero;
                    }
                    break;
            }
            return true;
        }
        #endregion
    }
}