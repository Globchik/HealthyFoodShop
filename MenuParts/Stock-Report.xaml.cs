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

namespace HealthyFood_Shop.MenuParts
{
    /// <summary>
    /// Interaction logic for Stock_Report.xaml
    /// </summary>
    public partial class Stock_Report : UserControl
    {
        private List<Classes.StockReportRow> stock = new();
        public Stock_Report()
        {
            InitializeComponent();
        }

        private void uc_stock_report_Loaded(object sender, RoutedEventArgs e)
        {
            List<string[]> tb;
            SQLite_Statistics.GetStockAvailableTable(out tb);
            foreach(var row in tb)
                stock.Add(new()
                {
                    ItemName = row[0],
                    ExpiryDate = row[1],
                    Amount = row[2]
                });
            datagrid_report.ItemsSource = null;
            datagrid_report.ItemsSource = stock;

        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            string? word_filepath = Word_Functions.GetFilepathFromSaveFileWindow(".docx", "Word documents", "Інвентарзаційний_звіт_"+DateTime.Now.ToString("yyyy_MM_dd"));
            if (word_filepath is not null)
                Word_Functions.CreateStorageLeftoversWordReport(word_filepath);
        }
    }
}
