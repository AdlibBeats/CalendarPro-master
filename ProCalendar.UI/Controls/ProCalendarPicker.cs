using ProCalendar.Core.BaseListDates;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace ProCalendar.UI.Controls
{
    public class ProCalendarPicker : Control
    {
        private Button _loadingButton;
        private ProgressRing _loadingProgress;
        private ProCalendarView _proCalendarView;
        private Flyout _rootFlyout;
        private Image _calendarIcon;
        private TextBlock _dateText;
        private Grid _flyoutBorder;

        public ProCalendarPicker()
        {
            this.DefaultStyleKey = typeof(ProCalendarPicker);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _calendarIcon = this.GetTemplateChild("CalendarIcon") as Image;
            if (_calendarIcon == null) return;

            _loadingProgress = this.GetTemplateChild("LoadingProgress") as ProgressRing;
            if (_loadingProgress == null) return;

            _rootFlyout = this.GetTemplateChild("RootFlyout") as Flyout;
            if (_rootFlyout == null) return;

            _flyoutBorder = this.GetTemplateChild("FlyoutBorder") as Grid;
            if (_flyoutBorder == null) return;

            _loadingButton = this.GetTemplateChild("LoadingButton") as Button;
            if (_loadingButton == null) return;

            _loadingButton.Tapped += loadingButton_Tapped;

            _dateText = this.GetTemplateChild("DateText") as TextBlock;
            if (_dateText == null) return;

            _proCalendarView = this.GetTemplateChild("ProCalendarView") as ProCalendarView;
            if (_proCalendarView == null) return;

            _proCalendarView.SelectionChanged += ProCalendar_SelectionChanged;
            _proCalendarView.UnselectionChanged += ProCalendar_UnselectionChanged;
        }

        private async void loadingButton_Tapped(object sender, RoutedEventArgs e)
        {
            _calendarIcon.Visibility = Visibility.Collapsed;
            _loadingProgress.IsActive = true;

            await Task.Run(async () =>
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    _rootFlyout.ShowAt(_flyoutBorder);
                });
            });

            _loadingProgress.IsActive = false;
            _calendarIcon.Visibility = Visibility.Visible;
        }

        private async void ProCalendar_SelectionChanged(object sender, RoutedEventArgs e)
        {
            _loadingProgress.IsActive = true;
            _calendarIcon.Visibility = Visibility.Collapsed;
            
            var args = e as SelectedItemEventArgs;
            if (args == null) return;

            var data = args.SelectedItem.DataContext as DateTimeModel;
            if (data == null) return;

            _dateText.Text = $"{data.DateTime.Day}/{data.DateTime.Month}/{data.DateTime.Year}";

            await Task.Run(async () =>
            {
                await Task.Delay(500);
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    _rootFlyout.Hide();
                });
            });

            _loadingProgress.IsActive = false;
            _calendarIcon.Visibility = Visibility.Visible;
        }

        private async void ProCalendar_UnselectionChanged(object sender, RoutedEventArgs e)
        {
            _loadingProgress.IsActive = true;
            _calendarIcon.Visibility = Visibility.Collapsed;

            _dateText.Text = "Select Date";

            await Task.Run(async () =>
            {
                await Task.Delay(300);
            });

            _loadingProgress.IsActive = false;
            _calendarIcon.Visibility = Visibility.Visible;
        }
    }
}
