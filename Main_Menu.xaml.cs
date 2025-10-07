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
using System.Windows.Shapes;
using static HealthyFood_Shop.General;

namespace HealthyFood_Shop
{
    /// <summary>
    /// Interaction logic for Main_Menu.xaml
    /// </summary>
    public partial class Main_Menu : Window
    {
        public Main_Menu()
        {
            InitializeComponent();
        }



        private void btn_main_page_Click(object sender, RoutedEventArgs e)
        {
            ChangeCurrentPage<MenuParts.MainPage>(ref this.grid_page_container, "Головна", ref this.l_page_name);
        }

        private void btn_items_page_Click(object sender, RoutedEventArgs e)
        {
            ChangeCurrentPage<MenuParts.ItemsPage>(ref this.grid_page_container, "Товари", ref this.l_page_name);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ChangeCurrentPage<MenuParts.MainPage>(ref this.grid_page_container, "Головна", ref this.l_page_name);
            this.tb_sidepanel_userPIB.Text = CurrentUser.current_user.PIB;
            if(CurrentUser.current_user.FK_Position != "Менеджер")
            {
                btn_other.Visibility = Visibility.Collapsed;
                btn_orders.Visibility = Visibility.Collapsed;
                btn_statistics.Visibility = Visibility.Collapsed;
                btn_workers.Visibility = Visibility.Collapsed;
                btn_items_page.Visibility = Visibility.Collapsed;
            }
            if(CurrentUser.current_user.FK_Position != "Продавець")
            {
                btn_sell.Visibility = Visibility.Collapsed;
            }
        }

        private void btn_exit_Click(object sender, RoutedEventArgs e)
        {
            CurrentUser.LogOut();
            MainWindow auth = new MainWindow();
            auth.Show();
            this.Close();
        }

        private void btn_sell_Click(object sender, RoutedEventArgs e)
        {
            ChangeCurrentPage<MenuParts.SellPage>(ref this.grid_page_container, "Продаж", ref this.l_page_name);
        }

        private void btn_other_Click(object sender, RoutedEventArgs e)
        {
            ChangeCurrentPage<MenuParts.OtherPage>(ref this.grid_page_container, "Інше", ref this.l_page_name);
        }

        private void btn_orders_Click(object sender, RoutedEventArgs e)
        {
            ChangeCurrentPage<MenuParts.AllOrdersPage>(ref this.grid_page_container, "Замовлення", ref this.l_page_name);
        }

        private void btn_statistics_Click(object sender, RoutedEventArgs e)
        {
            ChangeCurrentPage<MenuParts.StatisticsPage>(ref this.grid_page_container, "Статистичні дані", ref this.l_page_name);
        }

        private void btn_reports_Click(object sender, RoutedEventArgs e)
        {
            ChangeCurrentPage<MenuParts.ReportsPage>(ref this.grid_page_container, "Звіти", ref this.l_page_name);
        }

        private void btn_workers_Click(object sender, RoutedEventArgs e)
        {
            ChangeCurrentPage<MenuParts.WorkersPage>(ref this.grid_page_container, "Робітники", ref this.l_page_name);
        }

    }
}
