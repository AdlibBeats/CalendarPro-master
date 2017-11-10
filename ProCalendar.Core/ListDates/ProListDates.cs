using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace ProCalendar.Core.ListDates
{
    public class ProListDates : BaseListDates.BaseModel
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

        public ProListDates() : this(2017, 2018) { }

        public ProListDates(int minYear, int maxYear)
        {
            MinYear = minYear;
            MaxYear = maxYear;

            Init();
        }

        public void Init()
        {
            ListDates = new ObservableCollection<Core.ListDates.ListDates>();

            for (int i = MinYear; i <= MaxYear; i++)
            {
                for (int j = 1; j <= 12; j++)
                {
                    ListDates.Add(new Core.ListDates.ListDates(new BaseListDates.DateTimeModel()
                    {
                        DateTime = new DateTime(i, j, 1)
                    }));
                }
            }
        }

        public ObservableCollection<ListDates> ListDates
        {
            get { return (ObservableCollection<ListDates>)GetValue(ListDatesProperty); }
            set { SetValue(ListDatesProperty, value); }
        }

        public static readonly DependencyProperty ListDatesProperty =
            DependencyProperty.Register("ListDates", typeof(ObservableCollection<ListDates>), typeof(ProListDates), new PropertyMetadata(null));
    }
}
