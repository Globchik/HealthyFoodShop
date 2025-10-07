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

namespace HealthyFood_Shop.GeneralWindows
{
    /// <summary>
    /// Interaction logic for EditBatchWindow.xaml
    /// </summary>
    /// 
   
    public partial class EditBatchWindow : Window
    {
        private Classes.ItemBatch current_batch;
        public EditBatchWindow(in Classes.ItemBatch cur_batch)
        {
            this.current_batch = cur_batch;
            InitializeComponent();
        }
        public EditBatchWindow(in long it_id)
        {
            this.current_batch = new(null, "2023-12-01", 0, 0, it_id);
            this.current_batch.SetDate(DateTime.Now);
            InitializeComponent();
        }

        private void BatchEditWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if(current_batch.ExpiryDate is null)
            {
                this.chb_date.IsChecked = false;
                this.dp_date.IsEnabled = false;
                this.dp_date.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.chb_date.IsChecked = true;
                this.dp_date.IsEnabled = true;
                this.dp_date.Visibility = Visibility.Visible;
                this.dp_date.SelectedDate = current_batch.GetDate();
            }

            this.tb_available.Text = current_batch.Amount.ToString();
            this.tb_booked.Text = current_batch.Booked.ToString();
        }

        private void chb_date_Checked(object sender, RoutedEventArgs e)
        {
            this.dp_date.IsEnabled = true;
            this.dp_date.Visibility = Visibility.Visible;
        }

        private void chb_date_Unchecked(object sender, RoutedEventArgs e)
        {
            this.dp_date.IsEnabled = false;
            this.dp_date.Visibility = Visibility.Collapsed;
        }

        private void btn_apply_changes_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.dp_date.IsEnabled && dp_date.SelectedDate is not null)
                    current_batch.SetDate(this.dp_date.SelectedDate.Value);
                else
                    current_batch.SetDate(null);
                int tmpi;
                if (int.TryParse(tb_available.Text, out tmpi) && tmpi >=0)
                    current_batch.Amount = tmpi;
                else throw new Exception("Невалідна кількість товарів");
                if (int.TryParse(tb_booked.Text, out tmpi) && tmpi >=0)
                    current_batch.Booked = tmpi;
                else throw new Exception("Невалідна кількість товарів в обробці");

                if (current_batch.Id is null)
                    SQLite_Interaction.InsertBatchToDB(current_batch);
                else
                    SQLite_Interaction.UpdateBatch(current_batch);
                this.Close();
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}
