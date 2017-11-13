using ProCalendar.Core.BaseListDates;
using System;
using System.Collections.Generic;
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

            VisualStateManager.GoToState(this, _dateTimeModel.Equals(DateTime.Now) ? "ToodayTrue" : "ToodayFalse", true);
            VisualStateManager.GoToState(this, _dateTimeModel.IsWeekend ? "IsWeekendTrue" : "IsWeekendFalse", true);
            //VisualStateManager.GoToState(this, _dateTimeModel.IsSelected ? "CheckedNormal" : "Normal", true);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            VisualStateManager.GoToState(this, IsChecked ? "CheckedNormal" : "Normal", true);

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

            control.OnIsCheckedChangedHandler();
        }

        private void OnIsCheckedChangedHandler()
        {
            VisualStateManager.GoToState(this, this.IsChecked ? "CheckedNormal" : "Normal", true);

            var dateTimeModel = this.DataContext as DateTimeModel;
            if (dateTimeModel == null) return;
            
            Checked?.Invoke(this, new CalendarToggleButtonEventArgs(this.IsChecked, dateTimeModel));
        }

        private void ContentControl_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, this.IsChecked ? "CheckedPressed" : "Pressed", true);

            this.IsChecked = !this.IsChecked;
        }

        private void ContentControl_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, this.IsChecked ? "CheckedPointerOver" : "PointerOver", true);
        }

        private void ContentControl_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, this.IsChecked ? "CheckedNormal" : "Normal", true);
        }

        private void ContentControl_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, this.IsChecked ? "CheckedNormal" : "Normal", true);
        }
    }
}
