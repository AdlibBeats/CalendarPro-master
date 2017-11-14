using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using ProCalendar.Core.BaseListDates;

namespace ProCalendar.Core.ListDates
{
    public enum ProListDatesLoadingType
    {
        LoadingYears,
        LoadingMonths,
        LoadingDays
    }

    public sealed class ProListDates : BaseModel
    {
        public DateTime Min
        {
            get => _min;
            set => SetValue(ref _min, value);
        }
        private DateTime _min;

        public DateTime Max
        {
            get => _max;
            set => SetValue(ref _max, value);
        }
        private DateTime _max;

        public ProListDatesLoadingType ProListDatesLoadingType
        {
            get => _proListDatesLoadingType;
            set => SetValue(ref _proListDatesLoadingType, value);
        }
        private ProListDatesLoadingType _proListDatesLoadingType;

        public ProListDates() : this(DateTime.Now, DateTime.Now.AddMonths(3), ProListDatesLoadingType.LoadingMonths)
        {

        }

        public ProListDates(DateTime min, DateTime max, ProListDatesLoadingType proListDatesLoadingType)
        {
            ProListDatesLoadingType = proListDatesLoadingType;

            Min = min;
            Max = max;

            Initialize();
        }

        private void Initialize()
        {
            ListDates = new ObservableCollection<ListDates>();

            switch (ProListDatesLoadingType)
            {
                case ProListDatesLoadingType.LoadingYears:
                    {
                        for (DateTime i = Min; i <= Max;)
                        {
                            for (int j = 1; j <= 12; j++)
                            {
                                var dateTime = new DateTime(i.Year, j, 1);

                                var dateTimeModel = new DateTimeModel()
                                {
                                    DateTime = dateTime,
                                    IsWeekend = this.GetIsWeekend(dateTime),
                                    IsBlackout = false,
                                    IsSelected = false,
                                    IsDisabled = false,
                                    IsToday = this.GetIsToday(dateTime)
                                };

                                ListDates.Add(new ListDates(dateTimeModel));
                            }
                            i = i.AddYears(1);
                        }
                        break;
                    }
                case ProListDatesLoadingType.LoadingMonths:
                    {
                        for (DateTime j = Min; j <= Max;)
                        {
                            var dateTime = new DateTime(j.Year, j.Month, 1);

                            var dateTimeModel = new DateTimeModel()
                            {
                                DateTime = dateTime,
                                IsWeekend = this.GetIsWeekend(dateTime),
                                IsBlackout = false,
                                IsSelected = false,
                                IsDisabled = false,
                                IsToday = this.GetIsToday(dateTime)
                            };

                            ListDates.Add(new ListDates(dateTimeModel));
                            j = j.AddMonths(1);
                        }
                        break;
                    }
                case ProListDatesLoadingType.LoadingDays:
                    {
                        //TODO: ProListDatesLoadingType.LoadingDays
                        break;
                    }
            }
        }

        public bool GetIsWeekend(DateTime dateTime) =>
            dateTime.DayOfWeek == DayOfWeek.Saturday || dateTime.DayOfWeek == DayOfWeek.Sunday;

        public bool GetIsToday(DateTime dateTime) =>
            dateTime.Year == DateTime.Now.Year &&
            dateTime.Month == DateTime.Now.Month &&
            dateTime.Day == DateTime.Now.Day;

        public ObservableCollection<ListDates> ListDates
        {
            get { return (ObservableCollection<ListDates>)GetValue(ListDatesProperty); }
            set { SetValue(ListDatesProperty, value); }
        }

        public static readonly DependencyProperty ListDatesProperty =
            DependencyProperty.Register("ListDates", typeof(ObservableCollection<ListDates>), typeof(ProListDates), new PropertyMetadata(null));
    }
}
