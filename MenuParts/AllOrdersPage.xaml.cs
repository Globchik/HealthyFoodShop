using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Docs = System.Windows.Documents;

namespace HealthyFood_Shop.MenuParts
{
    /// <summary>
    /// Interaction logic for AllOrdersPage.xaml
    /// </summary>
    public partial class AllOrdersPage : UserControl
    {
        private List<Classes.Order> order_list = new();
        public AllOrdersPage()
        {
            InitializeComponent();
            SQLite_Interaction.FillOrderList(out order_list);
            datagrid_orders.ItemsSource = order_list;
        }

        private string CreateOrReadCheck(in Classes.Order ord)
        {
            string ch = ord.ReadCheckFromFile();
            bool exists = true;
            if (ch == "")
                exists = ord.WriteCheckToFile();
            if(exists)
                ch = ord.ReadCheckFromFile();
            return ch;
        }

        private void PrintReciept(in Classes.Order ord)
        {
            PrintDialog printDlg = new PrintDialog();
            Docs.FlowDocument doc = new Docs.FlowDocument(new Docs.Paragraph(new Docs.Run(CreateOrReadCheck(ord))));
            doc.Name = "Чек";
            Docs.IDocumentPaginatorSource idpSource = doc;
            printDlg.PrintDocument(idpSource.DocumentPaginator, "Чек " + ord.OrderTime);
        }

        private void menuitem_view_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var contextMenu = (ContextMenu)menuItem.Parent;
            var item = (DataGridRow)contextMenu.PlacementTarget;
            var it = (Classes.Order)item.Item;

            MessageBox.Show(CreateOrReadCheck(it));
        }

        private void menuitem_print_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var contextMenu = (ContextMenu)menuItem.Parent;
            var item = (DataGridRow)contextMenu.PlacementTarget;
            var it = (Classes.Order)item.Item;

            PrintReciept(it);
        }

        private void page_all_orders_Loaded(object sender, RoutedEventArgs e)
        {
            var performSortMethod = typeof(DataGrid)
                            .GetMethod("PerformSort",
            BindingFlags.Instance | BindingFlags.NonPublic);

            performSortMethod?.Invoke(datagrid_orders, new[] { datagrid_orders.Columns[0] });
            performSortMethod?.Invoke(datagrid_orders, new[] { datagrid_orders.Columns[0] });
        }
    }
}
