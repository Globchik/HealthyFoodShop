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
    /// Interaction logic for Z_Report.xaml
    /// </summary>
    public partial class Z_Report : UserControl
    {
        public Z_Report()
        {
            InitializeComponent();
        }

        private void PrintReport()
        {
            PrintDialog printDlg = new PrintDialog();
            FlowDocument doc = new FlowDocument(new Paragraph(new Run(tb_zreport.Text)));
            doc.Name = "Z_Звіт";
            IDocumentPaginatorSource idpSource = doc;
            printDlg.PrintDocument(idpSource.DocumentPaginator, "Z-Звіт " + DateTime.Now);
        }

        private void ShowZReport()
        {
            int checks_amount;
            double sum = SQLite_Statistics.GetDayShare(DateTime.Now, out checks_amount, CurrentUser.current_user.Id);
            string rep;
            General.CreateZReport(out rep,
                CurrentUser.current_user.PIB, DateTime.Now,
                sum, checks_amount);
            this.tb_zreport.Text = rep;
        }

        private void btn_print_Click(object sender, RoutedEventArgs e)
        {
            ShowZReport();
            PrintReport();
        }

        private void uc_z_report_Loaded(object sender, RoutedEventArgs e)
        {
            ShowZReport();
        }
    }
}
