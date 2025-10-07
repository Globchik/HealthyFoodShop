using System.Windows;
using System.Windows.Controls;

namespace HealthyFood_Shop.MenuParts
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.l_user_position.Content = CurrentUser.current_user.FK_Position;
            this.l_user_login.Content = CurrentUser.current_user.Login;
            this.l_user_phone.Content = CurrentUser.current_user.PhoneNumber;
            this.l_user_PIB.Content = CurrentUser.current_user.PIB;
        }
    }
}
