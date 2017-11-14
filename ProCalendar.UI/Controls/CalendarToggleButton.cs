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
            VisualStateManager.GoToState(this, this.Model.Equals(DateTime.Now) ? "ToodayTrue" : "ToodayFalse", true);
            VisualStateManager.GoToState(this, this.Model.IsWeekend ? "IsWeekendTrue" : "IsWeekendFalse", true);
            VisualStateManager.GoToState(this, this.Model.IsSelected ? "CheckedNormal" : "Normal", true);
            //TODO: IsBlackout
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

        /*
            private void StartLoadedAnimation()
            {
                Storyboard sb = new Storyboard();

                DoubleAnimation da = new DoubleAnimation()
                {
                    Duration = new Duration(TimeSpan.FromSeconds(1)),
                    From = 0,
                    To = 200,
                    EasingFunction = new ElasticEase()
                    {
                        EasingMode = EasingMode.EaseInOut,
                        Oscillations = 2,
                        Springiness = 1
                    },
                    RepeatBehavior = new RepeatBehavior(1),
                    AutoReverse = false
                };

                Storyboard.SetTarget(da, this);
                Storyboard.SetTargetProperty(da, "(Control.Width)");

                sb.Children.Add(da);

                sb.Begin();
            }
        */
    }
}
