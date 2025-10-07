using System;
using System.Windows;

namespace HealthyFood_Shop.GeneralWindows
{
    public partial class LoginPassWindow : Window
    {
        private string pass_l = String.Empty, login_l = String.Empty,
            pass_got = String.Empty, login_got = String.Empty;
        public string PassLabel { get => pass_l; set => pass_l = value; }
        public string LoginLabel{ get => login_l; set => login_l = value; }
        public string LoginEntered { get => login_got; }
        public string PassEntered { get => pass_got; }

        private void btn_apply_Click(object sender, RoutedEventArgs e)
        {
            if(General.ValidateString(tb_login.Text) && General.ValidateString(tb_pass.Text))
            {
                this.login_got = tb_login.Text;
                this.pass_got = tb_pass.Text;
                MessageBox.Show("Успіх!", "Логін і пароль прийнято",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            else
                MessageBox.Show("Невалідний логін чи пароль", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
        }
        private void login_pass_window_Loaded(object sender, RoutedEventArgs e)
        {
            if (pass_l != String.Empty)
                this.l_pass.Content = pass_l;
            if (login_l != String.Empty) 
                this.l_login.Content = login_l;
        }
        public LoginPassWindow()
        {
            InitializeComponent();
        }
        public LoginPassWindow(in string label_login, in string label_pass)
        {
            this.pass_l = label_pass;
            this.login_l = label_login;
            InitializeComponent();
        }
    }
}
