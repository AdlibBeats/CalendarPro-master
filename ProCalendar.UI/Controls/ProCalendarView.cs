﻿using System;
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

            this.ContentTemplateRoot = this.GetTemplateChild("ContentFlipView") as FlipView;
            if (ContentTemplateRoot == null) return;

            this.ContentTemplateRoot.Loaded += ContentTemplateRoot_Loaded;
            //this.ContentTemplateRoot.SelectionChanged += ContentTemplateRoot_SelectionChanged;

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
                UpdateSelectedItem(i => i.IsSelected);
            else
                UpdateSelectedItem(i => i.IsToday);
        }

        private void UpdateSelectedItem(Predicate<DateTimeModel> func)
        {
            var todayDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            var data = this.ContentTemplateRoot.DataContext as ProListDates;
            if (data == null) return;

            int index = 0;

            foreach (var x in data.ListDates)
            {
                foreach (var y in x.ContentDays)
                    if (func.Invoke(y))
                    {
                        this.ContentTemplateRoot.SelectedIndex = index;
                        if (y.DateTime.Day > 20)
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
            this.Children.ForEach((AdaptiveGridView adaptiveGridView) =>
            {
                adaptiveGridView.Children.ForEach((CalendarToggleButton calendarToggleButton) =>
                {
                    switch (this.SelectionMode)
                    {
                        case ProCalendarViewSelectionMode.None:
                            {
                                UpdateNoneMode(calendarToggleButton);
                                break;
                            }
                        case ProCalendarViewSelectionMode.Single:
                            {
                                UpdateSingleMode(calendarToggleButton);
                                break;
                            }
                        case ProCalendarViewSelectionMode.Multiple:
                            {
                                UpdateMultipleMode(calendarToggleButton);
                                break;
                            }
                        case ProCalendarViewSelectionMode.Extended:
                            {
                                UpdateExtendedMode(calendarToggleButton);
                                break;
                            }
                    }
                });
            });
        }

        private void UpdateNoneMode(CalendarToggleButton calendarToggleButton)
        {
            //TODO:
        }

        private void UpdateSingleMode(CalendarToggleButton calendarToggleButton)
        {
            var data = calendarToggleButton.DataContext as DateTimeModel;
            if (data == null) return;

            if (this.SelectedDateTimeModel.IsSelected)
            {
                if (!this.SelectedDateTimeModel.Equals(data.DateTime))
                    data.IsSelected = false;
                else
                    data.IsSelected = true;
            }
            else if (this.SelectedDateTimeModel.Equals(data.DateTime))
                data.IsSelected = false;
        }

        private void UpdateMultipleMode(CalendarToggleButton calendarToggleButton)
        {
            //TODO:
        }

        private void UpdateExtendedMode(CalendarToggleButton calendarToggleButton)
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
