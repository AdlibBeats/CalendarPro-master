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
        //public DateTimeModel DateTimeModel { get; }
        public CalendarToggleButtonEventArgs(bool isChecked)
        {
            IsChecked = isChecked;
            //DateTimeModel = dateTimeModel;
        }
    }

    public class ProCalendarToggleButton : ContentControl
    {
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(ProCalendarToggleButton), new PropertyMetadata(false, OnIsSelectedChanged));

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var proCalendarToggleButton = d as ProCalendarToggleButton;
            if (proCalendarToggleButton == null) return;

            proCalendarToggleButton.UpdateStates();
        }

        public bool IsBlackout
        {
            get { return (bool)GetValue(IsBlackoutProperty); }
            set { SetValue(IsBlackoutProperty, value); }
        }

        public static readonly DependencyProperty IsBlackoutProperty =
            DependencyProperty.Register("IsBlackout", typeof(bool), typeof(ProCalendarToggleButton), new PropertyMetadata(false, OnIsBlackoutChanged));

        private static void OnIsBlackoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var proCalendarToggleButton = d as ProCalendarToggleButton;
            if (proCalendarToggleButton == null) return;

            proCalendarToggleButton.UpdateStates();
        }

        public bool IsDisabled
        {
            get { return (bool)GetValue(IsDisabledProperty); }
            set { SetValue(IsDisabledProperty, value); }
        }

        public static readonly DependencyProperty IsDisabledProperty =
            DependencyProperty.Register("IsDisabled", typeof(bool), typeof(ProCalendarToggleButton), new PropertyMetadata(false, OnIsDisabledChanged));

        private static void OnIsDisabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var proCalendarToggleButton = d as ProCalendarToggleButton;
            if (proCalendarToggleButton == null) return;

            proCalendarToggleButton.UpdateStates();
        }

        public bool IsWeekend
        {
            get { return (bool)GetValue(IsWeekendProperty); }
            set { SetValue(IsWeekendProperty, value); }
        }

        public static readonly DependencyProperty IsWeekendProperty =
            DependencyProperty.Register("IsWeekend", typeof(bool), typeof(ProCalendarToggleButton), new PropertyMetadata(false, OnIsWeekendChanged));

        private static void OnIsWeekendChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var proCalendarToggleButton = d as ProCalendarToggleButton;
            if (proCalendarToggleButton == null) return;

            proCalendarToggleButton.UpdateStates();
        }

        public bool IsToday
        {
            get { return (bool)GetValue(IsTodayProperty); }
            set { SetValue(IsTodayProperty, value); }
        }

        public static readonly DependencyProperty IsTodayProperty =
            DependencyProperty.Register("IsToday", typeof(bool), typeof(ProCalendarToggleButton), new PropertyMetadata(false, OnIsTodayChanged));

        private static void OnIsTodayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var proCalendarToggleButton = d as ProCalendarToggleButton;
            if (proCalendarToggleButton == null) return;

            proCalendarToggleButton.UpdateStates();
        }

        public DateTime DateTime
        {
            get { return (DateTime)GetValue(DateTimeProperty); }
            set { SetValue(DateTimeProperty, value); }
        }

        public static readonly DependencyProperty DateTimeProperty =
            DependencyProperty.Register("DateTime", typeof(DateTime), typeof(ProCalendarToggleButton), new PropertyMetadata(DateTime.Now, OnDateTimeChanged));

        private static void OnDateTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var proCalendarToggleButton = d as ProCalendarToggleButton;
            if (proCalendarToggleButton == null) return;

            proCalendarToggleButton.UpdateStates();
        }

        private DateTimeModel dataContext;

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
            dataContext = args.NewValue as DateTimeModel;
            if (dataContext == null) return;

            dataContext.DateTimeModelChanged += (s, e) =>
            {
                UpdateStates();
            };
        }
        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, dataContext.IsSelected ? "CheckedPressed" : "Pressed", true);

            dataContext.IsSelected = !dataContext.IsSelected;

            Checked?.Invoke(this, new CalendarToggleButtonEventArgs(dataContext.IsSelected));
        }

        private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, dataContext.IsSelected ? "CheckedPointerOver" : "PointerOver", true);
        }

        private void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, dataContext.IsSelected ? "CheckedNormal" : "Normal", true);
        }

        private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, dataContext.IsSelected ? "CheckedNormal" : "Normal", true);
        }

        private void UpdateStates()
        {
            if (dataContext.IsSelected)
                UpdateSelectedStates("Checked");
            else
                UpdateSelectedStates();

            VisualStateManager.GoToState(this, dataContext.IsToday ? "IsToodayTrue" : "IsToodayFalse", true);
            VisualStateManager.GoToState(this, dataContext.IsWeekend ? "IsWeekendTrue" : "IsWeekendFalse", true);
        }

        private void UpdateSelectedStates(string stateName = null)
        {
            if (dataContext.IsDisabled)
            {
                this.IsEnabled = false;
                stateName += "Disabled";
            }
            else if (dataContext.IsBlackout)
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
