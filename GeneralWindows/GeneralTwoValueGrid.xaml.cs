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

namespace HealthyFood_Shop.GeneralWindows
{
    /// <summary>
    /// Interaction logic for GeneralTwoValueGrid.xaml
    /// </summary>
    public partial class GeneralTwoValueGrid : UserControl
    {
        SQLite_Interaction.DB_Tables tb;
        Dictionary<string, long> my_pairs = new();
        public string vname;
        public GeneralTwoValueGrid(SQLite_Interaction.DB_Tables tb_name, in string val_name)
        {
            tb = tb_name;
            vname=val_name;
            InitializeComponent();
        }

        private void mi_name_add_Click(object sender, RoutedEventArgs e)
        {
            EditValueIDWinfow wn = new(tb, vname);
            wn.ShowDialog();
            LoadValues();
        }

        private void mi_name_change_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var contextMenu = (ContextMenu)menuItem.Parent;
            var item = (DataGridRow)contextMenu.PlacementTarget;
            var v = (KeyValuePair<string, long>)item.Item;
            EditValueIDWinfow wn = new(tb, my_pairs[v.Key],v.Key, vname);
            wn.ShowDialog();
            LoadValues();
        }

        private void mi_name_remove_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var contextMenu = (ContextMenu)menuItem.Parent;
            var item = (DataGridRow)contextMenu.PlacementTarget;
            var v = (KeyValuePair<string,long>)item.Item;
            SQLite_Interaction.DeleteFromDB(my_pairs[v.Key], tb);
            LoadValues();
        }

        private void LoadValues()
        {
            SQLite_Interaction.GetValue_ID_Dictionary(out my_pairs, tb);
            datagrid_values.ItemsSource = null;
            datagrid_values.ItemsSource = my_pairs;
        }

        private void datagrid_items_in_order_Loaded(object sender, RoutedEventArgs e)
        {
            LoadValues();
            dgcol_name.Header = vname;
        }
    }
}
