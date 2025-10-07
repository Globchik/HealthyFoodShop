using ScottPlot.Colormaps;
using ScottPlot.TickGenerators.TimeUnits;
using ScottPlot.TickGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ScottPlot;
using SkiaSharp;
using ScottPlot.Plottables;
using Microsoft.Win32;

namespace HealthyFood_Shop.MenuParts
{
    /// <summary>
    /// Interaction logic for StatisticsPage.xaml
    /// </summary>
    public partial class StatisticsPage : UserControl
    {
        private static readonly string[] plot_types = { "Сума замовлень по дням", "Найпопулярніші товари" };
        public StatisticsPage()
        {
            InitializeComponent();
        }

        private void HideAllPlots()
        {
            this.wpfplot_sales_per_day.Visibility = Visibility.Collapsed;
            this.wpfplot_sales_per_day.Plot.Clear();
        }

        private void LoadPopularItemsPlot()
        {
            this.wpfplot_sales_per_day.Visibility = Visibility.Visible;
            this.wpfplot_sales_per_day.Plot.Clear();
            this.wpfplot_sales_per_day.Plot.Axes.Bottom.IsVisible = false;
            this.wpfplot_sales_per_day.Plot.Axes.Left.IsVisible = false;

            Dictionary<string, double> name_am;
            SQLite_Statistics.GetPopularItems(out name_am);
            double[] am = name_am.Values.ToArray();
            string[] names = name_am.Keys.ToArray();


            var pie = this.wpfplot_sales_per_day.Plot.Add.Pie(am);
            for (int i = 0; i < name_am.Count; ++i)
            {
                pie.Slices[i].Label = names[i] + " - " + am[i].ToString();
                pie.Slices[i].LabelFontColor = pie.Slices[i].FillColor;
            }
            pie.ShowSliceLabels = false;

            this.wpfplot_sales_per_day.Plot.Axes.AutoScale();
            this.wpfplot_sales_per_day.Plot.Axes.Frame(false);
            this.wpfplot_sales_per_day.Plot.Title("Найпопулярніші товари магазину");
            wpfplot_sales_per_day.Refresh();
        }

        private void LoadSumToDayPlot()
        {
            this.wpfplot_sales_per_day.Visibility = Visibility.Visible;
            this.wpfplot_sales_per_day.Plot.Clear();
            this.wpfplot_sales_per_day.Plot.Axes.Frame(true);

            Dictionary<DateTime, double> date_to_sum;
            SQLite_Statistics.GetSoldSumByDates(out date_to_sum);

            Plot my_pl = this.wpfplot_sales_per_day.Plot;
            my_pl.Clear();

            double[] double_dates = date_to_sum.Keys.Select(x => x.ToOADate()).ToArray();
            var bar_plot = my_pl.Add.Bars(double_dates, date_to_sum.Values.ToArray());
            foreach (var b in bar_plot.Bars)
            {
                b.Label = b.Value.ToString("F");
            }
            bar_plot.ValueLabelStyle.Bold = true;

            my_pl.Axes.DateTimeTicksBottom().TickGenerator =
                new DateTimeFixedInterval(new Day(), 1, new Day(), 1,
                dt => new DateTime(dt.Year, dt.Month, dt.Day)); ;

            my_pl.Axes.Bottom.TickLabelStyle.Rotation = 40;
            my_pl.Axes.Bottom.TickLabelStyle.Alignment = Alignment.MiddleLeft;
            my_pl.Axes.Bottom.TickLabelStyle.Bold = true;
            my_pl.Axes.Bottom.TickLabelStyle.OffsetY = 3;
            float largestLabelWidth = 60;
            my_pl.Axes.Bottom.MinimumSize = largestLabelWidth;
            my_pl.Axes.Right.MinimumSize = largestLabelWidth;

            this.wpfplot_sales_per_day.Plot.Axes.Bottom.IsVisible = true;
            this.wpfplot_sales_per_day.Plot.Axes.Left.IsVisible = true;

            my_pl.RenderManager.RenderStarting += (s, e) =>
            {
                Tick[] ticks = wpfplot_sales_per_day.Plot.Axes.Bottom.TickGenerator.Ticks;
                for (int i = 0; i < ticks.Length; i++)
                {
                    DateTime dt = DateTime.FromOADate(ticks[i].Position);
                    string label = $"{dt:yyyy}.{dt:MM}.{dt:dd}";
                    ticks[i] = new Tick(ticks[i].Position, label);
                }
            };
            my_pl.Title("Продано товарів на суму, грн");
            this.wpfplot_sales_per_day.Plot.Axes.AutoScale();
            wpfplot_sales_per_day.Refresh();
        }

        private void statistics_page_Loaded(object sender, RoutedEventArgs e)
        {
            this.cb_plot_type.ItemsSource = null;
            this.cb_plot_type.ItemsSource = plot_types;
            HideAllPlots();
        }

        private void cb_plot_type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.cb_plot_type.SelectedIndex == 0)
                LoadSumToDayPlot();
            else if (this.cb_plot_type.SelectedIndex == 1)
                LoadPopularItemsPlot();
            else
                HideAllPlots();
        }
    }
}
