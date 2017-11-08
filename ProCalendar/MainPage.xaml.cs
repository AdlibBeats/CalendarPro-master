using ProCalendar.Core.ListDates;
using ProCalendar.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace ProCalendar
{
    public interface IBaseModel : INotifyPropertyChanged
    {
        void SetValue<V>(ref V oldValue, V newValue, [CallerMemberName]string propertyName = null);
    }
    public class BaseModel : IBaseModel
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
    public class User : BaseModel
    {
        public string Day
        {
            get => _day;
            set => SetValue(ref _day, value);
        }
        private string _day;
    }
    public sealed partial class MainPage : Page
    {
        public DateTime MinDateTime { get; set; }
        public DateTime MaxDateTime { get; set; }

        public MainPage()
        {
            this.InitializeComponent();

            //MinDateTime = new DateTime(2017, 11, 1);
            //MaxDateTime = new DateTime(2017, 11, DateTime.DaysInMonth(2017, 11));

            //var listDates = new ListDates(MinDateTime, MaxDateTime);

            //if (lol != null)
            //    lol.ItemsSource = new ListDates(MinDateTime, MaxDateTime);
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            for (int i = 0; i < 42; i++)
                Users.Add(new User()
                {
                    Day = $"{i + 1}"
                });

            //superGrid.ItemsSource = Users;
        }

        public ObservableCollection<User> Users = new ObservableCollection<User>();
        
    }
}
