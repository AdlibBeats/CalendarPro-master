using ProCalendar.Core.BaseListDates;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace ProCalendar.UI.Controls
{
    public enum ProCalendarViewSelectionMode
    {
        None,
        Single,
        Multiple,
        Extended
    }

    public sealed class SelectedItemEventArgs : RoutedEventArgs
    {
        public CalendarToggleButton SelectedItem { get; }
        public DateTimeModel DateTimeModel { get; }
        public SelectedItemEventArgs(CalendarToggleButton selectedItem, DateTimeModel dateTimeModel)
        {
            SelectedItem = selectedItem;
            DateTimeModel = dateTimeModel;
        }
    }

    public class AdaptiveGridView : Control
    {
        public event RoutedEventHandler SelectionChanged;

        public AdaptiveGridView()
        {
            this.DefaultStyleKey = typeof(AdaptiveGridView);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            
            ItemsPanelRoot = this.GetTemplateChild("ItemsPanelRoot") as Grid;

            UpdateItemsPanelRoot();

            UpdateItemsSource();
        }

        private void UpdateItemsPanelRoot()
        {
            if (ItemsPanelRoot != null)
            {
                for (int column = 0; column < this.ColumnsCount; column++)
                    this.ItemsPanelRoot.ColumnDefinitions.Add(new ColumnDefinition()
                        { Width = new GridLength(0, GridUnitType.Auto) });

                for (int row = 0; row < this.RowsCount; row++)
                    this.ItemsPanelRoot.RowDefinitions.Add(new RowDefinition()
                        { Height = new GridLength(0, GridUnitType.Auto) });
            }
        }

        private void UpdateItemsSource()
        {
            var collection = this.ItemsSource as IEnumerable<object>;
            if (collection == null) return;

            this.Children = new List<CalendarToggleButton>();

            int column = 0;
            int row = 0;

            for (int i = 0; i < this.RowsCount * this.ColumnsCount; i++)
            {
                var content = LoadItemTemplateContent(column, row, collection.ElementAt(i));
                if (content == null) return;
                
                this.ItemsPanelRoot.Children.Add(content);
                //ItemsPanelRoot.UpdateLayout();

                column++;
                if (column != this.ColumnsCount)
                    continue;

                column = 0;
                row++;
                if (row != this.RowsCount)
                    continue;
            }
        }

        private FrameworkElement LoadItemTemplateContent(int gridColumn, int gridRow, object dataContext)
        {
            var itemControl = this.ItemTemplate?.LoadContent() as Control;
            if (itemControl == null) return null;

            var toggleButton = itemControl as CalendarToggleButton;
            if (toggleButton != null)
            {
                var listItems = Children as List<CalendarToggleButton>;
                listItems.Add(toggleButton);

                toggleButton.Checked += (sender, e) =>
                {
                    var ev = e as CalendarToggleButtonEventArgs;
                    if (ev == null) return;

                    var selectedItem = sender as CalendarToggleButton;
                    if (selectedItem == null) return;
                    
                    SelectionChanged?.Invoke(this, new SelectedItemEventArgs(selectedItem, ev.DateTimeModel));
                };
            }

            itemControl.BorderBrush = this.ItemBorderBrush;
            itemControl.BorderThickness = this.ItemBorderThickness;
            itemControl.Foreground = this.ItemForeground;
            itemControl.Background = this.ItemBackground;
            itemControl.Width = this.ItemWidth;
            itemControl.Height = this.ItemHeight;
            itemControl.HorizontalAlignment = this.ItemHorizontalAlignment;
            itemControl.VerticalAlignment = this.ItemVerticalAlignment;
            itemControl.Margin = this.ItemMargin;
            itemControl.Padding = this.ItemPadding;

            itemControl.DataContext = dataContext;

            Grid.SetColumn(itemControl, gridColumn);
            Grid.SetRow(itemControl, gridRow);

            return itemControl;
        }

        #region Dependency Properties

        #region Item Properties

        public Brush ItemBorderBrush
        {
            get { return (Brush)GetValue(ItemBorderBrushProperty); }
            set { SetValue(ItemBorderBrushProperty, value); }
        }

        public static readonly DependencyProperty ItemBorderBrushProperty =
            DependencyProperty.Register("ItemBorderBrush", typeof(Brush), typeof(AdaptiveGridView), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        public Thickness ItemBorderThickness
        {
            get { return (Thickness)GetValue(ItemBorderThicknessProperty); }
            set { SetValue(ItemBorderThicknessProperty, value); }
        }

        public static readonly DependencyProperty ItemBorderThicknessProperty =
            DependencyProperty.Register("ItemBorderThickness", typeof(Thickness), typeof(AdaptiveGridView), new PropertyMetadata(new Thickness(0, 0, 0.5, 0.5)));

        public Brush ItemForeground
        {
            get { return (Brush)GetValue(ItemForegroundProperty); }
            set { SetValue(ItemForegroundProperty, value); }
        }

        public static readonly DependencyProperty ItemForegroundProperty =
            DependencyProperty.RegisterAttached("ItemForeground", typeof(Brush), typeof(AdaptiveGridView), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public Brush ItemBackground
        {
            get { return (Brush)GetValue(ItemBackgroundProperty); }
            set { SetValue(ItemBackgroundProperty, value); }
        }

        public static readonly DependencyProperty ItemBackgroundProperty =
            DependencyProperty.RegisterAttached("ItemBackground", typeof(Brush), typeof(AdaptiveGridView), new PropertyMetadata(new SolidColorBrush(Colors.White)));

        public double ItemWidth
        {
            get { return (double)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.RegisterAttached("ItemWidth", typeof(double), typeof(AdaptiveGridView), new PropertyMetadata(0));

        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.RegisterAttached("ItemHeight", typeof(double), typeof(AdaptiveGridView), new PropertyMetadata(0));

        public HorizontalAlignment ItemHorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(ItemHorizontalAlignmentProperty); }
            set { SetValue(ItemHorizontalAlignmentProperty, value); }
        }

        public static readonly DependencyProperty ItemHorizontalAlignmentProperty =
            DependencyProperty.Register("ItemHorizontalAlignment", typeof(HorizontalAlignment), typeof(AdaptiveGridView), new PropertyMetadata(1));

        public VerticalAlignment ItemVerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(ItemVerticalAlignmentProperty); }
            set { SetValue(ItemVerticalAlignmentProperty, value); }
        }

        public static readonly DependencyProperty ItemVerticalAlignmentProperty =
            DependencyProperty.Register("ItemVerticalAlignment", typeof(VerticalAlignment), typeof(AdaptiveGridView), new PropertyMetadata(1));

        public Thickness ItemMargin
        {
            get { return (Thickness)GetValue(ItemMarginProperty); }
            set { SetValue(ItemMarginProperty, value); }
        }

        public static readonly DependencyProperty ItemMarginProperty =
            DependencyProperty.RegisterAttached("ItemMargin", typeof(Thickness), typeof(AdaptiveGridView), new PropertyMetadata(new Thickness(0)));

        public Thickness ItemPadding
        {
            get { return (Thickness)GetValue(ItemPaddingProperty); }
            set { SetValue(ItemPaddingProperty, value); }
        }

        public static readonly DependencyProperty ItemPaddingProperty =
            DependencyProperty.RegisterAttached("ItemPadding", typeof(Thickness), typeof(AdaptiveGridView), new PropertyMetadata(new Thickness(0)));

        #endregion

        #region Template Properties

        public List<CalendarToggleButton> Children
        {
            get { return (List<CalendarToggleButton>)GetValue(ChildrenProperty); }
            private set { SetValue(ChildrenProperty, value); }
        }

        public static readonly DependencyProperty ChildrenProperty =
            DependencyProperty.Register("Children", typeof(List<CalendarToggleButton>), typeof(AdaptiveGridView), new PropertyMetadata(null));

        public Grid ItemsPanelRoot
        {
            get { return (Grid)GetValue(ItemsPanelRootProperty); }
            private set { SetValue(ItemsPanelRootProperty, value); }
        }

        public static readonly DependencyProperty ItemsPanelRootProperty =
            DependencyProperty.Register("ItemsPanelRoot", typeof(Grid), typeof(AdaptiveGridView), new PropertyMetadata(null));

        public int RowsCount
        {
            get { return (int)GetValue(RowsCountProperty); }
            set { SetValue(RowsCountProperty, value); }
        }

        public static readonly DependencyProperty RowsCountProperty =
            DependencyProperty.RegisterAttached("RowsCount", typeof(int), typeof(AdaptiveGridView), new PropertyMetadata(0));

        public int ColumnsCount
        {
            get { return (int)GetValue(ColumnsCountProperty); }
            set { SetValue(ColumnsCountProperty, value); }
        }

        public static readonly DependencyProperty ColumnsCountProperty =
            DependencyProperty.RegisterAttached("ColumnsCount", typeof(int), typeof(AdaptiveGridView), new PropertyMetadata(0));

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(AdaptiveGridView), new PropertyMetadata(null));

        public object ItemsSource
        {
            get { return (object)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.RegisterAttached("ItemsSource", typeof(object), typeof(AdaptiveGridView), new PropertyMetadata(null));

        #endregion

        #endregion
    }
}
