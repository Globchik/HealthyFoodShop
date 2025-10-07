using DocumentFormat.OpenXml.Wordprocessing;
using HealthyFood_Shop.GeneralWindows;
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
using System.Xml.Linq;

namespace HealthyFood_Shop.MenuParts
{
    /// <summary>
    /// Interaction logic for OtherPage.xaml
    /// </summary>
    public partial class OtherPage : UserControl
    {
        public OtherPage()
        {
            InitializeComponent();
        }
        private void Other_page_Loaded(object sender, RoutedEventArgs e)
        {
            stack_other.Children.Add(new GeneralWindows.GeneralTwoValueGrid(SQLite_Interaction.DB_Tables.category, "Категорія"));
            stack_other.Children.Add(new GeneralWindows.GeneralTwoValueGrid(SQLite_Interaction.DB_Tables.manufacturer, "Виробник"));
            stack_other.Children.Add(new GeneralWindows.GeneralTwoValueGrid(SQLite_Interaction.DB_Tables.measurment_unit, "Одиниця виміру"));
            stack_other.Children.Add(new GeneralWindows.GeneralTwoValueGrid(SQLite_Interaction.DB_Tables.country, "Країна"));
            
        }
    }
}
