using ProCalendar.Core.BaseListDates;
using ProCalendar.Core.ListDates;
using ProCalendar.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void ProCalendar_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var selectedItem = sender as CalendarToggleButton;
            if (selectedItem == null) return;

            var ev = e as SelectedItemEventArgs;
            if (ev == null) return;

            var data = ev.SelectedItem.DataContext as DateTimeModel;
            if (data == null) return;

            Debug.WriteLine($"{selectedItem.IsChecked} {data.DateTime}");
        }
    }
}
