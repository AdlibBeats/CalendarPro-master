using ProCalendar.Core.BaseListDates;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace ProCalendar.Core.ListDates
{
    public sealed class ListDates : BaseListDates.BaseListDates<DateTime>
    {
        public int ContentDaysCapacity
        {
            get => 42;
        }

        public ListDates() : this(new DateTimeModel() { DateTime = DateTime.Now, IsBlackout = false }) { }

        public ListDates(DateTimeModel dateTimeModel) : base(dateTimeModel)
        {
            this.Initialize();
        }

        private void Initialize()
        {
            int count = 0;
            this.ContentDays = new ObservableCollection<DateTimeModel>();
            
            DateTime datetime =
                new DateTime(CurrentDay.DateTime.Year, CurrentDay.DateTime.Month, 1);

            int dayOfWeek = (int)this.CurrentDays[0].DateTime.DayOfWeek;

            count = (dayOfWeek == 0) ? 6 : dayOfWeek - 1;
            
            if (count != 0)
                AddRemainingDates(count, datetime.AddDays(-count));

            foreach (var dateTimeModel in CurrentDays)
                ContentDays.Add(dateTimeModel);

            count = ContentDaysCapacity - ContentDays.Count;
            AddRemainingDates(count, datetime.AddMonths(1));
        }

        private void AddRemainingDates(int daysCount, DateTime remainingDateTime)
        {
            for (int i = 0; i < daysCount; i++)
            {
                var dateTimeModel = new DateTimeModel()
                {
                    DateTime = remainingDateTime,
                    IsWeekend = this.GetIsWeekend(remainingDateTime),
                    IsBlackout = false,
                    IsSelected = false,
                    IsDisabled = false,
                    IsToday = this.GetIsToday(remainingDateTime)
                };

                ContentDays.Add(dateTimeModel);
                remainingDateTime = remainingDateTime.AddDays(1);
            }
        }

        public ObservableCollection<DateTimeModel> ContentDays
        {
            get { return (ObservableCollection<DateTimeModel>)GetValue(ContentDaysProperty); }
            set { SetValue(ContentDaysProperty, value); }
        }

        public static readonly DependencyProperty ContentDaysProperty =
            DependencyProperty.Register("ContentDays", typeof(ObservableCollection<DateTimeModel>), typeof(ListDates), new PropertyMetadata(null));
    }
}
