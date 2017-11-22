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

        public ListDates() : this(new DateTimeModel()) { }

        public ListDates(DateTimeModel dateTimeModel, params DateTime[] blackoutDays) : base(dateTimeModel, blackoutDays)
        {
            this.Initialize();
        }

        private void Initialize()
        {
            int count = 0;
            this.ContentDays = new List<DateTimeModel>();

            int dayOfWeek = (int)this.CurrentDay.DateTime.DayOfWeek;

            count = (dayOfWeek == 0) ? 6 : dayOfWeek - 1;
            if (count != 0)
                AddRemainingDates(count, this.CurrentDay.DateTime.AddDays(-count));

            foreach (var dateTimeModel in this.CurrentDays)
                this.ContentDays.Add(dateTimeModel);

            count = this.ContentDaysCapacity - this.ContentDays.Count;
            AddRemainingDates(count, this.CurrentDay.DateTime.AddMonths(1));
        }

        private void AddRemainingDates(int daysCount, DateTime remainingDateTime)
        {
            for (int i = 0; i < daysCount; i++)
            {
                var dateTimeModel = new DateTimeModel()
                {
                    DateTime = remainingDateTime,
                    IsWeekend = this.GetIsWeekend(remainingDateTime),
                    IsBlackout = this.GetIsBlackout(remainingDateTime),
                    IsSelected = this.CurrentDay.IsSelected,
                    IsDisabled = this.CurrentDay.IsDisabled,
                    IsToday = this.GetIsToday(remainingDateTime)
                };

                this.ContentDays.Add(dateTimeModel);
                remainingDateTime = remainingDateTime.AddDays(1);
            }
        }

        public List<DateTimeModel> ContentDays { get; set; }
    }
}
