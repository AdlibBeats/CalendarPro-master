﻿using ProCalendar.Core.BaseListDates;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;

namespace ProCalendar.UI.Controls
{
    public class CalendarToggleButtonEventArgs : RoutedEventArgs
    {
        public bool IsChecked { get; }
        public DateTimeModel DateTimeModel { get; }
        public CalendarToggleButtonEventArgs(bool isChecked, DateTimeModel dateTimeModel)
        {
            IsChecked = isChecked;
            DateTimeModel = dateTimeModel;
        }
    }

    public class CalendarToggleButton : ContentControl
    {
        private DateTimeModel Model { get; set; }

        public event RoutedEventHandler Checked;

        public CalendarToggleButton()
        {
            this.DefaultStyleKey = typeof(CalendarToggleButton);

            this.PointerPressed += ContentControl_PointerPressed;
            this.PointerReleased += ContentControl_PointerReleased;
            this.PointerEntered += ContentControl_PointerEntered;
            this.PointerExited += ContentControl_PointerExited;

            this.DataContextChanged += ProCalendarItem_DataContextChanged;

            this.Loaded += CalendarToggleButton_Loaded;
        }

        private void CalendarToggleButton_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateStates();
        }

        private void ProCalendarItem_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            var contentControl = sender as CalendarToggleButton;
            if (contentControl == null) return;

            this.Model = args.NewValue as DateTimeModel;
            if (this.Model == null) return;

            this.Model.PropertyChanged += (s, e) =>
            {
                contentControl.UpdateStates();
            };
        }

        private void UpdateStates()
        {
            switch (this.Model.IsSelected)
            {
                case true:
                    {
                        if (this.Model.IsDisabled)
                        {
                            this.IsEnabled = false;
                            VisualStateManager.GoToState(this, "CheckedDisabled", true);
                        }
                        else if (this.Model.IsBlackout)
                        {
                            this.IsEnabled = false;
                            VisualStateManager.GoToState(this, "CheckedBlackouted", true);
                        }
                        else
                        {
                            this.IsEnabled = true;
                            VisualStateManager.GoToState(this, "CheckedNormal", true);
                        }
                        break;
                    }
                case false:
                    {
                        if (this.Model.IsDisabled)
                        {
                            this.IsEnabled = false;
                            VisualStateManager.GoToState(this, "Disabled", true);
                        }
                        else if (this.Model.IsBlackout)
                        {
                            this.IsEnabled = false;
                            VisualStateManager.GoToState(this, "Blackouted", true);
                        }
                        else
                        {
                            this.IsEnabled = true;
                            VisualStateManager.GoToState(this, "Normal", true);
                        }
                        break;
                    }
            }

            VisualStateManager.GoToState(this, this.Model.IsToday ? "IsToodayTrue" : "IsToodayFalse", true);
            VisualStateManager.GoToState(this, this.Model.IsWeekend ? "IsWeekendTrue" : "IsWeekendFalse", true);
        }

        private void ContentControl_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, Model.IsSelected ? "CheckedPressed" : "Pressed", true);

            Model.IsSelected = !Model.IsSelected;

            Checked?.Invoke(this, new CalendarToggleButtonEventArgs(this.Model.IsSelected, this.Model));
        }

        private void ContentControl_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, Model.IsSelected ? "CheckedPointerOver" : "PointerOver", true);
        }

        private void ContentControl_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, Model.IsSelected ? "CheckedNormal" : "Normal", true);
        }

        private void ContentControl_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, Model.IsSelected ? "CheckedNormal" : "Normal", true);
        }
    }
}
