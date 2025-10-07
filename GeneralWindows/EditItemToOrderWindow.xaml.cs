using Microsoft.VisualBasic;
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
    /// Interaction logic for EditItemToOrderWindow.xaml
    /// </summary>
    public partial class EditItemToOrderWindow : Window
    {
        private long orderid;
        private Classes.ItemToOrder itoo;
        private List<Classes.ItemBatch> batches = new();
        private Dictionary<string, long> expdate_to_id = new();
        private bool is_new = true;
        public EditItemToOrderWindow(in long it_id, in long ord_id)
        {
            InitializeComponent();
            itoo = new();
            orderid = ord_id;
            SQLite_Interaction.GetBatchList(it_id, out batches);
        }

        public EditItemToOrderWindow(in long? it_id, in long ord_id, in Classes.ItemToOrder it)
        {
            is_new = false;
            this.itoo = it;
            InitializeComponent();
            orderid = ord_id;
            SQLite_Interaction.GetBatchList(it_id, out batches);
        }

        private void EditITO_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var i in batches)
            {
                if (i.Id is not null)
                    expdate_to_id[i.ExpiryDate is null? "": i.ExpiryDate] = i.Id.Value;
                else
                    throw new Exception("???");
            }
            cb_exp_date.ItemsSource = expdate_to_id.Keys;
            if(is_new)
            {
                cb_exp_date.SelectedIndex = 0;
                tb_amount.Text = "0";
            }
            else
            {
                cb_exp_date.SelectedItem = itoo.BatchExpiryDate;
                tb_amount.Text = itoo.Amount.ToString();
            }
           
        }

        private void btn_apply_changes_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                long? cur_batch_id = expdate_to_id[cb_exp_date.Text];
                if (!cur_batch_id.HasValue)
                    throw new Exception("Оберіть партію товару");
                int tmpi;
                if (int.TryParse(tb_amount.Text, out tmpi) && tmpi > 0)
                {
                    foreach(var i in batches)
                        if(cur_batch_id == i.Id && tmpi > i.Amount)
                            throw new Exception("Забагато товару");
                    itoo.Amount = tmpi;
                }
                else throw new Exception("Невалідна кількість товарів");

                itoo.Amount = tmpi;
                itoo.Batch_ID = cur_batch_id.Value;

                if (itoo.ID is null)
                    SQLite_Interaction.InsertITOToDB(itoo, orderid);
                else
                    SQLite_Interaction.UpdateITO(itoo);
                this.Close();
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void cb_exp_date_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var i in batches)
                if ((i.ExpiryDate is null? "" : i.ExpiryDate) == (string)cb_exp_date.SelectedItem)
                {
                    l_out_of.Content = "/" + i.Amount.ToString();
                    tb_amount.Text = "";
                }
        }
    }
}
