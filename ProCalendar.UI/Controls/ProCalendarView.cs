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
    public class ProCalendarView : Control
    {
        public event RoutedEventHandler SelectionChanged;
        public event RoutedEventHandler UnselectionChanged;

        public ProCalendarView()
        {
            this.DefaultStyleKey = typeof(ProCalendarView);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            UpdateContentTemplateRoot("ContentFlipView");

            UpdateNavigationButtons("PreviousButtonVertical", -1, i => i.SelectedIndex > 0);
            UpdateNavigationButtons("NextButtonVertical", 1, i => i.Items.Count - 1 > i.SelectedIndex);
        }

        private void UpdateContentTemplateRoot(string childName)
        {
            this.ContentTemplateRoot = this.GetTemplateChild(childName) as FlipView;
            if (ContentTemplateRoot == null) return;

            this.ContentTemplateRoot.Loaded += ContentTemplateRoot_Loaded;
            this.ContentTemplateRoot.SelectionChanged += ContentTemplateRoot_SelectionChanged;
        }

        private void UpdateNavigationButtons(string childName, int navigationIndex, Predicate<FlipView> func)
        {
            var navigationButton = this.GetTemplateChild(childName) as Button;
            if (navigationButton == null) return;

            navigationButton.Click += (s, e) =>
            {
                if (func.Invoke(ContentTemplateRoot))
                    ContentTemplateRoot.SelectedIndex += navigationIndex;
            };
        }

        private void ContentTemplateRoot_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this.IsContentTemplateRootLoaded)
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
            }

            this.IsContentTemplateRootLoaded = true;

            if (this.SelectedItem != null)
                LoadSelectedItems(i => i.IsSelected);
            else
                LoadSelectedItems(i => i.IsToday);
        }

        private int _contentTemplateRoot_CurrentIndex = 0;
        private void ContentTemplateRoot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.ContentTemplateRoot.SelectedIndex > -1)
            {
                var itemsSource = this.ContentTemplateRoot.ItemsSource as ObservableCollection<ListDates>;
                if (itemsSource == null) return;

                if (_contentTemplateRoot_CurrentIndex < this.ContentTemplateRoot.SelectedIndex)
                    UpdateSelectedItems(itemsSource, this.ContentTemplateRoot.SelectedIndex - 1);
                else
                    UpdateSelectedItems(itemsSource, this.ContentTemplateRoot.SelectedIndex + 1);

                _contentTemplateRoot_CurrentIndex =
                    this.ContentTemplateRoot.SelectedIndex;
            }
        }

        private void UpdateSelectedItems(ObservableCollection<ListDates> itemsSource, int index)
        {
            var listDates = itemsSource.ElementAt(index);
            if (listDates == null) return;

            foreach (var day in listDates.ContentDays)
            {
                if (day.IsSelected)
                {
                    var currentListDates = itemsSource.ElementAt(this.ContentTemplateRoot.SelectedIndex);

                    var currentDateTimeModel = currentListDates.ContentDays.FirstOrDefault(i => i.Equals(day.DateTime));
                    if (currentDateTimeModel == null) continue;

                    currentDateTimeModel.IsSelected = true;
                    day.IsSelected = false;
                }
            }
        }

        private void LoadSelectedItems(Predicate<DateTimeModel> func)
        {
            var itemsSource = ContentTemplateRoot.ItemsSource as ObservableCollection<ListDates>;
            if (itemsSource == null) return;

            int index = 0;

            foreach (var listDates in itemsSource)
            {
                foreach (var dayTimeModel in listDates.ContentDays)
                    if (func.Invoke(dayTimeModel))
                    {
                        this.ContentTemplateRoot.SelectedIndex = index;
                        if (dayTimeModel.DateTime.Day > 20)
                            return;
                    }
                index++;
            }
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

            UpdateChildren();

            UpdateSelectionChangedEvents();
        }

        private void UpdateSelectionChangedEvents()
        {
            if (this.SelectedDateTimeModel.IsSelected)
                SelectionChanged?.Invoke(this.SelectedItem, new SelectedItemEventArgs(this.SelectedItem, this.SelectedDateTimeModel));
            else
            {
                this.SelectedItem = null;
                this.SelectedDateTimeModel = null;

                UnselectionChanged?.Invoke(null, new SelectedItemEventArgs(null, null));
            }
        }

        private void UpdateChildren()
        {
            var itemsSource = ContentTemplateRoot.ItemsSource as ObservableCollection<ListDates>;
            if (itemsSource == null) return;

            int index = 0;

            foreach (var listDates in itemsSource)
            {
                foreach (var dayTimeModel in listDates.ContentDays)
                {
                    switch (this.SelectionMode)
                    {
                        case ProCalendarViewSelectionMode.None:
                            {
                                UpdateNoneMode(index, dayTimeModel);
                                break;
                            }
                        case ProCalendarViewSelectionMode.Single:
                            {
                                UpdateSingleMode(index, dayTimeModel);
                                break;
                            }
                        case ProCalendarViewSelectionMode.Multiple:
                            {
                                UpdateMultipleMode(index, dayTimeModel);
                                break;
                            }
                        case ProCalendarViewSelectionMode.Extended:
                            {
                                UpdateExtendedMode(index, dayTimeModel);
                                break;
                            }
                    }
                }
                index++;
            }
        }

        private void UpdateNoneMode(int index, DateTimeModel day)
        {
            //TODO:
        }

        private void UpdateSingleMode(int index, DateTimeModel day)
        {
            if (this.SelectedDateTimeModel.IsSelected && this.ContentTemplateRoot.SelectedIndex == index)
            {
                if (!this.SelectedDateTimeModel.Equals(day.DateTime))
                    day.IsSelected = false;
                else
                    day.IsSelected = true;
            }
            else
                day.IsSelected = false;
        }

        private void UpdateMultipleMode(int index, DateTimeModel day)
        {
            //TODO:
        }

        private void UpdateExtendedMode(int index, DateTimeModel day)
        {
            //TODO:
        }

        public bool IsContentTemplateRootLoaded
        {
            get { return (bool)GetValue(IsContentTemplateRootLoadedProperty); }
            private set { SetValue(IsContentTemplateRootLoadedProperty, value); }
        }

        public static readonly DependencyProperty IsContentTemplateRootLoadedProperty =
            DependencyProperty.Register("IsContentTemplateRootLoaded", typeof(bool), typeof(ProCalendarView), new PropertyMetadata(false));

        public FlipView ContentTemplateRoot
        {
            get { return (FlipView)GetValue(ContentTemplateRootProperty); }
            private set { SetValue(ContentTemplateRootProperty, value); }
        }

        public static readonly DependencyProperty ContentTemplateRootProperty =
            DependencyProperty.Register("ContentTemplateRoot", typeof(FlipView), typeof(ProCalendarView), new PropertyMetadata(null));

        public StackPanel ItemsPanelRoot
        {
            get { return (StackPanel)GetValue(ItemsPanelRootProperty); }
            private set { SetValue(ItemsPanelRootProperty, value); }
        }

        public static readonly DependencyProperty ItemsPanelRootProperty =
            DependencyProperty.Register("ItemsPanelRoot", typeof(StackPanel), typeof(ProCalendarView), new PropertyMetadata(null));

        public DateTimeModel SelectedDateTimeModel
        {
            get { return (DateTimeModel)GetValue(SelectedDateTimeModelProperty); }
            set { SetValue(SelectedDateTimeModelProperty, value); }
        }

        public static readonly DependencyProperty SelectedDateTimeModelProperty =
            DependencyProperty.Register("SelectedDateTimeModel", typeof(DateTimeModel), typeof(ProCalendarView), new PropertyMetadata(null));

        public CalendarToggleButton SelectedItem
        {
            get { return (CalendarToggleButton)GetValue(SelectedItemProperty); }
            private set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(CalendarToggleButton), typeof(ProCalendarView), new PropertyMetadata(null));

        public ProCalendarViewSelectionMode SelectionMode
        {
            get { return (ProCalendarViewSelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }

        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register("SelectionMode", typeof(ProCalendarViewSelectionMode), typeof(ProCalendarView), new PropertyMetadata(ProCalendarViewSelectionMode.Single));

        public List<AdaptiveGridView> Children
        {
            get { return (List<AdaptiveGridView>)GetValue(ChildrenProperty); }
            private set { SetValue(ChildrenProperty, value); }
        }

        public static readonly DependencyProperty ChildrenProperty =
            DependencyProperty.Register("Children", typeof(List<AdaptiveGridView>), typeof(ProCalendarView), new PropertyMetadata(null));
    }
}
