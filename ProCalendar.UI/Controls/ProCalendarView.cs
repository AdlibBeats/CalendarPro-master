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

            UpdateDaysOfWeekContent("DaysOfWeekContent");
            UpdateContentTemplateRoot("ContentFlipView");

            UpdateNavigationButtons("PreviousButtonVertical", -1, i => i.SelectedIndex > 0);
            UpdateNavigationButtons("NextButtonVertical", 1, i => i.Items.Count - 1 > i.SelectedIndex);
        }

        private void UpdateDaysOfWeekContent(string childName)
        {
            this.DaysOfWeekContent = this.GetTemplateChild(childName) as AdaptiveGridView;
            if (this.DaysOfWeekContent == null) return;

            this.DaysOfWeekContent.ItemsSource = new ListDates().ContentDays;
        }

        private void UpdateContentTemplateRoot(string childName)
        {
            this.ContentTemplateRoot = this.GetTemplateChild(childName) as Selector;
            if (ContentTemplateRoot == null) return;
            
            this.ContentTemplateRoot.ItemsSource = new ProListDates().ListDates;

            this.ContentTemplateRoot.Loaded -= ContentTemplateRoot_Loaded;
            this.ContentTemplateRoot.Loaded += ContentTemplateRoot_Loaded;

            this.ContentTemplateRoot.SelectionChanged -= ContentTemplateRoot_SelectionChanged;
            this.ContentTemplateRoot.SelectionChanged += ContentTemplateRoot_SelectionChanged;
        }

        private void UpdateNavigationButtons(string childName, int navigatedIndex, Predicate<Selector> func)
        {
            var navigationButton = this.GetTemplateChild(childName) as ButtonBase;
            if (navigationButton == null) return;

            navigationButton.Click -= OnNavigationButtonClick;
            navigationButton.Click += OnNavigationButtonClick;

            void OnNavigationButtonClick(object sender, RoutedEventArgs e)
            {
                if (func.Invoke(ContentTemplateRoot))
                    ContentTemplateRoot.SelectedIndex += navigatedIndex;
            }
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
            if (this.ContentTemplateRoot.ItemsPanelRoot == null) return;

            for (int i = 0; i < this.ContentTemplateRoot.ItemsPanelRoot.Children.Count; i++)
            {
                var selectorItem = this.ContentTemplateRoot.ItemsPanelRoot.Children.ElementAtOrDefault(i) as SelectorItem;
                if (selectorItem == null) return;

                var adaptiveGridView = selectorItem.ContentTemplateRoot as AdaptiveGridView;
                if (adaptiveGridView == null) return;

                adaptiveGridView.SelectionChanged -= AdaptiveGridView_SelectionChanged;
                adaptiveGridView.SelectionChanged += AdaptiveGridView_SelectionChanged;
            }
        }

        private int _contentTemplateRoot_CurrentIndex = 0;
        private void ContentTemplateRoot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.ContentTemplateRoot.SelectedIndex > -1)
            {
                OnScrollingUpdateChildren();

                _contentTemplateRoot_CurrentIndex =
                    this.ContentTemplateRoot.SelectedIndex;
            }
        }

        private Panel GetItemsPanelRootFromIndex(int index)
        {
            if (this.ContentTemplateRoot.ItemsPanelRoot == null) return null;

            var selectorItem = this.ContentTemplateRoot.ItemsPanelRoot.Children.ElementAtOrDefault(index) as SelectorItem;
            if (selectorItem == null) return null;

            var adaptiveGridView = selectorItem.ContentTemplateRoot as AdaptiveGridView;
            if (adaptiveGridView == null) return null;

            var itemsPanelRoot = adaptiveGridView.ItemsPanelRoot as Panel;
            if (itemsPanelRoot == null) return null;

            return itemsPanelRoot;
        }

        private void OnScrollingUpdateChildren()
        {
            int index = _contentTemplateRoot_CurrentIndex < this.ContentTemplateRoot.SelectedIndex ? -1 : 1;
            
            var itemsPanelRoot = GetItemsPanelRootFromIndex(this.ContentTemplateRoot.SelectedIndex + index);
            if (itemsPanelRoot == null) return;

            var currentItemsPanelRoot = GetItemsPanelRootFromIndex(this.ContentTemplateRoot.SelectedIndex);
            if (currentItemsPanelRoot == null) return;

            for (int i = 0; i < itemsPanelRoot.Children.Count; i++)
            {
                var proCalendarToggleButton = itemsPanelRoot.Children.ElementAtOrDefault(i) as ProCalendarToggleButton;
                if (proCalendarToggleButton == null) return;

                if (proCalendarToggleButton.IsSelected)
                {
                    var currentProCalendarToggleButton = currentItemsPanelRoot.Children.FirstOrDefault(j =>
                    {
                        var toggleButton = j as ProCalendarToggleButton;
                        if (toggleButton == null) return false;

                        return toggleButton.Equals(proCalendarToggleButton.DateTime);
                    }) as ProCalendarToggleButton;

                    if (currentProCalendarToggleButton == null) continue;

                    currentProCalendarToggleButton.IsSelected = true;
                    proCalendarToggleButton.IsSelected = false;
                }
            }
        }

        private void LoadSelectedChildren(Predicate<ProCalendarToggleButton> func)
        {
            int index = 0;

            for (int i = 0; i < this.ContentTemplateRoot.ItemsPanelRoot.Children.Count; i++)
            {
                var itemsPanelRoot = GetItemsPanelRootFromIndex(i);
                if (itemsPanelRoot == null) return;

                for (int j = 0; j < itemsPanelRoot.Children.Count; j++)
                {
                    var proCalendarToggleButton = itemsPanelRoot.Children.ElementAtOrDefault(j) as ProCalendarToggleButton;
                    if (proCalendarToggleButton == null) return;

                    if (func.Invoke(proCalendarToggleButton))
                    {
                        this.ContentTemplateRoot.SelectedIndex = index;
                        if (proCalendarToggleButton.DateTime.Day > 20)
                            return;
                    }
                }
                index++;
            }
        }

        private void AdaptiveGridView_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var selectedItem = sender as ProCalendarToggleButton;
            if (selectedItem == null) return;

            this.SelectedItem = selectedItem;

            OnSelectedChangedUpdateChildren();

            OnSelectedChangedUpdateEvents();
        }

        private void OnSelectedChangedUpdateEvents()
        {
            if (this.SelectedItem.IsSelected)
                SelectionChanged?.Invoke(this.SelectedItem, null);
            else
            {
                this.SelectedItem = null;
                UnselectionChanged?.Invoke(null, null);
            }
        }

        private void OnSelectedChangedUpdateChildren()
        {
            int index = 0;

            for (int i = 0; i < this.ContentTemplateRoot.ItemsPanelRoot.Children.Count; i++)
            {
                var itemsPanelRoot = GetItemsPanelRootFromIndex(i);
                if (itemsPanelRoot == null) return;

                for (int j = 0; j < itemsPanelRoot.Children.Count; j++)
                {
                    var proCalendarToggleButton = itemsPanelRoot.Children.ElementAtOrDefault(j) as ProCalendarToggleButton;
                    if (proCalendarToggleButton == null) return;

                    switch (this.SelectionMode)
                    {
                        case ProCalendarViewSelectionMode.None:
                            {
                                UpdateNoneMode(index, proCalendarToggleButton);
                                break;
                            }
                        case ProCalendarViewSelectionMode.Single:
                            {
                                UpdateSingleMode(index, proCalendarToggleButton);
                                break;
                            }
                        case ProCalendarViewSelectionMode.Multiple:
                            {
                                UpdateMultipleMode(index, proCalendarToggleButton);
                                break;
                            }
                        case ProCalendarViewSelectionMode.Extended:
                            {
                                UpdateExtendedMode(index, proCalendarToggleButton);
                                break;
                            }
                    }
                }
                index++;
            }
        }

        private void UpdateNoneMode(int index, ProCalendarToggleButton proCalendarToggleButton)
        {
            //TODO: UpdateNoneMode();
        }

        private void UpdateSingleMode(int index, ProCalendarToggleButton proCalendarToggleButton)
        {
            if (this.SelectedItem.IsSelected && this.ContentTemplateRoot.SelectedIndex == index)
                proCalendarToggleButton.IsSelected = this.SelectedItem.Equals(proCalendarToggleButton.DateTime);
            else
                proCalendarToggleButton.IsSelected = false;
        }

        private void UpdateMultipleMode(int index, ProCalendarToggleButton proCalendarToggleButton)
        {
            //TODO: UpdateMultipleMode();
        }

        private void UpdateExtendedMode(int index, ProCalendarToggleButton proCalendarToggleButton)
        {
            //TODO: UpdateExtendedMode();
        }

        public bool IsContentTemplateRootLoaded
        {
            get { return (bool)GetValue(IsContentTemplateRootLoadedProperty); }
            private set { SetValue(IsContentTemplateRootLoadedProperty, value); }
        }

        public static readonly DependencyProperty IsContentTemplateRootLoadedProperty =
            DependencyProperty.Register("IsContentTemplateRootLoaded", typeof(bool), typeof(ProCalendarView), new PropertyMetadata(false));

        public AdaptiveGridView DaysOfWeekContent
        {
            get { return (AdaptiveGridView)GetValue(DaysOfWeekContentProperty); }
            private set { SetValue(DaysOfWeekContentProperty, value); }
        }

        public static readonly DependencyProperty DaysOfWeekContentProperty =
            DependencyProperty.Register("DaysOfWeekContent", typeof(AdaptiveGridView), typeof(ProCalendarView), new PropertyMetadata(null));

        public Selector ContentTemplateRoot
        {
            get { return (Selector)GetValue(ContentTemplateRootProperty); }
            private set { SetValue(ContentTemplateRootProperty, value); }
        }

        public static readonly DependencyProperty ContentTemplateRootProperty =
            DependencyProperty.Register("ContentTemplateRoot", typeof(Selector), typeof(ProCalendarView), new PropertyMetadata(null));

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
            DependencyProperty.Register("SelectionMode", typeof(ProCalendarViewSelectionMode), typeof(ProCalendarView), new PropertyMetadata(ProCalendarViewSelectionMode.Single, OnSelectionModeChanged));

        private static void OnSelectionModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var proCalendarView = d as ProCalendarView;
            if (proCalendarView == null) return;

            //TODO: UpdateSelectionMode(newValue);
        }
    }
}
