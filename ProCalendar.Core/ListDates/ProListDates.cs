using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace ProCalendar.Core.ListDates
{
    public sealed class ProListDates : BaseListDates.BaseModel
    {
        public int MinYear
        {
            get => _minYear;
            set => SetValue(ref _minYear, value);
        }
        private int _minYear;

        public int MaxYear
        {
            get => _maxYear;
            set => SetValue(ref _maxYear, value);
        }
        private int _maxYear;

        public ProListDates() : this(DateTime.Now.Year, DateTime.Now.AddYears(1).Year)
        {

        }

        public ProListDates(int minYear, int maxYear)
        {
            MinYear = minYear;
            MaxYear = maxYear;

            Initialize();
        }

        private void Initialize()
        {
            ListDates = new ObservableCollection<Core.ListDates.ListDates>();

            for (int i = MinYear; i <= MaxYear; i++)
            {
                for (int j = 1; j <= 12; j++)
                {
                    DateTime dateTime = new DateTime(i, j, 1);
                    ListDates.Add(new Core.ListDates.ListDates(new BaseListDates.DateTimeModel()
                    {
                        DateTime = dateTime,
                        IsWeekend = this.GetIsWeekend(dateTime),
                        IsBlackout = false
                    }));
                }
            }
        }

        public bool GetIsWeekend(DateTime dateTime) =>
            dateTime.DayOfWeek == DayOfWeek.Saturday || dateTime.DayOfWeek == DayOfWeek.Sunday;

        public ObservableCollection<ListDates> ListDates
        {
            get { return (ObservableCollection<ListDates>)GetValue(ListDatesProperty); }
            set { SetValue(ListDatesProperty, value); }
        }

        public static readonly DependencyProperty ListDatesProperty =
            DependencyProperty.Register("ListDates", typeof(ObservableCollection<ListDates>), typeof(ProListDates), new PropertyMetadata(null));
    }
}
