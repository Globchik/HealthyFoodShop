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
    /// Interaction logic for ReportsPage.xaml
    /// </summary>
    public partial class ReportsPage : UserControl
    {
        public ReportsPage()
        {
            InitializeComponent();
        }

        private void uc_reports_page_Loaded(object sender, RoutedEventArgs e)
        {
            if(CurrentUser.current_user.FK_Position == "Продавець")
                General.ChangeCurrentPage<Z_Report>(ref grid_for_report);
            else if(CurrentUser.current_user.FK_Position == "Менеджер")
                General.ChangeCurrentPage<Stock_Report>(ref grid_for_report);
        }
    }
}
