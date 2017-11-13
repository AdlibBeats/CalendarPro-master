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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI;

namespace ProCalendar.UI.Controls
{
    public class ProCalendar : Control
    {
        public event RoutedEventHandler SelectionChanged;

        public ProCalendar()
        {
            this.DefaultStyleKey = typeof(ProCalendar);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var rootGrid = this.GetTemplateChild("Root") as Grid;
            if (rootGrid == null) return;

            Button b = this.GetTemplateChild("lol") as Button;
            if (b == null) return;

            b.Click += (s, e) =>
            {
                var c = this.GetTemplateChild("haha") as Grid;
                c.Visibility = Visibility.Visible;
                c.UpdateLayout();
            };

            this.ContentTemplateRoot = this.GetTemplateChild("ContentFlipView") as FlipView;
            if (ContentTemplateRoot == null) return;

            this.ContentTemplateRoot.Loaded += (sender, args) =>
            {
                this.ItemsPanelRoot = this.ContentTemplateRoot.ItemsPanelRoot as StackPanel;
                if (this.ItemsPanelRoot == null) return;

                this.Children = new List<AdaptiveGridView>();

                for (int i = 0; i < this.ItemsPanelRoot.Children.Count; i++)
                {
                    var flipViewItem = this.ItemsPanelRoot.Children.ElementAt(i) as FlipViewItem;
                    if (flipViewItem == null) return;

                    var adaptiveGridView = flipViewItem.ContentTemplateRoot as AdaptiveGridView;
                    if (adaptiveGridView == null) return;

                    adaptiveGridView.SelectionChanged += AdaptiveGridView_SelectionChanged;

                    this.Children.Add(adaptiveGridView);
                }

                UpdateSelectedItem();
            };

            this.ContentTemplateRoot.SelectionChanged += (s, e) =>
            {
                //this.Children.ForEach((AdaptiveGridView adaptiveGridView) =>
                //{
                //    adaptiveGridView.Children.ForEach((CalendarToggleButton calendarToggleButton) =>
                //    {
                //        if (calendarToggleButton.IsChecked)
                //        {
                //            Storyboard sb = new Storyboard()
                //            {
                //                FillBehavior = FillBehavior.HoldEnd,
                //                RepeatBehavior = new RepeatBehavior(1)
                //            };

                //            ColorAnimation ca1 = new ColorAnimation()
                //            {
                //                Duration = new Duration(TimeSpan.FromSeconds(1)),
                //                From = Colors.White,
                //                To = Colors.Red
                //            };

                //            ColorAnimation ca2 = new ColorAnimation()
                //            {
                //                Duration = new Duration(TimeSpan.FromSeconds(1)),
                //                From = Colors.White,
                //                To = Colors.Red
                //            };

                //            Storyboard.SetTargetProperty(ca1, "(Control.Background).(SolidColorBrush.Color)");
                //            Storyboard.SetTarget(ca1, calendarToggleButton);
                //            sb.Children.Add(ca1);
                //            Storyboard.SetTargetProperty(ca2, "(Control.BorderBrush).(SolidColorBrush.Color)");
                //            Storyboard.SetTarget(ca2, calendarToggleButton);
                //            sb.Children.Add(ca2);

                //            sb.Begin();
                //        }
                //    });
                //});
            };

            var previousButtonVertical = this.GetTemplateChild("PreviousButtonVertical") as Button;
            if (previousButtonVertical == null) return;

            var nextButtonVertical = this.GetTemplateChild("NextButtonVertical") as Button;
            if (nextButtonVertical == null) return;

            previousButtonVertical.Click += (s, e) =>
            {
                if (ContentTemplateRoot.SelectedIndex > 0)
                    ContentTemplateRoot.SelectedIndex--;
            };

            nextButtonVertical.Click += (s, e) =>
            {
                if (ContentTemplateRoot.Items.Count - 1 > ContentTemplateRoot.SelectedIndex)
                    ContentTemplateRoot.SelectedIndex++;
            };
        }

        private void AdaptiveGridView_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var selectedItemEventArgs = e as SelectedItemEventArgs;
            if (selectedItemEventArgs == null) return;

            var selectedItem = selectedItemEventArgs.SelectedItem;
            if (selectedItem == null) return;

            this.SelectedItem = selectedItem;

