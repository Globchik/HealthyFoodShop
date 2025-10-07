using System.Windows;
using System;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Globalization;

namespace HealthyFood_Shop.GeneralWindows
{
    /// <summary>
    /// Interaction logic for EditItemWindow.xaml
    /// </summary>
    public partial class EditItemWindow : Window
    {
        private bool ItemEditable = true;
        private Classes.Item current_item;
        private Dictionary<string, long> country_id_dict = new();
        private Dictionary<string, long> category_id_dict = new();
        private Dictionary<string, long> subcategory_id_dict = new();
        private Dictionary<string, long> manufacturer_id_dict = new();
        private Dictionary<string, long> mes_unit_id_dict = new();
        private List<Classes.ItemBatch> current_batches = new();

        private void LoadBatches()
        {
            if (this.current_item.ID is not null)
                SQLite_Interaction.GetBatchList((long)this.current_item.ID, out this.current_batches);
            this.datagrid_batches.ItemsSource = null;
            this.datagrid_batches.ItemsSource = this.current_batches;
        }

        public EditItemWindow()
        {
            this.ItemEditable = true;
            current_item = new Classes.Item();
            InitializeComponent();
        }
        public EditItemWindow(in long it_id, bool is_edit)
        {
            this.ItemEditable = is_edit;
            SQLite_Interaction.GetItemFromDB(it_id, out this.current_item);
            InitializeComponent();
        }

        public EditItemWindow(in Classes.Item it, bool is_edit)
        {
            this.ItemEditable = is_edit;
            this.current_item = it;
            InitializeComponent();
        }

        private void ItemsEditWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CultureInfo en = new CultureInfo("en-US");
            SQLite_Interaction.GetValue_ID_Dictionary(out country_id_dict, SQLite_Interaction.DB_Tables.country);
            this.cb_country.ItemsSource = country_id_dict.Keys;
            SQLite_Interaction.GetValue_ID_Dictionary(out category_id_dict, SQLite_Interaction.DB_Tables.category);
            this.cb_category.ItemsSource = category_id_dict.Keys;
            if(current_item.FK_Category is not null)
                SQLite_Interaction.GetSubcategory_ID_Dictionary(out subcategory_id_dict, current_item.FK_Category);
            this.cb_subcategory.ItemsSource = subcategory_id_dict.Keys;
            SQLite_Interaction.GetValue_ID_Dictionary(out manufacturer_id_dict, SQLite_Interaction.DB_Tables.manufacturer);
            this.cb_manufacturer.ItemsSource = manufacturer_id_dict.Keys;
            SQLite_Interaction.GetValue_ID_Dictionary(out mes_unit_id_dict, SQLite_Interaction.DB_Tables.measurment_unit);
            this.cb_measurment_unit.ItemsSource = mes_unit_id_dict.Keys;

            LoadBatches();


            if (this.current_item.ID is null)
                this.datagrid_batches.Visibility = Visibility.Collapsed;


            if (this.current_item.Discount is not null && this.current_item.Discount != 0)
            {
                this.chb_discount.IsChecked = true;
                this.tb_discount.Text = this.current_item.Discount.ToString();
            }
            else
            {
                this.chb_discount.IsChecked = false;
                this.tb_discount.IsEnabled = false;
            }

            if (this.current_item.FK_SubCategory is not null)
            {
                this.chb_sub_category.IsChecked = true;
                this.cb_subcategory.SelectedItem = this.current_item.FK_SubCategory;
            }
            else
            {
                this.chb_sub_category.IsChecked = false;
                this.cb_subcategory.IsEnabled = false;
            }


            this.tb_amount_in_item.Text = this.current_item.AmountInItem.ToString(en);
            this.tb_comment.Text = current_item.Comment;
            this.tb_price.Text = current_item.Price.ToString(en);
            this.tb_discount.Text = current_item.Discount.ToString();
            this.tb_name.Text = current_item.Name;
            this.cb_category.SelectedItem = this.current_item.FK_Category;
            this.cb_country.SelectedItem = this.current_item.FK_Country;
            this.cb_manufacturer.SelectedItem = this.current_item.FK_Manufacturer;
            this.cb_measurment_unit.SelectedItem = this.current_item.FK_MeasurmentUnit;
            this.cb_subcategory.SelectedItem = this.current_item.FK_SubCategory;
            this.chb_discount.IsEnabled = ItemEditable;

