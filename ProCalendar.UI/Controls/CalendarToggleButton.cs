using ProCalendar.Core.BaseListDates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace ProCalendar.UI.Controls
{
    public class CalendarToggleButtonEventArgs : RoutedEventArgs
    {
        public bool IsChecked { get; }
        public CalendarToggleButtonEventArgs(bool isChecked)
        {
            IsChecked = isChecked;
        }
    }

    public class CalendarToggleButton : ContentControl
    {
        private DateTimeModel _dateTimeModel;
        public event RoutedEventHandler Checked;
        private void ProCalendarItem_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            var dateTimeModel = args.NewValue as DateTimeModel;
            if (dateTimeModel == null) return;

            _dateTimeModel = dateTimeModel;

            UpdateCalendarStates();
        }

        public CalendarToggleButton()
        {
            this.DefaultStyleKey = typeof(CalendarToggleButton);

            this.PointerPressed += ContentControl_PointerPressed;
            this.PointerReleased += ContentControl_PointerReleased;
            this.PointerEntered += ContentControl_PointerEntered;
            this.PointerExited += ContentControl_PointerExited;

            this.DataContextChanged += ProCalendarItem_DataContextChanged;
        }

        private void UpdateCalendarStates()
        {
            if (_dateTimeModel == null) return;

            if (_dateTimeModel.Equals(DateTime.Now))
            {
                VisualStateManager.GoToState(this, "ToodayTrue", true);
                this.IsEnabled = false;
            }
            else
                VisualStateManager.GoToState(this, "ToodayFalse", true);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            VisualStateManager.GoToState(this, (IsChecked) ? "CheckedNormal" : "Normal", true);

            UpdateCalendarStates();
        }

        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(CalendarToggleButton), new PropertyMetadata(false, OnIsCheckedChanged));

        private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as CalendarToggleButton;
            if (control == null) return;
            //if (control != null)
            //{
            //    control.UpdateStates();
            //}

            control.OnIsCheckedChangedHandler();
        }

        private void OnIsCheckedChangedHandler()
        {
            Checked?.Invoke(this, new CalendarToggleButtonEventArgs(this.IsChecked));
        }

        private void ContentControl_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, (IsChecked) ? "CheckedPressed" : "Pressed", true);

            IsChecked = !IsChecked;
        }

        private void ContentControl_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, (IsChecked) ? "CheckedPointerOver" : "PointerOver", true);
        }

        private void ContentControl_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, (IsChecked) ? "CheckedNormal" : "Normal", true);
        }

        private void ContentControl_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, (IsChecked) ? "CheckedNormal" : "Normal", true);
        }
    }
}
