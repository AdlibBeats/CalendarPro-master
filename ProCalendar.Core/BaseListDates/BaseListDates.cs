﻿using System;
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
    public interface IBaseModel : INotifyPropertyChanged
    {
        void SetValue<V>(ref V oldValue, V newValue, [CallerMemberName]string propertyName = null);
    }

    public abstract class BaseModel : DependencyObject, IBaseModel
    {
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void SetValue<V>(ref V oldValue, V newValue, [CallerMemberName]string propertyName = null)
        {
            oldValue = newValue;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    public class DateTimeModel : BaseModel
    {
        //public bool IsSelected
        //{
        //    get => _isSelected;
        //    set => SetValue(ref _isSelected, value);
        //}
        //private bool _isSelected;

        public bool IsBlackout
        {
            get => _isBlackout;
            set => SetValue(ref _isBlackout, value);
        }
        private bool _isBlackout;

        public bool IsWeekend
        {
            get => _isWeekend;
            set => SetValue(ref _isWeekend, value);
        }
        private bool _isWeekend;

        public DateTime DateTime
        {
            get => _dateTime;
            set => SetValue(ref _dateTime, value);
        }
        private DateTime _dateTime;

        public override bool Equals(object obj)
        {
            if (obj is DateTime)
            {
                var dateTime = (DateTime)obj;
                if (this.DateTime.Year == dateTime.Year &&
                    this.DateTime.Month == dateTime.Month &&
                    this.DateTime.Day == dateTime.Day)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class BaseListDates<T> : BaseModel
    {
        public BaseListDates() : this(new DateTimeModel() { DateTime = DateTime.Now, IsBlackout = false })
        {

        }

        public BaseListDates(DateTimeModel currentDate)
        {
            CurrentDay = currentDate;

            this.Initialize();
        }

        private void Initialize()
        {
            CurrentDays = new ObservableCollection<DateTimeModel>();

            int countDays = DateTime.DaysInMonth(CurrentDay.DateTime.Year, CurrentDay.DateTime.Month);
            for (int day = 1; day <= countDays; day++)
            {
                DateTime dateTime = new DateTime(CurrentDay.DateTime.Year, CurrentDay.DateTime.Month, day);
                CurrentDays.Add(new DateTimeModel()
                {
                    DateTime = dateTime,
                    IsWeekend = this.GetIsWeekend(dateTime)
                });
            }
        }

        public virtual bool GetIsWeekend(DateTime dateTime) =>
            dateTime.DayOfWeek == DayOfWeek.Saturday || dateTime.DayOfWeek == DayOfWeek.Sunday;

        public DateTimeModel CurrentDay
        {
            get => _currentDay;
            set => SetValue(ref _currentDay, value);
        }
        private DateTimeModel _currentDay;

        public ObservableCollection<DateTimeModel> CurrentDays
        {
            get { return (ObservableCollection<DateTimeModel>)GetValue(CurrentDaysProperty); }
            set { SetValue(CurrentDaysProperty, value); }
        }

        public static readonly DependencyProperty CurrentDaysProperty =
            DependencyProperty.Register("CurrentDays", typeof(ObservableCollection<DateTimeModel>), typeof(BaseListDates<T>), new PropertyMetadata(null));
    }
}
