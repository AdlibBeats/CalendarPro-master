using ProCalendar.Core.BaseListDates;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace ProCalendar.UI.Controls
{
    public class ProCalendarItem : ToggleButton
    {
        private Grid _rootGrid;
        private DateTimeModel _dateTimeModel;
        public ProCalendarItem()
        {
            this.DefaultStyleKey = typeof(ProCalendarItem);

            this.DataContextChanged += ProCalendarItem_DataContextChanged;
        }

        private void ProCalendarItem_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            var dateTimeModel = args.NewValue as DateTimeModel;
            if (dateTimeModel == null) return;

            _dateTimeModel = dateTimeModel;

            UpdateCalendarStates();
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _rootGrid = this.GetTemplateChild("RootGrid") as Grid;
            if (_rootGrid == null) return;

            //VisualStateManager.GoToState(this, "ToodayFalse", true);

            UpdateCalendarStates();
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
    }
}
