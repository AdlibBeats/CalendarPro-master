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
    //public interface IBaseModel : INotifyPropertyChanged
    //{
    //    void SetValue<V>(ref V oldValue, V newValue, [CallerMemberName]string propertyName = null);
    //}

    //public abstract class BaseModel : DependencyObject, IBaseModel
    //{
    //    #region INotifyPropertyChanged Members

    //    public event PropertyChangedEventHandler PropertyChanged;

    //    public virtual void SetValue<V>(ref V oldValue, V newValue, [CallerMemberName]string propertyName = null)
    //    {
    //        oldValue = newValue;
    //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    //    }

    //    #endregion
    //}

    public class DateTimeModelEventArgs : EventArgs
    {
        public object NewValue { get; }

        public DateTimeModelEventArgs(object newValue)
        {
            NewValue = newValue;
        }
    }


    public class DateTimeModel
    {
        public event EventHandler<DateTimeModelEventArgs> DateTimeModelChanged;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                DateTimeModelChanged?.Invoke(this, new DateTimeModelEventArgs(value));
            }
        }
        private bool _isSelected;

        public bool IsBlackout
        {
            get => _isBlackout;
            set
            {
                _isBlackout = value;
                DateTimeModelChanged?.Invoke(this, new DateTimeModelEventArgs(value));
            }
        }
        private bool _isBlackout;

        public bool IsDisabled
        {
            get => _isDisabled;
            set
            {
                _isDisabled = value;
                DateTimeModelChanged?.Invoke(this, new DateTimeModelEventArgs(value));
            }
        }
        private bool _isDisabled;

        public bool IsWeekend
        {
            get => _isWeekend;
            set
            {
                _isWeekend = value;
                DateTimeModelChanged?.Invoke(this, new DateTimeModelEventArgs(value));
            }
        }
        private bool _isWeekend;

        public bool IsToday
        {
            get => _isToday;
            set
            {
                _isToday = value;
                DateTimeModelChanged?.Invoke(this, new DateTimeModelEventArgs(value));
            }
        }
        private bool _isToday;

        public DateTime DateTime
        {
            get => _dateTime;
            set
            {
                _dateTime = value;
                DateTimeModelChanged?.Invoke(this, new DateTimeModelEventArgs(value));
            }
        }
        private DateTime _dateTime;

        public bool Equals(DateTime dateTime) =>
            this.DateTime.Year == dateTime.Year &&
            this.DateTime.Month == dateTime.Month &&
            this.DateTime.Day == dateTime.Day;
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