            var selectedDateTimeModel = selectedItem.DataContext as DateTimeModel;
            if (selectedDateTimeModel == null) return;

            this.SelectedDateTimeModel = selectedDateTimeModel;

            this.Children.ForEach((AdaptiveGridView adaptiveGridView) =>
            {
                adaptiveGridView.Children.ForEach((CalendarToggleButton calendarToggleButton) =>
                {
                    switch (this.SelectionMode)
                    {
                        case AdaptiveGridViewSelectionMode.None:
                            {
                                break;
                            }
                        case AdaptiveGridViewSelectionMode.Single:
                            {
                                var data = calendarToggleButton.DataContext as DateTimeModel;
                                if (data == null) return;

                                if (selectedItem.IsChecked && !selectedDateTimeModel.Equals(data.DateTime))
                                    calendarToggleButton.IsChecked = false;
                                else if (selectedItem.IsChecked && selectedDateTimeModel.Equals(data.DateTime))
                                    calendarToggleButton.IsChecked = true;
                                else if (!selectedItem.IsChecked && selectedDateTimeModel.Equals(data.DateTime))
                                    calendarToggleButton.IsChecked = false;
                                break;
                            }
                        case AdaptiveGridViewSelectionMode.Multiple:
                            {
                                //TODO: List<SelectedItem> ...
                                break;
                            }
                        case AdaptiveGridViewSelectionMode.Extended:
                            {
                                break;
                            }
                    }
                });
            });

            //this.SelectedItem = selectedItem;

            if (selectedItem.IsChecked)
                SelectionChanged?.Invoke(selectedItem, new SelectedItemEventArgs(selectedItem, selectedDateTimeModel));
        }

        private void UpdateSelectedItem()
        {
            var todayDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            var data = this.ContentTemplateRoot.DataContext as ProListDates;
            if (data == null) return;

            int index = 0;

            foreach (var x in data.ListDates)
            {
                foreach (var y in x.ContentDays)
                    if (y.Equals(todayDateTime))
                        this.ContentTemplateRoot.SelectedIndex = index;

                index++;
            }
        }

        public FlipView ContentTemplateRoot
        {
            get { return (FlipView)GetValue(ContentTemplateRootProperty); }
            private set { SetValue(ContentTemplateRootProperty, value); }
        }

        public static readonly DependencyProperty ContentTemplateRootProperty =
            DependencyProperty.Register("ContentTemplateRoot", typeof(FlipView), typeof(ProCalendar), new PropertyMetadata(null));

        public StackPanel ItemsPanelRoot
        {
            get { return (StackPanel)GetValue(ItemsPanelRootProperty); }
            private set { SetValue(ItemsPanelRootProperty, value); }
        }

        public static readonly DependencyProperty ItemsPanelRootProperty =
            DependencyProperty.Register("ItemsPanelRoot", typeof(StackPanel), typeof(ProCalendar), new PropertyMetadata(null));

        public DateTimeModel SelectedDateTimeModel
        {
            get { return (DateTimeModel)GetValue(SelectedDateTimeModelProperty); }
            set { SetValue(SelectedDateTimeModelProperty, value); }
        }

        public static readonly DependencyProperty SelectedDateTimeModelProperty =
            DependencyProperty.Register("SelectedDateTimeModel", typeof(DateTimeModel), typeof(ProCalendar), new PropertyMetadata(null));

        public CalendarToggleButton SelectedItem
        {
            get { return (CalendarToggleButton)GetValue(SelectedItemProperty); }
            private set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(CalendarToggleButton), typeof(ProCalendar), new PropertyMetadata(null));

        public AdaptiveGridViewSelectionMode SelectionMode
        {
            get { return (AdaptiveGridViewSelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }

        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register("SelectionMode", typeof(AdaptiveGridViewSelectionMode), typeof(ProCalendar), new PropertyMetadata(AdaptiveGridViewSelectionMode.Single));

        public List<AdaptiveGridView> Children
        {
            get { return (List<AdaptiveGridView>)GetValue(ChildrenProperty); }
            private set { SetValue(ChildrenProperty, value); }
        }

        public static readonly DependencyProperty ChildrenProperty =
            DependencyProperty.Register("Children", typeof(List<AdaptiveGridView>), typeof(ProCalendar), new PropertyMetadata(null));
    }
}
