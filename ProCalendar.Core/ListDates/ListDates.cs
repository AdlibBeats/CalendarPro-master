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
        private const int ContentDaysCount = 42;

        public ListDates() : this(new DateTimeModel() { DateTime = DateTime.Now }) { }

        public ListDates(DateTimeModel dateTimeModel) : base(dateTimeModel)
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.ContentDays = new ObservableCollection<DateTimeModel>();

            int count = 0;
            DateTime datetime =
                new DateTime(CurrentDay.DateTime.Year, CurrentDay.DateTime.Month, 1);

            int dayOfWeek = (int)this.CurrentDays[0].DateTime.DayOfWeek;
            Debug.WriteLine(dayOfWeek);

            if (dayOfWeek == 0)
                count = 6;
            else
                count = dayOfWeek - 1;

            Debug.WriteLine(count);
            if (count != 0)
                AddRemainingDates(count, datetime.AddDays(-count));

            foreach (var dateTimeModel in CurrentDays)
                ContentDays.Add(dateTimeModel);

            count = ContentDaysCount - ContentDays.Count;
            AddRemainingDates(count, datetime.AddMonths(1));
        }

        private void AddRemainingDates(int daysCount, DateTime firstDateTime)
        {
            DateTime[] firstDates = new DateTime[daysCount];

            for (int i = 0; i < daysCount; i++)
            {
                firstDates[i] = firstDateTime;
                firstDateTime = firstDateTime.AddDays(1);
                DateTimeModel dateTimeModel = new DateTimeModel()
                { DateTime = firstDates[i] };
                ContentDays.Add(dateTimeModel);
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
