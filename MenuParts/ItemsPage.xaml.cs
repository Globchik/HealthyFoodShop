using HealthyFood_Shop.GeneralWindows;
using System.Windows;
using System.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Intrinsics.X86;

namespace HealthyFood_Shop.MenuParts
{
    /// <summary>
    /// Interaction logic for ItemsPage.xaml
    /// </summary>
    public partial class ItemsPage : UserControl
    {
        List<Classes.Item> current_items = new();

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
            catch(Exception)
            {
                err = true;
                MessageBox.Show("Невалідний фільтр ціни", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            if(!err)
            {
                SQLite_Interaction.FillItemList(out current_items, tb_f_name.Text, f_min, f_max, 
                    tb_f_country.Text, tb_f_manufacturer.Text, tb_f_category.Text, tb_f_subcategory.Text);
                this.datagrid_items.ItemsSource = null;
                this.datagrid_items.ItemsSource = current_items;
            }
            
        }

        public ItemsPage()
        {
            InitializeComponent();
        }

        private void ItemsPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            LoadItems();
        }

        private void GetItemFromSender(object sender, out Classes.Item it)
        {
            var menuItem = (MenuItem)sender;
            var contextMenu = (ContextMenu)menuItem.Parent;
            var item = (DataGridRow)contextMenu.PlacementTarget;
            it = (Classes.Item)item.Item;
        }

        private void RowContextEdit(object sender, System.Windows.RoutedEventArgs e)
        {
            Classes.Item toEdit;
            GetItemFromSender(sender, out toEdit);
            GeneralWindows.EditItemWindow edit_w = new EditItemWindow(toEdit, true);
            edit_w.ShowDialog();
            LoadItems();
        }

        private void RowContextView(object sender, System.Windows.RoutedEventArgs e)
        {
            Classes.Item toView;
            GetItemFromSender(sender, out toView);
            GeneralWindows.EditItemWindow edit_w = new EditItemWindow(toView, false);
            edit_w.ShowDialog();
        }

        private void RowContextDelete(object sender, System.Windows.RoutedEventArgs e)
        {
            Classes.Item toDelete;
            GetItemFromSender(sender, out toDelete);
            if (toDelete.ID is not null && MessageBoxResult.Yes ==
                MessageBox.Show("Ви впевнені?", "Це повністю видалить товар!", MessageBoxButton.YesNo, MessageBoxImage.Question))
            {
                SQLite_Interaction.DeleteFromDB((long)toDelete.ID, SQLite_Interaction.DB_Tables.item);
                LoadItems();
            }
        }

        private void RowContextAdd(object sender, System.Windows.RoutedEventArgs e)
        {
            GeneralWindows.EditItemWindow edit_w = new EditItemWindow();
            edit_w.ShowDialog();
            LoadItems();
        }

        private void MenuItemsPage_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key is System.Windows.Input.Key.Enter)
                LoadItems();
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
    }
}
