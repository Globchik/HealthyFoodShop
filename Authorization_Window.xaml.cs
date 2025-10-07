using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace HealthyFood_Shop
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LogInto()
        {
            bool is_logged = CurrentUser.Authenticate(this.tb_login.Text, this.passb_pass.Password);
            if (is_logged)
            {

                Main_Menu main_Menu = new Main_Menu();
                main_Menu.Show();
                this.Close();
            }
            else
                this.label_Error_info.Visibility = Visibility.Visible;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LogInto();
        }

        private void window_Auth_Loaded(object sender, RoutedEventArgs e)
        {
            this.KeyDown += new KeyEventHandler(HandleKeyPress);
            tb_login.Focus();
        }

        private void HandleKeyPress(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    LogInto();
                    break;
            }
        }

    }
}
