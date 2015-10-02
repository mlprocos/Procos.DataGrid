﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace Procos.DataGrid
{
    public class Row : BindableObject
    {
        public ICommand ExpandCommand
        {
            get
            {
                return new Command(() => IsExpanded = !IsExpanded);
            }
        }

        #region ActualHeight

        public static readonly BindableProperty ActualHeightProperty =
            BindableProperty.Create<Row, double>(
                p => p.ActualHeight, 80, BindingMode.Default, ValidateActualHeight);

        private static bool ValidateActualHeight(BindableObject bindable, double value)
        {
            // TODO are this all invalid values?
            return value >= 0;
        }

        public double ActualHeight
        {
            get { return (double) GetValue(ActualHeightProperty); }
            set { SetValue(ActualHeightProperty, value); }
        }

        #endregion

        #region Height

        public static readonly BindableProperty HeightProperty =
            BindableProperty.Create<Row, double>(
                p => p.Height, 80, BindingMode.Default, ValidateHeight);

        private static bool ValidateHeight(BindableObject bindable, double value)
        {
            // TODO are this all invalid values?
            return value >= 0;
        }

        public double Height
        {
            get { return (double) GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        #endregion

        #region IsVisible

        public static readonly BindableProperty IsVisibleProperty =
            BindableProperty.Create<Row, bool>(
                p => p.IsVisible, true);

        public bool IsVisible
        {
            get { return (bool) GetValue(IsVisibleProperty); }
            set { SetValue(IsVisibleProperty, value); }
        }

        #endregion

        #region Children

        public static readonly BindableProperty ChildrenProperty =
            BindableProperty.Create<Row, ObservableCollection<Row>>(
                p => p.Children, new ObservableCollection<Row>());

        public ObservableCollection<Row> Children
        {
            get { return (ObservableCollection<Row>) GetValue(ChildrenProperty); }
            set
            {
                SetValue(ChildrenProperty, value);
                Children.CollectionChanged += ChildrenOnCollectionChanged;
                OnPropertyChanged("HasVisibleChildren");
            }
        }

        private void ChildrenOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyArgs)
        {
            switch (notifyArgs.Action)
            {
                default:
                    OnPropertyChanged("Children");
                    OnPropertyChanged("HasVisibleChildren");
                    break;
            }
        }

        #endregion

        #region HasVisibleChildren

        public bool HasVisibleChildren
        {
            get
            {
                return Children != null
                       && Children.Any(row => row.IsVisible);
            }
        }

        #endregion

        #region IsExpanded

        public static readonly BindableProperty IsExpandedProperty =
            BindableProperty.Create<Row, bool>(
                p => p.IsExpanded, true);

        public bool IsExpanded
        {
            get { return (bool) GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        #endregion

        private readonly Dictionary<Column, Cell> _cellDictionary = new Dictionary<Column, Cell>();

        public Cell this[Column col]
        {
            get { return _cellDictionary[col]; }
            set { _cellDictionary[col] = value; }
        }


        public List<Row> AllVisibleChildRows
        {
            get
            {
                var allVisibleChildRows = new List<Row>();

                foreach (var row in Children)
                    if (row.IsVisible)
                    {
                        allVisibleChildRows.Add(row);

                        if (row.IsExpanded)
                            allVisibleChildRows.AddRange(row.AllVisibleChildRows);
                    }

                return allVisibleChildRows;
            }
        }
    }
}