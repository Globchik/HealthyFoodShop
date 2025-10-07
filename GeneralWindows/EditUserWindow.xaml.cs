using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace HealthyFood_Shop.GeneralWindows
{
    /// <summary>
    /// Interaction logic for EditUserWindow.xaml
    /// </summary>
    public partial class EditUserWindow : Window
    {
        private Classes.User selected_user = new();
        private List<Classes.LoggedAction> log_list = new();
        private bool editing_locked = true;
        private Dictionary<string, long> position_id_name_dict = new();
        private bool new_pass = false;

        private void LoadLogList()
        {
            this.datagrid_action_log.ItemsSource = null;
            if (selected_user.Id is null)
                return;

            SQLite_Interaction.FillLogActionList(out log_list, selected_user.Id.Value);
            this.datagrid_action_log.ItemsSource = log_list;
        }

        public EditUserWindow()
        {
            editing_locked = false;
            this.Title = "Новий користувач";
            InitializeComponent();
        }

        public EditUserWindow(Classes.User selected_user, bool lock_editing = true)
        {
            this.selected_user = selected_user;
            this.editing_locked = lock_editing;
            this.Title = selected_user.PIB;
            

            InitializeComponent();
        }

        private void edit_user_window_Loaded(object sender, RoutedEventArgs e)
        {
            SQLite_Interaction.GetValue_ID_Dictionary(out position_id_name_dict, SQLite_Interaction.DB_Tables.position);
            this.cb_position.ItemsSource = position_id_name_dict.Keys;
            if (editing_locked)
            {
                btn_apply_changes.IsEnabled = false;
                tb_phone.IsEnabled = false;
                tb_pib.IsEnabled = false;
                btn_change_logpas.IsEnabled = false;
            }

            if (selected_user.Id is null)
            {
                this.datagrid_action_log.Visibility = Visibility.Collapsed;
                return;
            }

            this.cb_position.SelectedItem = selected_user.FK_Position;
            this.tb_phone.Text = selected_user.PhoneNumber;
            this.tb_pib.Text = selected_user.PIB;
            LoadLogList();

            var performSortMethod = typeof(DataGrid)
                .GetMethod("PerformSort",
BindingFlags.Instance | BindingFlags.NonPublic);

            performSortMethod?.Invoke(datagrid_action_log, new[] { datagrid_action_log.Columns[0] });
            performSortMethod?.Invoke(datagrid_action_log, new[] { datagrid_action_log.Columns[0] });

            this.cb_position.IsEnabled = false;
        }

        private void btn_change_logpas_Click(object sender, RoutedEventArgs e)
        {
            Classes.User login_user;
            GeneralWindows.LoginPassWindow get_lp = new();
   
            if (selected_user.Id.HasValue)
            {
                get_lp.ShowDialog();
                if (!get_lp.DialogResult.HasValue || !get_lp.DialogResult.Value)
                    return;
                bool is_logged_in = SQLite_Interaction.LogIntoUser(get_lp.LoginEntered, get_lp.PassEntered, out login_user);
                if (!is_logged_in || login_user.Id is null)
                {
                    MessageBox.Show("Невірний логін чи пароль", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            get_lp.Close();
            get_lp = new("Новий логін", "Новий пароль");
            get_lp.ShowDialog();
            if (!get_lp.DialogResult.HasValue || !get_lp.DialogResult.Value)
                return;

            if(SQLite_Interaction.IsValueInTable(get_lp.LoginEntered,SQLite_Interaction.DB_Tables.employee,"login"))
            {
                MessageBox.Show("Логін вже існує", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            this.selected_user.Login = get_lp.LoginEntered;
            selected_user.Password= get_lp.PassEntered;
            new_pass = true;
        }

        private void btn_apply_changes_Click(object sender, RoutedEventArgs e)
        {
            if(!(General.ValidateString(tb_phone.Text) && General.ValidateString(tb_pib.Text) &&
                General.ValidateString(cb_position.Text) && General.ValidateString(selected_user.Password) &&
                General.ValidateString(selected_user.Login)))
            {
                MessageBox.Show("Заповніть усі поля", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (selected_user.Id is null)
                SQLite_Interaction.InsertUser(selected_user.Login,
                    new_pass? General.GetHash(selected_user.Password): selected_user.Password,
                    tb_pib.Text, tb_phone.Text, cb_position.Text, cb_position.Text == "Менеджер");
            else
                SQLite_Interaction.UpdateUser(selected_user.Id.Value, 
                    selected_user.Login,
                    new_pass ? General.GetHash(selected_user.Password) : selected_user.Password,
                    tb_pib.Text, tb_phone.Text, cb_position.Text, cb_position.Text == "Менеджер");
            this.DialogResult = true;
            this.Close();
        }
    }
}
