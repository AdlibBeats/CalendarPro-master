using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using ProCalendar.Core.ListDates;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using System.Globalization;
using Windows.Foundation;
using System.Diagnostics;
using ProCalendar.Core.BaseListDates;

namespace ProCalendar.UI.Controls
{
    public class ProCalendar : Control
    {
        private Grid _rootGrid;
        private AdaptiveGridView _contentGrid;
        public ProCalendar()
        {
            this.DefaultStyleKey = typeof(ProCalendar);
        }

        public object ItemsSource
        {
            get { return (object)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.RegisterAttached("ItemsSource", typeof(object), typeof(AdaptiveGridView), new PropertyMetadata(new ObservableCollection<ListDates>()
            {
                new ListDates(new DateTimeModel()
                {
                    DateTime = new DateTime(2017, 1, 1)
                }),
                new ListDates(new DateTimeModel()
                {
                    DateTime = new DateTime(2017, 2, 1)
                }),
                new ListDates(new DateTimeModel()
                {
                    DateTime = new DateTime(2017, 3, 1)
                }),
                new ListDates(new DateTimeModel()
                {
                    DateTime = new DateTime(2017, 4, 1)
                }),
                new ListDates(new DateTimeModel()
                {
                    DateTime = new DateTime(2017, 5, 1)
                }),
                new ListDates(new DateTimeModel()
                {
                    DateTime = new DateTime(2017, 6, 1)
                }),
                new ListDates(new DateTimeModel()
                {
                    DateTime = new DateTime(2017, 7, 1)
                }),
                new ListDates(new DateTimeModel()
                {
                    DateTime = new DateTime(2017, 8, 1)
                }),
                new ListDates(new DateTimeModel()
                {
                    DateTime = new DateTime(2017, 9, 1)
                }),
                new ListDates(new DateTimeModel()
                {
                    DateTime = new DateTime(2017, 10, 1)
                }),
                new ListDates(new DateTimeModel()
                {
                    DateTime = new DateTime(2017, 11, 1)
                }),
                new ListDates(new DateTimeModel()
                {
                    DateTime = new DateTime(2017, 12, 1)
                }),
            }));

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _rootGrid = this.GetTemplateChild("Root") as Grid;

            _contentGrid = this.GetTemplateChild("ContentRoot") as AdaptiveGridView;
            if (_contentGrid == null) return;

            _contentGrid.SelectionChanged += (sender, e) =>
            {
                var ev = e as SelectedItemEventArgs;
                if (ev == null) return;

                var dateTimeModel = ev.SelectedItem.DataContext as DateTimeModel;
                if (dateTimeModel == null) return;

                var button1 = this.GetTemplateChild("lol1") as Button;

                var button2 = this.GetTemplateChild("lol2") as Button;

                var superList = this.GetTemplateChild("superList") as ListBox;

                if (button1 != null && button2 != null && superList != null)
                {
                    button1.Click += (sender1, e1) =>
                    {
                        int currentIndex = superList.SelectedIndex;

                        if (currentIndex != 0)
                        {
                            superList.ScrollIntoView(superList.Items.ElementAt(currentIndex - 1));
                        }
                    };
                    
                    button2.Click += (sender1, e1) =>
                    {
                        superList.SelectedIndex++;
                    };
                }

                Debug.WriteLine(dateTimeModel.DateTime);
            };
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return base.ArrangeOverride(finalSize);
        }
    }
}
