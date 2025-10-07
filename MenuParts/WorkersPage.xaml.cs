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
    /// Interaction logic for WorkersPage.xaml
    /// </summary>
    public partial class WorkersPage : UserControl
    {
        private List<Classes.User> list_users = new();
        public WorkersPage()
        {
            InitializeComponent();
        }

        private void LoadUsers()
        {
            SQLite_Interaction.FillUserList(out list_users);
            this.datagrid_users.ItemsSource = null;
            this.datagrid_users.ItemsSource = list_users;

        }

        private void workers_page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadUsers();
        }

        private void mi_user_view_Click(object sender, RoutedEventArgs e)
        {
            Classes.User selected_user;
            General.GetObjectFromContextMenu(sender, out selected_user);
            GeneralWindows.EditUserWindow ed_us = new(selected_user, true);
            ed_us.ShowDialog();
        }

        private void mi_user_edit_Click(object sender, RoutedEventArgs e)
        {
            Classes.User selected_user;
            General.GetObjectFromContextMenu(sender, out selected_user);
            if (!(CurrentUser.current_user.IsAdmin || CurrentUser.current_user.Id == selected_user.Id))
            {
                MessageBox.Show("Авторизуйтеся в цьому акаунті, щоб отримати доступ!", "Увага", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            GeneralWindows.EditUserWindow ed_us = new(selected_user, false);
            bool is_acc = ed_us.ShowDialog().GetValueOrDefault(false);
            if (is_acc && selected_user.Id == CurrentUser.current_user.Id)
            {
                MessageBox.Show("Повторно авторизуйтеся в цьому акаунті!", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
                MainWindow mw = new();
                mw.Show();
                Window.GetWindow(this).Close();
            }
            LoadUsers();
        }

        private void mi_user_delete_Click(object sender, RoutedEventArgs e)
        {
            Classes.User selected_user;
            General.GetObjectFromContextMenu(sender, out selected_user);
            if(selected_user.IsAdmin)
            {
                MessageBox.Show("Неможливо видалити менеджера!", "Увага", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!selected_user.Id.HasValue)
                return;

            SQLite_Interaction.DeleteFromDB(selected_user.Id.Value, SQLite_Interaction.DB_Tables.employee);
            LoadUsers();
        }

        private void mi_user_add_Click(object sender, RoutedEventArgs e)
        {
            GeneralWindows.EditUserWindow ed_us = new();
            ed_us.ShowDialog();
            LoadUsers();
        }
    }
}
