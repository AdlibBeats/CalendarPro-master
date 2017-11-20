using ProCalendar.Core.BaseListDates;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

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

    public class ProCalendarToggleButton : ContentControl
    {
        private DateTimeModel _model;

        public event RoutedEventHandler Checked;

        public ProCalendarToggleButton()
        {
            this.DefaultStyleKey = typeof(ProCalendarToggleButton);

            this.PointerPressed += OnPointerPressed;
            this.PointerReleased += OnPointerReleased;
            this.PointerEntered += OnPointerEntered;
            this.PointerExited += OnPointerExited;

            this.DataContextChanged += OnDataContextChanged;

            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            UpdateStates();
        }

        private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            _model = args.NewValue as DateTimeModel;
            if (_model == null) return;

            _model.PropertyChanged += (s, e) =>
            {
                UpdateStates();
            };
        }
        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, _model.IsSelected ? "CheckedPressed" : "Pressed", true);

            _model.IsSelected = !_model.IsSelected;

            Checked?.Invoke(this, new CalendarToggleButtonEventArgs(_model.IsSelected, _model));
        }

        private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, _model.IsSelected ? "CheckedPointerOver" : "PointerOver", true);
        }

        private void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, _model.IsSelected ? "CheckedNormal" : "Normal", true);
        }

        private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, _model.IsSelected ? "CheckedNormal" : "Normal", true);
        }

        private void UpdateStates()
        {
            if (_model.IsSelected)
                UpdateSelectedStates("Checked");
            else
                UpdateSelectedStates();

            VisualStateManager.GoToState(this, _model.IsToday ? "IsToodayTrue" : "IsToodayFalse", true);
            VisualStateManager.GoToState(this, _model.IsWeekend ? "IsWeekendTrue" : "IsWeekendFalse", true);
        }

        private void UpdateSelectedStates(string stateName = null)
        {
            if (_model.IsDisabled)
            {
                this.IsEnabled = false;
                stateName += "Disabled";
            }
            else if (_model.IsBlackout)
            {
                this.IsEnabled = false;
                stateName += "Blackouted";
            }
            else
            {
                this.IsEnabled = true;
                stateName += "Normal";
            }
            VisualStateManager.GoToState(this, stateName, true);
        }
    }
}
