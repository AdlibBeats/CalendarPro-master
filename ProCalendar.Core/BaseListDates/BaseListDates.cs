using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace ProCalendar.Core.BaseListDates
{
    public class DateTimeModel
    {
        public bool IsSelected { get; set; }
        public bool IsBlackout { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsWeekend { get; set; }
        public bool IsToday { get; set; }
        public DateTime DateTime { get; set; }
    }

    public class BaseListDates<T>
    {
        public BaseListDates(DateTimeModel currentDay, params DateTime[] blackoutDays)
        {
            this.CurrentDay = currentDay;

            if (blackoutDays != null && blackoutDays.Any())
                this.BlackoutDays = new List<DateTime>(blackoutDays);

            this.Initialize();
        }

        private void Initialize()
        {
            this.CurrentDays = new List<DateTimeModel>();

            int countDays = DateTime.DaysInMonth(CurrentDay.DateTime.Year, CurrentDay.DateTime.Month);
            for (int day = 1; day <= countDays; day++)
            {
                var dateTime = new DateTime(CurrentDay.DateTime.Year, CurrentDay.DateTime.Month, day);

                var dateTimeModel = new DateTimeModel()
                {
                    DateTime = dateTime,
                    IsWeekend = this.GetIsWeekend(dateTime),
                    IsBlackout = this.GetIsBlackout(dateTime),
                    IsSelected = this.CurrentDay.IsSelected,
                    IsDisabled = this.CurrentDay.IsDisabled,
                    IsToday = this.GetIsToday(dateTime)
                };

                this.CurrentDays.Add(dateTimeModel);
            }
        }

        public virtual bool GetIsBlackout(DateTime dateTime)
        {
            if (BlackoutDays == null || !BlackoutDays.Any()) return false;

            var blackoutDay =
                this.BlackoutDays.FirstOrDefault(i =>
                    i.Year == dateTime.Year &&
                    i.Month == dateTime.Month &&
                    i.Day == dateTime.Day);

            return blackoutDay.Year != 0001;
        }

        public virtual bool GetIsWeekend(DateTime dateTime) =>
            dateTime.DayOfWeek == DayOfWeek.Saturday || dateTime.DayOfWeek == DayOfWeek.Sunday;

        public virtual bool GetIsToday(DateTime dateTime) =>
            dateTime.Year == DateTime.Now.Year &&
            dateTime.Month == DateTime.Now.Month &&
            dateTime.Day == DateTime.Now.Day;

        public List<DateTime> BlackoutDays { get; set; }

        public DateTimeModel CurrentDay { get; set; }

        public List<DateTimeModel> CurrentDays { get; set; }
    }
}
