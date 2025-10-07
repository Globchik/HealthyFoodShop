using HealthyFood_Shop.GeneralWindows;
using System;
using System.Collections.Generic;
using System.Globalization;
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


using System.Printing;


namespace HealthyFood_Shop.MenuParts
{
    /// <summary>
    /// Interaction logic for SellPage.xaml
    /// </summary>
    public partial class SellPage : UserControl
    {
        Classes.Order current_order = new();
        List<Classes.ItemToOrder> items_to_order = new();
        List<Classes.Item> current_items = new();
        public SellPage()
        {
            InitializeComponent();
            Loaded += (s, e) => { 
                Window.GetWindow(this) // Видаляємо замовлення при закритті
                      .Closing += (s1, e1) => DisposeOfOrder();
            };
            }

        private void LoadItems()
        {
            bool err = false;
            double? f_min = null, f_max = null;
            try
            {
                CultureInfo en = new CultureInfo("en-US");
                if (tb_max_price.Text != String.Empty)
                    f_max = double.Parse(tb_max_price.Text, en);
                if (tb_min_price.Text != String.Empty)
                    f_min = double.Parse(tb_min_price.Text, en);

            }
            catch (Exception)
            {
                err = true;
                MessageBox.Show("Невалідний фільтр ціни", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            if (!err)
            {
                SQLite_Interaction.FillItemList(out current_items, tb_f_name.Text, f_min, f_max,
                    tb_f_country.Text, tb_f_manufacturer.Text, tb_f_category.Text, tb_f_subcategory.Text);
                this.datagrid_items.ItemsSource = null;
                this.datagrid_items.ItemsSource = current_items;
            }
            //В замовленні
            if(current_order.Id.HasValue)
                SQLite_Interaction.FillItemToOrderList(out items_to_order, current_order.Id);
            this.datagrid_items_in_order.ItemsSource = null;
            this.datagrid_items_in_order.ItemsSource = items_to_order;

        }

        private void NewOrder()
        {
            current_order = new(null, CurrentUser.current_user.PIB,
            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Обробка", CurrentUser.current_user.Id);
            current_order.Id = SQLite_Interaction.CreateNewOrder(current_order);
            LoadItems();
        }

        private void GetObjectFromSenderContextMenu<T>(object sender, out T it)
        {
            var menuItem = (MenuItem)sender;
            var contextMenu = (ContextMenu)menuItem.Parent;
            var item = (DataGridRow)contextMenu.PlacementTarget;
            it = (T)item.Item;
        }

        private void AddItemToOrder(in Classes.Item toAdd)
        {
            if (!current_order.Id.HasValue)
                NewOrder();
            if (toAdd.ID is not null && current_order.Id is not null)
            {
                GeneralWindows.EditItemToOrderWindow add_win = new(toAdd.ID.Value, current_order.Id.Value);
                add_win.ShowDialog();
                LoadItems();
            }
            else
                throw new Exception("Wrong ID!");
        }

        private void RowContextAdd(object sender, System.Windows.RoutedEventArgs e)
        {
            Classes.Item toAdd;
            GetObjectFromSenderContextMenu(sender, out toAdd);
            AddItemToOrder(toAdd);
        }

        private void RowContextView(object sender, System.Windows.RoutedEventArgs e)
        {
            Classes.Item toView;
            GetObjectFromSenderContextMenu(sender, out toView);
            GeneralWindows.EditItemWindow edit_w = new EditItemWindow(toView, false);
            edit_w.ShowDialog();
        }

        private void btn_clear_filters_Click(object sender, RoutedEventArgs e)
        {
            tb_f_name.Text = String.Empty;
            tb_max_price.Text = String.Empty;
            tb_min_price.Text = String.Empty;
            tb_f_country.Text = String.Empty;
            tb_f_manufacturer.Text = String.Empty;
            tb_f_category.Text = String.Empty;
            tb_f_subcategory.Text = String.Empty;
            LoadItems();
        }

        private void OrderPage_Loaded(object sender, RoutedEventArgs e)
        {
            DisposeOfOrder();
            LoadItems();
        }

        private void OrderPage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key is System.Windows.Input.Key.Enter)
            {
                LoadItems();
            }
        }

        private void DisposeOfOrder()
        {
            if (current_order.FK_Status != "Завершено")
            {
                foreach (var i in items_to_order)
                    SQLite_Interaction.UnbookItem(i);
                if (current_order.Id is not null)
                    SQLite_Interaction.DeleteFromDB((long)current_order.Id, SQLite_Interaction.DB_Tables.order_table);
            }
            this.current_order = new();
            items_to_order = new();
        }

        private void OrderPage_Unloaded(object sender, RoutedEventArgs e)
        {
            DisposeOfOrder();
        }

        private void mi_order_remove_Click(object sender, RoutedEventArgs e)
        {
            Classes.ItemToOrder it;
            GetObjectFromSenderContextMenu(sender, out it);
            SQLite_Interaction.UnbookItem(it);
            LoadItems();
        }

        private void btn_confirm_order_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxResult.Yes ==
                MessageBox.Show("Ви впевнені?", "Точно перейти до сплати?", MessageBoxButton.YesNo, MessageBoxImage.Question))
            {
                this.current_order.FK_Status = "Завершено";
                SQLite_Interaction.FinishOrder(current_order, items_to_order);
                string ch = SQLite_Interaction.FormReciept(current_order);
                current_order.WriteCheckToFile();

                //Друк
                PrintDialog printDlg = new PrintDialog();
                FlowDocument doc = new FlowDocument(new Paragraph(new Run(ch)));
                doc.Name = "Чек";
                IDocumentPaginatorSource idpSource = doc;
                printDlg.PrintDocument(idpSource.DocumentPaginator, "Чек " + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"));


                DisposeOfOrder();
                NewOrder();
                LoadItems();
            }
        }

        private void btn_cancel_order_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxResult.Yes ==
                MessageBox.Show("Ви впевнені?", "Точно відмінити замовлення?", MessageBoxButton.YesNo, MessageBoxImage.Question))
            {
                DisposeOfOrder();
                NewOrder();
                LoadItems();
            }
        }
    }
}