            if (!ItemEditable)
            {
                this.tb_amount_in_item.IsReadOnly = true;
                this.tb_comment.IsReadOnly = true;
                this.tb_price.IsReadOnly = true;
                this.tb_discount.IsReadOnly = true;
                this.tb_name.IsReadOnly = true;
                this.cb_category.IsEnabled = false;
                this.cb_country.IsEnabled = false;
                this.cb_manufacturer.IsEnabled = false;
                this.cb_measurment_unit.IsEnabled = false;
                this.cb_subcategory.IsEnabled = false;
                this.chb_sub_category.IsEnabled = false;
            }
        }

        private void chb_discount_Checked(object sender, RoutedEventArgs e)
        {
            this.tb_discount.IsEnabled = true;
        }

        private void chb_discount_Unchecked(object sender, RoutedEventArgs e)
        {
            this.tb_discount.Text = String.Empty;
            this.tb_discount.IsEnabled = false;
        }

        private void chb_sub_category_Checked(object sender, RoutedEventArgs e)
        {
            this.cb_subcategory.IsEnabled = true;
        }

        private void chb_sub_category_Unchecked(object sender, RoutedEventArgs e)
        {
            this.cb_subcategory.SelectedIndex = -1;
            this.cb_subcategory.IsEnabled = false;
        }

        private void btn_apply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ItemEditable)
                {
                    CultureInfo en = new CultureInfo("en-US");
                    double tmp;
                    int tmpi;
                        current_item.Name = tb_name.Text;
                        if (General.ValidatePriceString(tb_price.Text))
                            current_item.Price = double.Parse(tb_price.Text, en);
                        else throw new Exception("Невалідна ціна");
                        current_item.FK_Category = cb_category.Text;
                        current_item.FK_SubCategory = cb_subcategory.IsEnabled ? cb_subcategory.Text : null;
                        current_item.FK_Country = cb_country.Text;
                        current_item.FK_Manufacturer = cb_manufacturer.Text;
                        current_item.FK_MeasurmentUnit = cb_measurment_unit.Text;
                    if (double.TryParse(tb_amount_in_item.Text, NumberStyles.AllowDecimalPoint, en, out tmp))
                        current_item.AmountInItem = double.Parse(tb_amount_in_item.Text, en);
                        else throw new Exception("Невалідна кількість");
                        current_item.Comment = tb_comment.Text;
                        if (!tb_discount.IsEnabled)
                            current_item.Discount = 0;
                        else if (int.TryParse(tb_discount.Text, out tmpi))
                            current_item.Discount = int.Parse(tb_discount.Text);
                        else throw new Exception("Невалідна кількість");

                    if (current_item.ID is null)
                        SQLite_Interaction.InsertItemToDB(current_item);
                    else 
                        SQLite_Interaction.UpdateItem(current_item);
                }
                this.Close();
            }
            catch(Exception er)
            {
                MessageBox.Show(er.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cb_category_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.cb_subcategory.SelectedIndex = -1;
            SQLite_Interaction.GetSubcategory_ID_Dictionary(out subcategory_id_dict, (string)cb_category.SelectedItem);
            this.cb_subcategory.ItemsSource = null;
            this.cb_subcategory.ItemsSource = subcategory_id_dict.Keys;
        }

        private void mi_batch_delete_Click(object sender, RoutedEventArgs e)
        {
            if (!this.ItemEditable)
                return;
            Classes.ItemBatch toDelete;
            var menuItem = (MenuItem)sender;
            var contextMenu = (ContextMenu)menuItem.Parent;
            var item = (DataGridRow)contextMenu.PlacementTarget;
            toDelete = (Classes.ItemBatch)item.Item;
            if (toDelete.Id is not null && MessageBoxResult.Yes ==
                MessageBox.Show("Ви впевнені?", "Це повністю видалить партію!", MessageBoxButton.YesNo, MessageBoxImage.Question))
            {
                  SQLite_Interaction.DeleteFromDB(toDelete.Id.Value, SQLite_Interaction.DB_Tables.item_batch);
                LoadBatches();
            }
        }

        private void mi_batch_edit_Click(object sender, RoutedEventArgs e)
        {
            if (!this.ItemEditable)
                return;
            Classes.ItemBatch toEdit;
            var menuItem = (MenuItem)sender;
            var contextMenu = (ContextMenu)menuItem.Parent;
            var item = (DataGridRow)contextMenu.PlacementTarget;
            toEdit = (Classes.ItemBatch)item.Item;
            GeneralWindows.EditBatchWindow edit_w = new(toEdit);
            edit_w.ShowDialog();
            LoadBatches();
        }

        private void mi_batch_add_Click(object sender, RoutedEventArgs e)
        {
            if (!this.ItemEditable)
                return;
            if (current_item.ID is not null)
            {
                EditBatchWindow edit_w = new(current_item.ID.Value);
                edit_w.ShowDialog();
                LoadBatches();
            }
        }

    }
}
