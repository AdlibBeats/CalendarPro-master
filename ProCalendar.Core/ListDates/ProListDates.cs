using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using ProCalendar.Core.BaseListDates;
using System.Diagnostics;

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

        public ProListDates() : this(DateTime.Now, DateTime.Now.AddMonths(3), ProListDatesLoadingType.LoadingMonths, DateTime.Now.AddDays(3), DateTime.Now.AddDays(12))
        {

        }

        public ProListDates(DateTime min, DateTime max, ProListDatesLoadingType proListDatesLoadingType, params DateTime[] blackoutDays)
        {
            this.BlackoutDays = blackoutDays;
            this.ProListDatesLoadingType = proListDatesLoadingType;
            this.Min = min;
            this.Max = max;

            Initialize();
        }

        private void Initialize()
        {
            this.ListDates = new ObservableCollection<ListDates>();

            switch (this.ProListDatesLoadingType)
            {
                case ProListDatesLoadingType.LoadingYears:
                    {
                        LoadYears();
                        break;
                    }
                case ProListDatesLoadingType.LoadingMonths:
                    {
                        LoadMonths();
                        break;
                    }
                case ProListDatesLoadingType.LoadingDays:
                    {
                        LoadDays();
                        break;
                    }
            }
        }

        private void LoadYears()
        {
            for (DateTime i = this.Min; i <= this.Max;)
            {
                for (int j = 1; j <= 12; j++)
                {
                    var dateTime = new DateTime(i.Year, j, 1);

                    var dateTimeModel = new DateTimeModel()
                    {
                        DateTime = dateTime,
                        IsWeekend = false,
                        IsBlackout = false,
                        IsSelected = false,
                        IsDisabled = false,
                        IsToday = false
                    };

                    this.ListDates.Add(new ListDates(dateTimeModel));
                }
                i = i.AddYears(1);
            }
        }

        private void LoadMonths()
        {
            for (DateTime i = this.Min; i <= this.Max;)
            {
                var dateTime = new DateTime(i.Year, i.Month, 1);

                var dateTimeModel = new DateTimeModel()
                {
                    DateTime = dateTime,
                    IsWeekend = false,
                    IsBlackout = false,
                    IsSelected = false,
                    IsDisabled = false,
                    IsToday = false
                };

                this.ListDates.Add(new ListDates(dateTimeModel, this.BlackoutDays));
                i = i.AddMonths(1);
            }
        }

        private void LoadDays()
        {
            //TODO: 
        }

        public DateTime[] BlackoutDays
        {
            get { return (DateTime[])GetValue(BlackoutDaysProperty); }
            set { SetValue(BlackoutDaysProperty, value); }
        }

        public static readonly DependencyProperty BlackoutDaysProperty =
            DependencyProperty.Register("BlackoutDays", typeof(DateTime[]), typeof(ProListDates), new PropertyMetadata(null));

        public ObservableCollection<ListDates> ListDates
        {
            get { return (ObservableCollection<ListDates>)GetValue(ListDatesProperty); }
            set { SetValue(ListDatesProperty, value); }
        }

        public static readonly DependencyProperty ListDatesProperty =
            DependencyProperty.Register("ListDates", typeof(ObservableCollection<ListDates>), typeof(ProListDates), new PropertyMetadata(null));
    }
}
