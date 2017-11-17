using ProCalendar.Core.BaseListDates;
using ProCalendar.Core.ListDates;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace ProCalendar.UI.Controls
{
    public enum ProCalendarViewSelectionMode
    {
        None,
        Single,
        Multiple,
        Extended
    }

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
            this.ContentTemplateRoot = this.GetTemplateChild(childName) as Selector;
            if (ContentTemplateRoot == null) return;

            this.ContentTemplateRoot.Loaded += ContentTemplateRoot_Loaded;
            this.ContentTemplateRoot.SelectionChanged += ContentTemplateRoot_SelectionChanged;
        }

        private void UpdateNavigationButtons(string childName, int navigatedIndex, Predicate<Selector> func)
        {
            var navigationButton = this.GetTemplateChild(childName) as Button;
            if (navigationButton == null) return;

            navigationButton.Click += (s, e) =>
            {
                if (func.Invoke(ContentTemplateRoot))
                    ContentTemplateRoot.SelectedIndex += navigatedIndex;
            };
        }

        private void ContentTemplateRoot_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this.IsContentTemplateRootLoaded)
                OnLoadingUpdateChildren();

            this.IsContentTemplateRootLoaded = true;

            if (this.SelectedItem != null)
                LoadSelectedChildren(i => i.IsSelected);
            else
                LoadSelectedChildren(i => i.IsToday);
        }

        private void OnLoadingUpdateChildren()
        {
            this.ItemsPanelRoot = this.ContentTemplateRoot.ItemsPanelRoot as StackPanel;
            if (this.ItemsPanelRoot == null) return;

            this.Children = new List<AdaptiveGridView>();

            for (int i = 0; i < this.ItemsPanelRoot.Children.Count; i++)
            {
                var selectorItem = this.ItemsPanelRoot.Children.ElementAtOrDefault(i) as SelectorItem;
                if (selectorItem == null) return;

                var adaptiveGridView = selectorItem.ContentTemplateRoot as AdaptiveGridView;
                if (adaptiveGridView == null) return;

                adaptiveGridView.SelectionChanged += AdaptiveGridView_SelectionChanged;

                this.Children.Add(adaptiveGridView);
            }
        }

        private int _contentTemplateRoot_CurrentIndex = 0;
        private void ContentTemplateRoot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.ContentTemplateRoot.SelectedIndex > -1)
            {
                OnScrollingUpdateChildren(i => _contentTemplateRoot_CurrentIndex < i.SelectedIndex);

                _contentTemplateRoot_CurrentIndex =
                    this.ContentTemplateRoot.SelectedIndex;
            }
        }

        private void OnScrollingUpdateChildren(Predicate<Selector> func)
        {
            int index = func.Invoke(this.ContentTemplateRoot) ? -1 : 1;

            var itemsSource = this.ContentTemplateRoot.ItemsSource as ObservableCollection<ListDates>;
            if (itemsSource == null) return;

            var listDates = itemsSource.ElementAtOrDefault(this.ContentTemplateRoot.SelectedIndex + index);
            if (listDates == null) return;

            var currentListDates = itemsSource.ElementAtOrDefault(this.ContentTemplateRoot.SelectedIndex);
            if (currentListDates == null) return;

            foreach (var dayTimeModel in listDates.ContentDays)
            {
                if (dayTimeModel.IsSelected)
                {
                    var currentDateTimeModel = currentListDates.ContentDays.FirstOrDefault(i => i.Equals(dayTimeModel.DateTime));
                    if (currentDateTimeModel == null) continue;

                    currentDateTimeModel.IsSelected = true;
                    dayTimeModel.IsSelected = false;
                }
            }
        }

        private void LoadSelectedChildren(Predicate<DateTimeModel> func)
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
            var args = e as SelectedItemEventArgs;
            if (args == null) return;

            var selectedItem = args.SelectedItem;
            if (selectedItem == null) return;

            this.SelectedItem = selectedItem;

            var selectedDateTimeModel = selectedItem.DataContext as DateTimeModel;
            if (selectedDateTimeModel == null) return;

            this.SelectedDateTimeModel = selectedDateTimeModel;

            OnSelectedChangedUpdateChildren();

            OnSelectedChangedUpdateEvents();
        }

        private void OnSelectedChangedUpdateEvents()
        {
            if (this.SelectedDateTimeModel.IsSelected)
                SelectionChanged?.Invoke(this.SelectedItem, new SelectedItemEventArgs(this.SelectedItem, this.SelectedDateTimeModel));
            else
            {
                this.SelectedItem = null;
                this.SelectedDateTimeModel = null;

                UnselectionChanged?.Invoke(this.SelectedItem, new SelectedItemEventArgs());
            }
        }

        private void OnSelectedChangedUpdateChildren()
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

        private void UpdateNoneMode(int index, DateTimeModel dayTimeModel)
        {
            //TODO:
        }

        private void UpdateSingleMode(int index, DateTimeModel dateTimeModel)
        {
            if (this.SelectedDateTimeModel.IsSelected && this.ContentTemplateRoot.SelectedIndex == index)
                dateTimeModel.IsSelected = this.SelectedDateTimeModel.Equals(dateTimeModel.DateTime);
            else
                dateTimeModel.IsSelected = false;
        }

        private void UpdateMultipleMode(int index, DateTimeModel dayTimeModel)
        {
            //TODO:
        }

        private void UpdateExtendedMode(int index, DateTimeModel dayTimeModel)
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

        public Selector ContentTemplateRoot
        {
            get { return (Selector)GetValue(ContentTemplateRootProperty); }
            private set { SetValue(ContentTemplateRootProperty, value); }
        }

        public static readonly DependencyProperty ContentTemplateRootProperty =
            DependencyProperty.Register("ContentTemplateRoot", typeof(Selector), typeof(ProCalendarView), new PropertyMetadata(null));

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

        public ProCalendarToggleButton SelectedItem
        {
            get { return (ProCalendarToggleButton)GetValue(SelectedItemProperty); }
            private set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(ProCalendarToggleButton), typeof(ProCalendarView), new PropertyMetadata(null));

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
